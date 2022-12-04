using UnityEngine;

public class PollingStation : MonoBehaviour
{
    public InputManager inputManager;
    public MapGenerator mapGenerator; 
    public PlayerController movementController;
    public Score score;

    public static PollingStation instance;
    public static bool TryGetPollingStation(out PollingStation pollingStation, GameObject oj) {
        if (instance != null) {
            pollingStation = instance;
            return true;
        }

        pollingStation = FindObjectOfType<PollingStation>();

        if (!pollingStation) {
            Debug.LogError(oj.name + " could not find a Polling Station");
            return false;
        }

        instance = pollingStation;
        return true;
    }
}
