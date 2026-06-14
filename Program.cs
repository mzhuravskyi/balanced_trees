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

    public Node FindNode(TKey key)
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
                return current;
            }
        }

        throw new KeyNotFoundException($"{key} not found");
    }

    public TValue GetValue(TKey key)
    {
        Node node = FindNode(key);
        
        return node.value;
    }

    public Node InOrderSucc(Node node)                  // helper for a node deletion; doesn't work in general
    {
        Node? current = node.right;
        Debug.Assert(current != null, "no successor");

        while (current.left != null)
        {
            current = current.left;
        }

        return current;
    }

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

    AVLNode RotationLeft(AVLNode ynode)
    {
        AVLNode xnode = (AVLNode) ynode.left!;

        ynode.left = xnode.right;
        xnode.right = ynode;

        UpdateHeight(ynode);
        UpdateHeight(xnode);

        return xnode;
    }

    AVLNode RotationRight(AVLNode ynode)
    {
        AVLNode xnode = (AVLNode) ynode.right!;

        ynode.right = xnode.left;
        xnode.left = ynode;

        UpdateHeight(ynode);
        UpdateHeight(xnode);

        return xnode;
    }

    AVLNode DoubleRotationLeft(AVLNode znode)
    {
        AVLNode xnode = (AVLNode) znode.left!;
        AVLNode ynode = (AVLNode) xnode.right!;

        xnode.right = ynode.left;
        znode.left = ynode.right;
        ynode.left = xnode;
        ynode.right = znode;

        UpdateHeight(xnode);
        UpdateHeight(znode);
        UpdateHeight(ynode);

        return ynode;
    }

    AVLNode DoubleRotationRight(AVLNode znode)
    {
        AVLNode xnode = (AVLNode) znode.right!;
        AVLNode ynode = (AVLNode) xnode.left!;

        xnode.left = ynode.right;
        znode.right = ynode.left;
        ynode.left = znode;
        ynode.right = xnode;

        UpdateHeight(xnode);
        UpdateHeight(znode);
        UpdateHeight(ynode);

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

                if (lnode_balance <= 0)             // 0-case is possible when deleting
                {
                    return RotationLeft(node);
                }
                else
                {
                    return DoubleRotationLeft(node);
                }
            case 2:
                int rnode_balance = GetBalance((AVLNode) node.right!);
                
                if (rnode_balance >= 0)
                {
                    return RotationRight(node);
                }
                else
                {
                    return DoubleRotationRight(node);
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

    AVLNode? AVLDelete(AVLNode? node, TKey key)
    {
        Debug.Assert(node != null, "not in the tree");
        int a = node.key.CompareTo(key);

        if (a < 0)
        {
            node.right = AVLDelete((AVLNode?) node.right, key);
        }
        else if (a > 0)
        {
            node.left = AVLDelete((AVLNode?) node.left, key);
        }
        else
        {
            if (node.left == null)
            {
                count--;
                return (AVLNode?) node.right;
            }
            if (node.right == null)
            {
                count--;
                return (AVLNode?) node.left;    
            }
            
            AVLNode succ = (AVLNode) InOrderSucc(node);
            node.key = succ.key;
            node.value = succ.value;
            node.right = AVLDelete((AVLNode?) node.right, succ.key);
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
        root = AVLDelete((AVLNode?) root, key);
    }
}

class LLRBTree<TKey, TValue> : BST<TKey, TValue> where TKey : IComparable<TKey>
{
    public enum Color {RED, BLACK};

    public class RBNode : Node
    {
        public Color color;
        
        public RBNode(TKey key, TValue value, Color color) : base (key, value)
        {
            this.color = color;
        }
    }

    RBNode ColorFlip(RBNode ynode)
    {
        RBNode xnode = (RBNode) ynode.left!;
        RBNode znode = (RBNode) ynode.right!;

        xnode.color = Color.BLACK;
        znode.color = Color.BLACK;
        ynode.color = Color.RED;

        return ynode;
    }

    RBNode RotationLeft(RBNode xnode)
    {
        RBNode ynode = (RBNode) xnode.right!;

        xnode.right = ynode.left;
        ynode.left = xnode;

        ynode.color = xnode.color;
        xnode.color = Color.RED;

        return ynode; 
    }

    RBNode RotationRight(RBNode ynode)
    {
        RBNode xnode = (RBNode) ynode.left!;

        ynode.left = xnode.right;
        xnode.right = ynode;

        xnode.color = ynode.color;
        ynode.color = Color.RED;

        return xnode;
    }

    Color GetColor(RBNode? node)
    {
        if (node == null || node.color == Color.BLACK)
        {
            return Color.BLACK;
        }
        else
        {
            return Color.RED;
        }
    }

    RBNode FixUp(RBNode ynode)
    {
        if (GetColor((RBNode?) ynode.left) == Color.BLACK && GetColor((RBNode?) ynode.right) == Color.RED)
        {
            ynode = RotationLeft(ynode);
        }

        if (GetColor((RBNode?) ynode.left) == Color.RED && GetColor((RBNode?) ynode.left!.left) == Color.RED)
        {
            ynode = RotationRight(ynode);    
        }

        if (GetColor((RBNode?) ynode.left) == Color.RED && GetColor((RBNode?) ynode.right) == Color.RED)
        {
            ynode = ColorFlip(ynode);
        }

        return ynode;
    }

    RBNode RBInsert(RBNode? node, TKey key, TValue value)
    {
        if (node == null)
        {
            count++;
            return new RBNode(key, value, Color.RED);
        }

        int a = node.key.CompareTo(key);
        if (a < 0)
        {
            node.right = RBInsert((RBNode?) node.right, key, value);
        }
        else if (a > 0)
        {
            node.left = RBInsert((RBNode?) node.left, key, value);
        }
        else
        {
            return node;
        }

        return FixUp(node);
    }

    public List<RBNode> PreOrderNodes(RBNode? node, List<RBNode> list)
    {
        if (node == null)
        {
            return list;
        }

        list.Add(node);
        PreOrderNodes((RBNode?) node.left, list);
        PreOrderNodes((RBNode?) node.right, list);

        return list;
    }

    public override void Insert(TKey key, TValue value)
    {
        root = RBInsert((RBNode?) root, key, value);
        ((RBNode) root).color = Color.BLACK;
    }
    public override void Delete(TKey key)
    {
        
    }
}

// class GenericDicitonary<TKey, TValue>
// {
//     IKeyValuePair<TKey, TValue> structure;
// }

// class GenericSet<T>
// {
//     IKeyValuePair<T, T> structure;
// }


class Tests
{
    static void Main()
    {
        AVLTree<int, string> avl_tree = new AVLTree<int, string>();
        avl_tree.Insert(70, "x");
        avl_tree.Insert(60, "a");
        avl_tree.Insert(50, "b");
        avl_tree.Insert(40, "c");
        avl_tree.Insert(30, "a");
        avl_tree.Insert(20, "b");
        avl_tree.Insert(10, "c");
        
        List<(int, string)> avl_kvps = avl_tree.InOrderKVP();

        AVLTree<int, string>.AVLNode root = (AVLTree<int, string>.AVLNode) avl_tree.root!;
        Console.WriteLine($"count : {avl_tree.Count}  | height : {root.height}");
        Console.WriteLine($"–––––––––––––––––––––––");
        foreach (var kvp in avl_kvps)
        {
            Console.WriteLine($"key   : {kvp.Item1} | value  : {kvp.Item2}");
        }
        Console.WriteLine($"key(50) : {avl_tree.GetValue(50)}");
        Console.WriteLine($"key(30) : {avl_tree.GetValue(30)}");

        Console.WriteLine($"Deleting...");
        avl_tree.Delete(30);
        avl_tree.Delete(10);
        avl_tree.Delete(50);
        avl_tree.Delete(40);

        root = (AVLTree<int, string>.AVLNode) avl_tree.root!;
        avl_kvps = avl_tree.InOrderKVP();
        Console.WriteLine($"count : {avl_tree.Count}  | height : {root.height}");
        Console.WriteLine($"–––––––––––––––––––––––");
        foreach (var kvp in avl_kvps)
        {
            Console.WriteLine($"key   : {kvp.Item1} | value  : {kvp.Item2}");
        }

        Console.WriteLine("\n");
        LLRBTree<int,string> rb_tree = new LLRBTree<int, string>();
        rb_tree.Insert(50, "x");
        rb_tree.Insert(20, "a");
        rb_tree.Insert(70, "b");
        rb_tree.Insert(30, "c");
        rb_tree.Insert(40, "a");
        rb_tree.Insert(10, "b");
        rb_tree.Insert(60, "c");

        List<LLRBTree<int, string>.RBNode> list = new List<LLRBTree<int, string>.RBNode>();
        foreach(var node in rb_tree.PreOrderNodes((LLRBTree<int, string>.RBNode?) rb_tree.root, list))
        {
            Console.WriteLine($"{node.key} : {node.value} : {node.color}");
        }
    }
}
