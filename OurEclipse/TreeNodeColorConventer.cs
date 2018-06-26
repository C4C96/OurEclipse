using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace OurEclipse
{
	class TreeNodeColorConventer : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool? isLeaf = value as bool?;
			if (isLeaf != null)
			{
				if (isLeaf == true)
					return new SolidColorBrush(Color.FromRgb(20, 177, 28));
				else
					return new SolidColorBrush(Color.FromRgb(0, 0, 0));
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
