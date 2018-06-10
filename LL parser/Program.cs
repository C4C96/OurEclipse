using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LexDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "{\r\ni = 10;\r\nj = 100;\r\nn = 1;\r\nsum = 0;\r\nmult = 1;\r\nwhile (i > 0) { n = n + 1; i = i - 1; ; }\r\nif (j >= 50) then sum = sum + j; else { mult = mult * (j + 1); sum = sum + i; n}\r\nif (i <= 10) then sum = sum - i; else mult = mult + i / 2;\r\nif (i == j) then sum = sum - j; else mult = mult - j / 2;\r\nif (n > 1) then n = n - 1; else n = n + 1;\r\nif (n < 2) then n = n + 2; else n = n - 2;\r\n}\r\n";
            LexAna la = new LexAna(input);
            la.print();

            LL a = new LL(@"C:\Users\zhao\Desktop\1.txt");
            a.result(@"C:\Users\zhao\Desktop\2.txt");

            Parser p = new Parser(a.nonTerminalSet, a.terminalSet, a.table, la.Token);
            p.analyze();
            Console.ReadKey();
        }
    }
}
