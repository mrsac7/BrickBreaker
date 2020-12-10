public class ShootingPaddle : Collectible {
    protected override void ApplyEffect () {
        Paddle.Instance.StartShooting ();
    }
}