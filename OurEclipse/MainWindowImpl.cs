using Compiler;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace OurEclipse
{
	public partial class MainWindow
    {
		private string path = null;

		#region New & Open

		private void New()
		{
			if (!AskForSave()) return;
			path = null;
			CodeBox.Clear();
			// 清空撤销栈
			CodeBox.IsUndoEnabled = false;
			CodeBox.IsUndoEnabled = true;
		}

		private void Open()
		{
			if (!AskForSave()) return;
			OpenFileDialog openFileDialog = new OpenFileDialog()
			{
				Filter = "Java文件(*.java) |*.java|文本文件(*.txt) |*.txt",
				FilterIndex = 1,
				RestoreDirectory = true,
			};
			if (openFileDialog.ShowDialog() == true)
			{
				path = openFileDialog.FileName;
				CodeBox.textChangedEnable = false;
				CodeBox.Text = File.ReadAllText(path, Encoding.Default);
				CodeBox.textChangedEnable = true;
				// 清空撤销栈
				CodeBox.IsUndoEnabled = false;
				CodeBox.IsUndoEnabled = true;
			}
		}

		/// <summary>
		/// 询问是否需要保存
		/// </summary>
		/// <returns>用户无需保存，已保存或放弃保存，可以放弃已有文本</returns>
		private bool AskForSave()
		{
			if (!CodeBox.CanUndo) return true;
			MessageBoxResult result = MessageBox.Show("是否保存修改？", "Our Eclipse", MessageBoxButton.YesNoCancel);
			switch (result)
			{
				case MessageBoxResult.Yes:
					return Save();
				case MessageBoxResult.No:
					return true;
				case MessageBoxResult.Cancel:
					return false;
			}
			return false;
		}

		#endregion

		#region Save & SaveAs

		/// <summary>
		/// 保存
		/// </summary>
		/// <returns>是否成功保存</returns>
		private bool Save()
		{
			if (string.IsNullOrEmpty(path))
			{
				path = SaveAs();
				return !string.IsNullOrEmpty(path);
			}
			else
				return SaveToPath(path);
		}

		/// <summary>
		/// 另存为
		/// </summary>
		/// <returns>另存为的路径</returns>
		private string SaveAs()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog()
			{
				Filter = "Java文件(*.java) |*.java|文本文件(*.txt) |*.txt",
				FilterIndex = 1,
				RestoreDirectory = true,
			};
			if (saveFileDialog.ShowDialog() == true)
			{
				string savePath = saveFileDialog.FileName;
				if (SaveToPath(savePath))
					return savePath;
				else
					return null;
			}
			return null;
		}

		/// <summary>
		/// 写入指定文件
		/// </summary>
		/// <param name="path"></param>
		/// <returns>是否成功</returns>
		private bool SaveToPath(string path)
		{
			StateSBI.Content = "保存中...";
			try
			{
				File.WriteAllText(path, CodeBox.Text, Encoding.Default);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message);
				return false;
			}
			finally
			{
				StateSBI.Content = "就绪";
			}
			return true;
		}

		#endregion
		
		#region Comment & Uncomment

		/// <summary>
		/// 注释
		/// </summary>
		private void Comment()
		{
			if (CodeBox.SelectedText.Contains("\n"))
				CommentMultiLines();
			else
				CommentOneLine();
		}

		/// <summary>
		/// 注释单行
		/// </summary>
		private void CommentOneLine()
		{
			int lineindex = CodeBox.GetLineIndexFromCharacterIndex(CodeBox.SelectionStart);
			int firstCharIndex = CodeBox.GetCharacterIndexFromLineIndex(lineindex);
			int lastCharIndex = firstCharIndex + CodeBox.GetLineLength(lineindex) - 1;
			// 判断是否选择了该行所有非空白字符
			bool selectWholeLine = true;
			for (int i = firstCharIndex; i <= lastCharIndex; i++)
				if (i >= CodeBox.SelectionStart && i < CodeBox.SelectionStart + CodeBox.SelectionLength) continue;
				else if (char.IsWhiteSpace(CodeBox.Text[i])) continue;
				else
				{
					selectWholeLine = false;
					break;
				}

			CodeBox.textChangedEnable = false;
			if (selectWholeLine)
				CodeBox.SelectedText = "//" + CodeBox.SelectedText;
			else
				CodeBox.SelectedText = "/*" + CodeBox.SelectedText + "*/";
			CodeBox.textChangedEnable = true;
		}

		/// <summary>
		/// 注释多行
		/// </summary>
		private void CommentMultiLines()
		{
			// 将选择区域修正成首行行头到末行行尾
			int firstLine = CodeBox.GetLineIndexFromCharacterIndex(CodeBox.SelectionStart);
			int lastLine = CodeBox.GetLineIndexFromCharacterIndex(CodeBox.SelectionStart + CodeBox.SelectionLength);
			CodeBox.SelectionStart = CodeBox.GetCharacterIndexFromLineIndex(firstLine);
			CodeBox.SelectionLength = CodeBox.GetCharacterIndexFromLineIndex(lastLine)
									+ CodeBox.GetLineLength(lastLine)
									- CodeBox.SelectionStart;

			string[] lines = CodeBox.SelectedText.Split('\n');
			StringBuilder sb = new StringBuilder();
			int i = 0;
			for (; i < lines.Length - 1; i++)
				sb.Append("//").Append(lines[i]).Append('\n');
			sb.Append("//").Append(lines[i]);
			CodeBox.textChangedEnable = false;
			CodeBox.SelectedText = sb.ToString();
			CodeBox.textChangedEnable = true;
		}

		/// <summary>
		/// 取消注释
		/// </summary>
		private void Uncomment()
		{
			string selectedString = CodeBox.SelectedText;
			if (selectedString.Length >= 2
				&& selectedString.Substring(0, 2) == "/*"
				&& selectedString.Substring(selectedString.Length - 2) == "*/")
			{
				CodeBox.textChangedEnable = false;
				CodeBox.SelectedText = selectedString.Substring(2, selectedString.Length - 4);
				CodeBox.textChangedEnable = true;
			}
			else
			{
				string[] lines = selectedString.Split('\n');
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < lines.Length; i++)
					if (lines[i].Length < 2 || lines[i].Substring(0, 2) != "//")
						return;
					else
						sb.Append(lines[i].Substring(2)).Append(i == lines.Length - 1 ? "" : "\n");
				CodeBox.textChangedEnable = false;
				CodeBox.SelectedText = sb.ToString();
				CodeBox.textChangedEnable = true;
			}
		}

		#endregion

		#region Run

		private ParserAdapter parserAdapter;

		/// <summary>
		/// 启动编译
		/// </summary>
		private async void RunAsync()
		{
			if (parserAdapter == null)
				parserAdapter = new ParserAdapter();
			var result = await parserAdapter.Compile(CodeBox.Text, 
													rulePath: @"G:\CCProject\C#\OurEclipse\LexicalRules.rules", 
													CFGPath: @"G:\CCProject\C#\OurEclipse\1.txt");
			FirstTableDataGrid.ItemsSource = result.First;
			FollowTableDataGrid.ItemsSource = result.Follow;
			ASTView.ItemsSource = new List<Node>() { result.Root };
		}

		#endregion
	}
}