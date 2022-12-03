using UnityEngine;

public class Array2D<T> {
    public Vector2Int size { get; private set; }
    private T[] array;

    public Array2D(Vector2Int size) {
        this.size = size;
        array = new T[GetSize()];
    }

    public int Length { get { return array.Length; } }

    public int GetSize() { return size.x * size.y; }
    public Vector2Int GetPos(int i) {
        Vector2Int vec = new Vector2Int(0, 0);
        vec.x = i % size.x;
        vec.y = i / size.x;
        return vec;
    }
    public int GetIndex(Vector2Int vec) {
        return vec.x + vec.y * size.x;
    }
    public bool InBounds(Vector2Int vec) {
        return vec.x >= 0 && vec.x < size.x &&
               vec.y >= 0 && vec.y < size.y;
    }

    public T this[int i] {
        get { return array[i]; }
        set { array[i] = value; }
    }
    public T this[Vector2Int vec] {
        get { return array[GetIndex(vec)]; }
        set { array[GetIndex(vec)] = value; }
    }
}
