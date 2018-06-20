using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE2DFA
{
    /*
     * 用于NFA生成DFA，在State基础上增加了一些属性信息，包括
     * ε-closure(move(State, Symbol))
     * move(State, Symbol）
        */
    public class IState : State
    {
        public new Dictionary<Symbol, IState> transfer;
        public List<State> collection;
        public bool terminate = false;
        public IState(int num) : base(num)
        {
        }
        public override bool Equals(object obj)
        {
            IState other = (IState)obj;
            if (other.collection == null || collection == null||collection.Count!=other.collection.Count)
                return false;
            foreach (State state in other.collection)
                if (!collection.Contains<State>(state))
                    return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
