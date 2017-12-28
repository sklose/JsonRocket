using System;
using System.Text;

namespace JsonRocket
{
    public class Trie
    {
        private readonly Node _root;

        public Trie()
        {
            _root = new Node(null);
        }
        
        public void Add(string path)
        {
            var current = _root;
            var buffer = Encoding.UTF8.GetBytes(path);
            foreach (var b in buffer)
            {
                if (current.Nodes[b] == null)
                {
                    current = current.Nodes[b] = new Node(current);
                }
            }

            current.Value = path;
        }

        public Result Find(byte[] buffer, Node start = null)
        {
            return Find(buffer, 0, buffer.Length, start);
        }

        public Result Find(ArraySegment<byte> buffer, Node start = null)
        {
            return Find(buffer.Array, buffer.Offset, buffer.Count);
        }

        public Result Find(byte[] buffer, int index, int count, Node start = null)
        {
            var result = new Result
            {
                Node = start ?? _root
            };

            for (int i = index; i < index + count; i++)
            {
                result.Node = result.Node.Nodes[buffer[i]];
                if (result.Node == null) break;
                result.StepsTaken++;
            }

            return result;
        }
        
        public struct Result
        {
            public int StepsTaken { get; set; }
            public Node Node { get; set; }
        }

        public class Node
        {
            public Node(Node parent)
            {
                Parent = parent;
                Nodes = new Node[byte.MaxValue];
            }

            public Node Parent { get; }
            public Node[] Nodes { get; }
            public bool IsMatch => Value != null;
            public string Value { get; internal set; }
        }
    }
}
