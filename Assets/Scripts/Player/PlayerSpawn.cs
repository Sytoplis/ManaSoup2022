using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    void Start()
    {
        PollingStation.TryGetPollingStation(out PollingStation pollingStation, gameObject);
        PlayerController player = pollingStation.playerController;
        player.GetComponent<Rigidbody2D>().position = transform.position;
        player.velocity = Vector2.zero;
    }
}
