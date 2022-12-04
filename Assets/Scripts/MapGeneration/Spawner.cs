using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float probability = 1.0f;
    public GameObject prefab;

    private void Awake() {
        if (Random.value >= probability)
            return;//rolled against spawning

        Instantiate(prefab, transform.position, transform.rotation, transform.parent);
    }
}
