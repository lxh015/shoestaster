using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static St.Code.DynamicClass;

namespace St.Code
{
    public class CreateWebSet
    {
        public static string DynamicAttributeStr = "St.Domain.Entity.LevelInfo";

        public static object GetWebSet(string context)
        {
            List<ProPertyVale> propertyVal = new List<ProPertyVale>();
            List<CustPropertyInfo> custList = GetPropertiesFromStr(context, out propertyVal);
            if (custList.Count == 0)
                throw new ArgumentNullException("context");

            var obj = CreateInstance("WebSet", custList);

            foreach (var item in propertyVal)
            {
                SetPropertyValue(obj,item);
            }
            return obj;
        }

        public static List<CustPropertyInfo> GetPropertiesFromStr(string context,out List<ProPertyVale> propertyValue)
        {
            propertyValue = new List<ProPertyVale>();

            string[] propertiesStrArray = context.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (propertiesStrArray.Length == 0)
                return new List<DynamicClass.CustPropertyInfo>();

            List<CustPropertyInfo> temp = new List<CustPropertyInfo>();
            foreach (var item in propertiesStrArray)
            {
                string[] itemArray = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                DynamicClass.CustPropertyInfo info = new DynamicClass.CustPropertyInfo(typeof(St.Domain.Entity.LevelInfo), itemArray[1], itemArray[0]);
                ProPertyVale itemVal = new ProPertyVale();
                itemVal.Name = itemArray[1];
                itemVal.Value = itemArray[2];
                propertyValue.Add(itemVal);
                temp.Add(info);
            }

            return temp;
        }
    }

    public struct ProPertyVale
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
