using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace St.Code
{
    public class DynamicClass
    {
        //公有方法
        #region 公有方法
        /// <summary>
        /// 根据类的类型型创建类实例。
        /// </summary>
        /// <param name="t">将要创建的类型。</param>
        /// <returns>返回创建的类实例。</returns>
        public static object CreateInstance(Type t)
        {
            return Activator.CreateInstance(t);
        }

        /// <summary>
        /// 根据类的名称，属性列表创建型实例。
        /// </summary>
        /// <param name="className">将要创建的类的名称。</param>
        /// <param name="lcpi">将要创建的类的属性列表。</param>
        /// <returns>返回创建的类实例</returns>
        public static object  CreateInstance(string className, List<CustPropertyInfo> lcpi)
        {
            Type t = SetObjectAndAddPropertyToType(className, lcpi);
            return Activator.CreateInstance(t);
        }
        
        /// <summary>
        /// 根据属性列表创建类的实例，默认类名为DefaultClass，由于生成的类不是强类型，所以类名可以忽略。
        /// </summary>
        /// <param name="lcpi">将要创建的类的属性列表</param>
        /// <returns>返回创建的类的实例。</returns>
        public static object CreateInstance(List<CustPropertyInfo> lcpi)
        {
            return CreateInstance("DefaultClass", lcpi);
        }

        /// <summary>
        /// 根据类的实例设置类的属性。
        /// </summary>
        /// <param name="classInstance">将要设置的类的实例。</param>
        /// <param name="propertyName">将要设置属性名。</param>
        /// <param name="propertSetValue">将要设置属性值。</param>
        public static void SetPropertyValue(object classInstance, string propertyName, object propertSetValue)
        {
            classInstance.GetType().InvokeMember(propertyName, BindingFlags.SetProperty,
                                          null, classInstance, new object[] { Convert.ChangeType(propertSetValue, propertSetValue.GetType()) });
        }

        public static void SetPropertyValue(object classInstance, ProPertyVale valueInfo)
        {
            var properties = classInstance.GetType().GetProperties();
            if (properties.Where(p => p.Name == valueInfo.Name).Count() <= 0)
                return;

            var item = properties.Single(p => p.Name == valueInfo.Name);
            item.SetValue(classInstance, StrToLevelEnum(valueInfo.Value));
        }

        public static St.Domain.Entity.LevelInfo StrToLevelEnum(string value)
        {
            int val = Convert.ToInt32(value);
            return (Domain.Entity.LevelInfo)val;
        }

        /// <summary>
        /// 根据类的实例获取类的属性。
        /// </summary>
        /// <param name="classInstance">将要获取的类的实例</param>
        /// <param name="propertyName">将要设置的属性名。</param>
        /// <returns>返回获取的类的属性。</returns>
        public static object GetPropertyValue(object classInstance, string propertyName)
        {
            return classInstance.GetType().InvokeMember(propertyName, BindingFlags.GetProperty,
                                                          null, classInstance, new object[] { });
        }
        #endregion

        // 私有方法
        #region 私有方法
        /// <summary>
        /// 把类型的实例t和lcpi参数里的属性进行合并。
        /// </summary>
        /// <param name="t">实例t</param>
        /// <param name="lcpi">里面包含属性列表的信息。</param>
        private static void MergeProperty(Type t, List<CustPropertyInfo> lcpi)
        {
            CustPropertyInfo cpi;
            foreach (PropertyInfo pi in t.GetProperties())
            {
                cpi = new CustPropertyInfo(pi.PropertyType, pi.Name, pi.Attributes.ToString());
                lcpi.Add(cpi);
            }
        }

        /// <summary>
        /// 从类型的实例t的属性移除属性列表lcpi，返回的新属性列表在lcpi中。
        /// </summary>
        /// <param name="t">类型的实例t。</param>
        /// <param name="lcpi">要移除的属性列表。</param>
        private static List<CustPropertyInfo> SeparateProperty(Type t, List<string> ls)
        {
            List<CustPropertyInfo> ret = new List<CustPropertyInfo>();
            CustPropertyInfo cpi;
            foreach (PropertyInfo pi in t.GetProperties())
            {
                foreach (string s in ls)
                {
                    if (pi.Name != s)
                    {
                        cpi = new CustPropertyInfo(pi.PropertyType, pi.Name);
                        ret.Add(cpi);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// 把lcpi参数里的属性加入到myTypeBuilder中。注意：该操作会将其它成员清除掉，其功能有待完善。
        /// </summary>
        /// <param name="myTypeBuilder">类型构造器的实例。</param>
        /// <param name="lcpi">里面包含属性列表的信息。</param>
        private static void AddPropertyToTypeBuilder(TypeBuilder myTypeBuilder, List<CustPropertyInfo> lcpi)
        {
            FieldBuilder customerNameBldr;
            PropertyBuilder custNamePropBldr;
            MethodBuilder custNameGetPropMthdBldr;
            MethodBuilder custNameSetPropMthdBldr;
            MethodAttributes getSetAttr;
            ILGenerator custNameGetIL;
            ILGenerator custNameSetIL;

            // 属性Set和Get方法要一个专门的属性。这里设置为Public。
            getSetAttr =
                MethodAttributes.Public | MethodAttributes.SpecialName |
                    MethodAttributes.HideBySig;

            // 添加属性到myTypeBuilder。
            foreach (CustPropertyInfo cpi in lcpi)
            {
                //定义字段。
                customerNameBldr = myTypeBuilder.DefineField(cpi.FieldName,
                                                                 cpi.Type,
                                                                 FieldAttributes.Private);

                //获取构造器信息
                ConstructorInfo classCtorInfo = typeof(DisplayNameAttribute).GetConstructor(new Type[] { typeof(string) });
                //动态创建ClassCreatorAttribute
                CustomAttributeBuilder tyAttribute = new CustomAttributeBuilder(classCtorInfo, new object[] { cpi.AttributeName });


                //定义属性。
                //最后一个参数为null，因为属性没有参数。
                custNamePropBldr = myTypeBuilder.DefineProperty(cpi.PropertyName,
                                                                 PropertyAttributes.HasDefault,
                                                                 cpi.Type,
                                                                 null);

                custNamePropBldr.SetCustomAttribute(tyAttribute);
                //var tyS = custNamePropBldr.CustomAttributes;

                //定义Get方法。
                custNameGetPropMthdBldr =
        myTypeBuilder.DefineMethod(cpi.GetPropertyMethodName,
                                   getSetAttr,
                                   cpi.Type,
                                   Type.EmptyTypes);

                custNameGetIL = custNameGetPropMthdBldr.GetILGenerator();

                custNameGetIL.Emit(OpCodes.Ldarg_0);
                custNameGetIL.Emit(OpCodes.Ldfld, customerNameBldr);
                custNameGetIL.Emit(OpCodes.Ret);

                //定义Set方法。
                custNameSetPropMthdBldr =
                    myTypeBuilder.DefineMethod(cpi.SetPropertyMethodName,
                                               getSetAttr,
                                               null,
                                               new Type[] { cpi.Type });

                custNameSetIL = custNameSetPropMthdBldr.GetILGenerator();

                custNameSetIL.Emit(OpCodes.Ldarg_0);
                custNameSetIL.Emit(OpCodes.Ldarg_1);
                custNameSetIL.Emit(OpCodes.Stfld, customerNameBldr);
                custNameSetIL.Emit(OpCodes.Ret);

                //把创建的两个方法(Get,Set)加入到PropertyBuilder中。
                custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
                custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
            }
        }

        /// <summary>
        /// 把属性加入到类型的实例。
        /// </summary>
        /// <param name="classType">类型的实例。</param>
        /// <param name="lcpi">要加入的属性列表。</param>
        /// <returns>返回处理过的类型的实例。</returns>
        public static Type SetObjectAndAddPropertyToType(string name, List<CustPropertyInfo> lcpi)
        {
            AppDomain myDomain = Thread.GetDomain();
            AssemblyName myAsmName = new AssemblyName();
            myAsmName.Name = "DynamicAssemblyT";

            //创建一个永久程序集，设置为AssemblyBuilderAccess.RunAndSave。
            AssemblyBuilder myAsmBuilder = myDomain.DefineDynamicAssembly(myAsmName,
                                                            AssemblyBuilderAccess.RunAndSave);

            //创建一个永久单模程序块。
            ModuleBuilder myModBuilder =
                myAsmBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
            //创建TypeBuilder。
            TypeBuilder myTypeBuilder = myModBuilder.DefineType(name,
                                                            TypeAttributes.Public);

            //把lcpi中定义的属性加入到TypeBuilder。将清空其它的成员。其功能有待扩展，使其不影响其它成员。
            AddPropertyToTypeBuilder(myTypeBuilder, lcpi);
            //AddPropertyForAttribute(myTypeBuilder, lcpi);
            
            //创建类型。
            Type retval = myTypeBuilder.CreateType();

            //保存程序集，以便可以被Ildasm.exe解析，或被测试程序引用。
            //myAsmBuilder.Save(myAsmName.Name + ".dll");
            return retval;
        }
        #endregion

        // 辅助类
        #region 辅助类
        /// <summary>
        /// 自定义的属性信息类型。
        /// </summary>
        public class CustPropertyInfo
        {
            private string propertyName;
            private Type type;
            private string attributeName;

            /// <summary>
            /// 空构造。
            /// </summary>
            public CustPropertyInfo() { }


            /// <summary>
            /// 根据属性类型名称，属性名称构造实例。
            /// </summary>
            /// <param name="type">属性类型名称。</param>
            /// <param name="propertyName">属性名称。</param>
            public CustPropertyInfo(Type type, string propertyName)
            {
                this.type = type;
                this.propertyName = propertyName;
            }

            /// <summary>
            /// 根据属性类型名称，属性名称构造实例。
            /// </summary>
            /// <param name="type">属性类型名称。</param>
            /// <param name="propertyName">属性名称。</param>
            /// <param name="attributeName">描述特性值</param>
            public CustPropertyInfo(Type type, string propertyName, string attributeName)
            {
                this.type = type;
                this.propertyName = propertyName;
                this.attributeName = attributeName;
            }

            /// <summary>
            /// 获取或设置属性类型名称。
            /// </summary>
            public Type Type
            {
                get { return type; }
                set { type = value; }
            }

            /// <summary>
            /// 获取或设置属性名称。
            /// </summary>
            public string PropertyName
            {
                get { return propertyName; }
                set { propertyName = value; }
            }

            /// <summary>
            /// 获取或设置特性名称
            /// </summary>
            public string AttributeName
            {
                get { return attributeName; }
                set { attributeName = value; }
            }

            /// <summary>
            /// 获取属性字段名称。
            /// </summary>
            public string FieldName
            {
                get { return $"_{ propertyName.Substring(0, 1).ToLower()} {propertyName.Substring(1)}"; }
            }

            /// <summary>
            /// 获取属性在IL中的Set方法名。
            /// </summary>
            public string SetPropertyMethodName
            {
                get { return "set_" + PropertyName; }
            }

            /// <summary>
            ///  获取属性在IL中的Get方法名。
            /// </summary>
            public string GetPropertyMethodName
            {
                get { return "get_" + PropertyName; }
            }
        }
        #endregion
    }
}
