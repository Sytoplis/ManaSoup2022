using UnityEngine;

public class PollingStation : MonoBehaviour
{
    public InputManager inputManager;
    public MapGenerator mapGenerator; 
    public PlayerController playerController;
    public Score score;


    private void Awake() {
        //DontDestroyOnLoad(gameObject);
    }


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
