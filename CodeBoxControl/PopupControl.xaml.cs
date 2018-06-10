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
		private List<Token> tokens = new List<Token>()
		{
			new Token("a", Token.TokenType.@class),
			new Token("aa", Token.TokenType.@class),
			new Token("aaa", Token.TokenType.keyword),
			new Token("aaaa", Token.TokenType.@enum),
			new Token("aaaaa", Token.TokenType.method),
			new Token("aaaaaa", Token.TokenType.@class),
			new Token("aaaaaaa", Token.TokenType.variable),
			new Token("aaaaaaaa", Token.TokenType.keyword),
			new Token("aaaaaaaaa", Token.TokenType.@class),
		};

		public int Offset { get; set; }

		private string input = string.Empty;

		public string Input
		{
			get { return input; }
			set
			{
				input = value ?? string.Empty;
				int index = tokens.BinarySearch(new Token(input, Token.TokenType.@class));
				if (index < 0)
					index = ~index - 1;
				ListBox.SelectedIndex = index;
				if (index > 0)
					ListBox.ScrollIntoView(ListBox.Items[index]);
			}
		}

		public PopupControl()
		{
			InitializeComponent();
			ListBox.ItemsSource = tokens;
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			foreach (Token selectedItem in e.AddedItems)
			{
				ListBoxItem selected = ListBox.ItemContainerGenerator.ContainerFromItem(selectedItem) as ListBoxItem;
				if (selected == null) continue;
				if (selected.ToolTip == null)
				{
					selected.ToolTip = new ToolTip()
					{
						Content = selectedItem.Tip,
						PlacementTarget = selected,
						Placement = PlacementMode.Right,
						StaysOpen = true,
					};
					ToolTipService.SetIsEnabled(selected, false); // 取消使能，只能被代码控制
					ToolTipService.SetShowDuration(selected, int.MaxValue);
				}
				ToolTip toolTip = selected.ToolTip as ToolTip;
				toolTip.IsOpen = true;
			}

			foreach (Token unselectedItem in e.RemovedItems)
			{
				ListBoxItem unselected = ListBox.ItemContainerGenerator.ContainerFromItem(unselectedItem) as ListBoxItem;
				if (unselected.ToolTip is ToolTip toolTip)
					toolTip.IsOpen = false;
			}
		}

		public class Token : IComparable<Token>
		{
			public string Name { get; set; }

			public TokenType Type { get; set; }

			public string Tip { get { return Type.ToString() + " " + Name; } }

			public Token(string name, TokenType type)
			{
				Name = name;
				Type = type;
			}

			public enum TokenType
			{
				method,
				@enum,
				@class,
				keyword,
				variable
			}

			public int CompareTo(Token other)
			{
				return this.Name.CompareTo(other.Name);
			}
		}
	}
}
