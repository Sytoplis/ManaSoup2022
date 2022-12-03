using UnityEngine;

public class Coin : Collectable
{
    public float scoreIncr = 1.0f;

    private PollingStation station;
    private void Awake() {
        if (!PollingStation.TryGetPollingStation(out station, gameObject))
            return;
    }

    public override void OnCollect() {
        base.OnCollect();

        station.score.ChangeScore(scoreIncr);
    }
}
