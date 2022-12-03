using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour
{

    public InputActionAsset inputAsset;

    public InputAction GetAction(string actionName)
    {
        return inputAsset.FindAction(actionName);
    }
    public InputAction GetAction(InputPreset aPresetName)
    {
        return registeredInput[aPresetName];
    }


    public bool GetButton(InputPreset aPreset)
    {
        if (registeredInput == null || !registeredInput.ContainsKey(aPreset)) return false;

        var action = registeredInput[aPreset];

        if (action.type == InputActionType.Button)
        {
            var value = action.ReadValue<float>();
            return value > 0;
        }
        Debug.LogWarning($"Preset {aPreset} is not a Button!");
        return false;
    }

    public bool GetButtonDown(InputPreset aPreset)
    {
        if (registeredInput == null || !registeredInput.ContainsKey(aPreset)) return false;

        var action = registeredInput[aPreset];

        if (action.type == InputActionType.Button)
        {
            return action.ReadValue<float>() > 0 && action.triggered;
        }

        Debug.LogWarning($"Preset {aPreset} is not a Button!");
        return false;
    }

    public float GetSingleAxis(InputPreset aPreset)
    {
        if (registeredInput == null || !registeredInput.ContainsKey(aPreset)) return 0;

        var action = registeredInput[aPreset];

        if (action.type != InputActionType.Button)
        {
            return action.ReadValue<float>();
        }
        Debug.LogWarning($"Preset {aPreset} is not a Value Input!");
        return 0;
    }



    Dictionary<InputPreset, InputAction> registeredInput;
    Dictionary<InputPreset, List<string>> inputKeywords;
    public enum InputPreset
    {
        Movement, Jump, Interact, Attack
    }


    private void Awake()
    {
        inputKeywords = new Dictionary<InputPreset, List<string>>();
        registeredInput = new Dictionary<InputPreset, InputAction>();

        inputKeywords.Add(InputPreset.Movement, new List<string>() { "Move", "Movement" });
        inputKeywords.Add(InputPreset.Jump, new List<string>() { "Jump" });
        inputKeywords.Add(InputPreset.Interact, new List<string>() { "Interact" });
        inputKeywords.Add(InputPreset.Attack, new List<string>() { "Attack" });


        if (inputAsset.actionMaps.Count > 0)
            for (int i = 0; i < inputAsset.actionMaps[0].actions.Count; i++)
            {
                var action = inputAsset.actionMaps[0].actions[i];
                var name = action.name;

                foreach (var keyword in inputKeywords)
                {
                    if (keyword.Value.Any(k => name.ToLower().Contains(k.ToLower())))
                    {
                        action.Enable();
                        registeredInput.Add(keyword.Key, action);
                        break;
                    }
                }
            }
    }
}
