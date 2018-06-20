using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE2DFA;
namespace Tests
{
    [TestClass()]
    public class SymbolTests
    {
        [TestMethod()]
        public void SymbolTest()
        {
            Symbol symbol = Symbol.NonAlphabet;
            Assert.AreEqual(symbol.IsValid('c'), false);
            Assert.AreEqual(symbol.IsValid('2'), true);
            Assert.AreEqual(symbol.IsValid('-'), true);
            
        }

        [TestMethod()]
        public void SymbolTest1()
        {
            Symbol symbol = Symbol.Alphabet;
            Assert.AreEqual(symbol.IsValid('c'), true);
            Assert.AreEqual(symbol.IsValid('2'), false);
            Assert.AreEqual(symbol.IsValid('-'), false);
        }
        [TestMethod()]
        public void SymbolTest2()
        {
            Symbol symbol = Symbol.Digital;
            Assert.AreEqual(symbol.IsValid('c'), false);
            Assert.AreEqual(symbol.IsValid('2'), true);
            Assert.AreEqual(symbol.IsValid('-'), false);
        }
        [TestMethod()]
        public void SymbolTest3()
        {
            Symbol symbol = Symbol.NonDigital;
            Assert.AreEqual(symbol.IsValid('c'), true);
            Assert.AreEqual(symbol.IsValid('2'), false);
            Assert.AreEqual(symbol.IsValid('-'), true);
        }
        [TestMethod()]
        public void SymbolTest4()
        {
            Symbol symbol =new Symbol("3-9");
            Assert.AreEqual(symbol.IsValid('c'), false);
            Assert.AreEqual(symbol.IsValid('2'), false);
            Assert.AreEqual(symbol.IsValid('4'), true);
        }
        [TestMethod()]
        public void SymbolTest5()
        {
            Symbol symbol = new Symbol("3-9,a-r");
            Assert.AreEqual(symbol.IsValid('c'), true);
            Assert.AreEqual(symbol.IsValid('2'), false);
            Assert.AreEqual(symbol.IsValid('4'), true);
            Assert.AreEqual(symbol.IsValid('R'), false);
        }
        [TestMethod()]
        public void SymbolTest6()
        {
            Symbol symbol = new Symbol("!3-9,a-r,P-Q");
            Assert.AreEqual(symbol.IsValid('c'), false);
            Assert.AreEqual(symbol.IsValid('2'), true);
            Assert.AreEqual(symbol.IsValid('4'), false);
            Assert.AreEqual(symbol.IsValid('P'), false);
        }
        [TestMethod()]
        public void SymbolTest7()
        {
            Symbol symbol = new Symbol("c");
            Assert.AreEqual(symbol.IsValid('c'), true);
            Assert.AreEqual(symbol.IsValid('2'), false);
            Assert.AreEqual(symbol.IsValid('4'), false);
            Assert.AreEqual(symbol.IsValid('P'), false);
        }
        [TestMethod()]
        public void SymbolTest8()
        {
            Symbol symbol = new Symbol("!c");
            Assert.AreEqual(symbol.IsValid('c'), false);
            Assert.AreEqual(symbol.IsValid('2'), true);
            Assert.AreEqual(symbol.IsValid('4'), true);
            Assert.AreEqual(symbol.IsValid('P'), true);
        }
        [TestMethod()]
        public void SymbolTest9()
        {
            Symbol symbol = new Symbol("(");
            Assert.AreEqual(symbol.IsValid('c'), false);
            Assert.AreEqual(symbol.IsValid('('), true);
        }
    }
}