using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace CropApp.Backend
{
    internal class TypeConverterStringTouple
        : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
            sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

        public override object ConvertFrom(ITypeDescriptorContext context,
                                           CultureInfo            culture, object value)
        {
            string[] elements = ((string) value)!.Split(new[] {'(', ',', ')'},
                                                        StringSplitOptions.RemoveEmptyEntries);

            return (elements.First().Trim(), elements.Last().Trim());
        }
    }
}