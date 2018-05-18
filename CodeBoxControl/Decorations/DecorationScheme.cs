using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CodeBoxControl.Decorations
{
	[TypeConverter(typeof(DecorationSchemeTypeConverter))]
	public class DecorationScheme
	{
		public List<Decoration> BaseDecorations { get; set; } = new List<Decoration>();
		public string Name { get; set; }
	}
}
