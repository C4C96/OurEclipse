using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LexDemo
{
    class LexAna
    {
        private string text;
        private string[] keywords= {"int", "double","float", "if", "else","then", "while", "ID" };//关键字
        private string[] operators = {"+", "-", "*", "/", "=", "<", ">", "!", ">=", "<=", "==", "!=" };//操作符
        private string[] delimiters = {"(", ")", "{", "}", ";", "," };//分隔符
        public Dictionary<int, List<string>> Token = new Dictionary<int, List<string>>();//第一个参数是行数，第二个参数是被分开的单词组成的list
        public List<string> errorMessag = new List<string>();

        public string[] Keywords
        {
            get
            {
                return keywords;
            }

            set
            {
                keywords = value;
            }
        }
        public string[] Operators
        {
            get
            {
                return operators;
            }

            set
            {
                operators = value;
            }
        }
        public string[] Delimiters
        {
            get
            {
                return delimiters;
            }

            set
            {
                delimiters = value;
            }
        }

        public LexAna(string inputText)
        {
            text = inputText;
        }

        public bool isKeywords(string s)//是否是关键字
        {
            for(int i=0;i<keywords.Length;i++)
            {
                if (s == keywords[i])
                    return true;
            }
            return false;
        }

        public bool isOperators(string s)//是否是操作符
        {
            for (int i = 0; i < operators.Length; i++)
            {
                if (s == operators[i])
                    return true;
            }
            return false;
        }

        public bool isDelimiters(string s)//是否是分隔符
        {
            for (int i = 0; i < delimiters.Length; i++)
            {
                if (s == delimiters[i])
                    return true;
            }
            return false;
        }

        public bool isDigit(string s)//是否是数字
        {
            return Regex.IsMatch(s, @"^[+-]?/d*[.]?/d*$");
        }

        public bool isValidVar(string s)//是否满足变量名规范
        {
            if(Regex.IsMatch(s, @"^[+-]?\d+(\.\d+)?$"))
            {
                return true;
            }
            else
                return Regex.IsMatch(s, @"^[a-zA-Z\$_][a-zA-Z\d_]*$");
        }

        private void dividByLine()//将text按行分隔
        {
            string[] sentences = text.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for(int i=0;i<sentences.Length;i++)
            {
                segmentation(i + 1, sentences[i]);
            }
        }

        private void segmentation(int num , string s)//对每一行的单词进行分割,放入Token
        {
            List<string> list = new List<string>();            
            int startPos = 0;
            int endPos = 1;
            string token = "";

            for(int i=0;i<s.Length;i++)
            {
                if(s[i]==' ')//如果是空格，截取前面的单词
                {
                    endPos = i;
                    token = s.Substring(startPos, endPos-startPos);
                    startPos = endPos + 1;
                    if (token != " " && token.Length != 0)
                    {
                        list.Add(token);
                        if (!isDelimiters(token) && !isOperators(token) && !isKeywords(token) && !isValidVar(token))//变量名出错，则记录在错误信息中
                            errorMessag.Add("error ：Invalid identifier in line "+num);
                    }
                }
                else if(isDelimiters(s[i]+""))//如果是分隔符，截取前面的单词和分隔符自身
                {
                    endPos = i + 1;
                    token = s.Substring(startPos, endPos - startPos - 1);//分隔符前的单词
                    if (token != " " && token.Length != 0)
                    {
                        list.Add(token);
                        if (!isDelimiters(token) && !isOperators(token) && !isKeywords(token) && !isValidVar(token))//变量名出错，则记录在错误信息中
                            errorMessag.Add("error ：Invalid identifier in line " + num);
                    }
                    token = s[i]+"";//当前分隔符
                    startPos = endPos;
                    if (token != " " && token.Length != 0)
                        list.Add(token);
                }
                else if(isOperators(s[i]+""))//如果是操作符
                {
                    if(i < s.Length-1)
                    {
                        if(isOperators(s[i+1] + ""))//当前字符下一个字符也是操作符
                        {
                            endPos = i + 2;
                            token = s.Substring(startPos, endPos - startPos - 2);//操作符前的单词
                            if (token != " " && token.Length != 0)
                            {
                                list.Add(token);
                                if (!isDelimiters(token) && !isOperators(token) && !isKeywords(token) && !isValidVar(token))//变量名出错，则记录在错误信息中
                                    errorMessag.Add("error ：Invalid identifier in line " + num);
                            }
                            token = ""+ s[i] + s[i + 1];//当前操作符（2个字符）
                            startPos = endPos;
                            if (token != " " && token.Length != 0)
                                list.Add(token);
                        }
                        else
                        {
                            if(i >= endPos)
                            {
                                endPos = i + 1;
                                token = s.Substring(startPos, endPos - startPos - 1);//操作符前的单词
                                if (token != " " && token.Length != 0)
                                {
                                    list.Add(token);
                                    if (!isDelimiters(token) && !isOperators(token) && !isKeywords(token) && !isValidVar(token))//变量名出错，则记录在错误信息中
                                        errorMessag.Add("error ：Invalid identifier in line " + num);
                                }
                                token = s[i] + "";//当前操作符（1个字符）
                                startPos = endPos;
                                if (token != " " && token.Length != 0)
                                    list.Add(token);
                            }
                        }
                    }
                }
            }
            Token.Add(num, list);
        }

        public void print()
        {
            dividByLine();
            foreach (var item in Token)
            {
                Console.Write(item.Key+"  ");
                for(int i=0;i<item.Value.Count();i++)
                {
                    item.Value[i] = check(item.Value[i]);
                    Console.Write(item.Value[i] + "  ");
                }
                Console.WriteLine();
            }

            foreach(var err in errorMessag)
            {
                Console.WriteLine(err);
            }
        }

        public string check(string s)
        {
            string name = s;
            if(!isDelimiters(s) && !isKeywords(s) && !isOperators(s))
            {
                int a = 0;
                if (int.TryParse(s, out a) == true)
                    name = "NUM";
                else
                    name = "ID";
            }
            return name;
        }
    }
}
