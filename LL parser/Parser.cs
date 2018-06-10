using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LexDemo
{
    class Parser
    {
        public Dictionary<int, List<string>> token = new Dictionary<int, List<string>>();
        public Dictionary<string, List<Dictionary<string, string>>> table = new Dictionary<string, List<Dictionary<string, string>>>();//储存表格，第一个string是最左侧的列名，第二个string是最上方的行名，第三个string是这个位置该填的语句
        public List<string> nonTerminalSet = new List<string>();
        public List<string> terminalSet = new List<string>();
        public Stack<string> stack = new Stack<string>();

        public Parser(List<string> nonT,List<string> T, Dictionary<string, List<Dictionary<string, string>>> tab,Dictionary<int, List<string>> to)
        {
            table = tab;
            nonTerminalSet = nonT;
            terminalSet = T;
            token = to;
        }

        public void analyze()
        {
            stack.Push("$");
            stack.Push("program");
            foreach(var item in token)
            {
                step(item.Key, item.Value);
            }
        }
        public void step(int lineNum,List<string> list)
        {
            string X = stack.Peek();
            string sentence = "";
            int i = 0;
            while(i<list.Count())//遍历输入内容
            {
                //if (X == "$")
                //    return;
                if(!terminalSet.Contains(X))
                {
                    sentence = find(X, list[i]);
                }
                //Console.WriteLine(X + " " + list[i]);
                if(X==list[i])//X=当前指向的符号a
                {
                    checkStack(stack);
                    string s = stack.Pop();
                    //Console.WriteLine("第 " + lineNum + " 行，匹配到 " + s);
                    X = stack.Peek();
                    i++;
                    continue;
                }
                else if(terminalSet.Contains(X))//X是终结符号
                {
                    checkStack(stack);
                    error(lineNum, i);
                    Console.Write(X + " " + list[i]+" ");
                    Console.WriteLine("这里应该有 " + X);
                    stack.Pop();
                    X = stack.Peek();
                    //i++;
                    continue;
                }
                else if(sentence=="Error")//[X,a]是一个报错条目
                {
                    checkStack(stack);
                    error(lineNum, i);                    
                    stack.Pop();
                    X = stack.Peek();
                    i++;
                    continue;
                }
                else if(sentence != "Error")//[X,a]有内容
                {
                    checkStack(stack);
                    //Console.WriteLine(sentence);
                    stack.Pop();
                    List<string> items = cut(sentence);
                    if (items[0] != "ε")
                    {
                        for (int x = items.Count() - 1; x >= 0; x--)//倒着压栈
                        {
                            if (items[x] != "")
                                stack.Push(items[x]);
                        }
                    }
                    X = stack.Peek();
                }
            }
        }

        public List<string> cut(string s)
        {
            int startPos = 0,endPos=0;
            List<string> tokens = new List<string>();
            for(endPos=0; endPos < s.Length; endPos++)
            {
                if(s[endPos] ==' ')
                {
                    tokens.Add(s.Substring(startPos, endPos - startPos));
                    startPos = endPos + 1;
                }
            }
            if(startPos<endPos)
            {
                tokens.Add(s.Substring(startPos, endPos - startPos));
            }
            return tokens;
        }

        public void error(int row,int colume)
        {
            Console.WriteLine("出错啦!第 " + row + " 行，第 " + colume + " 列");
        }

        public string find(string symbol,string terminal)//第一个参数为栈顶的符号，第二个是指针所指向的输入符号，返回对应产生式
        {
            string sentence = "";
            List<Dictionary<string, string>> textline = new List<Dictionary<string, string>>();
            table.TryGetValue(symbol, out textline);
            foreach(var item in textline)
            {
                foreach(var dic in item)
                {
                    if(dic.Key == terminal)
                    {
                        sentence = dic.Value;
                        return sentence;
                    }
                }
            }
            return "Error";
        }

        public void checkStack(Stack<string> s)
        {
            string[] str = s.ToArray();
            Console.Write("------------- ");
            for (int i = 0; i < str.Length; i++)
            {
                Console.Write(str[i] + " ");
            }
            Console.WriteLine();
        }
    }
}
