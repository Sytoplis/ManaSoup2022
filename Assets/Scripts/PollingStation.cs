using UnityEngine;

public class PollingStation : MonoBehaviour
{
    public InputManager inputManager;
    public MapGenerator mapGenerator; 
    public MovementController movementController;

    public static bool TryGetPollingStation(ref PollingStation pollingStation, GameObject oj) {
        pollingStation = FindObjectOfType<PollingStation>();

        if (!pollingStation) {
            Debug.LogError(oj.name + " could not find a Polling Station");
            return false;
        }

        return true;
    }
}
