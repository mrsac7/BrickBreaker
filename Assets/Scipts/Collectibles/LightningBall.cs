public class LightningBall : Collectible {
    protected override void ApplyEffect () {
        foreach (var ball in BallsManager.Instance.Balls) {
            ball.StartLightningBall ();
        }
    }
}