using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class LL
    {
        public List<string> nonTerminalSet = new List<string>();
        public List<string> terminalSet = new List<string>();
        private Dictionary<string, List<string>> firstMap = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> followMap = new Dictionary<string, List<string>>();
        private Dictionary<string, List<string>> LLGrammar = new Dictionary<string, List<string>>();
        public Dictionary<string, List<Dictionary<string, string>>> table = new Dictionary<string, List<Dictionary<string, string>>>();//储存表格，第一个string是最左侧的列名，第二个string是最上方的行名，第三个string是这个位置该填的语句

        public LL(string filePath)
        {
            setLLGrammar(filePath);
            setTerminalSet();
            first();
            follow();
            tabulation();
        }

        public void setLLGrammar(string filePath)//从文件读取产生式
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            StreamReader read = new StreamReader(fs, Encoding.Default);
            string strReadline;
            char[] seperator = { ' ', '→' };
            while ((strReadline = read.ReadLine()) != null)
            {
                List<string> line = new List<string>();
                string[] marks = strReadline.Split(seperator);//通过空格和→分隔每一行的内容

                for (int i = 0; i < marks.Length; i++)
                {
                    if (marks[i] != "")
                        line.Add(marks[i]);
                }
                LLGrammar.Add(line[0], line);
                nonTerminalSet.Add(line[0]);//箭头左边的字符串一定是nonterminal
            }

            fs.Close();
            read.Close();
        }

        public void setTerminalSet()
        {
            foreach (var item in LLGrammar)
            {
                foreach (var token in item.Value)
                {
                    if (!nonTerminalSet.Contains(token) && token != "|" && !terminalSet.Contains(token))//不在非终结符集合中并且不是|，那一定是终结符
                    {
                        terminalSet.Add(token);
                    }
                }
            }
        }

        public void first()//求解fist集合
        {
            foreach (var item in LLGrammar)
            {
                List<string> first = new List<string>();//存放first内容
                List<string> line = item.Value;
                if (calFirst(first, line[0]))
                    firstMap.Add(line[0], first);
            }
        }

        public bool calFirst(List<string> list, string symbol)//递归求解,参数是箭头左边的非终结符号
        {
            if (terminalSet.Contains(symbol))//若参数是终结符，则加入first集合
            {
                list.Add(symbol);
                return true;
            }

            List<string> nextLine = new List<string>();
            LLGrammar.TryGetValue(symbol, out nextLine);
            if (!terminalSet.Contains(symbol))//不在终结符集合中，递归
            {
                calFirst(list, nextLine[1]);
            }

            if (nextLine.Contains("|"))//如果有|符号
                for (int i = 1; i < nextLine.Count(); i++)//遍历对应语句中箭头后面（list中）的值
                    if (nextLine[i] == "|")
                        calFirst(list, nextLine[i + 1]);//此时对后面的字符求first

            return true;
        }


        public void follow()//求解follow集合
        {
            foreach (var item in LLGrammar)
            {
                List<string> follow = new List<string>();//存放follow
                follow = calFollow(follow, item.Key);
                followMap.Add(item.Value[0], follow);
            }
        }

        public List<string> calFollow(List<string> follow, string s)//参数是每句话箭头左边的非终结符
        {
            //case1:第一条产生式，要把$放进去
            if (s == "program")
                if (!follow.Contains("$"))
                    follow.Add("$");

            foreach (var item in LLGrammar)
            {
                for (int i = 1; i < item.Value.Count() - 1; i++)//遍历对应语句箭头后面的符号
                {
                    if (item.Value[i] == s && item.Value[i + 1].Length != 0 && terminalSet.Contains(item.Value[i + 1]))//如果当前非终结符后面是终结符
                        if (!follow.Contains(item.Value[i + 1]))
                            follow.Add(item.Value[i + 1]);

                    //case2:A→Bβ，则First(β)中除了ε其余都在Follow(B)中
                    if (item.Value[i] == s && item.Value[i + 1].Length != 0 && nonTerminalSet.Contains(item.Value[i + 1]))//如果当前非终结符后面是非终结符
                    {
                        List<string> line = new List<string>();
                        firstMap.TryGetValue(item.Value[i + 1], out line);
                        for (int j = 0; j < line.Count(); j++)
                            if (line[j] != "ε")
                                if (!follow.Contains(line[j]))
                                    follow.Add(line[j]);

                        //case3：A→Bβ，且First(β)含有ε，则Follow(A)中所有符号都在Follow(B)中
                        if (line.Contains("ε"))
                        {
                            List<string> followA = calFollow(follow, item.Key);
                            for (int k = 0; k < followA.Count(); k++)
                            {
                                if (!follow.Contains(followA[k]))
                                    follow.Add(followA[k]);
                            }
                        }
                    }
                }

                //case4：A→βB，则Follow(A)中所有符号都在Follow(B)中
                if (item.Value[item.Value.Count() - 1] == s)
                {
                    List<string> followA = calFollow(follow, item.Key);
                    for (int k = 0; k < followA.Count(); k++)
                    {
                        if (!follow.Contains(followA[k]))
                            follow.Add(followA[k]);
                    }
                }
            }


            return follow;
        }

        public void tabulation()
        {
            foreach (var item in LLGrammar)
            {
                List<Dictionary<string, string>> List = new List<Dictionary<string, string>>();
                if (item.Value.Contains("|"))//如果产生式中有|
                {
                    List<string> temp = new List<string>();
                    temp.Add(item.Key);

                    for (int i = 1; i < item.Value.Count(); i++)//按|分割产生式(跳过第一个项，因为第一个项与key相同)
                    {
                        if (item.Value[i] != "|")
                            temp.Add(item.Value[i]);
                        if (item.Value[i] == "|" || i == item.Value.Count() - 1)
                        {
                            foreach (var tmp in calculate(temp))
                            {
                                List.Add(tmp);
                            }
                            temp.Clear();
                            temp.Add(item.Key);
                            continue;
                        }
                    }
                    table.Add(item.Key, List);
                }
                else
                {
                    List<string> temp = new List<string>();
                    temp.Add(item.Key);
                    for (int i = 1; i < item.Value.Count(); i++)
                    {
                        temp.Add(item.Value[i]);
                    }
                    List = calculate(temp);
                    table.Add(item.Key, List);
                }
            }
        }

        public List<Dictionary<string, string>> calculate(List<string> list)
        {
            List<Dictionary<string, string>> L = new List<Dictionary<string, string>>();
            //第一个string是最左侧的列名，第二个string是最上方的行名，第三个string是这个位置该填的语句
            string secondString = "";
            //string thirdString = list[0] + "→";
            string thirdString = "";

            for (int i = 1; i < list.Count(); i++)//将thirdstring构造成产生式形式
            {
                thirdString = thirdString + list[i] + " ";
            }

            //case1:A→ε，此时看Follow(A)
            if (list[1] == "ε")
            {
                List<string> followA = new List<string>();
                followMap.TryGetValue(list[0], out followA);

                for (int i = 0; i < followA.Count(); i++)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    secondString = followA[i];
                    dic.Add(secondString, thirdString);
                    L.Add(dic);
                }
            }
            //case2:含有终结符
            else if (terminalSet.Contains(list[1]))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                secondString = list[1];
                dic.Add(secondString, thirdString);
                L.Add(dic);
            }
            else
            {
                //case3:A→Bβ，在First(B)中对应的地方，填上A→Bβ
                List<string> firstB = new List<string>();
                firstMap.TryGetValue(list[1], out firstB);
                for (int i = 0; i < firstB.Count(); i++)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    secondString = firstB[i];
                    if (secondString != "ε")
                    {
                        dic.Add(secondString, thirdString);
                        L.Add(dic);
                    }
                }

                //case4：如果ε存在于First(B)中，则对于Follow(A)中的每一个终结符对应的地方，填上A→Bβ
                //case5：如果ε存在于First(B)中，$在Follow(A)中，则将A→Bβ填到$处
                if (firstB.Contains("ε"))
                {
                    List<string> followA = new List<string>();
                    followMap.TryGetValue(list[0], out followA);
                    for (int i = 0; i < followA.Count(); i++)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        secondString = followA[i];
                        dic.Add(secondString, thirdString);
                        L.Add(dic);
                    }
                }
            }
            return L;
        }


        public int getColNum(string s, Dictionary<string, int> tDic)//查找终结符编号
        {
            int col;
            tDic.TryGetValue(s, out col);
            return col;
        }
        public int getRowNum(string s, Dictionary<string, int> ntDic)//查找非终结符编号
        {
            int row;
            ntDic.TryGetValue(s, out row);
            return row;
        }
        public void result(string filepath)
        {
            List<string> text = new List<string>();//存放二维数组中每一行对应的字符串
            List<List<string>> textTable = new List<List<string>>();//二维数组
            int rowNum, colNum = 0;
            string blank = "\t\t\t\t\t";
            Dictionary<string, int> terminalDic = new Dictionary<string, int>();
            Dictionary<string, int> nonTerminalDic = new Dictionary<string, int>();

            string line0 = blank;//第一行内容

            int j = 0;
            for (int i = 0; i < terminalSet.Count(); i++)//遍历终结符集合
            {
                if (terminalSet[i] != "ε")
                {
                    line0 = line0 + terminalSet[i] + blank;
                    terminalDic.Add(terminalSet[i], j);//将终结符编号
                    j++;
                }
                if (i == terminalSet.Count() - 1)
                {
                    line0 += "$";
                    terminalDic.Add("$", j);
                    colNum = j + 1;
                }
            }

            rowNum = nonTerminalSet.Count();
            for (int i = 0; i < rowNum; i++)//初始化二维数组
            {
                List<string> temp = new List<string>();
                for (int k = 0; k < colNum; k++)
                {
                    temp.Add("\t\t\t\t\t\t");
                }
                textTable.Add(temp);
            }


            for (int i = 0; i < nonTerminalSet.Count(); i++)
            {
                nonTerminalDic.Add(nonTerminalSet[i], i);//将非终结符编号
                foreach (var item in table)
                {
                    if (item.Key == nonTerminalSet[i])
                    {
                        List<Dictionary<string, string>> itemsInLine = new List<Dictionary<string, string>>();//这一行里面的有内容的项
                        table.TryGetValue(nonTerminalSet[i], out itemsInLine);
                        for (int k = 0; k < itemsInLine.Count(); k++)
                        {
                            foreach (var dic in itemsInLine[k])
                            {
                                textTable[i][getColNum(dic.Key, terminalDic)] = nonTerminalSet[i] + "→" + dic.Value;
                            }
                        }
                    }
                }
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath, false))
            {
                file.WriteLine(line0);
                for (int i = 0; i < nonTerminalSet.Count(); i++)
                {
                    string line = "";
                    line += nonTerminalSet[i] + blank;
                    foreach (var l in textTable[i])
                    {
                        line += l + blank;
                    }
                    file.WriteLine(line);
                }
                file.Close();
            }
        }
    }
}
