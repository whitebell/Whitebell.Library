using System;
using System.Collections.Concurrent;
using System.Linq;
using Whitebell.Library.Extension;

namespace Whitebell.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class LabelNameAttribute : Attribute
    {
        private readonly string _label;
        private static readonly ConcurrentDictionary<Enum, string> cache = new ConcurrentDictionary<Enum, string>();

        public LabelNameAttribute(string label) => _label = label;

        public static string GetLabelName(Enum value) => cache.GetOrAdd(value, GetLabelNameFromEnum);

        private static string GetLabelNameFromEnum(Enum value)
        {
            var t = value.GetType();
            var n = Enum.GetName(t, value);
            if (n == null)
                return null;
            return t.GetField(n)?.GetCustomAttributes<LabelNameAttribute>(false)?.SingleOrDefault()?._label;
        }
    }
}
