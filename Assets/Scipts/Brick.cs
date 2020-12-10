using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Brick : MonoBehaviour {

    private SpriteRenderer sr;
    private BoxCollider2D boxCollider;

    public int Hitpoints = 1;
    public ParticleSystem DestroyEffect;
    public static event Action<Brick> OnBrickDestruction;

    private void Awake () {
        this.sr = this.GetComponent<SpriteRenderer> ();
        this.boxCollider = this.GetComponent<BoxCollider2D> ();
        Ball.OnLightningBallEnable += OnLightningBallEnable;
        Ball.OnLightningBallDisable += OnLightningBallDisable;
    }

    private void OnLightningBallDisable (Ball obj) {
        if (this != null) {
            this.boxCollider.isTrigger = false;
        }
    }

    private void OnLightningBallEnable (Ball obj) {
        if (this != null) {
            this.boxCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D (Collision2D collision) {
        bool instantKill = false;

        if (collision.collider.tag == "Ball") {
            Ball ball = collision.gameObject.GetComponent<Ball> ();
            instantKill = ball.isLightningBall;
        }

        if (collision.collider.tag == "Ball" || collision.collider.tag == "Projectile") {
            this.TakeDamage (instantKill);
        }
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        bool instantKill = false;

        if (collision.tag == "Ball") {
            Ball ball = collision.gameObject.GetComponent<Ball> ();
            instantKill = ball.isLightningBall;
        }

        if (collision.tag == "Ball" || collision.tag == "Projectile") {
            this.TakeDamage (instantKill);
        }
    }

    private void TakeDamage (bool instantKill) {
        this.Hitpoints--;

        if (this.Hitpoints <= 0 || instantKill) {
            BricksManager.Instance.RemainingBricks.Remove (this);
            OnBrickDestruction?.Invoke (this);
            OnBrickDestroy ();
            SpawnDestroyEffect ();
            Destroy (this.gameObject);
        } else {
            this.sr.sprite = BricksManager.Instance.Sprites[this.Hitpoints - 1];
        }
    }

    private void OnBrickDestroy () {
        float buffSpawnChance = UnityEngine.Random.Range (0, 100f);
        float deBuffSpawnChance = UnityEngine.Random.Range (0, 100f);
        bool alreadySpawned = false;

        if (buffSpawnChance <= CollectiblesManager.Instance.BuffChance) {
            alreadySpawned = true;
            Collectible newBuff = this.SpawnCollectible (true);
        }

        if (deBuffSpawnChance <= CollectiblesManager.Instance.DebuffChance && !alreadySpawned) {
            Collectible newDebuff = this.SpawnCollectible (false);
        }
    }

    private Collectible SpawnCollectible (bool isBuff) {
        List<Collectible> collection;

        if (isBuff) {
            collection = CollectiblesManager.Instance.AvailableBuffs;
        } else {
            collection = CollectiblesManager.Instance.AvailableDebuffs;
        }

        int buffIndex = UnityEngine.Random.Range (0, collection.Count);
        Collectible prefab = collection[buffIndex];
        Collectible newCollectible = Instantiate (prefab, this.transform.position, Quaternion.identity) as Collectible;

        return newCollectible;

    }

    private void SpawnDestroyEffect () {
        Vector3 brickPos = gameObject.transform.position;
        Vector3 spawnPosition = new Vector3 (brickPos.x, brickPos.y, brickPos.z - 0.2f);
        GameObject effect = Instantiate (DestroyEffect.gameObject, spawnPosition, Quaternion.identity);

        MainModule mm = effect.GetComponent<ParticleSystem> ().main;
        mm.startColor = this.sr.color;
        Destroy (effect, DestroyEffect.main.startLifetime.constant);
    }

    public void Init (Transform containerTransform, Sprite sprite, Color color, int hitpoints) {
        this.transform.SetParent (containerTransform);
        this.sr.sprite = sprite;
        this.sr.color = color;
        this.Hitpoints = hitpoints;
    }

    private void OnDisable () {
        Ball.OnLightningBallEnable -= OnLightningBallEnable;
        Ball.OnLightningBallDisable -= OnLightningBallDisable;
    }
}