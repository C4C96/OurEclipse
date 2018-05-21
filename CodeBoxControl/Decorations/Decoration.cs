using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CodeBoxControl.Decorations
{
	/// <summary>
	/// 对于文字装饰的抽象类
	/// </summary>
	public abstract class Decoration : DependencyObject
	{
		/// <summary>
		/// 装饰的类型，默认是TextColor
		/// </summary>
		public static DependencyProperty DecorationTypeProperty = DependencyProperty.Register("DecorationType", typeof(EDecorationType), typeof(Decoration),
	  new PropertyMetadata(EDecorationType.TextColor));

		public EDecorationType DecorationType
		{
			get { return (EDecorationType)GetValue(DecorationTypeProperty); }
			set { SetValue(DecorationTypeProperty, value); }
		}

		/// <summary>
		/// 使用的Brush
		/// </summary>
		public static DependencyProperty BrushProperty = DependencyProperty.Register("Brush", typeof(Brush), typeof(Decoration),
		new PropertyMetadata(null));

		public Brush Brush
		{
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}

		public abstract List<Pair> Ranges(string text);

		public abstract bool AreRangesSorted { get; }
		
		public bool IsDirty { get; protected set; }

		public Decoration()
		{
			IsDirty = true;
		}
	}
}
