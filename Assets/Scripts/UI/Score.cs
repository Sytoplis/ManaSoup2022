using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    private PollingStation station;
    private void Awake() {
        if (!PollingStation.TryGetPollingStation(out station, gameObject))
            return;

        station.score = this;
    }

    [SerializeField]
    private float score = 0.0f;

    [SerializeField]
    private TextMeshProUGUI scoreText;


    public void ChangeScore(float change) {
        score += change;
        UpdateScore();
    }


    public void UpdateScore() {
        scoreText.text = score.ToString();
    }
}
