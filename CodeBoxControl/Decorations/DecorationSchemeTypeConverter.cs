using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeBoxControl.Decorations
{
	public class DecorationSchemeTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string testString)
				switch (testString)
				{
					case "CSharp3":
					case "CSharp":
						return DecorationSchemes.CSharp3;
					case "SQLServer2008":
					case "SQL":
						return DecorationSchemes.SQLServer2008;
					case "JAVA":
					case "Java":
						return DecorationSchemes.Java;
				}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
