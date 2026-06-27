using System.Diagnostics;

interface IKeyValuePair<TKey, TValue>
{
    void Insert(TKey key, TValue value);
    void Delete(TKey key);
    TValue GetValue(TKey key);
    List<(TKey key, TValue value)> InOrderKVP();
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

    protected Node? root = null;
    protected int count = 0;

    public int Count => count;

    protected Node FindNode(TKey key)
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

    protected Node InOrderSucc(Node node)                  // helper for a node deletion; doesn't work in general
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

    public TValue GetMinValue(Node? root, out TKey min_key)
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

    public List<(TKey key, TValue value)> InOrderKVP() {
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

    public int Height => GetHeight((AVLNode?) root);

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
        RBNode xnode = (RBNode?) ynode.left!;
        RBNode znode = (RBNode?) ynode.right!;

        if (ynode.color == Color.BLACK)
        {
            xnode.color = Color.BLACK;
            znode.color = Color.BLACK;
            ynode.color = Color.RED;
        }
        else
        {
            xnode.color = Color.RED;
            znode.color = Color.RED;
            ynode.color = Color.BLACK;
        }

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

    RBNode MoveRedLeft(RBNode node)
    {
        ColorFlip(node);

        if (GetColor((RBNode?) node.right!.left) == Color.RED)
        {
            node.right = RotationRight((RBNode) node.right);
            node = RotationLeft(node);
            node = ColorFlip(node);
        }

        return node;
    }

    RBNode MoveRedRight(RBNode node)
    {
        node = ColorFlip(node);
        if (GetColor((RBNode?) node.left!.left) == Color.RED)
        {
            node = RotationRight(node);
            node = ColorFlip(node);
        }

        return node;
    }

    RBNode? RBDeleteMin(RBNode node)
    {
        if (node.left == null)
        {
            return null;
        }
        
        if (GetColor((RBNode?) node.left) == Color.BLACK && GetColor((RBNode?) node.left.left) == Color.BLACK)
        {
            node = MoveRedLeft(node);
        }

        node.left = RBDeleteMin((RBNode) node.left!);
        return FixUp(node);
    }

    RBNode? RBDelete(RBNode? node, TKey key)
    {
        Debug.Assert(node != null, "not in the tree");

        int a = node.key.CompareTo(key);
        if (a > 0)
        {
            if (node.left != null && GetColor((RBNode?) node.left) == Color.BLACK && GetColor((RBNode?) node.left.left) == Color.BLACK)     // removed && node.left.left != null. Fixed by ai, diverges from the book here!!!
            {
                node = MoveRedLeft(node);
            }
            node.left = RBDelete((RBNode?) node.left, key);
        }
        else
        {
            if (GetColor((RBNode?) node.left) == Color.RED)
            {
                node = RotationRight(node);
            }
            if (node.key.Equals(key) && node.right == null)
            {
                count--;
                return null;
            }
            if (node.right != null && GetColor((RBNode?) node.right) == Color.BLACK && GetColor((RBNode?) node.right.left) == Color.BLACK) // also removed
            {
                node = MoveRedRight(node);
            }
            if (node.key.Equals(key))
            {
                node.value = GetMinValue(node.right, out TKey min_key);
                node.key = min_key;
                node.right = RBDeleteMin((RBNode) node.right!);
                count--;
            }
            else
            {
                node.right = RBDelete((RBNode) node.right!, key);
            }
        }
        node = FixUp(node);
        return node;
    }

    public List<(TKey key, TValue value, Color color)> PreOrderKVP()
    {
        List<RBNode> nodes = PreOrderNodes((RBNode?) root);
        List<(TKey, TValue, Color)> tuples = new();

        foreach (RBNode node in nodes)
        {
            tuples.Add((node.key, node.value, node.color));
        }
        
        return tuples;
    }

    List<RBNode> PreOrderNodes(RBNode? node)
    {
        List<RBNode> list = new();
        helper(node);

        void helper(RBNode? node)
        {
            if (node == null)
            {
                return;
            }

            list.Add(node);
            helper((RBNode?) node.left);
            helper((RBNode?) node.right);
        }

        return list;
    }

    public override void Insert(TKey key, TValue value)
    {
        root = RBInsert((RBNode?) root, key, value);
        ((RBNode) root).color = Color.BLACK;
    }
    public override void Delete(TKey key)
    {
        root = RBDelete((RBNode?) root, key);
    }
}

class GenericDicitonary<TKey, TValue>
{
    IKeyValuePair<TKey, TValue> structure;

    public GenericDicitonary(IKeyValuePair<TKey, TValue> structure)
    {
        this.structure = structure;
    }

    public TValue GetValue(TKey key)
    {
        return structure.GetValue(key);
    }

    public void Add(TKey key, TValue value)
    {
        structure.Insert(key, value);
    }

    public void Remove(TKey key)
    {
        structure.Delete(key);
    }

    public List<(TKey key, TValue value)> GetKVPs()
    {
        return structure.InOrderKVP();
    }
}

class GenericSet<T>
{
    IKeyValuePair<T, T> structure;

    public GenericSet(IKeyValuePair<T, T> structure)
    {
        this.structure = structure;
    }

    public bool Contains(T value)
    {
        try
        {
            structure.GetValue(value);
            return true;
        } catch (KeyNotFoundException)
        {
            return false;
        }
    } 

    public void Add(T value)
    {
        structure.Insert(value, value);
    }

    public void Remove(T value)
    {
        structure.Delete(value);
    }

    public List<T> GetValues()
    {
        List<(T key, T value)> list = structure.InOrderKVP();
        List<T> values = new List<T>();

        foreach (var kvp in list)
        {
            values.Add(kvp.value);
        }

        return values;
    }
}

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
        
        Console.WriteLine($"count : {avl_tree.Count}  | height : {avl_tree.Height}");
        Console.WriteLine($"–––––––––––––––––––––––");
        foreach (var kvp in avl_tree.InOrderKVP())
        {
            Console.WriteLine($"key   : {kvp.key} | value  : {kvp.value}");
        }

        Console.WriteLine();
        Console.WriteLine("Trying to get values...");
        Console.WriteLine($"key(50) : {avl_tree.GetValue(50)}");
        Console.WriteLine($"key(30) : {avl_tree.GetValue(30)}");

        Console.WriteLine();
        Console.WriteLine($"Deleting...");
        avl_tree.Delete(30);
        avl_tree.Delete(10);
        avl_tree.Delete(20);
        avl_tree.Delete(40);

        Console.WriteLine($"count : {avl_tree.Count}  | height : {avl_tree.Height}");
        Console.WriteLine($"–––––––––––––––––––––––");
        foreach (var kvp in avl_tree.InOrderKVP())
        {
            Console.WriteLine($"key   : {kvp.key} | value  : {kvp.value}");
        }

        Console.WriteLine();
        LLRBTree<int,string> rb_tree = new LLRBTree<int, string>();
        rb_tree.Insert(50, "x");
        rb_tree.Insert(20, "a");
        rb_tree.Insert(70, "b");
        rb_tree.Insert(30, "c");
        rb_tree.Insert(40, "a");
        rb_tree.Insert(10, "b");
        rb_tree.Insert(60, "c");

        Console.WriteLine();
        foreach(var tuple in rb_tree.PreOrderKVP())
        {
            Console.WriteLine($"{tuple.key} : {tuple.value} : {tuple.color}");
        }
        Console.WriteLine($"count = {rb_tree.Count}");

        Console.WriteLine();
        Console.WriteLine($"Deleting...");
        rb_tree.Delete(40);

        foreach(var tuple in rb_tree.PreOrderKVP())
        {
            Console.WriteLine($"{tuple.key} : {tuple.value} : {tuple.color}");
        }
        Console.WriteLine($"count = {rb_tree.Count}");

        Console.WriteLine();
        AVLTree<string, int> structure = new AVLTree<string, int>();
        GenericDicitonary<string, int> dict = new GenericDicitonary<string, int>(structure);
        dict.Add("apple", 1);
        dict.Add("banana", 10);
        dict.Add("orange", 1000);
        dict.Add("cherry", 10);
        dict.Add("melon", 8);
        dict.Add("cucumber", -5);

        foreach (var kvp in dict.GetKVPs())
        {
            Console.WriteLine($"{kvp.key} : {kvp.value}");
        }

        Console.WriteLine();
        Console.WriteLine("Trying to get values...");
        Console.WriteLine($"key(orange) : {dict.GetValue("orange")}");
        Console.WriteLine($"key(cucumber) : {dict.GetValue("cucumber")}");
        // Console.WriteLine($"key(gibberish) : {dict.GetValue("gibberish")}");

        Console.WriteLine();
        Console.WriteLine($"Deleting...");
        dict.Remove("orange");
        dict.Remove("cucumber");
        foreach (var kvp in dict.GetKVPs())
        {
            Console.WriteLine($"{kvp.key} : {kvp.value}");
        }

        Console.WriteLine();
        LLRBTree<int, int> nstructure = new LLRBTree<int, int>();
        GenericSet<int> set = new GenericSet<int>(nstructure);
        set.Add(10);
        set.Add(-10);
        set.Add(0);
        set.Add(20);
        set.Add(1000);
        set.Add(15);
        set.Add(-10000);

        foreach (var val in set.GetValues())
        {
            Console.WriteLine(val);
        }

        Console.WriteLine();
        Console.WriteLine("Testing Constains()");
        Console.WriteLine($"2 : {set.Contains(2)}");
        Console.WriteLine($"0 : {set.Contains(0)}");
        Console.WriteLine($"1 : {set.Contains(1)}");
    }
}
