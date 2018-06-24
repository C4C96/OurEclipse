using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexical_Analyzer;
using RE2DFA;

namespace Compiler
{
    public class ParserAdapter
    {
        ParserResult PR = new ParserResult();
        LexicalAnalyzer la = new LexicalAnalyzer();
        List<ErrorInfo> Errors = new List<ErrorInfo>();
        List<LexicalData> inputData;
        LL llgrammar;
        Parser parser;

        public async Task<ParserResult> Compile(string code, string rulePath,string CFGPath)
        {
            return await Task<ParserResult>.Factory.StartNew(() => 
            {
                GenLLGrammar(CFGPath);
				llgrammar.result(@"G:\CCProject\C#\OurEclipse\2.txt");
                LexicalAnalyze(rulePath,code);
                Parse();
                PR.Errors = Errors;
                PR.First = llgrammar.firstMap;
                PR.Follow = llgrammar.followMap;
                PR.Table = llgrammar.table;
                PR.Root = parser.root;
                if(Errors!=null)
                {
                    PR.ArithResult = ArithResult();
                }
                
                return PR ; 
            });
        }

        //词法分析部分，路径为程序目录下的.rule文件
        private void LexicalAnalyze(string filepath,string input)
        {
            if (!la.LoadRules(filepath))
                return;
            else
            {
                la.Analyze(input);
                inputData = la.data;
            }
                
        }

        //获取LL表，最开始就调用，路径为目录下的.txt,存放的是产生式
        private void GenLLGrammar(string filepath)
        {
            llgrammar = new LL(filepath);
        }

        //语法分析部分
        private void Parse()
        {
            parser = new Parser(llgrammar.nonTerminalSet, llgrammar.terminalSet, llgrammar.table, inputData);
            parser.analyze();
            parser.getArithValue();
            Errors = parser.Errors;
        }

        //得到需要输出的int或者double类型变量的值
        private List<Tuple<string,string>> ArithResult()
        {
            if (parser.arithResult != null)
                return parser.arithResult;

            return null;
        }
		
        public class ParserResult
        {
            public List<ErrorInfo> Errors { get; set; }
            public Dictionary<string, List<string>> First { get; set; }
            public Dictionary<string, List<string>> Follow { get; set; }
            public Dictionary<string, List<Dictionary<string, string>>> Table { get; set; }
            public Node Root { get; set; }
            public List<Tuple<string,string>> ArithResult { get; set; }
        }
    }
}
