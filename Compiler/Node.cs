using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Node
    {
        public string symbol;
        public Node parent;
        public List<Node> children = new List<Node>();
        public bool isChecked = false;
        public Node(string s)
        {
            symbol = s;
        }

        public void Add(Node n)
        {
            children.Add(n);
            n.parent = this;
        }

        public bool hasChild()
        {
            return children.Count() != 0;
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
                    Console.Write(tmp.symbol + " ");
                    for (int j = 0; j < tmp.children.Count; j++)
                    {
                        q.Enqueue(tmp.children[j]);
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
