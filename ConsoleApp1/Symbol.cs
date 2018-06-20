using System;
namespace RE2DFA
{
    public class Symbol
    {
        static int MAX = 1024;
        public String expression;
        public Symbol(bool _default)
        {
            unicode = new bool[MAX];
            for (int i = 0; i < MAX; i++)
                unicode[i] = _default;
        }
        public bool IsValid(char c)
        {
            return (unicode[c]);
        }
        public bool SetExpression(String expression)
        {
            this.expression = expression;
            String[] tokens;
            if (expression[0] == '!'&&expression.Length>1)
            {
                unicode = new bool[MAX];
                for (int i = 0; i < MAX; i++)
                    unicode[i] = true;
                if ((tokens = expression.Split(',')) != null)
                {
                    tokens[0] = tokens[0].Remove(0, 1);
                    foreach (String token in tokens)
                    {
                        if (token.Length == 1)
                        {
                            unicode[token[0]] = false;
                            continue;
                        }
                        if (token.Length == 3&&token[1] == '-')
                            for (int i = token[0]; i <= token[2]; i++)
                                unicode[i] = false;
                        else
                        {
                            for (int i = 0; i < MAX; i++)
                                unicode[i] = true;
                            return false;
                        }
                    }
                }
            }
            else
            if ((tokens = expression.Split(',')) != null)
            {
                unicode = new bool[MAX];
                for (int i = 0; i < MAX; i++)
                    unicode[i] = false;
                foreach (String token in tokens)
                {
                    if (token.Length == 1)
                    {
                        unicode[token[0]] = true;
                        continue;
                    }
                    if (token.Length == 3&&token[1] == '-')
                        for (int i = token[0]; i <= token[2]; i++)
                            unicode[i] = true;
                    else
                    {
                        for (int i = 0; i < MAX; i++)
                            unicode[i] = false;
                        return false;
                    }
                }
            }
            return true;
        }
        public Symbol(String expression)
        {
            SetExpression(expression);
        }
        public static Symbol NonDigital
        {
            get
            {
                Symbol val = new Symbol(true);
                for (int i = 0x30; i < 0x3A; i++)
                    val.unicode[i] = false;
                return val;
            }
        }
        public static Symbol Digital
        {
            get
            {
                Symbol val = new Symbol(false);
                for (int i = 0x30; i < 0x3A; i++)
                    val.unicode[i] = true;
                return val;
            }
        }
        public static Symbol Alphabet
        {
            get
            {
                Symbol val = new Symbol(false);
                for (int i = 0x41; i < 0x5B; i++)
                    val.unicode[i] = true;
                for (int i = 0x61; i < 0x7B; i++)
                    val.unicode[i] = true;
                return val;
            }
        }
        public static Symbol NonAlphabet
        {
            get
            {
                Symbol val = new Symbol(true);
                for (int i = 0x41; i < 0x5B; i++)
                    val.unicode[i] = false;
                for (int i = 0x61; i < 0x7B; i++)
                    val.unicode[i] = false;
                return val;
            }
        }
        public override bool Equals(object obj)
        {
            Symbol symbolB = (Symbol)obj;
            for (int i = 0; i < MAX; i++)
                if (symbolB.unicode[i] != unicode[i])
                    return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool[] unicode;

    }

}