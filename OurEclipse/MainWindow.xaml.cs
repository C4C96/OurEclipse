using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

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
			CodeBox.DecorationScheme = CodeBoxControl.Decorations.DecorationSchemes.Java;
			CodeBox.Decorations.Add(new CodeBoxControl.Decorations.MultiRegexWordDecoration
			{
				Brush = new SolidColorBrush(Color.FromArgb(30, 0, 0, 255)),
				DecorationType = CodeBoxControl.Decorations.EDecorationType.Hilight,
				Words = new System.Collections.Generic.List<string>() { "abc" }
			});
			CodeBox.Decorations.Add(new CodeBoxControl.Decorations.MultiRegexWordDecoration
			{
				Brush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
				DecorationType = CodeBoxControl.Decorations.EDecorationType.Underline,
				Words = new System.Collections.Generic.List<string>() { "def" }
			});
		}
		
		private void CommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			//if (e.Command == ApplicationCommands.New
			//	|| e.Command == ApplicationCommands.Open
			//	|| e.Command == ApplicationCommands.Save
			//	|| e.Command == ApplicationCommands.SaveAs)
			e.CanExecute = true;
			e.Handled = true;
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
			else if (e.Command == ApplicationCommands.Undo)
				Undo();
			else if (e.Command == ApplicationCommands.Redo)
				Redo();
			else if (e.Command == ApplicationCommands.Cut)
				Cut();
			else if (e.Command == ApplicationCommands.Copy)
				Copy();
			else if (e.Command == ApplicationCommands.Paste)
				Paste();
			else if (e.Command == TryFindResource("Comment"))
				Comment();
			else if (e.Command == TryFindResource("Uncomment"))
				Uncomment();
			else
				e.Handled = false;
		}
	}
}
