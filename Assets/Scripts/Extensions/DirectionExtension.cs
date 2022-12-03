using UnityEngine;

public enum Direction { Top, Right, Bottom, Left }
public static class DirExt
{
    public static Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
    public static int InvertDir(int d) { return (d + 2) % 4; }
    public static Direction InvertDir(this Direction d) { return (Direction)InvertDir((int)d); }

    public static Direction Flip(this Direction d, bool flip) {//clockwise
        if (d == Direction.Top || d == Direction.Bottom)
            return d;
        else
            return InvertDir(d);
    }

    public static Direction ToDir(Vector2Int dir) {
        for (int d = 0; d < directions.Length; d++) {
            if (dir == directions[d])
                return (Direction)d;
        }
        Debug.LogError("Vector could not be converted to Direction");
        return Direction.Top;
    }
}