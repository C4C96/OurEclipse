namespace RE2DFA
{
    public class NFA_Node :Node
    {
        public new NFA_Node[] sons;
        public Automata automata;
        public NFA_Node()
        {

        }
        public NFA_Node(Node node)
        {
            this.operation = node.operation;
            this.parent = null;
            this.sonNum = node.sonNum;
            this.value = node.value;
            sons = new NFA_Node[2];
            if (node.sons[0] != null)
                sons[0] =new NFA_Node(node.sons[0]);
            if (node.sons[1] != null)
                sons[1] =new NFA_Node(node.sons[1]);
        }
        public void BuildAutomata()
        {
            automata = new Automata();
            if (sonNum==0&&value!=null)
            {
                automata.entry = new State();
                automata.states.Add(automata.entry);
                State state1 = new State();
                State state2 = new State();
                State state3 = new State();
                automata.entry.nullTransfer.Add(state1);
                automata.states.Add(state1);
                state1.transfer.Add(this.value, state2);
                automata.states.Add(state2);
                state2.nullTransfer.Add(state3);
                automata.states.Add(state3);
                automata.terminators.Add(state3);
                return;
            }
            switch (operation.c)
            {
                case '+':
                    if (sons[0].automata == null)
                        sons[0].BuildAutomata();
                    if (sons[1].automata == null)
                        sons[1].BuildAutomata();
                    automata.entry = sons[0].automata.entry;
                    automata.terminator = sons[1].automata.terminator;
                    sons[0].automata.terminator.nullTransfer.Add(sons[1].automata.entry);
                    break;
                case '|':
                    if (sons[0].automata == null)
                        sons[0].BuildAutomata();
                    if (sons[1].automata == null)
                        sons[1].BuildAutomata();
                    automata.entry = new State();
                    automata.entry.nullTransfer.Add(sons[0].automata.entry);
                    automata.entry.nullTransfer.Add(sons[1].automata.entry);
                    automata.terminator = new State();
                    sons[0].automata.terminator.nullTransfer.Add(automata.terminator);
                    sons[1].automata.terminator.nullTransfer.Add(automata.terminator);
                    break;
                case '*':
                    if (sons[0].automata == null)
                        sons[0].BuildAutomata();
                    automata.entry = new State();
                    automata.entry.nullTransfer.Add(sons[0].automata.entry);
                    automata.terminator = new State();
                    automata.entry.nullTransfer.Add(automata.terminator);
                    sons[0].automata.terminator.nullTransfer.Add(automata.terminator);
                    sons[0].automata.terminator.nullTransfer.Add(sons[0].automata.entry);
                    break;

            }
        }
    }
}