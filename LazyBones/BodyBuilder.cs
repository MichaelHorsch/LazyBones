using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LazyBones
{
    public class BodyBuilder
    {
        public BodyBuilder()
        {
        }

        public string Create(object obj, Dictionary<string, string> additionalData = null)
        {
            return PocoToEmailBody(obj, additionalData, 0);
        }

        protected string PocoToEmailBody(object obj, Dictionary<string, string> additionalData = null, int indents = 0)
        {
            var builder = new StringBuilder();

            if (obj != null)
            {
                var type = obj.GetType();

                if (!type.IsSimpleType())
                {
                    HandleComplexType(type, "", obj, builder, indents);
                }
                else
                {
                    // simple type
                    builder = builder.AppendLine(SimpleTypeToString(obj));
                }
            }

            if (additionalData != null)
            {
                foreach (var item in additionalData)
                {
                    builder = builder.AppendLine(BuildEmailPropertyLine(item.Key, item.Value));
                }
            }

            return builder.ToString();
        }

        protected StringBuilder NestedObjectPocoToEmailBody(string name, object obj, StringBuilder builder, int indents = 0)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (obj != null)
                {
                    var type = obj.GetType();

                    if (!type.IsSimpleType())
                    {
                        HandleComplexType(type, name, obj, builder, indents + 1);
                    }
                    else
                    {
                        builder = builder.AppendLine(BuildEmailPropertyLine(name, SimpleTypeToString(obj), indents));
                    }
                }
                else
                {
                    builder = builder.AppendLine(BuildEmailPropertyLine(name, string.Empty, indents));
                }
            }

            return builder;
        }

        protected StringBuilder HandleComplexType(Type type, string name, object obj, StringBuilder builder, int indents = 0)
        {
            if (obj is IEnumerable)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    builder = builder.AppendLine(BuildEmailPropertyLine(name, string.Empty, indents - 1));
                }

                var enumerable = (IEnumerable)obj;

                var index = 1;
                foreach (var item in enumerable)
                {
                    builder = builder.AppendLine(BuildArrayItemLine(index, indents));
                    builder = builder.Append(PocoToEmailBody(item, null, indents));
                    index++;
                }
            }
            else
            {
                var properties = type.GetProperties();

                if (!string.IsNullOrEmpty(name))
                {
                    // Put it at the level above.
                    builder = builder.AppendLine(BuildEmailPropertyLine(name, string.Empty, indents - 1));
                }

                foreach (var property in properties.Where(x => x.CanRead))
                {
                    var attr = FetchLazyDataAttr(property);
                    var propName = property.Name;
                    bool ignore = false;

                    if (attr != null)
                    {
                        if (!string.IsNullOrEmpty(attr.name))
                        {
                            propName = attr.name;
                        }

                        ignore = attr.ignore;
                    }

                    if (!ignore)
                    {
                        object propValue = property.SafeGetValue(obj);
                        builder = NestedObjectPocoToEmailBody(propName, propValue, builder, indents);
                    }
                }
            }

            return builder;
        }

        protected LazyDataAttribute FetchLazyDataAttr(PropertyInfo property)
        {
            LazyDataAttribute dataAttr = null;

            object[] attrs = property.GetCustomAttributes(true);
            foreach (object attr in attrs)
            {
                LazyDataAttribute tempAttr = attr as LazyDataAttribute;
                if (tempAttr != null)
                {
                    dataAttr = tempAttr;
                }
            }

            return dataAttr;
        }

        protected string SimpleTypeToString(object obj)
        {
            return obj.ToString();
        }

        protected string BuildArrayItemLine(int index, int indents = 0)
        {
            return string.Format("{0}*item {1}*", BuildIndents(indents), index);
        }

        protected string BuildEmailPropertyLine(string name, string value, int indents = 0)
        {
            return string.Format("{0}{1}: {2}", BuildIndents(indents), name, value);
        }

        protected string BuildIndents(int indents = 0)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < indents; i++)
            {
                builder = builder.Append("-");
            }
            if (indents > 0)
            {
                builder = builder.Append(" ");
            }
            return builder.ToString();
        }
    }
}
