using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE2DFA
{
    public class ParseTree
    {
        public Node root;
        public List<Node> nodes;
        //外部优先级
        public int priority;
        //当前所在节点
        public Node currentNode;
        public ParseTree parentTree;
        //是否在括号内
        public bool bracketed;
        public ParseTree()
        {
            priority = 0;
            nodes = new List<Node>();
            bracketed = false;
        }
        public bool CombineTree(ParseTree subTree,Node node)
        {
            if (!nodes.Contains(node)||subTree==null)
                return false;
            
            node.sonNum = subTree.root.sonNum;
            node.operation = subTree.root.operation;
            node.value = subTree.root.value;
            node.sons = subTree.root.sons;
            return true;
        }
        public ParseTree Append(Symbol symbol)
        {
            if (currentNode == null)
            {
                currentNode = new Node();
                currentNode.value = symbol;
                nodes.Add(currentNode);
                return null;
            }
            else
            {
                ParseTree output = this;
                if ((currentNode.sonNum == 0 && currentNode.value != null) || (currentNode.sonNum == currentNode.operation.sonNum))
                    output = Append(Operation.And);
                ParseTree subTree = new ParseTree();
                subTree.priority = output.currentNode.operation.priority;
                subTree.parentTree = output;
                subTree.Append(symbol);
                return subTree;
                
            }

        }
        public ParseTree Append(Operation operation)
        {
            if (currentNode==null)
                throw new InvalidExpressionException();
            if ((currentNode.sonNum == 0 && currentNode.value != null) || (currentNode.sonNum == currentNode.operation.sonNum))
            {
                if (operation!=null&&priority >= operation.priority)
                {
                    parentTree.BackTrack(this);
                    return parentTree.Append(operation);
                }
                currentNode.parent = new Node();
                currentNode.parent.sonNum = 1;
                currentNode.parent.sons[0] = currentNode;
                currentNode = currentNode.parent;
                nodes.Add(currentNode);
                currentNode.operation = operation;
                return this;
            }
            else
                throw new InvalidExpressionException();
        }
        public ParseTree AppendLeftBracket()
        {
            ParseTree output = this;
            if (currentNode != null && (currentNode.sonNum == 0 || (currentNode.operation != null && currentNode.sonNum == currentNode.operation.sonNum)))
            {
                output = Append(Operation.And);
            }
            ParseTree subTree = new ParseTree();
            subTree.priority = 0;
            subTree.parentTree = output;
            subTree.bracketed = true;
            return subTree;
        }
        public ParseTree AppendRightBracket()
        {
            ParseTree output = this;
            if (!output.bracketed)
            while (!output.bracketed)
            {
                if (output.parentTree == null)
                    throw new InvalidExpressionException();
                output.parentTree.BackTrack(output);
                output = output.parentTree;
            }
            output.bracketed = false;
            return output;
        }
        public void BackTrack(ParseTree subTree)
        {
            if (subTree.currentNode == null)
                return;
            if (currentNode == null)
            {
                subTree.SetRoot();
                currentNode = subTree.root;
                foreach (Node node in subTree.nodes)
                    this.nodes.Add(node);
                return;
            }
            if (currentNode.sonNum == 0)
                throw new InvalidExpressionException();
            subTree.SetRoot();
            subTree.root.parent = currentNode;
            currentNode.sonNum++;
            currentNode.sons[currentNode.sonNum-1] = subTree.root;
            foreach (Node node in subTree.nodes)
                this.nodes.Add(node);
        }
        public ParseTree AppendTerminator()
        {
            ParseTree output = this;

            while (output.parentTree!=null)
            {
                output.parentTree.BackTrack(output);
                output = output.parentTree;
            }
            output.SetRoot();
            return output;
        }
        public void SetRoot()
        {
            root = null;
            if (currentNode == null)
                return;
            root = currentNode;
            while (root.parent != null)
                root = root.parent;
            if (currentNode.operation != null && currentNode.sonNum != currentNode.operation.sonNum)
                throw new InvalidExpressionException();
        }
    }
    public class Node
    {
        public int sonNum;
        public Node[] sons;
        public Node parent;
        public Operation operation;
        public Symbol value;
        public Node()
        {
            sonNum = 0;
            parent = null;
            operation = null;
            value = null;
            sons = new Node[2];
        }
    }

}
