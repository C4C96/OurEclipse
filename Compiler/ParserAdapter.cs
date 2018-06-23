using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexical_Analyzer;
using RE2DFA;

namespace Compiler
{
    class ParserAdapter
    {
        LexicalAnalyzer la = new LexicalAnalyzer();
        List<ErrorInfo> Errors = new List<ErrorInfo>();
        List<LexicalData> inputData;
        LL llgrammar;
        Parser parser;


        //词法分析部分，路径为程序目录下的.rule文件
        public void LexicalAnalyze(string filepath)
        {
            if (!la.LoadRules(filepath))
                return;
            else
                inputData = la.data;
        }

        //获取LL表，最开始就调用，路径为目录下的.txt,存放的是产生式
        public void GenLLGrammar(string filepath)
        {
            llgrammar = new LL(filepath);
        }

        //语法分析部分
        public void Parse()
        {
            parser = new Parser(llgrammar.nonTerminalSet, llgrammar.terminalSet, llgrammar.table, inputData);
            parser.analyze();
            parser.getArithValue();
            Errors = parser.Errors;
        }

        //得到需要输出的int或者double类型变量的值
        public List<List<string>> ArithResult()
        {
            if (parser.arithResult != null)
                return parser.arithResult;

            return null;
        }
    }
}
