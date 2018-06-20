using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using RE2DFA;
namespace Lexical_Analyzer
{
    public class LexicalAnalyzer
    {
        public List<LexicalData> data;
        List<DFA> dfas;
        List<string> propertys;
        int line;
        int column;
        public bool LoadRules(string rulesFile)
        {
            dfas = new List<DFA>();
            propertys = new List<string>();
            
            StreamReader sr = new StreamReader(rulesFile, Encoding.Default);
            string newline=null;
            try
            {
                while ((newline = sr.ReadLine()) != null)
                {
                    string[] rule = null;
                    rule = newline.Split(' ');
                    propertys.Add(rule[0]);
                    dfas.Add(DFA_Builder.BuildDFA(rule[1]));
                }
            }catch (Exception e)
            {
                Console.WriteLine("LoadRules Failed.");
                Console.WriteLine(e.Data);
            }
            sr.Close();
            return true;
        }
        public bool Analyze(String code)
        {
            line = 1;
            data = new List<LexicalData>();
            int index = 0;
            while (nextToken(code, ref index)) ;
            if (index != code.Length)
                return false;//报错
            else
                return true;
        }
        bool nextToken(String code,ref int startPos)
        {
            int index;
            while (startPos < code.Length && (code[startPos] == ' ' || code[startPos] == '\r' || code[startPos] == '\n'))
            {
                if (code[startPos] == '\n')
                {
                    column = 1;
                    line++;
                }
                startPos++;
                column++;
            }
            if (startPos == code.Length)
                return false;
            foreach (DFA dfa in dfas)
            {
                index = startPos;
                if (runDFA(dfa.entry,dfa))
                {
                    LexicalData newToken = new LexicalData();
                    newToken.property = propertys[dfas.IndexOf(dfa)];
                    newToken.position = startPos;
                    newToken.value = code.Substring(startPos, index - startPos );
                    newToken.line = line;
                    column+=index-startPos;
                    newToken.column = column;
                    data.Add(newToken);
                    startPos = index;
                    return true;
                    
                }
            }
            return false;
            bool runDFA(State state,DFA dfa)
            {
                State last = state;
                while (index<code.Length&&(state=last.Transfer(code[index]))!=null)
                {
                    last = state;
                    index++;
                }
                if (dfa.terminators.Contains(last))
                    return true;
                else
                    return false;
            }
        }
    }
}
