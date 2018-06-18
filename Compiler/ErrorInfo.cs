using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
	public class ErrorInfo
	{
		public int Row { get; set; }
		public int Col { get; set; }

		public ErrorType Type { get; set; }
		public String ErrorString { get; set; }

		public ErrorInfo(int row, int col, ErrorType type, String errorString)
		{
			Row = row;
			Col = col;
			Type = type;
			ErrorString = errorString;
		}
		
		// 需要生成报错信息就重写，否则就删掉吧
		public override string ToString()
		{
			return base.ToString();
		}

		/// <summary>
		/// 错误类型
		/// </summary>
		public enum ErrorType
		{
			Type1,
			Type2,
			Type3,
		}
	}
}
