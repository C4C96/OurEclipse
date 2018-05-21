using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeBoxControl.Decorations
{
	public class MultiRegexDecoration : Decoration
	{
		private List<string> mRegexStrings = new List<string>();

		public List<string> RegexStrings
		{
			get { return mRegexStrings; }
			set { mRegexStrings = value; }
		}

		public override List<Pair> Ranges(string text)
		{
			List<Pair> pairs = new List<Pair>();
			foreach (string rString in mRegexStrings)
			{

				Regex rx = new Regex(rString);
				MatchCollection mc = rx.Matches(text);
				foreach (Match m in mc)
					pairs.Add(new Pair(m.Index, m.Length));
			}
			return pairs;
		}

		public override bool AreRangesSorted
		{
			get { return false; }
		}
	}
}
