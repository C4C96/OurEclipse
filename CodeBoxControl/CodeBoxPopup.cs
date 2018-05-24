using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace CodeBoxControl
{
	public partial class CodeBox
	{
		private Popup popup;
		private ListBox listBox;

		private void InitPopup()
		{
			listBox = new ListBox()
			{
				Width = 200,
				MaxHeight = 120,
			};
			listBox.Items.Add("a");
			listBox.Items.Add("aa");
			listBox.Items.Add("aaa");
			listBox.Items.Add("aaaa");
			listBox.Items.Add("aaaaa");
			listBox.Items.Add("aaaaaa");
			listBox.Items.Add("aaaaaaa");
			listBox.Items.Add("aaaaaaaa");

			popup = new Popup()
			{
				StaysOpen = false,
				Child = listBox,
				PlacementTarget = this,
			};

			this.TextChanged += (s, e) =>
			{
				if (!popup.IsOpen
					&& (this.CaretIndex <= 1
					|| !char.IsLetter(this.Text[this.CaretIndex - 2]))) // TODO Popup弹出条件，暂时设成前一个字符不为字母
				{
					popup.IsOpen = true;
					FixPopupPosition(this.CaretIndex == 0 ? 0 : this.CaretIndex - 1);
				}
			};
		}

		private void FixPopupPosition(int charIndex)
		{
			int firstLine = GetFirstVisibleLineIndex();
			int lastLine = GetLastVisibleLineIndex();
			int firstChar = (firstLine == 0) ? 0 : GetCharacterIndexFromLineIndex(firstLine);
			int lastChar = GetCharacterIndexFromLineIndex(lastLine) + GetLineLength(lastLine);

			if (charIndex >= firstChar && charIndex <= lastChar)
			{
				Rect rect = GetRectFromCharacterIndex(charIndex);
				popup.PlacementRectangle = new Rect(rect.X, rect.Y + rect.Height, popup.ActualWidth, popup.ActualHeight);
			}
			else
				popup.IsOpen = false;
		}
	}
}
