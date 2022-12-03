using UnityEngine;

public class PollingStation : MonoBehaviour
{
    public InputManager inputManager;
    public MapGenerator mapGenerator; 
    public PlayerController movementController;
    public Score score;

    public static bool TryGetPollingStation(out PollingStation pollingStation, GameObject oj) {
        pollingStation = FindObjectOfType<PollingStation>();

        if (!pollingStation) {
            Debug.LogError(oj.name + " could not find a Polling Station");
            return false;
        }

        return true;
    }
}
