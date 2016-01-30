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
    public GameObject firePixel;
    public GameObject waterPixel;

    public GameObject[] spawnpointsTeam1;
    public GameObject[] spawnpointsTeam2;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="team">1 for first player 2 for second player</param>
    /// <param name="lane">0-4 </param>
    /// <param name="type">1=rock  2=fire  3=water</param>
    public void Spawn(int team, int lane, int type) {
        //Instantiate();
    }
}
