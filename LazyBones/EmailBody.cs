using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyBones
{
    public class EmailBody
    {
        public EmailBody()
        {
        }

        public string Create(object obj, Dictionary<string, string> additionalData = null)
        {
            return PocoToEmailBody(obj, additionalData);
        }

        protected string PocoToEmailBody(object obj, Dictionary<string, string> additionalData = null)
        {
            var builder = new StringBuilder();

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                var name = property.Name;
                var value = property.GetValue(obj, null);

                builder = NestedObjectPocoToEmailBody(name, value, builder);
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

        private StringBuilder NestedObjectPocoToEmailBody(string name, object obj, StringBuilder builder, int indents = 0)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (obj != null)
                {
                    var type = obj.GetType();

                    if (!type.IsSimpleType())
                    {
                        var properties = type.GetProperties();

                        foreach (var property in properties.Where(x => x.CanRead))
                        {
                            var propName = property.Name;
                            object propValue = null;
                            try
                            {
                                propValue = property.GetValue(obj, null);
                            }
                            catch { } // Swallow the error.  I dont know what to do with it and I dont want it to stop all processing.

                            builder = NestedObjectPocoToEmailBody(propName, propValue, builder, indents + 1);
                        }
                    }
                    else
                    {
                        builder = builder.AppendLine(BuildEmailPropertyLine(name, obj.ToString(), indents));
                    }
                }
                else
                {
                    builder = builder.AppendLine(BuildEmailPropertyLine(name, string.Empty, indents));
                }
            }

            return builder;
        }

        private string BuildEmailPropertyLine(string name, string value, int indents = 0)
        {
            return string.Format("{0}{1} : {2}", BuildIndents(indents), name, value);
        }

        private string BuildIndents(int indents = 0)
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
