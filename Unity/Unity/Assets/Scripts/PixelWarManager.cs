using UnityEngine;

/// <summary>
/// Steen 
/// Papier 
/// Schaar
/// 
/// </summary>

public class PixelWarManager : MonoBehaviour {
    public static PixelWarManager instance;

    public GameObject rockPixel;
    public GameObject paperPixel;
    public GameObject scissorsPixel;

    public GameObject[] spawnpointsTeam1;
    public GameObject[] spawnpointsTeam2;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public void Spawn(int team, int lane, int type) {
        
    }
}
