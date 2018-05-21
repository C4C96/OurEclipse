using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeBoxControl
{
	/// <summary>
	/// 表示一片字符的范围，包含开始位置与长度
	/// </summary>
	public class Pair
	{
		/// <summary>
		/// 字符的开始位置，从0开始计数
		/// </summary>
		public int Start { get; set; }

		/// <summary>
		/// 字符的长度
		/// </summary>
		public int Length { get; set; }

		public Pair() { }

		public Pair(int start, int length)
		{
			Start = start;
			Length = length;
		}

		public int End
		{
			get { return Start + Length; }
		}
	}
}
