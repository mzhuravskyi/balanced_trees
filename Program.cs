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
        
    }
    public TValue GetValue(TKey key);
    public TKey GetNextKey(TKey current_key);
    public TValue GetMaxValue(out TKey max_key);
    public TValue GetMinValue(out TKey min_key);
    public TKey[] InOrderKeys();
    public TValue[] InOrderValues();

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

