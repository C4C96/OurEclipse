using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeBoxControl.Decorations
{
	/// <summary>
	/// 文字装饰的模式，包含多个对文字的装饰
	/// </summary>
	[TypeConverter(typeof(DecorationSchemeTypeConverter))]
	public class DecorationScheme
	{
		public List<Decoration> BaseDecorations { get; set; } = new List<Decoration>();
		public string Name { get; set; }
	}
}
