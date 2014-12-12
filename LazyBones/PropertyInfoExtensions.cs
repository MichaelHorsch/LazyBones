using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LazyBones
{
    public static class PropertyInfoExtensions
    {
        public static object SafeGetValue(this PropertyInfo property, object obj)
        {
            object propValue = null;
            try
            {
                propValue = property.GetValue(obj);
            }
            catch { } // Swallow the error.  I dont know what to do with it and I dont want it to stop all processing.

            return propValue;
        }
    }
}
