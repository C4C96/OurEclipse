using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE2DFA
{
    public class Operation
    {
        public char c;

        public int sonNum;
        public int priority;
        public Operation(char c)
        {
            this.c = c;
            switch (c)
                {
                case '*':sonNum = 1; priority=6; break;
                case '|':sonNum = 2; priority = 4; break;
                case '+':sonNum = 2; priority = 5; break;
            }
        }
        public static Operation Star { get
            {
                return new Operation('*');
            }
        }
        public static Operation And{ get
            {
                return new Operation('+');
            }
        }
        public static Operation Or
        {
            get
            {
                return new Operation('|');
            }
        }
        public override bool Equals(object obj)
        {
            Operation operation = (Operation)(obj);
            return c==operation.c;
        }
        public override int GetHashCode()
        {
            return 100007+this.c;
        }
    }
}
