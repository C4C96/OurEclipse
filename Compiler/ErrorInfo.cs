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
        public int WordLength { get; set; }
		public String ErrorString { get; set; }

		public ErrorInfo(int row, int col,int wl, String errorString)
		{
			Row = row;
			Col = col;
            WordLength = wl;
			ErrorString = errorString;
		}
		
		// 需要生成报错信息就重写，否则就删掉吧
		public override string ToString()
		{
			return base.ToString();
		}

	}
}
