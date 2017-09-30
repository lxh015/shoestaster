using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace St.Code
{
    public class ValueClone
    {
        public static void Clone<T>(T old,T newItem)
        {
            var properties = old.GetType().GetProperties();
            foreach (var item in properties)
            {
                var itemValue = item.GetValue(newItem);
                item.SetValue(old, itemValue);
            }
        }
    }
}
