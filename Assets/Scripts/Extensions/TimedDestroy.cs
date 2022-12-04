using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float destroyTime = 3.0f;
    private float initTime = 0.0f;

    private void Awake() {
        initTime = Time.time;
    }

    private void Update() {
        if(Time.time - initTime >= destroyTime)
            Destroy(gameObject);
    }

}
