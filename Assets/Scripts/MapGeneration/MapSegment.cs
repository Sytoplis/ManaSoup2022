using UnityEngine;

[CreateAssetMenu(fileName = "New Map Segment", menuName = "Map/MapSegment")]
public class MapSegment : ScriptableObject
{   
    public Socket2D socket;
    public float weight = 1.0f;
    public bool flippable = false;

    public GameObject prefab;

    public TileStruct GetTileStruct() {
        return prefab.GetComponentInChildren<TileStruct>();
    }
}
