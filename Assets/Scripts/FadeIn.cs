using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FadeIn : MonoBehaviour
{
    public float intensityIncrSeconds = 1.0f;
    public float maxLightIntensity = 1.0f;
    public float maxTextTransparency = 1.0f;

    private TextMeshProUGUI[] texts;
    private Light2D[] lights;


    private void Awake() {
        maxTextTransparency = Mathf.Min(1.0f, maxTextTransparency);

        texts = GetComponentsInChildren<TextMeshProUGUI>();
        lights = GetComponentsInChildren<Light2D>();

        for (int i = 0; i < texts.Length; i++) {
            texts[i].color = texts[i].color.ChangeAlpha(0.0f);
        }
            
        for (int i = 0; i < lights.Length; i++)
            lights[i].intensity = 0.0f;
    }

    private void Update() {
        UpdateIntensities();
    }

    void UpdateIntensities() {
        bool change = false;

        float incr = intensityIncrSeconds * Time.deltaTime;
        for (int i = 0; i < texts.Length; i++) {
            float alpha = texts[i].color.a;
            alpha = Mathf.Min(maxTextTransparency, alpha + incr * maxTextTransparency);
            change |= alpha != texts[i].color.a;//set change to true if there was a change, else keep it as it was
            texts[i].color = texts[i].color.ChangeAlpha(alpha);
        }

        for (int i = 0; i < lights.Length; i++) {
            float intens = lights[i].intensity;
            intens = Mathf.Min(maxLightIntensity, intens + incr * maxLightIntensity);
            change |= intens != lights[i].intensity;
            lights[i].intensity = intens;
        }

        if (!change)
            Destroy(this);//selfdestruct if the fade is over
    }
}
