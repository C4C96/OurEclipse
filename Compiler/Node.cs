using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    public class Node
    {
        public string Symbol { get; set; }//用来输出的节点内容
        public bool IsChecked { get; set; } = false;
        public string Lexeme { get; set; } = "";//终结符的词位
        public string value { get; set; } = "";//节点的值
        public string syn { get; set; } = "";//节点的综合属性
        public string inh { get; set; } = "";//节点的继承属性
        public Env Environment { get; set; }//如果symbol是ID，那么为当前叶子节点所使用的符号表
        public Node Parent { get; set; }
        public List<Node> Children { get; } = new List<Node>();
        public int row { get; set; }
        public int col { get; set; }

        public bool HasChild
        {
            get
            {
                return Children.Count != 0;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return Lexeme != "";
            }
        }

        public Node(string s)
        {
            Symbol = s;
        }

        public Node(string s, string l)
        {
            Symbol = s;
            Lexeme = l;
        }


        public void Add(Node n)
        {
            Children.Add(n);
            n.Parent = this;
        }


        public void print(Node root)
        {
            Queue<Node> q = new Queue<Node>();
            q.Enqueue(root);
            while (q.Count != 0)
            {
                int count = q.Count;
                for (int i = 0; i < count; i++)
                {
                    Node tmp = q.Dequeue();
                    Console.Write(tmp.Lexeme + " ");
                    for (int j = 0; j < tmp.Children.Count; j++)
                    {
                        q.Enqueue(tmp.Children[j]);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
