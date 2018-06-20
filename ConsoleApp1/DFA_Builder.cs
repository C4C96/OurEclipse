using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE2DFA
{
    public static class DFA_Builder
    {
        public static DFA BuildDFA(String re)
        {
            Re_Resolver re_resolver = new Re_Resolver();
            if (!re_resolver.Resolve(re))
            {
                Console.WriteLine("Incorrect Format of RE");
                return null;
            }
            State.count = 0;
            NFA_ParseTree nfa = new NFA_ParseTree(re_resolver.parseTree);
            State.count = 0;
            return new DFA(nfa.automata);
        }
        
    }
}
