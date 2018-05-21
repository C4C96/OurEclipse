using CodeBoxControl.Decorations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CodeBoxControl
{
	public partial class CodeBox : TextBox
	{
		/// <summary>
		/// 用于失败时重新渲染的Timer
		/// </summary>
		private System.Windows.Threading.DispatcherTimer renderTimer = new System.Windows.Threading.DispatcherTimer
		{
			IsEnabled = false,
			Interval = TimeSpan.FromMilliseconds(50)
		};

		/// <summary>
		/// 用于缓存渲染信息
		/// </summary>
		private CodeBoxRenderInfo renderinfo = new CodeBoxRenderInfo();

		public CodeBox()
		{
			this.TextChanged += (s, e) => InvalidateVisual();
			this.SelectionChanged += (s, e) => InvalidateVisual();			
			renderTimer.Tick += (s, e) =>
			{
				renderTimer.IsEnabled = false;
				this.InvalidateVisual();
			};
			InitializeComponent();
		}

		#region DependencyProperty

		public static DependencyProperty BaseForegroundProperty = DependencyProperty.Register("BaseForeground", typeof(Brush), typeof(CodeBox),
			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

		[Bindable(true)]
		public Brush BaseForeground
		{
			get { return (Brush)GetValue(BaseForegroundProperty); }
			set { SetValue(BaseForegroundProperty, value); }
		}

		public static DependencyProperty CodeBoxBackgroundProperty = DependencyProperty.Register("CodeBoxBackground", typeof(Brush), typeof(CodeBox),
			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));

		[Bindable(true)]
		public Brush CodeBoxBackground
		{
			get { return (Brush)GetValue(CodeBoxBackgroundProperty); }
			set { SetValue(CodeBoxBackgroundProperty, value); }
		}

		public static DependencyProperty CaretLineForegroundProperty = DependencyProperty.Register("CaretLineForeground", typeof(Brush), typeof(CodeBox),
			new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(220, 220, 220)), FrameworkPropertyMetadataOptions.AffectsRender));

		[Bindable(true)]
		public Brush CaretLineForeground
		{
			get { return (Brush)GetValue(CaretLineForegroundProperty); }
			set { SetValue(CaretLineForegroundProperty, value); }
		}

		#region LineNumber Properties

		public static DependencyProperty ShowLineNumbersProperty = DependencyProperty.Register("ShowLineNumbers", typeof(bool), typeof(CodeBox),
			new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
		[Category("LineNumbers")]
		public bool ShowLineNumbers
		{
			get { return (bool)GetValue(ShowLineNumbersProperty); }
			set { SetValue(ShowLineNumbersProperty, value); }
		}

		public static DependencyProperty LineNumberForegroundProperty = DependencyProperty.Register("LineNumberForeground", typeof(Brush), typeof(CodeBox),
			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Gray), FrameworkPropertyMetadataOptions.AffectsRender));
		[Category("LineNumbers")]
		public Brush LineNumberForeground
		{
			get { return (Brush)GetValue(LineNumberForegroundProperty); }
			set { SetValue(LineNumberForegroundProperty, value); }
		}


		public static DependencyProperty LineNumberMarginWidthProperty = DependencyProperty.Register("LineNumberMarginWidth", typeof(double), typeof(CodeBox),
			new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.AffectsRender));
		[Category("LineNumbers")]
		public double LineNumberMarginWidth
		{
			get { return (Double)GetValue(LineNumberMarginWidthProperty); }
			set { SetValue(LineNumberMarginWidthProperty, value); }
		}

		public static DependencyProperty StartingLineNumberProperty = DependencyProperty.Register("StartingLineNumber", typeof(int), typeof(CodeBox),
			new FrameworkPropertyMetadata(1, FrameworkPropertyMetadataOptions.AffectsRender));
		[Category("LineNumbers")]
		public int StartingLineNumber
		{
			get { return (int)GetValue(StartingLineNumberProperty); }
			set { SetValue(StartingLineNumberProperty, value); }
		}

		#endregion

		public static DependencyProperty DecorationSchemeProperty = DependencyProperty.Register("DecorationScheme", typeof(DecorationScheme), typeof(CodeBox),
	new FrameworkPropertyMetadata(new DecorationScheme(), FrameworkPropertyMetadataOptions.AffectsRender));

		/// <summary>
		/// 用于CodeBox的文字装饰模式
		/// </summary>
		public DecorationScheme DecorationScheme
		{
			get { return (DecorationScheme)GetValue(DecorationSchemeProperty); }
			set { SetValue(DecorationSchemeProperty, value); }
		}

		#endregion
		
		#region OnRender

		private List<Decoration> mDecorations = new List<Decoration>();

		/// <summary>
		/// 外部可编辑的文字装饰
		/// </summary>
		public List<Decoration> Decorations
		{
			get { return mDecorations; }
			set { mDecorations = value; }
		}

		private FormattedText formattedText;

		/// <summary>
		/// 重载OnRender
		/// </summary>
		/// <param name="drawingContext"></param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			EnsureScrolling();
			base.OnRender(drawingContext);

			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
				OnRenderDesigner(drawingContext);
			else
			{
				if (this.LineCount == 0)
				{
					ReRenderLastRuntimeRender(drawingContext);
					renderTimer.IsEnabled = true;
				}
				else
					OnRenderRuntime(drawingContext);
			}
		}

		/// <summary>
		/// 主要渲染逻辑
		/// </summary>
		/// <param name="drawingContext"></param>
		protected void OnRenderRuntime(DrawingContext drawingContext)
		{
			drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight))); // 限定绘制区域
			drawingContext.DrawRectangle(CodeBoxBackground, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight)); // 绘制背景

			// 绘制光标所在行的背景
			Rect caretLineRect = GetRectFromCharacterIndex(this.CaretIndex);
			caretLineRect.X = LineNumberMarginWidth;
			caretLineRect.Width = this.ActualWidth;
			if (this.SelectionLength == 0)
				drawingContext.DrawRectangle(null, new Pen(CaretLineForeground, 2), caretLineRect);
			

			int firstLine = GetFirstVisibleLineIndex();
			int firstChar = (firstLine == 0) ? 0 : GetCharacterIndexFromLineIndex(firstLine);
			Point renderPoint = GetRenderPoint(firstChar);

			// 绘制行号
			if (ShowLineNumbers && this.LineNumberMarginWidth > 0)
			{
				if (this.GetLastVisibleLineIndex() != -1)
				{
					FormattedText lineNumbers = GenerateLineNumbers();
					if (lineNumbers.Width + 6 > LineNumberMarginWidth)
						LineNumberMarginWidth = lineNumbers.Width + 6;
					drawingContext.DrawText(lineNumbers, new Point(3, renderPoint.Y));
					renderinfo.LineNumbers = lineNumbers;
				}
				else
					drawingContext.DrawText(renderinfo.LineNumbers, new Point(3, renderPoint.Y));
			}

			if (this.Text == "") return;

			string visibleText = VisibleText;
			if (visibleText == null) return;

			formattedText = new FormattedText(
					visibleText,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(this.FontFamily.Source),
					this.FontSize,
					BaseForeground);
			formattedText.Trimming = TextTrimming.None;
			
			ApplyTextWrapping(formattedText);

			Pair visiblePair = new Pair(firstChar, visibleText.Length);			

			//Generates the prepared decorations for the BaseDecorations
			Dictionary<EDecorationType, Dictionary<Decoration, List<Geometry>>> basePreparedDecorations
				= GeneratePreparedDecorations(visiblePair, DecorationScheme.BaseDecorations);
			//Displays the prepared decorations for the BaseDecorations
			DisplayPreparedDecorations(drawingContext, basePreparedDecorations, renderPoint);

			//Generates the prepared decorations for the Decorations
			Dictionary<EDecorationType, Dictionary<Decoration, List<Geometry>>> preparedDecorations
				= GeneratePreparedDecorations(visiblePair, mDecorations);
			//Displays the prepared decorations for the Decorations
			DisplayPreparedDecorations(drawingContext, preparedDecorations, renderPoint);

			// 文本着色
			ColorText(firstChar, DecorationScheme.BaseDecorations);
			ColorText(firstChar, mDecorations);
			drawingContext.DrawText(formattedText, renderPoint);

			// 缓存渲染信息
			renderinfo.BoxText = formattedText;
			renderinfo.CaretLineRect = caretLineRect;
			renderinfo.BasePreparedDecorations = basePreparedDecorations;
			renderinfo.PreparedDecorations = preparedDecorations;
		}

		/// <summary>
		/// 设计模式下的渲染
		/// </summary>
		/// <param name="drawingContext"></param>
		protected void OnRenderDesigner(DrawingContext drawingContext)
		{
			int firstChar = 0;

			Double leftMargin = 4.0 + this.BorderThickness.Left;
			Double topMargin = 2.0 + this.BorderThickness.Top;

			string visibleText = VisibleText;

			formattedText = new FormattedText(
				   this.Text,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(this.FontFamily.Source),
					this.FontSize,
					BaseForeground);
			formattedText.Trimming = TextTrimming.None;

			string lineNumberString = "1\n2\n3\n";
			FormattedText lineNumbers = new FormattedText(
				  lineNumberString,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(this.FontFamily.Source),
					this.FontSize,
					LineNumberForeground);
			
			Pair visiblePair = new Pair(firstChar, Text.Length);

			drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight)));
			Point renderPoint = new Point(this.LineNumberMarginWidth + leftMargin, topMargin);

			drawingContext.DrawRectangle(CodeBoxBackground, null, new Rect(0, 0, this.ActualWidth, this.ActualHeight));

			Dictionary<Decoration, List<Geometry>> hilightgeometryDictionary = PrepareGeometries(visiblePair, formattedText, mDecorations, EDecorationType.Hilight, HilightGeometryMaker);
			DisplayGeometry(drawingContext, hilightgeometryDictionary, renderPoint);

			Dictionary<Decoration, List<Geometry>> strikethroughGeometryDictionary = PrepareGeometries(visiblePair, formattedText, mDecorations, EDecorationType.Strikethrough, StrikethroughGeometryMaker);
			DisplayGeometry(drawingContext, strikethroughGeometryDictionary, renderPoint);

			Dictionary<Decoration, List<Geometry>> underlineGeometryDictionary = PrepareGeometries(visiblePair, formattedText, mDecorations, EDecorationType.Underline, UnderlineGeometryMaker);
			DisplayGeometry(drawingContext, underlineGeometryDictionary, renderPoint);
			ColorText(firstChar, mDecorations);
			if (!ShowLineNumbers)
			{
				drawingContext.DrawText(lineNumbers, new Point(3, renderPoint.Y));
			}
			drawingContext.DrawText(formattedText, renderPoint);

		}

		/// <summary>
		/// 根据缓存信息来渲染
		/// </summary>
		/// <param name="drawingContext"></param>
		protected void ReRenderLastRuntimeRender(DrawingContext drawingContext)
		{
			drawingContext.DrawText(renderinfo.BoxText, renderinfo.RenderPoint);
			DisplayPreparedDecorations(drawingContext, renderinfo.PreparedDecorations, renderinfo.RenderPoint);
			DisplayPreparedDecorations(drawingContext, renderinfo.BasePreparedDecorations, renderinfo.RenderPoint);
			if (this.SelectionLength == 0)
				drawingContext.DrawRectangle(null, new Pen(CaretLineForeground, 2), renderinfo.CaretLineRect);
			if(ShowLineNumbers && this.LineNumberMarginWidth > 0)
				drawingContext.DrawText(renderinfo.LineNumbers, new Point(3, renderinfo.RenderPoint.Y));
		}

		/// <summary>
		/// 实现EDecorationType.TextColor类型的文本装饰
		/// </summary>
		/// <param name="firstChar"></param>
		/// <param name="decorations"></param>
		private void ColorText(int firstChar, List<Decoration> decorations)
		{
			if (decorations != null)
				foreach (Decoration dec in decorations.Where(d => d.DecorationType == EDecorationType.TextColor))
					foreach (Pair p in dec.Ranges(this.Text))
						if (p.End > firstChar && p.Start < firstChar + formattedText.Text.Length)
						{
							int adjustedStart = Math.Max(p.Start - firstChar, 0);
							int adjustedLength = Math.Min(p.Length + Math.Min(p.Start - firstChar, 0), formattedText.Text.Length - adjustedStart);
							formattedText.SetForegroundBrush(dec.Brush, adjustedStart, adjustedLength);
						}
		}

		public void ApplyTextWrapping(FormattedText formattedText)
		{
			switch (this.TextWrapping)
			{
				case TextWrapping.NoWrap:
					break;
				case TextWrapping.Wrap:
					formattedText.MaxTextWidth = this.ViewportWidth; // 仅用于Wrap
					break;
				case TextWrapping.WrapWithOverflow:
					formattedText.SetMaxTextWidths(VisibleLineWidthsIncludingTrailingWhitespace());
					break;
			}

		}

		/// <summary>
		/// Displays the Decorations for a List of Decorations
		/// </summary>
		/// <param name="drawingContext">The drawing Context from the OnRender</param>
		/// <param name="visiblePair">The pair representing the first character of the Visible text with respect to the whole text</param>
		/// <param name="renderPoint">The Point representing the offset from (0,0) for rendering</param>
		/// <param name="decorations">The List of Decorations</param>
		private void DisplayDecorations(DrawingContext drawingContext, Pair visiblePair, Point renderPoint, List<Decoration> decorations)
		{
			Dictionary<Decoration, List<Geometry>> hilightgeometryDictionary = PrepareGeometries(visiblePair, formattedText, decorations, EDecorationType.Hilight, HilightGeometryMaker);
			DisplayGeometry(drawingContext, hilightgeometryDictionary, renderPoint);

			Dictionary<Decoration, List<Geometry>> strikethroughGeometryDictionary = PrepareGeometries(visiblePair, formattedText, decorations, EDecorationType.Strikethrough, StrikethroughGeometryMaker);
			DisplayGeometry(drawingContext, strikethroughGeometryDictionary, renderPoint);

			Dictionary<Decoration, List<Geometry>> underlineGeometryDictionary = PrepareGeometries(visiblePair, formattedText, decorations, EDecorationType.Underline, UnderlineGeometryMaker);
			DisplayGeometry(drawingContext, underlineGeometryDictionary, renderPoint);

		}

		/// <summary>
		/// The first part of the split version of Display decorations.
		/// </summary>
		/// <param name="visiblePair">The pair representing the first character of the Visible text with respect to the whole text</param>
		/// <param name="decorations">The List of Decorations</param>
		/// <returns></returns>
		private Dictionary<EDecorationType, Dictionary<Decoration, List<Geometry>>> GeneratePreparedDecorations(Pair visiblePair, List<Decoration> decorations)
		{
			Dictionary<EDecorationType, Dictionary<Decoration, List<Geometry>>> preparedDecorations = new Dictionary<EDecorationType, Dictionary<Decoration, List<Geometry>>>();
			Dictionary<Decoration, List<Geometry>> hilightgeometryDictionary = PrepareGeometries(visiblePair, formattedText, decorations, EDecorationType.Hilight, HilightGeometryMaker);
			preparedDecorations.Add(EDecorationType.Hilight, hilightgeometryDictionary);
			Dictionary<Decoration, List<Geometry>> strikethroughGeometryDictionary = PrepareGeometries(visiblePair, formattedText, decorations, EDecorationType.Strikethrough, StrikethroughGeometryMaker);
			preparedDecorations.Add(EDecorationType.Strikethrough, strikethroughGeometryDictionary);
			Dictionary<Decoration, List<Geometry>> underlineGeometryDictionary = PrepareGeometries(visiblePair, formattedText, decorations, EDecorationType.Underline, UnderlineGeometryMaker);
			preparedDecorations.Add(EDecorationType.Underline, underlineGeometryDictionary);
			return preparedDecorations;
		}

		/// <summary>
		/// The second half of the  DisplayDecorations.
		/// </summary>
		/// <param name="drawingContext">The drawing Context from the OnRender</param>
		/// <param name="preparedDecorations">The previously prepared decorations</param>
		/// <param name="renderPoint">The Point representing the offset from (0,0) for rendering</param>
		private void DisplayPreparedDecorations(DrawingContext drawingContext, Dictionary<EDecorationType, Dictionary<Decoration, List<Geometry>>> preparedDecorations, Point renderPoint)
		{
			DisplayGeometry(drawingContext, preparedDecorations[EDecorationType.Hilight], renderPoint);
			DisplayGeometry(drawingContext, preparedDecorations[EDecorationType.Strikethrough], renderPoint);
			DisplayGeometry(drawingContext, preparedDecorations[EDecorationType.Underline], renderPoint);
		}

		#endregion

		/// <summary>
		/// 获取RenderPoint，即第一个字符的左上角的点
		/// 可能因textbox的滚动而产生负值
		/// </summary>
		/// <param name="firstChar">第一个可见的字符位置</param>
		/// <returns></returns>
		private Point GetRenderPoint(int firstChar)
		{
			try
			{
				Rect cRect = GetRectFromCharacterIndex(firstChar);
				Point renderPoint = new Point(cRect.Left, cRect.Top);
				if (!double.IsInfinity(cRect.Top))
					renderinfo.RenderPoint = renderPoint;
				else
					this.renderTimer.IsEnabled = true;
				return renderinfo.RenderPoint;
			}
			catch
			{
				this.renderTimer.IsEnabled = true;
				return renderinfo.RenderPoint;
			}
		}

		private void DisplayGeometry(DrawingContext drawingContext, Dictionary<Decoration, List<Geometry>> geometryDictionary, Point renderPoint)
		{
			foreach (Decoration dec in geometryDictionary.Keys)
			{
				List<Geometry> GeomList = geometryDictionary[dec];
				foreach (Geometry g in GeomList)
				{
					g.Transform = new System.Windows.Media.TranslateTransform(renderPoint.X, renderPoint.Y);
					drawingContext.DrawGeometry(dec.Brush, null, g);
				}
			}
		}

		private Dictionary<Decoration, List<Geometry>> PrepareGeometries(Pair pair, FormattedText visibleFormattedText, List<Decoration> decorations, EDecorationType decorationType, GeometryMaker gMaker)
		{
			Dictionary<Decoration, List<Geometry>> geometryDictionary = new Dictionary<Decoration, List<Geometry>>();
			foreach (Decoration dec in decorations.Where(d=>d.DecorationType == decorationType))
			{
				List<Geometry> geomList = new List<Geometry>();
				List<Pair> ranges = dec.Ranges(this.Text);
				foreach (Pair p in ranges)
					if (p.End > pair.Start && p.Start < pair.Start + VisibleText.Length)
					{
						int adjustedStart = Math.Max(p.Start - pair.Start, 0);
						int adjustedLength = Math.Min(p.Length + Math.Min(p.Start - pair.Start, 0), pair.Length - adjustedStart);
						Geometry geom = gMaker(visibleFormattedText, new Pair(adjustedStart, adjustedLength));
						geomList.Add(geom);
					}
				geometryDictionary.Add(dec, geomList);
			}
			return geometryDictionary;
		}

		/// <summary>
		/// Delegate used with the PrepareGeomeries method.
		/// </summary>
		/// <param name="text">The FormattedText used for the decoration</param>
		/// <param name="p">The pair defining the begining character and the length of the character range</param>
		/// <returns></returns>
		private delegate Geometry GeometryMaker(FormattedText text, Pair p);

		/// <summary>
		/// 实现了Hilight类型的文本装饰
		/// </summary>
		/// <param name="text">待装饰的文本</param>
		/// <param name="p">待装饰的区域</param>
		/// <returns></returns>
		private Geometry HilightGeometryMaker(FormattedText text, Pair p)
		{
			return text.BuildHighlightGeometry(new Point(0, 0), p.Start, p.Length);
		}

		/// <summary>
		/// 实现了Underline类型的文本装饰
		/// </summary>
		/// <param name="text">待装饰的文本</param>
		/// <param name="p">待装饰的区域</param>
		/// <returns></returns>
		private Geometry UnderlineGeometryMaker(FormattedText text, Pair p)
		{
			Geometry geom = text.BuildHighlightGeometry(new Point(0, 0), p.Start, p.Length);
			if (geom != null)
			{
				StackedRectangleGeometryHelper srgh = new StackedRectangleGeometryHelper(geom);
				return srgh.BottomEdgeRectangleGeometry();
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 实现了Strikethrough类型的文本装饰.
		/// </summary>
		/// <param name="text">待装饰的文本</param>
		/// <param name="p">待装饰的区域</param>
		/// <returns></returns>
		private Geometry StrikethroughGeometryMaker(FormattedText text, Pair p)
		{
			Geometry geom = text.BuildHighlightGeometry(new Point(0, 0), p.Start, p.Length);
			if (geom != null)
			{
				StackedRectangleGeometryHelper srgh = new StackedRectangleGeometryHelper(geom);
				return srgh.CenterLineRectangleGeometry();
			}
			else
				return null;
		}

		#region Ensure Scrolling

		private bool mScrollingEventEnabled;

		/// <summary>
		/// 使得ScrollViewr能监听到
		/// </summary>
		private void EnsureScrolling()
		{
			if (!mScrollingEventEnabled)
			{
				try
				{
					DependencyObject dp = VisualTreeHelper.GetChild(this, 0);
					dp = VisualTreeHelper.GetChild(dp, 0);
					ScrollViewer sv = VisualTreeHelper.GetChild(dp, 0) as ScrollViewer;
					sv.ScrollChanged += (s, e) => this.InvalidateVisual();
					mScrollingEventEnabled = true;
				}
				catch { }
			}
		}

		#endregion

		/// <summary>
		/// 获取Textbox中可见的文本
		/// </summary>
		private string VisibleText
		{
			get
			{
				if (this.Text == "") { return ""; }
				string visibleText = "";
				try
				{
					int textLength = Text.Length;
					int firstLine = GetFirstVisibleLineIndex();
					int lastLine = GetLastVisibleLineIndex();

					int lineCount = this.LineCount;
					int firstChar = (firstLine == 0) ? 0 : GetCharacterIndexFromLineIndex(firstLine);

					int lastChar = GetCharacterIndexFromLineIndex(lastLine) + GetLineLength(lastLine) - 1;
					int length = lastChar - firstChar + 1;
					int maxlenght = textLength - firstChar;
					string text = Text.Substring(firstChar, Math.Min(maxlenght, length));
					if (text != null)
					{
						visibleText = text;
					}
				}
				catch
				{
					Debug.WriteLine("GetVisibleText failure");
				}
				return visibleText;
			}
		}

		/// <summary>
		/// wrap with overflow情况下，可见的每行的宽度
		/// </summary>
		/// <returns></returns>
		private Double[] VisibleLineWidthsIncludingTrailingWhitespace()
		{
			int firstLine = this.GetFirstVisibleLineIndex();
			int lastLine = Math.Max(this.GetLastVisibleLineIndex(), firstLine);
			Double[] lineWidths = new Double[lastLine - firstLine + 1];
			for (int i = firstLine; i <= lastLine; i++)
			{
				string lineString = this.GetLineText(i);
				lineWidths[i - firstLine] = MeasureString(lineString);
			}
			return lineWidths;
		}

		/// <summary>
		/// 返回当前字号字体下，一行文本的宽度
		/// 仅用于wrap with overflow.
		/// </summary>
		/// <param name="str">代测量的文本</param>
		/// <returns></returns>
		private double MeasureString(string str)
		{
			FormattedText formattedText = new FormattedText(
					str,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(this.FontFamily.Source),
					this.FontSize,
					new SolidColorBrush(Colors.Black));
			//if (str == "")
			//	return formattedText.WidthIncludingTrailingWhitespace;
			//else if (str.Substring(0, 1) == "\t")
			//	return formattedText.WidthIncludingTrailingWhitespace;
			//else
			//	return formattedText.WidthIncludingTrailingWhitespace;
			return formattedText.WidthIncludingTrailingWhitespace;
		}

		#region line number calculations

		/// <summary>
		/// 生成显示行号的FormattedText
		/// </summary>
		/// <returns></returns>
		private FormattedText GenerateLineNumbers()
		{
			switch (this.TextWrapping)
			{
				case TextWrapping.NoWrap:
					return LineNumberWithoutWrap();
				case TextWrapping.Wrap:
					return LineNumberWithWrap();
				case TextWrapping.WrapWithOverflow:
					return LineNumberWithWrap();
			}
			return null;
		}

		/// <summary>
		/// TextWrapping = None时生成行号
		/// </summary>
		/// <returns></returns>
		private FormattedText LineNumberWithoutWrap()
		{
			int firstLine = GetFirstVisibleLineIndex();
			int lastLine = GetLastVisibleLineIndex();
			StringBuilder sb = new StringBuilder();
			for (int i = firstLine; i <= lastLine; i++)
				sb.Append((i + StartingLineNumber) + "\n");
			string lineNumberString = sb.ToString();
			FormattedText lineNumbers = new FormattedText(
				  lineNumberString,
					CultureInfo.GetCultureInfo("en-us"),
					FlowDirection.LeftToRight,
					new Typeface(this.FontFamily.Source),
					this.FontSize,
					LineNumberForeground);
			return lineNumbers;
		}

		/// <summary>
		/// TextWrapping = Wrap or WrapWithOverflow时显示行号
		/// </summary>
		/// <returns></returns>
		private FormattedText LineNumberWithWrap()
		{
			try
			{
				int[] linePos = MinLineStartCharcterPositions();
				int[] lineStart = VisibleLineStartCharcterPositions();
				if (lineStart != null)
				{
					string lineNumberString = LineNumbers(lineStart, linePos);
					FormattedText lineText = new FormattedText(
						  lineNumberString,
							CultureInfo.GetCultureInfo("en-us"),
							FlowDirection.LeftToRight,
							new Typeface(this.FontFamily.Source),
							this.FontSize,
							LineNumberForeground);
					renderinfo.LineNumbers = lineText;
					return lineText;
				}
				else
					return renderinfo.LineNumbers;
			}
			catch
			{
				return renderinfo.LineNumbers;
			}
		}

		/// <summary>
		/// Returns the character positions that start lines as determined only by the characters.
		/// </summary>
		/// <returns></returns>
		private int[] MinLineStartCharcterPositions()
		{
			int totalChars = this.Text.Length;
			char[] boxChars = this.Text.ToCharArray();
			char newlineChar = Convert.ToChar("\n");
			char returnChar = Convert.ToChar("\r");
			char formfeed = Convert.ToChar("\f");
			char vertQuote = Convert.ToChar("\v");

			List<int> breakChars = new List<int>() { 0 };

			//This looks a bit exotic but keep in mind that \r\n or \r or \n or \f or \v all will 
			//signify a new line to the textbox.
			if (boxChars.Length > 1)
			{
				for (int i = 0; i < boxChars.Length; i++)
					if (boxChars[i] == newlineChar)
						breakChars.Add(i + 1);
				//for (int i = 2; i < boxChars.Length; i++)
				//{
				//	if (boxChars[i - 1] == returnChar && boxChars[i - 2] == newlineChar)
				//	{
				//		breakChars.Add(i);
				//	}
				//	if (boxChars[i - 1] == newlineChar && boxChars[i] != returnChar)
				//	{
				//		breakChars.Add(i);
				//	}
				//	if (boxChars[i - 1] == formfeed || boxChars[i - 1] == vertQuote)
				//	{
				//		breakChars.Add(i);
				//	}
				//}
			}
			int[] MinPositions = new int[breakChars.Count];
			breakChars.CopyTo(MinPositions);
			return MinPositions;
		}

		/// <summary>
		/// Returns the character positions that the textbox declares to begin the 
		/// visible lines.
		/// </summary>
		/// <returns></returns>
		private int[] VisibleLineStartCharcterPositions()
		{
			int firstLine = GetFirstVisibleLineIndex();
			int lastLine = GetLastVisibleLineIndex();
			if (lastLine != -1)
			{
				int lineCount = lastLine - firstLine + 1;
				int[] startingPositions = new int[lineCount];
				for (int i = firstLine; i <= lastLine; i++)
				{
					int startPos = this.GetCharacterIndexFromLineIndex(i);
					startingPositions[i - firstLine] = startPos;
				}
				return startingPositions;
			}
			else
				return null;
		}

		/// <summary>
		/// Create the String of line numbers. Uses merge algorithm http://en.wikipedia.org/wiki/Merge_algorithm
		/// </summary>
		/// <param name="listA">The List of the first characters of the visible lines. This is affected by box width.</param>
		/// <param name="listB">The List of First Characters of the Lines determined by characters rather than  box width.</param>
		/// <returns></returns>
		private string LineNumbers(int[] listA, int[] listB)
		{
			StringBuilder sb = new StringBuilder();
			int a = 0;
			int b = 0;
			List<int> matches = new List<int>();
			List<int> skipped = new List<int>();
			while (a < listA.Length && b < listB.Length)
			{
				if (listA[a] == listB[b])
				{
					matches.Add(b);
					a++;
					b++;
				}
				else if (listA[a] < listB[b])
				{
					matches.Add(-1);
					a++;
				}
				else
				{
					skipped.Add(b);
					b++;
				}
			}
			while (a < listA.Length) a++;

			while (b < listB.Length) b++;

			//There will be missing lien numbers where the lines are blank.
			//The skipped lines are returned.

			// in reverse because ther could be more than one sequential blank line.
			for (int i = (skipped.Count - 1); i >= 0; i--)
			{
				// index  is the position directly before the index in the matches array of
				//one greaer than the missing elements. 
				int index = matches.IndexOf(skipped[i] + 1) - 1;
				if (index > -1)
					matches[index] = skipped[i];
			}

			//Adjusts the line numbers so that line 0 has the value of StartingLineNumber
			for (int i = 0; i < matches.Count; i++)
				if (matches[i] != -1)
					matches[i] += this.StartingLineNumber;

			StringBuilder sb2 = new StringBuilder();
			foreach (int i in matches)
				if (i == -1)
					sb2.Append("\n");
				else
					sb2.Append(i + "\n");
			return sb2.ToString();
		}

		#endregion

	}
}

