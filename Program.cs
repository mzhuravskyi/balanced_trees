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
        public Node? left;
        public Node? right;
        public TKey key;
        public TValue value;

        public Node(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
            this.left = null;
            this.right = null;
        }
    }

    Node? root = null;
    protected int count = 0;

    public int Count => count;

    public TValue GetValue(TKey key)
    {
        Debug.Assert(root != null, "empty tree");
        Node? current = root;
    
        while (current != null)
        {
            int a = current.key.CompareTo(key);
            if (a < 0)
            {
                current = current.right;
            }
            else if (a > 0)
            {
                current = current.left;
            }
            else
            {
                return current.value;
            }
        }

        throw new KeyNotFoundException($"{key} not found");
    }
    // public TKey GetNextKey(TKey currentent_key);
    public TValue GetMaxValue(out TKey max_key)
    {
        Debug.Assert(root != null, "empty tree");
        Node current = root;

        while (current.right != null) 
        {
            current = current.right;
        }
        max_key = current.key;

        return current.value;
    }
    public TValue GetMinValue(out TKey min_key)
    {
        Debug.Assert(root != null, "empty tree");
        Node current = root;

        while(current.left != null)
        {
            current = current.left;
        }
        min_key = current.key;

        return current.value;
    }
    public List<TKey> InOrderKeys()
    {
        Debug.Assert(root != null, "empty tree");
        List<TKey> keys = new List<TKey>();
        Stack<Node> stack = new Stack<Node>();
        Node? current = root;

        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.left;
            }
            current = stack.Pop();
            keys.Add(current.key);
            current = current.right;
        }

        return keys;
    }
    public List<TValue> InOrderValues() {
        Debug.Assert(root != null, "empty tree");
        List<TValue> values = new List<TValue>();
        Stack<Node> stack = new Stack<Node>();
        Node? current = root;
        
        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.left;
            }
            current = stack.Pop();
            values.Add(current.value);
            current = current.right;
        }

        return values;
    }
    public List<(TKey, TValue)> InOrderKVP() {
        Debug.Assert(root != null, "empty tree");
        List<(TKey, TValue)> kvps = new List<(TKey, TValue)>();
        Stack<Node> stack = new Stack<Node>();
        Node? current = root;
        
        while (current != null || stack.Count > 0)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.left;
            }
            current = stack.Pop();
            kvps.Add((current.key, current.value));
            current = current.right;
        }

        return kvps;
    }
    

    public abstract void Insert(TKey key, TValue value);
    public abstract void Delete(TKey key);
    
}

class AVLTree<TKey, TValue> : BST<TKey, TValue> where TKey : IComparable<TKey>
{
    protected class AVLNode : Node
    {
        public int height;

        public AVLNode(TKey key, TValue value, int height) : base(key, value)
        {
            this.height = height;
        }
    }
    void RotationLeft()
    {
        
    }
    void RotationRight()
    {
        
    }
    void DoubleRotationLeft()
    {
        
    }
    void DoubleRotationRight()
    {
        
    }
    void UpdateHeight(AVLNode node)
    {
        node.height = Math.Max((AVLNode) node.right.height, (AVLNode) node.left.height) + 1;
    }
    void CheckBalance(AVLNode node)
    {
        
    }
    void FixHeight()
    {
        
    }
    AVLNode InsertReal(AVLNode node, TKey key, TValue value)
    {
        if (node == null)
        {
            count++;
            return new AVLNode(key, value, 0);
        }

        int a = node.key.CompareTo(key);
        if (a < 0)
        {
            node.right = InsertReal((AVLNode) node.right!, key, value);         // how to avoid typecasting?
        }
        else if (a > 0)
        {
            node.left = InsertReal((AVLNode) node.left!, key, value);            // typecasting again
        }
        else
        {
            return node;
        }

        CheckBalance(node);
        FixHeight(node);

        return node;
    }

    public override void Insert(TKey key, TValue value)
    {
        
    }
    public override void Delete(TKey key)
    {
        
    }
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

