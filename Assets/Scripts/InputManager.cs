using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum InputPreset { Jump, Movement }

    public bool GetButton(InputPreset preset) {
        return false;//TODO: implement
    }

    public bool GetButtonDown(InputPreset preset) {
        return false;//TODO: implement
    }

    public float GetSingleAxis(InputPreset preset) {
        return 0;//TODO: implement
    }
}
