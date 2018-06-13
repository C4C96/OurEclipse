using System;
using System.Collections.Generic;
using System.Linq;

namespace Compiler
{
	public class Node
    {
        public string Symbol { get; set; }        
        public bool IsChecked { get; set; } = false;

		public Node Parent { get; set; }
		public List<Node> Children { get; } = new List<Node>();

		public Node(string s)
        {
            Symbol = s;
        }

        public void Add(Node n)
        {
            Children.Add(n);
            n.Parent = this;
        }

        public bool HasChild()
        {
            return Children.Count() != 0;
        }
    }
}
