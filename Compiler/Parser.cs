using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lexical_Analyzer;

namespace Compiler
{
    class Parser
    {
        public List<LexicalData> Token = new List<LexicalData>();
        public Dictionary<string, List<Dictionary<string, string>>> table = new Dictionary<string, List<Dictionary<string, string>>>();//储存表格，第一个string是最左侧的列名，第二个string是最上方的行名，第三个string是这个位置该填的语句
        public List<string> nonTerminalSet = new List<string>();
        public List<string> terminalSet = new List<string>();
        public Stack<string> stack = new Stack<string>();//用来对产生式进行操作的栈
        public Node root;
        private Node currentNode;
        public Env env0 = new Env(null);//符号表的头
        private Env currentEnv;//当前符号表
        private string type = "";//待插入符号表的类型
        private string variable = "";//当前读到的源程序中的字符串,用来记录词位
        private Stack<Node> IDNodes = new Stack<Node>();//用来对树中内容为ID的节点进行词位的赋值
        private Stack<Node> NUMNodes = new Stack<Node>();//用来对树中内容为NUM的节点进行词位的赋值
        private int row = 0, col = 0;
        //public List<List<string>> arithResult = new List<List<string>>();
        public List<ErrorInfo> Errors = new List<ErrorInfo>();
        public List<Tuple<string,string>> arithResult = new List<Tuple<string,string>>();

        public Parser(List<string> nonT, List<string> T, Dictionary<string, List<Dictionary<string, string>>> tab, List<LexicalData> input)
        {
            table = tab;
            nonTerminalSet = nonT;
            terminalSet = T;
            Token = input;
        }

        public void analyze()
        {
            stack.Push("$");
            stack.Push("program");
            root = new Node("program");
            currentNode = root;
            currentEnv = env0;

            step(Token);
            //Console.WriteLine(root.children.Count);
            //root.print(root);

        }
        public void step(List<LexicalData> list)
        {
            string X = stack.Peek();
            string sentence = "";
            int i = 0;
            while (i < list.Count())//遍历输入内容
            {
                int lineNum = list[i].line;
                row = list[i].line;
                col = list[i].column; 
                //if (X == "$")
                //    return;

                //当前单词的具体内容（主要针对ID和NUM的具体内容）
                string info = list[i].value;
                variable = info;

                if (!terminalSet.Contains(X))
                {
					if (list[i].property == "Num")
						sentence = find(X, "NUM");
					else if (list[i].property == "ID")
						sentence = find(X, list[i].property);
					else
						sentence = find(X, list[i].value);
				}
				//Console.WriteLine(X + " " + list[i]);
				string terminalVal = "";
				if (list[i].property == "Num" )
					terminalVal = "NUM";
				else if(list[i].property == "ID")
					terminalVal = "ID";
				else
					terminalVal = list[i].value;


				Console.WriteLine(X + "   " + terminalVal);
				
                if (X == terminalVal)//X=当前指向的符号a
                {
                    if (X == "{")//建立符号表，每遇到一个{}，新建一个符号表
                    {
                        Env newE = new Env(currentEnv);
                        currentEnv = newE;
                    }
                    else if (X == "}")//回退
                    {
                        currentEnv = currentEnv.prev;
                    }

                    //符号表的插入
                    if (X == "int" || X == "long" || X == "float" || X == "double")
                    {
                        type = X;
                    }
                    if (type != "" && X == "ID")
                    {
                        insertToTable(info, type);

                        //往后看一个单词，看看是不是逗号
                        if (list[i + 1] != null && list[i + 1].value == ",")
                        {
                            int j = i + 1;
                            while (list[j].value == ",")
                            {
                                if (list[j + 1] != null && list[j + 1].property == "ID")
                                {
                                    insertToTable(list[j + 1].value, type);//如果逗号后面又是一个ID，那么也加入表
                                }
                                else
                                    break;

                                if (list[j + 2] != null && list[j + 2].property == ",")//如果逗号后面有变量，看后面还有没有逗号
                                {
                                    j = j + 2;
                                }
                                else
                                    break;
                            }
                        }

                        type = "";
                    }


                    //对树中内容为ID或者NUM的节点进行词位的赋值，因为此时才读到源程序对应的词位值
                    if (X == "ID")
                    {
                        Node IDNode = IDNodes.Pop();
                        for (int k = 0; k < IDNode.Children.Count(); k++)
                        {
                            if (IDNode.Children[k].Symbol == "ID")
                            {
                                IDNode.Children[k].Lexeme = variable;
                                IDNode.Children[k].Environment = currentEnv;
                                IDNode.Children[k].row = lineNum;
                                IDNode.Children[k].col = i;
                            }
                        }
                    }
                    else if (X == "NUM")
                    {
                        Node NUMNode = NUMNodes.Pop();
                        for (int k = 0; k < NUMNode.Children.Count(); k++)
                        {
                            if (NUMNode.Children[k].Symbol == "NUM")
                            {
                                NUMNode.Children[k].Lexeme = variable;
                            }
                        }
                    }


                    checkStack(stack);
                    string s = stack.Pop();
                    //Console.WriteLine("第 " + lineNum + " 行，匹配到 " + s);
                    X = stack.Peek();
                    i++;
                    continue;
                }
                else if (terminalSet.Contains(X))//X是终结符号
                {
                    //checkStack(stack);
                    //error(lineNum, i);
                    //Console.Write(X + " " + list[i] + " ");
                    //Console.WriteLine("这里应该有 " + X);
                    Errors.Add(new ErrorInfo(list[i].line, list[i].column, 0, ""));
                    stack.Pop();
                    X = stack.Peek();
                    //i++;
                    continue;
                }
                else if (sentence == "Error")//[X,a]是一个报错条目
                {
                    checkStack(stack);
                    //error(lineNum, i);
                    Errors.Add(new ErrorInfo(list[i].line, list[i].column, 0, ""));
                    stack.Pop();
                    X = stack.Peek();
                    i++;
                    continue;
                }
                else if (sentence != "Error")//[X,a]有内容
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
            int startPos = 0, endPos = 0;
            List<string> tokens = new List<string>();
            for (endPos = 0; endPos < s.Length; endPos++)
            {
                if (s[endPos] == ' ')
                {
                    tokens.Add(s.Substring(startPos, endPos - startPos));
                    startPos = endPos + 1;
                }
            }
            if (startPos < endPos)
            {
                tokens.Add(s.Substring(startPos, endPos - startPos));
            }
            return tokens;
        }

        //public void error(int r, int c)
        //{
        //    Console.WriteLine("出错啦!第 " + r + " 行，第 " + c + " 列");
        //}

        public string find(string symbol, string terminal)//第一个参数为栈顶的符号，第二个是指针所指向的输入符号，返回对应产生式
        {
            string sentence = "";
            List<Dictionary<string, string>> textline = new List<Dictionary<string, string>>();
            if (table.TryGetValue(symbol, out textline))
				foreach (var item in textline)
				{
					foreach (var dic in item)
					{
						if (dic.Key == terminal)
						{
							sentence = dic.Value;
							//Console.WriteLine(symbol +" "+ sentence);
							constructTree(symbol, cut(sentence));

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

        public void constructTree(string symbol, List<string> list)//symbol是产生式左边的符号，list存放产生式右边的符号
        {
            bool finish = false;
            while (!finish)
            {
                if (!currentNode.HasChild)//如果没有子节点
                {
                    if (currentNode.Symbol == symbol)//当前node就是要找的，那么直接增加子节点
                    {
                        foreach (string s in list)
                        {
                            if (!nonTerminalSet.Contains(s))//如果s是终结符
                            {
                                if (s == "ID")//记录下产生ID的产生式，放入栈中，当下次匹配到ID时，就可以找到这个节点的位置
                                {
                                    IDNodes.Push(currentNode);
                                    currentNode.Add(new Node(s));
                                }
                                else if (s == "NUM")
                                {
                                    NUMNodes.Push(currentNode);
                                    currentNode.Add(new Node(s));
                                }
                                else
                                {
                                    currentNode.Add(new Node(s, s));
                                }
                            }
                            else
                            {
                                currentNode.Add(new Node(s));
                            }
                        }
                        finish = true;
                    }
                    else
                    {
                        currentNode.IsChecked = true;
                        currentNode = currentNode.Parent;
                    }
                }
                else
                {
                    bool find = false;
                    foreach (Node n in currentNode.Children)
                    {
                        if (!n.IsChecked && n.Symbol == symbol)
                        {
                            currentNode = n;
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        currentNode.IsChecked = true;
                        currentNode = currentNode.Parent;
                    }
                }
            }
        }

        public void insertToTable(string var, string type)
        {
            if (!currentEnv.HasItem(var))
                currentEnv.add(var, type);
            else
            {
                //Console.WriteLine("出错啦!第 " + row + " 行，第 " + col + " 列" + " 重复的定义");
                Errors.Add(new ErrorInfo(row, col, 0, ""));
            }
        }

        private bool checkType(Node idNode, string value)
        {
            List<string> list = idNode.Environment.get(idNode.Lexeme);
            if (list == null)
            {
                //Console.WriteLine("出错啦!第 " + idNode.row + " 行，第 " + idNode.col + " 列" + " 变量未被定义");
                Errors.Add(new ErrorInfo(idNode.row, idNode.col, 0, ""));
                return false;
            }
            else
            {
                string idType = list[0];
                string valueType = "";
                Regex reg = new Regex(@"^\d+\.\d+$");
                if (reg.IsMatch(value))//判断等号右边值的类型
                    valueType = "double";
                else
                {
                    int v = 0;
                    bool result = int.TryParse(value, out v);
                    if (result == true)
                    {
                        valueType = "int";
                    }
                    else
                    {
                        valueType = "other";
                    }
                }

                if (valueType == "double")
                {
                    if (idType == "double" || idType == "float")
                        return true;
                    else
                        return false;
                }
                else if (valueType == "int")
                {
                    if (idType == "int" || idType == "long" || idType == "double" || idType == "float")
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        public void getArithValue()
        {
            //对树进行深度优先遍历
            Stack<Node> s = new Stack<Node>();
            s.Push(root);
            while (s.Count != 0)
            {
                Node n = s.Pop();
                if (n.Symbol == "variableDeclaration")
                {
                    handleVariableDeclaration(n);
                }
                else if (n.Symbol == "assgstmt")
                {
                    handleAssgstmt(n);
                }
                if (n.HasChild)//如果栈顶节点有子节点，将子节点逆序压栈
                {
                    for (int i = n.Children.Count() - 1; i >= 0; i--)
                    {
                        s.Push(n.Children[i]);
                    }
                }
            }
        }

        public void handleVariableDeclaration(Node n)
        {
            //variableDeclaration → numericType ID  assgValue ;
            Node id = n.Children[1];
            if (n.Children[2].Children.Count == 1)
                return;
            else
            {
                string value = "";
                value = getAssgValue(n.Children[2]);
                if (checkType(id, value))//如果等号左右类型匹配或者可以隐式转换
                {
                    id.value = value;
                    //Console.WriteLine(id.Lexeme + " " + value);
                    arithResult.Add(Tuple.Create<string,string>(id.Lexeme,value));
                    //在符号表相应位置插入该变量的值
                    List<string> tmp = n.Children[1].Environment.get(n.Children[1].Lexeme);
                    if (tmp != null)
                    {
                        tmp[1] = value;
                    }
                }
                else//等号左右两边类型不匹配
                {
                    //Error
                    //Console.WriteLine("出错啦!第 " + id.row + " 行，第 " + id.col + " 列" + " 变量类型不匹配");
                    Errors.Add(new ErrorInfo(id.row, id.col, 0, ""));
                }

            }
        }

        public void handleAssgstmt(Node n)
        {
            //assgstmt →  ID assignmentOperator arithexpr ;
            Node id = n.Children[0];
            string op = getAssignmentOperator(n.Children[1]);
            string value = "";
            if (op == "=")
            {
                value = getArithexprValue(n.Children[2]);
            }
            else if (op == "*=")
            {
                value = calaulateString(getIDValue(id), "*", getArithexprValue(n.Children[2]));
            }
            else if (op == "/=")
            {
                value = calaulateString(getIDValue(id), "/", getArithexprValue(n.Children[2]));
            }
            else if (op == "+=")
            {
                value = calaulateString(getIDValue(id), "+", getArithexprValue(n.Children[2]));
            }
            else if (op == "-=")
            {
                value = calaulateString(getIDValue(id), "-", getArithexprValue(n.Children[2]));
            }

            if(value=="Error")
                Errors.Add(new ErrorInfo(n.row, n.col, 0, "类型不匹配，无法运算"));

            if (checkType(id, value))//如果等号左右类型匹配或者可以隐式转换
            {
                id.value = value;
                arithResult.Add(Tuple.Create<string, string>(id.Lexeme, value));
                //在符号表相应位置插入该变量的值
                List<string> tmp = n.Children[0].Environment.get(n.Children[1].Lexeme);
                if (tmp != null)
                {
                    tmp[1] = value;
                }
            }
            else//等号左右两边类型不匹配
            {
                //Error
                //Console.WriteLine("出错啦!第 " + id.row + " 行，第 " + id.col + " 列" + " 变量类型不匹配");
                Errors.Add(new ErrorInfo(id.row, id.col, 0, ""));
            }
        }

        private string getAssgValue(Node n)
        {
            //assgValue → = simpleexpr | ε;
            if (n.Children.Count != 1)
            {
                return getSimpleexprValue(n.Children[1]);
            }

            return "";
        }

        private string getSimpleexprValue(Node n)
        {
            //simpleexpr →  ID  |  NUM  |  ( arithexpr )
            if (n.Children.Count == 1)
            {
                if (n.Children[0].Symbol == "ID")
                {
                    string value = getIDValue(n.Children[0]);
                    n.value = value;
                    return value;
                }
                else if (n.Children[0].Symbol == "NUM")
                {
                    n.value = n.Children[0].Lexeme;
                    return n.Children[0].Lexeme;
                }
            }
            else
            {
                string value = getArithexprValue(n.Children[1]);
                n.value = value;
                return value;
            }
            return "";
        }

        private string getArithexprValue(Node n)
        {
            //arithexpr  →  multexpr arithexprprime
            n.value = getArithexprprimeSyn(n.Children[1]);
            return n.value;
        }

        private string getArithexprprimeSyn(Node n)
        {
            //arithexprprime →  + multexpr arithexprprime  |  - multexpr arithexprprime  |   ε
            if (n.Parent.Symbol == "arithexpr")
            {
                n.inh = getMultexprValue(n.Parent.Children[0]);
            }
            else if (n.Parent.Symbol == "arithexprprime")
            {
                n.inh = calaulateString(n.Parent.inh, n.Parent.Children[0].Symbol, getMultexprValue(n.Parent.Children[1]));
            }


            if (n.inh == "Error")
                Errors.Add(new ErrorInfo(n.row, n.col, 0, "类型不匹配，无法运算"));

            if (n.Children.Count == 1)//生成ε
            {
                n.syn = n.inh;
                return n.syn;
            }
            else
            {
                n.syn = getArithexprprimeSyn(n.Children[2]);
                return n.syn;
            }
        }

        private string getMultexprValue(Node n)
        {
            //multexpr →  simpleexpr  multexprprime
            n.value = getMultexprprimeSyn(n.Children[1]);
            return n.value;
        }

        private string getMultexprprimeSyn(Node n)
        {
            //multexprprime →  *simpleexpr multexprprime |  / simpleexpr multexprprime | ε
            if (n.Parent.Symbol == "multexpr")
            {
                n.inh = getSimpleexprValue(n.Parent.Children[0]);
            }
            else if (n.Parent.Symbol == "multexprprime")
            {
                n.inh = calaulateString(n.Parent.inh, n.Parent.Children[0].Symbol, getSimpleexprValue(n.Parent.Children[1]));
            }


            if (n.inh == "Error")
                Errors.Add(new ErrorInfo(n.row, n.col, 0, "类型不匹配，无法运算"));

            if (n.Children.Count == 1)//生成ε
            {
                n.syn = n.inh;
                return n.syn;
            }
            else
            {
                n.syn = getMultexprprimeSyn(n.Children[2]);
                return n.syn;
            }
        }

        private string getAssignmentOperator(Node n)
        {
            return n.Children[0].Symbol;
        }

        private string calaulateString(string leftItem, string op, string rightItem)//a+b,a*b,a-b,a/b
        {
            if (leftItem == "" || rightItem == "")
                return "";
            string leftType = getStringType(leftItem);
            string rightType = getStringType(rightItem);
            if (leftType == "double" || rightType == "double")
            {
                double left, right;
                if (leftType != "other")
                    left = double.Parse(leftItem);
                else
                {
                    //非数字无法进行运算，报错
                    //Console.WriteLine("出错啦! " + " 无法进行运算");
                    return "Error";
                }
                if (rightType != "other")
                    right = double.Parse(rightItem);
                else
                {
                    //非数字无法进行运算，报错
                    //Console.WriteLine("出错啦! " + " 无法进行运算");
                    return "Error";
                }
                return calculateNum(left, op, right);
            }
            else if (leftType == "int" && rightType == "int")
            {
                int left, right;
                left = int.Parse(leftItem);
                right = int.Parse(rightItem);
                return calculateNum(left, op, right);
            }
            else
            {
                //非数字无法进行运算，报错
                //Console.WriteLine("出错啦! " + " 无法进行运算");
                return "Error";
            }
        }

        public string calculateNum(double l, string op, double r)
        {
            switch (op)
            {
                case "*":
                    return (l * r).ToString();
                case "/":
                    return (l / r).ToString();
                case "+":
                    return (l + r).ToString();
                case "-":
                    return (l - r).ToString();
            }
            return "";
        }
        public string calculateNum(int l, string op, int r)
        {
            switch (op)
            {
                case "*":
                    return (l * r).ToString();
                case "/":
                    return (l / r).ToString();
                case "+":
                    return (l + r).ToString();
                case "-":
                    return (l - r).ToString();
            }
            return "";
        }

        private string getIDValue(Node n)
        {
            List<string> list = n.Environment.get(n.Lexeme);
            if (list == null)
            {
                //变量未被定义，报错
                //Console.WriteLine("出错啦!第 " + n.row + " 行，第 " + n.col + " 列" + " 变量未被定义");
                Errors.Add(new ErrorInfo(n.row, n.col, 0, ""));
            }
            else
            {
                if (list[1] == "")
                {
                    //变量未被赋值，报错
                    //Console.WriteLine("出错啦!第 " + n.row + " 行，第 " + n.col + " 列" + " 变量未被赋值");
                    Errors.Add(new ErrorInfo(n.row, n.col, 0, ""));
                }
                else
                {
                    return list[1];
                }
            }
            return "";
        }

        private string getIDType(Node n)
        {
            List<string> list = n.Environment.get(n.Lexeme);
            if (list == null)
            {
                //变量未被定义，报错
                //Console.WriteLine("出错啦!第 " + n.row + " 行，第 " + n.col + " 列" + " 变量未被定义");
                Errors.Add(new ErrorInfo(n.row, n.col, 0, ""));
            }
            else
            {
                return list[0];
            }
            return "";
        }

        private string getStringType(string s)
        {
            string valueType = "";
            Regex reg = new Regex(@"^\d+\.\d+$");
            if (reg.IsMatch(s))
                valueType = "double";
            else
            {
                int v = 0;
                bool result = int.TryParse(s, out v);
                if (result == true)
                {
                    valueType = "int";
                }
                else
                {
                    valueType = "other";
                }
            }
            return valueType;
        }//判断字符串是int还是double

    }
}
