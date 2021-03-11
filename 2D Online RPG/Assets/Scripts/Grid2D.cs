// grid structure: get/set values of type T at any point
// -> not named 'Grid' because Unity already has a Grid type. causes warnings.
using System.Collections.Generic;
using UnityEngine;

public class Grid2D<T>
{
    Dictionary<Vector2Int, HashSet<T>> grid = new Dictionary<Vector2Int, HashSet<T>>();

    // cache a 9 neighbor grid of vector2 offsets so we can use them more easily
    Vector2Int[] neighbourOffsets =
    {
        Vector2Int.up,
        Vector2Int.up + Vector2Int.left,
        Vector2Int.up + Vector2Int.right,
        Vector2Int.left,
        Vector2Int.zero,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.down + Vector2Int.left,
        Vector2Int.down + Vector2Int.right
    };

    // helper function to remove entries
    public void Remove(Vector2Int position, T value)
    {
        if(grid.TryGetValue(position, out HashSet<T> hashSet))
        {
            hashSet.Remove(value);

            if(hashSet.Count == 0)
                grid.Remove(position);
        }
    }


    // helper function to add entries
    public void Add(Vector2Int position, T value)
    {
        // initialize set in grid if it's not in there yet
        if (!grid.TryGetValue(position, out HashSet<T> hashSet))
        {
            hashSet = new HashSet<T>();
            grid[position] = hashSet;
        }

        // add to it
        hashSet.Add(value);
    }

    // helper function to get set at position without worrying
    // -> result is passed as parameter to avoid allocations
    // -> result is not cleared before. this allows us to pass the HashSet from
    //    GetWithNeighbours and avoid .UnionWith which is very expensive.
    void GetAt(Vector2Int position, HashSet<T> result)
    {
        // return the set at position
        if (grid.TryGetValue(position, out HashSet<T> hashSet))
        {
            foreach (T entry in hashSet)
                result.Add(entry);
        }
    } 


    // helper function to get at position and it's 8 neighbors without worrying
    // -> result is passed as parameter to avoid allocations
    public void GetWithNeighbours(Vector2Int position, HashSet<T> result)
    {
        // note: we do NOT call result.Clear() first. the caller is responsible.
        // -> this allows us to pass .observers directly in RebuildObservers.
        foreach (Vector2Int offset in neighbourOffsets)
            GetAt(position + offset, result);
    }
}
