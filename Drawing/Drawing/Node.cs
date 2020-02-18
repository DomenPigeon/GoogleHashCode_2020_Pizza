using System;
using System.Collections.Generic;

namespace Drawing {
    public class Node : Node<int> {
        public Node(int value) : base(value) {
        }
    }

    public class Node<T> {
        public Node(T value) {
            Value = value;
        }

        public T Value { get; }
        public Node<T> Parent { get; private set; }
        private List<Node<T>> Children { get; } = new List<Node<T>>();

        public Node<T> Root {
            get {
                var root = this;
                while (root.Parent != null) {
                    root = root.Parent;
                }

                return root;
            }
        }

        public Node<T> RootVerbose {
            get {
                var root = this;
                var indent = 0;
                while (root.Parent != null) {
                    Console.WriteLine(root.ToString(new string('*', indent++)));
                    root = root.Parent;
                }

                Console.WriteLine(root.ToString(new string('*', indent++)));
                return root;
            }
        }

        public void AddChild(Node<T> child) {
            if (Children.IndexOf(child) >= 0) {
                if (child.Parent == this) return;
                if (child.Parent != this)
                    throw new Exception("Child has the wrong parent, something is off ...");
            }

            Children.Add(child);
            child.Parent = this;
        }

        public void AddChildren(Node<T>[] children) {
            foreach (var child in children) {
                AddChild(child);
            }
        }

        public void SetParent(Node<T> parent) {
            if (IsNullParent()) return;
            if (IsAlreadyParent()) return;

            if (parent.Children.IndexOf(this) >= 0)
                throw new Exception("Parent has this child, but is not parent ... Something is wrong!");

            Parent = parent;
            parent.Children.Add(this);

            bool IsNullParent() {
                if (parent == null) {
                    Parent = null;
                    return true;
                }

                return false;
            }

            bool IsAlreadyParent() {
                return Parent == parent;
            }
        }

        public IEnumerable<Node<T>> DFS {
            get {
                IEnumerable<Node<T>> traverse(Node<T> current) {
                    if (current.Children.Count > 0) {
                        foreach (var child in current.Children) {
                            foreach (var grandChild in traverse(child)) {
                                yield return grandChild;
                            }
                        }
                    }

                    yield return current;
                }
                return traverse(Root);
            }
        }

        public IEnumerable<Node<T>> BFS {
            get {
                IEnumerable<Node<T>> traverse(Node<T> current) {
                    yield return current;
                    if (current.Children.Count > 0) {
                        foreach (var child in current.Children) {
                            yield return child;
                            foreach (var grandchild in traverse(child)) {
                                yield return grandchild;
                            }
                        }
                    }
                }

                return traverse(Root);
            }
        }

        public override string ToString() {
            return Value.ToString();
        }

        public string ToString(string indent) {
            return indent + Value.ToString();
        }
    }
}