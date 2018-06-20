using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE2DFA;
namespace Tests
{
    [TestClass]
    public class Re_ResolverTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            Re_Resolver re = new Re_Resolver();
            re.Resolve("a$");
            Assert.AreEqual(new Symbol("a"),re.parseTree.root.value);
        }
        [TestMethod]
        public void TestMethod2()
        {
            Re_Resolver re = new Re_Resolver();
            re.Resolve("a+b$");
            Assert.AreEqual(new Symbol("b"), re.parseTree.root.sons[1].value);
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
        }
        [TestMethod]
        public void TestMethod3()
        {
            Re_Resolver re = new Re_Resolver();
            re.Resolve("(a)$");
            Assert.AreEqual(new Symbol("a"), re.parseTree.root.value);
        }
        [TestMethod]
        public void TestMethod4()
        {
            Re_Resolver re = new Re_Resolver();
            re.Resolve("(a+b)$");
            Assert.AreEqual(new Symbol("b"), re.parseTree.root.sons[1].value);
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
        }
        [TestMethod]
        public void TestMethod5()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("ab$"));
            Assert.AreEqual(new Symbol("b"), re.parseTree.root.sons[1].value);
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
        }
        [TestMethod]
        public void TestMethod6()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("a|bc$"));
            Assert.AreEqual(new Symbol("b"), re.parseTree.root.sons[1].sons[0].value);
            Assert.AreEqual(Operation.Or, re.parseTree.root.operation);
            Assert.AreEqual(Operation.And, re.parseTree.root.sons[1].operation);
        }
        [TestMethod]
        public void TestMethod7()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("(a|b)c$"));
            Assert.AreEqual(new Symbol("b"), re.parseTree.root.sons[0].sons[1].value);
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
            Assert.AreEqual(Operation.Or, re.parseTree.root.sons[0].operation);
            Assert.AreEqual(new Symbol("c"), re.parseTree.root.sons[1].value);
        }
        [TestMethod]
        public void TestMethod8()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("(a|b)*$"));
            Assert.AreEqual(new Symbol("b"), re.parseTree.root.sons[0].sons[1].value);
            Assert.AreEqual(Operation.Star, re.parseTree.root.operation);
            Assert.AreEqual(Operation.Or, re.parseTree.root.sons[0].operation);
        }
        [TestMethod]
        public void TestMethod9()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("(a|[b,1-9])*$"));
            Assert.AreEqual(new Symbol("b,1-9"), re.parseTree.root.sons[0].sons[1].value);
            Assert.AreEqual(Operation.Star, re.parseTree.root.operation);
            Assert.AreEqual(Operation.Or, re.parseTree.root.sons[0].operation);
        }
        [TestMethod]
        public void TestMethod10()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("(a|[b,1-9]r)*(a|b)$"));
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
            Assert.AreEqual(Operation.Star, re.parseTree.root.sons[0].operation);
            Assert.AreEqual(Operation.Or, re.parseTree.root.sons[1].operation);
            Assert.AreEqual(new Symbol("b,1-9"), re.parseTree.root.sons[0].sons[0].sons[1].sons[0].value);
        }
        [TestMethod]
        public void TestMethod11()
        {
            Re_Resolver re = new Re_Resolver();
            bool result=re.Resolve(")");
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void TestMethod12()
        {
            Re_Resolver re = new Re_Resolver();
            bool result = re.Resolve("a+$");
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void TestMethod13()
        {
            Re_Resolver re = new Re_Resolver();
            bool result = re.Resolve("(a|b))$");
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void TestMethod14()
        {
            Re_Resolver re = new Re_Resolver();
            bool result = re.Resolve("(a+|b))$");
            Assert.AreEqual(false, result);
        }
        [TestMethod]
        public void TestMethod15()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("(aa*(b|c))*$"));
            Assert.AreEqual(Operation.Star, re.parseTree.root.operation);
            Assert.AreEqual(Operation.And, re.parseTree.root.sons[0].operation);
            Assert.AreEqual(Operation.And, re.parseTree.root.sons[0].sons[0].operation);
        }
        [TestMethod]
        public void TestMethod16()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("(ab*c|a)(ab)*$"));
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
            Assert.AreEqual(Operation.Or, re.parseTree.root.sons[0].operation);
            Assert.AreEqual(Operation.Star, re.parseTree.root.sons[1].operation);
        }
        [TestMethod]
        public void TestMethod17()
        {
            Re_Resolver re = new Re_Resolver();
            Assert.IsTrue(re.Resolve("import$"));
            Assert.AreEqual(Operation.And, re.parseTree.root.operation);
        }
    }
}
