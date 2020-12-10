using System.Collections.Generic;
using UnityEngine;

public class CollectiblesManager : MonoBehaviour {
    #region Singleton
    private static CollectiblesManager _instance;

    public static CollectiblesManager Instance => _instance;

    private void Awake () {
        if (_instance != null) {
            Destroy (gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion

    public List<Collectible> AvailableBuffs;
    public List<Collectible> AvailableDebuffs;

    [Range (0, 100)]
    public float BuffChance;

    [Range (0, 100)]
    public float DebuffChance;
}