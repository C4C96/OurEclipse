using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeBoxControl.Decorations
{
	public class PairDecoration : Decoration
	{
		public List<Pair> Pairs { get; set; } = new List<Pair>();

		public override bool AreRangesSorted
		{
			get
			{
				return false;
			}
		}

		public override List<Pair> Ranges(string text)
		{
			return Pairs;
		}
	}
}
