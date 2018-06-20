using System.Collections.Generic;
namespace RE2DFA
{
    public class NFA_ParseTree :ParseTree
    {
        public new NFA_Node root;
        public Automata automata;
        public NFA_ParseTree(ParseTree parseTree)
        {
            root =new NFA_Node(parseTree.root);
            root.BuildAutomata();
            automata = root.automata;
            BuildStates();


        }
        void BuildStates()
        {
            automata.states = new List<State>();
            Recur(automata.entry);
            void Recur(State state)
            {
                automata.states.Add(state);
                foreach (State next in state.nullTransfer)
                {
                    if (!automata.states.Contains(next))
                        Recur(next);
                }
                foreach(State next in state.transfer.Values)
                {
                    if (!automata.states.Contains(next))
                        Recur(next);
                }
            }
        }

    }
}