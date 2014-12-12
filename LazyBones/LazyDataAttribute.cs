using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyBones
{
    [System.AttributeUsage(System.AttributeTargets.Property | System.AttributeTargets.Field)]
    public class LazyDataAttribute : Attribute
    {
        public string name;
        public bool ignore;
    }
}
