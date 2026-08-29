using System;

namespace NFramework.Core
{
    enum Color
    {
        Red,
        Black
    }

    class Node<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public Node<TKey, TValue> Left;
        public Node<TKey, TValue> Right;
        public Node<TKey, TValue> Parent;
        public Color Color;

        public Node(TKey key, TValue value)
        {
            Key = key;
            Value = value;
            Color = Color.Red;
        }
    }

    /// <summary>
    /// 红黑树
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class RedBlackTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private Node<TKey, TValue> _root;
        private Node<TKey, TValue> _nullNode;

        public RedBlackTree()
        {
            _nullNode = new Node<TKey, TValue>(default(TKey), default(TValue)) { Color = Color.Black };
            _root = _nullNode;
        }

        public void Insert(TKey key, TValue value)
        {
            Node<TKey, TValue> newNode = new Node<TKey, TValue>(key, value);
            Node<TKey, TValue> y = null;
            Node<TKey, TValue> x = _root;

            while (x != _nullNode)
            {
                y = x;
                if (newNode.Key.CompareTo(x.Key) < 0)
                    x = x.Left;
                else
                    x = x.Right;
            }

            newNode.Parent = y;
            if (y == null)
            {
                _root = newNode;
            }
            else if (newNode.Key.CompareTo(y.Key) < 0)
            {
                y.Left = newNode;
            }
            else
            {
                y.Right = newNode;
            }

            newNode.Left = _nullNode;
            newNode.Right = _nullNode;
            newNode.Color = Color.Red;

            FixInsert(newNode);
        }

        private void FixInsert(Node<TKey, TValue> k)
        {
            Node<TKey, TValue> u;
            while (k.Parent.Color == Color.Red)
            {
                if (k.Parent == k.Parent.Parent.Right)
                {
                    u = k.Parent.Parent.Left;
                    if (u.Color == Color.Red)
                    {
                        u.Color = Color.Black;
                        k.Parent.Color = Color.Black;
                        k.Parent.Parent.Color = Color.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Left)
                        {
                            k = k.Parent;
                            RotateRight(k);
                        }

                        k.Parent.Color = Color.Black;
                        k.Parent.Parent.Color = Color.Red;
                        RotateLeft(k.Parent.Parent);
                    }
                }
                else
                {
                    u = k.Parent.Parent.Right;

                    if (u.Color == Color.Red)
                    {
                        u.Color = Color.Black;
                        k.Parent.Color = Color.Black;
                        k.Parent.Parent.Color = Color.Red;
                        k = k.Parent.Parent;
                    }
                    else
                    {
                        if (k == k.Parent.Right)
                        {
                            k = k.Parent;
                            RotateLeft(k);
                        }

                        k.Parent.Color = Color.Black;
                        k.Parent.Parent.Color = Color.Red;
                        RotateRight(k.Parent.Parent);
                    }
                }

                if (k == _root) break;
            }

            _root.Color = Color.Black;
        }

        private void RotateLeft(Node<TKey, TValue> x)
        {
            Node<TKey, TValue> y = x.Right;
            x.Right = y.Left;
            if (y.Left != _nullNode)
                y.Left.Parent = x;

            y.Parent = x.Parent;
            if (x.Parent == null)
                _root = y;
            else if (x == x.Parent.Left)
                x.Parent.Left = y;
            else
                x.Parent.Right = y;

            y.Left = x;
            x.Parent = y;
        }

        private void RotateRight(Node<TKey, TValue> x)
        {
            Node<TKey, TValue> y = x.Left;
            x.Left = y.Right;
            if (y.Right != _nullNode)
                y.Right.Parent = x;

            y.Parent = x.Parent;
            if (x.Parent == null)
                _root = y;
            else if (x == x.Parent.Right)
                x.Parent.Right = y;
            else
                x.Parent.Left = y;

            y.Right = x;
            x.Parent = y;
        }

        public void InOrderWalk(Action<TKey, TValue> action)
        {
            InOrderWalk(_root, action);
        }

        private void InOrderWalk(Node<TKey, TValue> node, Action<TKey, TValue> action)
        {
            if (node == _nullNode) return;

            InOrderWalk(node.Left, action);
            action(node.Key, node.Value);
            InOrderWalk(node.Right, action);
        }
    }

    // 使用示例
    public class SortedMap<TKey, TValue> where TKey : IComparable<TKey>
    {
        private RedBlackTree<TKey, TValue> _tree = new RedBlackTree<TKey, TValue>();

        public void Insert(TKey key, TValue value)
        {
            _tree.Insert(key, value);
        }

        public void TraverseInOrder(Action<TKey, TValue> action)
        {
            _tree.InOrderWalk(action);
        }
    }
}