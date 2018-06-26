using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace OurEclipse
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			// 更新状态栏
			CodeBox.SelectionChanged += (s, e) =>
			{
				int lineIndex = CodeBox.GetLineIndexFromCharacterIndex(CodeBox.CaretIndex);
				LineIndexSBI.Content = lineIndex + 1;
				ColIndexSBI.Content = CodeBox.CaretIndex - CodeBox.GetCharacterIndexFromLineIndex(lineIndex) + 1;
				SelectionLengthSBI.Content = CodeBox.SelectionLength;
			};
			CodeBox.DecorationScheme = CodeBoxControl.Decorations.DecorationSchemes.Java;
		}
		
		private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (e.Command == ApplicationCommands.Undo && CodeBox != null)
			{
				e.CanExecute = CodeBox.CanUndo;
				e.Handled = true;
			}
			else if (e.Command == ApplicationCommands.Redo && CodeBox != null)
			{
				e.CanExecute = CodeBox.CanRedo;
				e.Handled = true;
			}
			else //if (e.Command == ApplicationCommands.New
			//		|| e.Command == ApplicationCommands.Open
			//		|| e.Command == ApplicationCommands.Save
			//		|| e.Command == ApplicationCommands.SaveAs)
			{
				e.CanExecute = true;
				e.Handled = true;
			}
		}

		private void CommandExecute(object sender, ExecutedRoutedEventArgs e)
		{
			e.Handled = true;
			if (e.Command == ApplicationCommands.New)
				New();
			else if (e.Command == ApplicationCommands.Open)
				Open();
			else if (e.Command == ApplicationCommands.Save)
				Save();
			else if (e.Command == ApplicationCommands.SaveAs)
				SaveAs();
			else if (e.Command == ApplicationCommands.Close)
				Close();
			else if (e.Command == TryFindResource("Comment"))
				Comment();
			else if (e.Command == TryFindResource("Uncomment"))
				Uncomment();
			else if (e.Command == TryFindResource("Run"))
				RunAsync();
			else
				e.Handled = false;
		}

		private void Button_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender == UndoButton)
			{
				if (UndoButton.Content is Image image)
					image.Source = new BitmapImage(new Uri(UndoButton.IsEnabled ? @"icon/Undo_16x.png" : @"icon/Undo_grey_16x.png", UriKind.Relative));
			}
			else if (sender == RedoButton)
			{
				if (RedoButton.Content is Image _image)
					_image.Source = new BitmapImage(new Uri(RedoButton.IsEnabled ? @"icon/Redo_16x.png" : @"icon/Redo_grey_16x.png", UriKind.Relative));
			}
		}

		private void About_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("组长：赵玉乔\n词法分析：鲍家耕\n语法分析&语义分析：邱汇石&赵玉乔\n界面：陈聪");
		}
	}
}
