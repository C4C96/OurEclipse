using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OurEclipse
{
    /// <summary>
    /// TextEditor.xaml 的交互逻辑
    /// </summary>
    public partial class CodeEditor : UserControl
    {
        public CodeEditor()
        {
            InitializeComponent();			
        }

		private void CodeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			UpdateLineNum();
		}

		private void CodeTextBox_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.WidthChanged)
				UpdateLineNum();
		}

		private void UpdateLineNum()
		{
			int count = CodeTextBox.LineCount;
			int returnCount = 1;
			StringBuilder sb = new StringBuilder();
			bool lastLineHasReturn = true;
			for (int i = 0; i < count; i++)
			{
				string lineText = CodeTextBox.GetLineText(i);
				if (lastLineHasReturn)
					sb.Append(returnCount++);
				sb.Append('\n');
				lastLineHasReturn = lineText != string.Empty && lineText.Last() == '\n';
			}
			LineNumTextBlock.Text = sb.ToString();
		}
	}
}
