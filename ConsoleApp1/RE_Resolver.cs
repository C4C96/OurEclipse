using System;
using System.Collections;
using System.Collections.Generic;
namespace RE2DFA
{
    public class Re_Resolver
    {
        public Re_Resolver()
        {
        }
        private String expression;
        public ParseTree parseTree;
        int bracketNum;
        public int index;
        /// <summary>
        /// 将一个RE表达式转换成parseTree
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public bool Resolve(String expression)
        {
            bracketNum = 0;
            this.expression = expression;
            index = 0;
            parseTree = new ParseTree();
            while (index < expression.Length)
            {
                if (!Shift(expression[index]))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 读入下一个符号
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        bool Shift(char c)
        {
            try
            {
                if (c == '[')
                {
                    int lastIndex = index;
                    index = expression.IndexOf(']', index);
                    if (index < 0)
                        return false;
                    Symbol symbol = new Symbol(true);
                    if (symbol.SetExpression(expression.Substring(lastIndex + 1, index - lastIndex - 1)))
                    {
                        ParseTree newTree = parseTree.Append(symbol);
                        if (newTree != null)
                            parseTree = newTree;
                    }
                    else
                        return false;
                    index++;
                }
                else
                {
                    if (c == '*' || c == '+' || c == '|')
                    {
                        ParseTree newTree = parseTree.Append(new Operation(c));
                        if (newTree != null)
                            parseTree = newTree;
                    }
                    else
                    if (c == '(')
                    {
                        bracketNum++;
                        ParseTree newTree = parseTree.AppendLeftBracket();
                        if (newTree != null)
                            parseTree = newTree;
                    }
                    else
                if (c == ')')
                    {
                        bracketNum--;
                        if (bracketNum < 0)
                            return false;
                        ParseTree newTree = parseTree.AppendRightBracket();
                        if (newTree != null)
                            parseTree = newTree;
                    }
                    else
                if (c == '$')
                    {
                        if (bracketNum != 0)
                            return false;
                        ParseTree newTree = parseTree.AppendTerminator();
                        if (newTree != null)
                            parseTree = newTree;
                    }
                    else
                    {
                        Symbol symbol = new Symbol(true);
                        symbol.SetExpression(""+c);
                        ParseTree newTree = parseTree.Append(symbol);
                        if (newTree != null)
                            parseTree = newTree;
                    }
                    index++;
                }
                return true;
            }
            catch (InvalidExpressionException)
            {
                return false;
            }
        }


    }
}
    public class InvalidExpressionException :Exception
    {
        
    }
