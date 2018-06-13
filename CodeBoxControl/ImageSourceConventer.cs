using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CodeBoxControl
{
	public class ImageSourceConventer : IValueConverter
	{
		private static readonly BitmapImage CLASS_IMAGE = new BitmapImage(new Uri(@"Snippet\Class_16x.png", UriKind.Relative));
		private static readonly BitmapImage INTERFACE_IMAGE = new BitmapImage(new Uri(@"Snippet\Interface_16x.png", UriKind.Relative));
		private static readonly BitmapImage KEYWORD_IMAGE = new BitmapImage(new Uri(@"Snippet\KeywordSnippet_16x.png", UriKind.Relative));
		private static readonly BitmapImage METHOD_IMAGE = new BitmapImage(new Uri(@"Snippet\Method_16x.png", UriKind.Relative));
		private static readonly BitmapImage ENUM_IMAGE = new BitmapImage(new Uri(@"Snippet\Enumerator_16x.png", UriKind.Relative));
		private static readonly BitmapImage LOCAL_VARIABLE_IMAGE = new BitmapImage(new Uri(@"Snippet\LocalVariable_16x.png", UriKind.Relative));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is PopupControl.Token.TokenType)
				switch ((PopupControl.Token.TokenType)value)
				{
					case PopupControl.Token.TokenType.@class:
						return CLASS_IMAGE;
					case PopupControl.Token.TokenType.@enum:
						return ENUM_IMAGE;
					case PopupControl.Token.TokenType.keyword:
						return KEYWORD_IMAGE;
					case PopupControl.Token.TokenType.method:
						return METHOD_IMAGE;
					case PopupControl.Token.TokenType.variable:
						return LOCAL_VARIABLE_IMAGE;
				}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
