using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileStruct : MonoBehaviour
{
    public BoundsInt bounds;
    public TileBase[] tiles;

    public void LoadTiles(bool flip) {
        if (bounds.size.z != 1) Debug.LogError("incorrect size: using 3D");

        if (flip) {
            LoadTilesFlipped();
            return;
        }

        Tilemap tm = GetComponent<Tilemap>();
        tiles = tm.GetTilesBlock(bounds);
    }
    private void LoadTilesFlipped() {
        Tilemap tm = GetComponent<Tilemap>();
        tiles = new TileBase[bounds.size.x * bounds.size.y];
        Vector2Int pos = Vector2Int.zero;
        for(int x = 0; x < bounds.size.x; x++) {
            for(int y = 0; y < bounds.size.y; y++) {
                pos.x = bounds.xMax - x - 1;//flip the loaded tiles
                pos.y = bounds.yMin + y;
                tiles[x + y * bounds.size.x] = tm.GetTile((Vector3Int)pos);
            }
        }
    }

    public void OnDrawGizmos() {
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
