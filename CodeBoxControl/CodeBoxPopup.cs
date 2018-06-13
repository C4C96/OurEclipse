using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CodeBoxControl
{
	public partial class CodeBox
	{
		// textChange时是否需要处理
		public bool textChangedEnable = true;

		private PopupControl popup;

		private void InitPopup()
		{
			popup = new PopupControl
			{
				PlacementTarget = this
			};
			DataObject.AddPastingHandler(this, OnPaste);
		}

		// 是否是粘贴引起的文本改变
		private bool isPasted = false;
		private void OnPaste(object sender, DataObjectPastingEventArgs e)
		{
			isPasted = true;
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			if (!textChangedEnable) return;
			if (isPasted) { isPasted = false; return; }
			foreach (var change in e.Changes)
			{
				if (popup.IsOpen)
				{
					for (int i = 0; i < change.AddedLength; i++)
						if (!char.IsLetterOrDigit(Text, change.Offset + i))
						{
							popup.IsOpen = false;
							return;
						}
				}
				if (!popup.IsOpen
					&& change.AddedLength > 0
					&& (change.Offset <= 0
					|| !char.IsLetterOrDigit(Text, change.Offset - 1)))
				{
					popup.Input = string.Empty;
					popup.Offset = change.Offset;
					popup.IsOpen = true;
					popup.ListBox.SelectedIndex = 0;
					FixPopupPosition(change.Offset);
				}
			}
			if (popup.IsOpen)
			{
				int inputlast;
				for (inputlast = popup.Offset; inputlast < Text.Length && char.IsLetterOrDigit(Text[inputlast]); inputlast++) ;
				if (inputlast > popup.Offset)
					popup.Input = Text.Substring(popup.Offset, inputlast - popup.Offset);
				else
					popup.IsOpen = false;
			}
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (popup.IsOpen)
				switch (e.Key)
				{
					case Key.Up:
						if (popup.ListBox.SelectedIndex > 0)
							popup.ListBox.SelectedIndex--;
						e.Handled = true;
						break;
					case Key.Down:
						if (popup.ListBox.SelectedIndex < popup.ListBox.Items.Count - 1)
							popup.ListBox.SelectedIndex++;
						e.Handled = true;
						break;
					case Key.Enter:
						int inputlast;
						for (inputlast = popup.Offset; inputlast < Text.Length && char.IsLetterOrDigit(Text[inputlast]); inputlast++) ;
						popup.IsOpen = false;
						string token = (popup.ListBox.SelectedItem as PopupControl.Token).Name;
						textChangedEnable = false;
						Text = Text.Remove(popup.Offset, inputlast - popup.Offset)
									.Insert(popup.Offset, token);
						textChangedEnable = true;
						CaretIndex = popup.Offset + token.Length;
						e.Handled = true;
						break;
					case Key.Escape:
					case Key.LeftCtrl:
					case Key.RightCtrl:
						popup.IsOpen = false;
						break;
				}
			//else if (e.Key == Key.F1)
			//{
			//	popup.Input = string.Empty;
			//	popup.Offset = CaretIndex;
			//	popup.IsOpen = true;
			//	popup.ListBox.SelectedIndex = 0;
			//	FixPopupPosition(this.CaretIndex == 0 ? 0 : this.CaretIndex - 1);
			//}
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
