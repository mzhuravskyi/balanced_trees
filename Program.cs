using System.Diagnostics;

interface IKeyValuePair<TKey, TValue>
{
    void Insert(TKey key, TValue value);
    void Delete(TKey key);
    TValue GetValue(TKey key);
}

abstract class BST<TKey, TValue> : IKeyValuePair<TKey, TValue> where TKey : IComparable<TKey>
{
    protected class Node
    {
        public Node? right;
        public Node? left;
        public TKey key;
        public TValue value;

        public Node(TKey key, TValue value, Node? right, Node? left)
        {
            this.key = key;
            this.value = value;
            this.right = right;
            this.left = left;
        }
    }

    Node? root = null;
    int count = 0;

    public TValue GetValue(TKey key)
    {
        Debug.Assert(root != null, "empty tree");
        Node? curr = root;
    
        while (curr != null)
        {
            int a = curr.key.CompareTo(key);
            if (a < 0)
            {
                curr = curr.right;
            }
            else if (a > 0)
            {
                curr = curr.left;
            }
            else
            {
                return curr.value;
            }
        }

        throw new KeyNotFoundException($"{key} not found");
    }
    // public TKey GetNextKey(TKey current_key);
    public TValue GetMaxValue(out TKey max_key)
    {
        Debug.Assert(root != null, "empty tree");
        Node curr = root;

        while (curr.right != null) 
        {
            curr = curr.right;
        }
        max_key = curr.key;

        return curr.value;
    }
    public TValue GetMinValue(out TKey min_key)
    {
        Debug.Assert(root != null, "empty tree");
        Node curr = root;

        while(curr.left != null)
        {
            curr = curr.left;
        }
        min_key = curr.key;

        return curr.value;
    }
    public List<TKey> InOrderKeys()
    {
        Debug.Assert(root != null, "empty tree");
        List<TKey> keys = new List<TKey>();
        Stack<Node> stack = new Stack<Node>();
        Node? curr = root;

        while (curr != null || stack.Count > 0)
        {
            while (curr != null)
            {
                stack.Push(curr);
                curr = curr.left;
            }
            curr = stack.Pop();
            keys.Add(curr.key);
            curr = curr.right;
        }

        return keys;
    }
    public List<TValue> InOrderValues() {
        Debug.Assert(root != null, "empty tree");
        List<TValue> keys = new List<TValue>();
        Stack<Node> stack = new Stack<Node>();
        Node? curr = root;
        
        while (curr != null || stack.Count > 0)
        {
            while (curr != null)
            {
                stack.Push(curr);
                curr = curr.left;
            }
            curr = stack.Pop();
            keys.Add(curr.value);
            curr = curr.right;
        }

        return keys;
    }
    public List<(TKey, TValue)> InOrderKVP() {
        Debug.Assert(root != null, "empty tree");
        List<(TKey, TValue)> kvps = new List<(TKey, TValue)>();
        Stack<Node> stack = new Stack<Node>();
        Node? curr = root;
        
        while (curr != null || stack.Count > 0)
        {
            while (curr != null)
            {
                stack.Push(curr);
                curr = curr.left;
            }
            curr = stack.Pop();
            kvps.Add((curr.key, curr.value));
            curr = curr.right;
        }

        return kvps;
    }
    

    public abstract void Insert(TKey key, TValue value);
    public abstract void Delete(TKey key);
    
}

class AVLTree<TKey, TValue> : BST<TKey, TValue> where TKey : IComparable<TKey>
{
    class AVLNode : Node
    {
        
    }

    public override void Insert(TKey key, TValue value);
    public override void Delete(TKey key);
}

class RBTree<TKey, TValue> : BST<TKey, TValue> where TKey : IComparable<TKey>
{
    class RBNode : Node
    {
        
    }

    public override void Insert(TKey key, TValue value);
    public override void Delete(TKey key);
}

class GenericDicitonary<TKey, TValue>
{
    IKeyValuePair<TKey, TValue> structure;
}

class GenericSet<T>
{
    IKeyValuePair<T, T> structure;
}

