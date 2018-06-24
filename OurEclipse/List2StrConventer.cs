using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace OurEclipse
{
	class List2StrConventer : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is List<string> list)
			{
				StringBuilder sb = new StringBuilder();
				foreach (var str in list)
					sb.Append(str).Append(", ");
				return sb.ToString();
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
