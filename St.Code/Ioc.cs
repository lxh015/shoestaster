using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    public class Ioc
    {
        private static readonly UnityContainer container;

        static Ioc()
        {
            container = new UnityContainer();
        }

        public static void Register<TInterface, TImpmentation>() where TImpmentation : TInterface
        {
            container.RegisterType<TInterface, TImpmentation>();
        }

        public static void RegisterInheritedTypes(Assembly assembly, Type baseType)
        {
            container.RegisterInheritedTypes(assembly, baseType);
        }

        public static T GetService<T>()
        {
            return container.Resolve<T>();
        }
    }

    public static class UnityContainerExtensions
    {
        public static void RegisterInheritedTypes(this IUnityContainer container, Assembly assembly, Type baseType)
        {
            var allTypes = assembly.GetTypes();
            var baseInterfaces = baseType.GetInterfaces();

            foreach (var type in allTypes)
            {
                if (type.BaseType != null && type.BaseType.GenericEq(baseType))
                {
                    var typeInterface = type.GetInterfaces().FirstOrDefault(p => !baseInterfaces.Any(f => f.GenericEq(p)));
                    if (typeInterface == null)
                        continue;
                    container.RegisterType(typeInterface, type);
                }
            }
        }
    }
    public static class TypeExtensions
    {
        public static bool GenericEq(this Type type, Type toCompare)
        {
            return type.Namespace == toCompare.Namespace && type.Name == toCompare.Name;
        }
    }
}
