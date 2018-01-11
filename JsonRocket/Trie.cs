using System;
using System.Text;

namespace JsonRocket
{
    public sealed class Trie
    {
        private readonly Node _root;

        public Trie()
        {
            _root = new Node(null);
        }

        public int Count { get; private set; }

        public void Add(string path)
        {
            var current = _root;
            var buffer = Encoding.UTF8.GetBytes(path);
            for (int i = 0; i < buffer.Length; i++)
            {
                var b = buffer[i];
                if (current.Nodes[b] == null)
                {
                    current = current.Nodes[b] = new Node(current);
                    var str = Encoding.UTF8.GetString(buffer, i, 1);
                    current.Value = $"{current.Parent?.Value}{str}";
                }
                else
                {
                    current = current.Nodes[b];
                }
            }

            current.IsMatch = true;
            current.Value = path;
            Count++;
        }

        public Node Find(byte[] buffer, Node start = null)
        {
            return Find(buffer, 0, buffer.Length, start);
        }

        public Node Find(ArraySegment<byte> buffer, Node start = null)
        {
            return Find(buffer.Array, buffer.Offset, buffer.Count, start);
        }

        public Node Find(byte[] buffer, int index, int count, Node start = null)
        {
            var node = start ?? _root;
            for (int i = index; i < index + count; i++)
            {
                node = node.Nodes[buffer[i]];
                if (node == null) break;
            }

            return node;
        }

        public Node Find(byte b, Node start = null)
        {
            var node = start ?? _root;
            return node.Nodes[b];
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
            public bool IsMatch { get; internal set; }
            public string Value { get; internal set; }
        }
    }
}