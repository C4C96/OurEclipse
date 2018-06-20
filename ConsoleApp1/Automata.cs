using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE2DFA
{
    public class Automata
    {
        public List<State> states;
        public State entry;
        public List<State> terminators;
        public State terminator { get { return terminators.ElementAt(0); } set { terminators.Clear(); terminators.Add(value); } } 
        public Automata()
        {
            states = new List<State>();
            terminators = new List<State>();
        }
    }
}
