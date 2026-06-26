<h1>AVL & LLRB Trees in C#</h1>

## Description
This project provides a scalable, ground-up implementation of self-balancing search trees in **C#**.
It features **AVL** and **Left-Leaning-Red-Black (LLRB)** trees that guarantee `O(log n)` time complexity for insertions, deletions, and lookups.

Additionally, the project includes standard higher-level library collections (`GenericDictionary` and `GenericSet`) that utilize balanced trees. These can be easily reinitialized to use any other data-structure thanks to the generic design.

## Goal
The primary goal of this project was to put theory into practice and to extensively explore C# Object-Oriented Programming (OOP) structures.

## Implementations

- **`BST-Tree`**: A generic abstract base class that handles basic binary tree operations.
- **`AVL-Tree`**: A standard balanced AVL Tree that maintains the height invariant via single/double rotations. Height updates are propagated to the root of the tree, which allows cutting several corner-cases, although sacrificing a bit of performance compared to the signals implementation.
- **`LLRB-Tree`**: A standard textbook variant of a more general Red-Black tree, which maintains all its complexity analysis.
- **`Generic-Dictionary`**: A key-value map that can be injected with an arbitrary data-structure satisfying the `IKeyValuePair` interface.
- **`Generic-Set`**: A collection of unique elements that can be injected with an arbitrary data-structure satisfying the `IKeyValuePair` interface.

## Major Issues
Multiple node casts (e.g., `Node` into `RBNode`, `Node` into `AVLNode`, and vice-versa) stand out from the OOP design paradigm and potentially halt efficiency.

## Usage
The `Tests` class provides basic usage examples.
