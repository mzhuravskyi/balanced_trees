using System.Diagnostics;

interface IKeyValuePair<TKey, TValue>
{
    void Insert(TKey key, TValue value);
    void Delete(TKey key);
    TValue GetValue(TKey key);
}

abstract class BST<TKey, TValue> : IKeyValuePair<TKey, TValue> where TKey : IComparable<TKey>
{
    public class Node
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

    public Node? root = null;
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
    public class AVLNode : Node
    {
        public int height;

        public AVLNode(TKey key, TValue value, int height) : base(key, value)
        {
            this.height = height;
        }
    }
    AVLNode RotationRight(AVLNode ynode)
    {
        AVLNode xnode = (AVLNode) ynode.left!;

        ynode.left = xnode.right;
        xnode.right = ynode;

        UpdateHeight(xnode);
        UpdateHeight(ynode);

        return xnode;
    }
    AVLNode RotationLeft(AVLNode ynode)
    {
        AVLNode xnode = (AVLNode) ynode.right!;

        ynode.right = xnode.left;
        xnode.left = ynode;

        UpdateHeight(xnode);
        UpdateHeight(ynode);

        return xnode;
    }
    AVLNode DoubleRotationLeft(AVLNode znode)
    {
        AVLNode xnode = (AVLNode) znode.left!;
        AVLNode ynode = (AVLNode) xnode.right!;

        xnode.right = ynode.left;
        znode.left = ynode.right;
        ynode.right = znode;

        UpdateHeight(xnode);
        UpdateHeight(ynode);
        UpdateHeight(znode);

        return ynode;
    }
    AVLNode DoubleRotationRight(AVLNode znode)
    {
        AVLNode xnode = (AVLNode) znode.right!;
        AVLNode ynode = (AVLNode) xnode.left!;

        xnode.left = ynode.right;
        znode.right = ynode.left;
        ynode.left = znode;

        UpdateHeight(xnode);
        UpdateHeight(ynode);
        UpdateHeight(znode);

        return ynode;
    }
    int GetHeight(AVLNode? node)
    {
        return node == null ? -1 : node.height;
    }

    void UpdateHeight(AVLNode node)
    {
        node.height = Math.Max(GetHeight((AVLNode?) node.right), GetHeight((AVLNode?) node.left)) + 1; // ugly typecasting
    }
    int  GetBalance(AVLNode node)
    {
        return GetHeight((AVLNode?) node.right) - GetHeight((AVLNode?) node.left);
    }
    AVLNode RebalanceHeight(AVLNode node)
    {
        int balance = GetBalance(node);
        switch (balance)
        {
            case -2:
                int lnode_balance = GetBalance((AVLNode) node.left!);

                if (lnode_balance == -1)
                {
                    return RotationRight(node);
                }
                else
                {
                    return DoubleRotationRight(node);
                }
            case 2:
                int rnode_balance = GetBalance((AVLNode) node.right!);
                
                if (rnode_balance == 1)
                {
                    return RotationLeft(node);
                }
                else
                {
                    return DoubleRotationLeft(node);
                }
            default:
                return node;
        }
    }
    AVLNode AVLInsert(AVLNode? node, TKey key, TValue value)
    {
        if (node == null)
        {
            count++;
            return new AVLNode(key, value, 0);
        }

        int a = node.key.CompareTo(key);
        if (a < 0)
        {
            node.right = AVLInsert((AVLNode?) node.right, key, value);         // how to avoid typecasting?
        }
        else if (a > 0)
        {
            node.left = AVLInsert((AVLNode?) node.left, key, value);            // typecasting again
        }
        else
        {
            return node;
        }

        UpdateHeight(node);
        return RebalanceHeight(node);

    }

    public override void Insert(TKey key, TValue value)
    {
        root = AVLInsert((AVLNode?) root, key, value);
    }
    public override void Delete(TKey key)
    {
        
    }
}

// class RBTree<TKey, TValue> : BST<TKey, TValue> where TKey : IComparable<TKey>
// {
//     class RBNode : Node
//     {
        
//     }

//     public override void Insert(TKey key, TValue value);
//     public override void Delete(TKey key);
// }

// class GenericDicitonary<TKey, TValue>
// {
//     IKeyValuePair<TKey, TValue> structure;
// }

// class GenericSet<T>
// {
//     IKeyValuePair<T, T> structure;
// }


class Program
{
    static void Main()
    {
        AVLTree<int, string> tree = new AVLTree<int, string>();
        tree.Insert(60, "a");
        tree.Insert(50, "b");
        tree.Insert(40, "c");
        tree.Insert(30, "a");
        tree.Insert(20, "b");
        tree.Insert(10, "c");
        
        List<(int, string)> kvps = tree.InOrderKVP();

        foreach (var kvp in kvps)
        {
            Console.WriteLine($"{kvp.Item1} : {kvp.Item2}");
        }
    }
}
