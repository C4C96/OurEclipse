using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RE2DFA;
namespace Tests
{
    [TestClass()]
    public class DFATest
    {
        [TestMethod()]
        public void DFAtest1()
        {
            DFA dfa = DFA_Builder.BuildDFA("a*$");
            Assert.IsFalse(dfa.states.Count > 2);
            Assert.IsTrue(StartDFA(dfa, "aa"));
        }
        [TestMethod()]
        public void DFAtest2()
        {
            DFA dfa=DFA_Builder.BuildDFA("a*b$");
            
            Assert.IsTrue(StartDFA(dfa, "aab"));
            Assert.IsFalse(StartDFA(dfa, "aa"));
        }
        [TestMethod()]
        public void DFAtest3()
        {
            DFA dfa = DFA_Builder.BuildDFA("a*(b|c)$");
            Assert.IsTrue(StartDFA(dfa, "aab"));
            Assert.IsTrue(StartDFA(dfa, "aac"));
        }
        [TestMethod()]
        public void DFAtest4()
        {
            DFA dfa = DFA_Builder.BuildDFA("(aa*(b|c))*$");
            Assert.IsFalse(StartDFA(dfa, "aabcb"));
            Assert.IsTrue(StartDFA(dfa, "aab"));
            Assert.IsTrue(StartDFA(dfa, ""));
        }
        [TestMethod()]
        public void DFAtest5()
        {
            DFA dfa = DFA_Builder.BuildDFA("(ab*c|a)(ab)*$");
            Assert.IsTrue(StartDFA(dfa, "abbbcabab"));
        }

        public bool StartDFA(Automata dfa, string input)
        {
            //从[state,index]处开始跑
            bool runDFA(State state, int index)
            {
                if (index == input.Length)
                    if (dfa.terminators.Exists(terminator => state == terminator))
                        return true;
                    else
                        return false;
                State next = null;
                if (null != (next = state.Transfer(input[index])))
                    return runDFA(next, index + 1);
                return false;
            }
            return runDFA(dfa.entry, 0);
        }
    }
}
