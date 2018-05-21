using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBoxControl.Decorations
{
	/// <summary>
	/// 基于多个字符串的文字装饰
	/// </summary>
	public class MultiStringDecoration : Decoration
	{
		public List<string> Strings { get; set; }
	
		public StringComparison StringComparison { get; set; }

		public override List<Pair> Ranges(string text)
		{
			List<Pair> pairs = new List<Pair>();
			foreach (string word in Strings)
			{
				int index = text.IndexOf(word, 0, StringComparison);
				while (index != -1)
				{
					pairs.Add(new Pair(index, word.Length));
					index = text.IndexOf(word, index + word.Length, StringComparison);
				}
			}
			return pairs;
		}

		public override bool AreRangesSorted
		{
			get { return false; }
		}
	}
}
