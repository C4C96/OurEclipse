using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lexical_Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexical_Analyzer.Tests
{
    [TestClass()]
    public class Lexical_Analyzer_Test
    {
        String path = "C:\\Users\\bao\\Desktop\\LexicalRules.rules";
        [TestMethod()]
        public void LoadRulesLA_test()
        {
            LexicalAnalyzer la = new LexicalAnalyzer();
            if (!la.LoadRules(path))
                Assert.Fail();
            la.Analyze("import iopew 2;");
            Assert.AreEqual(la.data[0].property, "Import");
            Assert.AreEqual(la.data[1].value, "iopew");
            Assert.AreEqual(la.data[2].value, "2");
            Assert.AreEqual(la.data[3].value, ";");
            
        }
        [TestMethod()]
        public void LoadRulesLA_test1()
        {
            LexicalAnalyzer la = new LexicalAnalyzer();
            if (!la.LoadRules(path))
                Assert.Fail();
            la.Analyze("sdaew rwo;ro!>=rep[]]]<*=" +
                "dsew" +
                "f;");
            Assert.AreEqual(la.data[0].property, "ID");
            Assert.AreEqual(la.data[1].value, "rwo");
            Assert.AreEqual(la.data[2].value, ";");
            Assert.AreEqual(la.data[4].property, "Not");
            Assert.AreEqual(la.data[5].value, ">=");

        }

    }
}