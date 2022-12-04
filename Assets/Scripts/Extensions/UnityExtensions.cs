using UnityEngine;

public static class UnityExtensions
{
    public static Vector3Int to3Size(this Vector2Int vec) {
        return new Vector3Int(vec.x, vec.y, 1);
    }

    public static int GetRndm(int length) {
        return UnityEngine.Random.Range(0, length);
    }

    public static Color ChangeAlpha(this Color clr, float alpha) {
        return new Color(clr.r, clr.g, clr.b, alpha);
    }

}
