using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE2DFA
{
    public class State
    {
        public Dictionary<Symbol,State> transfer;
        public List<State> nullTransfer;
        public int num;
        public static int count=0;
        public State()
        {
            num = count++;
            transfer = new Dictionary<Symbol, State>();
            nullTransfer = new List<State>();
        }
        public State(int num)
        {
            this.num = num;
            transfer = new Dictionary<Symbol, State>();
            nullTransfer = new List<State>();
        }
        public State Transfer(Symbol symbol)
        {
            return transfer[symbol];
        }
        public State Transfer(char c)
        {
            foreach(Symbol symbol in transfer.Keys)
            {
                if (symbol.IsValid(c))
                {
                    return transfer[symbol];
                }
            }
            return null;
        }
    }
    
}
