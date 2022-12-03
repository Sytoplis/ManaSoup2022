using UnityEngine;
using UnityEngine.Tilemaps;

public class TileStruct : MonoBehaviour
{
    public BoundsInt bounds;
    public Tilemap[] tilemaps;
    [System.NonSerialized]
    public TileBase[][] tiles;
    public GameObject prefab;

    public void LoadTiles(bool flip) {
        if (bounds.size.z != 1) Debug.LogError("incorrect size: using 3D");

        tiles = new TileBase[tilemaps.Length][];
        for(int tm_idx = 0; tm_idx < tilemaps.Length; tm_idx++)
            LoadTiles(flip, tm_idx);
    }

    public void LoadTiles(bool flip, int tilemap_idx) {
        if (flip) {
            LoadTilesFlipped(tilemap_idx);
            return;
        }
        tiles[tilemap_idx] = tilemaps[tilemap_idx].GetTilesBlock(bounds);
    }
    private void LoadTilesFlipped(int tilemap_idx) {
        tiles[tilemap_idx] = new TileBase[bounds.size.x * bounds.size.y];
        Vector2Int pos = Vector2Int.zero;
        for(int x = 0; x < bounds.size.x; x++) {
            for(int y = 0; y < bounds.size.y; y++) {
                pos.x = bounds.xMax - x - 1;//flip the loaded tiles
                pos.y = bounds.yMin + y;
                tiles[tilemap_idx][x + y * bounds.size.x] = tilemaps[tilemap_idx].GetTile((Vector3Int)pos);
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
