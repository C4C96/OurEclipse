using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RE2DFA;

namespace Tests
{
    [TestClass()]
    public class NFATest
    {
        [TestMethod()]
        public void test1()
        {
            String re = "a*$";
            Re_Resolver re_resolver = new Re_Resolver();
            if (!re_resolver.Resolve(re))
            {
                Console.WriteLine("Incorrect Format of RE");
                return;
            }
            State.count = 0;
            NFA_ParseTree nfa = new NFA_ParseTree(re_resolver.parseTree);
            Assert.IsTrue(StartNFA(nfa.automata,"aa"));
        }
        [TestMethod()]
        public void test2()
        {
            String re = "a*b$";
            Re_Resolver re_resolver = new Re_Resolver();
            if (!re_resolver.Resolve(re))
            {
                Console.WriteLine("Incorrect Format of RE");
                return;
            }
            State.count = 0;
            NFA_ParseTree nfa = new NFA_ParseTree(re_resolver.parseTree);
            Assert.IsTrue(StartNFA(nfa.automata, "aab"));
            Assert.IsFalse(StartNFA(nfa.automata, "aa"));
        }
        [TestMethod()]
        public void test3()
        {
            String re = "a*(b|c)$";
            Re_Resolver re_resolver = new Re_Resolver();
            if (!re_resolver.Resolve(re))
            {
                Console.WriteLine("Incorrect Format of RE");
                return;
            }
            State.count = 0;
            NFA_ParseTree nfa = new NFA_ParseTree(re_resolver.parseTree);
            Assert.IsTrue(StartNFA(nfa.automata, "aab"));
            Assert.IsTrue(StartNFA(nfa.automata, "aac"));
        }
        [TestMethod()]
        public void test4()
        {
            String re = "(aa*(b|c))*$";
            Re_Resolver re_resolver = new Re_Resolver();
            if (!re_resolver.Resolve(re))
            {
                Console.WriteLine("Incorrect Format of RE");
                return;
            }
            State.count = 0;
            NFA_ParseTree nfa = new NFA_ParseTree(re_resolver.parseTree);
            Assert.IsFalse(StartNFA(nfa.automata, "aabcb"));
            Assert.IsTrue(StartNFA(nfa.automata, "aab"));
            Assert.IsTrue(StartNFA(nfa.automata, ""));
        }
        [TestMethod()]
        public void test5()
        {
            String re = "(ab*c|a)(ab)*$";
            Re_Resolver re_resolver = new Re_Resolver();
            if (!re_resolver.Resolve(re))
            {
                Console.WriteLine("Incorrect Format of RE");
                Assert.Fail();
                return;
            }
            State.count = 0;
            NFA_ParseTree nfa = new NFA_ParseTree(re_resolver.parseTree);
            Assert.IsTrue(StartNFA(nfa.automata, "abbbcabab"));
            Assert.IsTrue(StartNFA(nfa.automata, "aab"));
            Assert.IsTrue(StartNFA(nfa.automata, "a"));
            Assert.IsFalse(StartNFA(nfa.automata, "ab"));
        }
        //如果nfa有一个路径可以接受input且终止，则返回true，否则返回false；
        public bool StartNFA(Automata nfa,string input)
        {
            Dictionary<State, SortedSet<int>> dict = new Dictionary<State, SortedSet<int>>();
            //从[state,index]处开始跑
            bool runNFA(State state,int index)
            {
                if (index == input.Length && state == nfa.terminator)
                    return true;
                foreach (State next in state.nullTransfer)
                {
                    if (!dict.ContainsKey(next))
                        dict.Add(next, new SortedSet<int>());
                    if (!dict[next].Contains(index))
                    {
                        dict[next].Add(index);
                        if (runNFA(next, index))
                            return true;
                    }
                }
                if (index < input.Length)
                foreach(Symbol symbol in state.transfer.Keys)
                {
                    if (symbol.IsValid(input[index]))
                    {
                        if (!dict.ContainsKey(state.Transfer(symbol)))
                            dict.Add(state.Transfer(symbol), new SortedSet<int>());
                        if (!dict[state.Transfer(symbol)].Contains(index + 1))
                        {
                            dict[state.Transfer(symbol)].Add(index + 1);
                            if (runNFA(state.Transfer(symbol), index + 1))
                                return true;
                        }
                    }
                }
                return false;
            }
            dict.Add(nfa.entry, new SortedSet<int>());
            return runNFA(nfa.entry, 0);
        }
    }
}
