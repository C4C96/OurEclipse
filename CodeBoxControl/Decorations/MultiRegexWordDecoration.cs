using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CodeBoxControl.Decorations
{
	/// <summary>
	/// 基于断词后的正则匹配的文字装饰
	/// </summary>
	public class MultiRegexWordDecoration : Decoration
	{
		private List<string> mWords = new List<string>();
		public List<string> Words
		{
			get { return mWords; }
			set { mWords = value; }
		}
		public bool IsCaseSensitive { get; set; }

		public override List<Pair> Ranges(string text)
		{
			List<Pair> pairs = new List<Pair>();
			foreach (string word in mWords)
			{
				string rstring;
				if (IsCaseSensitive)
					rstring = @"\b" + word + @"\b";
				else
					rstring = @"(?i:\b" + word + @"\b)";
				Regex rx = new Regex(rstring);
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
