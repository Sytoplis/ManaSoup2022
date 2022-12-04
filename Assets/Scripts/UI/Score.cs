using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private PollingStation station;
    private void Awake() {
        if (!PollingStation.TryGetPollingStation(out station, gameObject))
            return;

        station.score = this;
        UpdateScore();
    }

    //[SerializeField]
    private static float score = 0.0f;

    [SerializeField]
    private TextMeshProUGUI scoreText;


    public void ChangeScore(float change) {
        score += change;
        UpdateScore();
    }
    public static void SetScore(float set) {
        score = set;
    }


    public void UpdateScore() {
        scoreText.text = score.ToString();
    }
}
