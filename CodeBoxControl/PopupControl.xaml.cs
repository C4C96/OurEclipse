using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CodeBoxControl
{
	/// <summary>
	/// PopupControl.xaml 的交互逻辑
	/// </summary>
	public partial class PopupControl : Popup
	{
		private List<string> tokens = new List<string>();
		public int Offset { get; set; }

		private string input = string.Empty;

		public string Input
		{
			get { return input; }
			set
			{
				input = value ?? string.Empty;
				if (string.IsNullOrEmpty(input)) return;
				int index = tokens.BinarySearch(input);
				if (index < 0)
				{
					index = ~index;
					if (index > tokens.Count || !IsPre(input, tokens[index]))
					{
						ListBox.SelectedIndex = -1;
						return;
					}
				}
				ListBox.SelectedIndex = index;
				ListBox.ScrollIntoView(ListBox.Items[index]);
			}
		}

		/// <summary>
		/// str1是否是str2的前缀
		/// </summary>
		/// <param name="str1"></param>
		/// <param name="str2"></param>
		/// <returns></returns>
		private bool IsPre(string str1, string str2)
		{
			if (str1 == null || str2 == null) return false;
 			if (str1.Length > str2.Length) return false;
			return str1 == str2.Substring(0, str1.Length);
		}

		private void ClearTokens()
		{
			tokens = Decorations.DecorationSchemes.JavaReservedWords();
		}

		public PopupControl()
		{
			InitializeComponent();
			ClearTokens();
			ListBox.ItemsSource = tokens;
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ListBox.SelectedIndex > 0)
				ScrollViewer.ScrollToVerticalOffset(ListBox.SelectedIndex * 15);
		}
	}
}
