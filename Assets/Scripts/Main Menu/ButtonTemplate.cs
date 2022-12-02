using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonTemplate : MonoBehaviour
{
    private Button button;
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public abstract void OnClick();
}