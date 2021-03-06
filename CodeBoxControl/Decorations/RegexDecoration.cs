﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
namespace CodeBoxControl.Decorations
{
	/// <summary>
	/// 基于正则表达式的文字装饰
	/// </summary>
	public class RegexDecoration : Decoration
	{
		#region RegexString

		/// <summary>
		/// 用于匹配字符串的正则表达式
		/// </summary>
		public static DependencyProperty RegexStringProperty = DependencyProperty.Register("RegexString", typeof(String), typeof(RegexDecoration),
		new PropertyMetadata("", new PropertyChangedCallback(RegexDecoration.OnRegexStringChanged)));

		public String RegexString
		{
			get { return (String)GetValue(RegexStringProperty); }
			set { SetValue(RegexStringProperty, value); }
		}

		private static void OnRegexStringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (e.NewValue != e.OldValue)
			{
				RegexDecoration dObj = (RegexDecoration)d;
				dObj.IsDirty = true;
			}
		}

		#endregion

		public override List<Pair> Ranges(string text)
		{
			List<Pair> pairs = new List<Pair>();
			if (RegexString != "")
			{
				try
				{
					Regex rx = new Regex(RegexString);
					MatchCollection mc = rx.Matches(text);
					foreach (Match m in mc)
						if (m.Length > 0)
							pairs.Add(new Pair(m.Index, m.Length));
				}
				catch { }
			}
			IsDirty = false;
			return pairs;
		}

		public override bool AreRangesSorted
		{
			get { return true; }
		}
	}
}
