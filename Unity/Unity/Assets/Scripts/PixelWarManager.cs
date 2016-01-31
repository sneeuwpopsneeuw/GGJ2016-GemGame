using UnityEngine;

/// <summary>
/// Steen 
/// Papier 
/// Schaar
/// 
/// </summary>

public class PixelWarManager : MonoBehaviour {
    public static PixelWarManager instance;

    public GameObject[] Pixels;
    public Transform[] spawnpointsTeam1;
    public Transform[] spawnpointsTeam2;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="team">0 for first player 1 for second player</param>
    /// <param name="lane">0-4 </param>
    /// <param name="type">0=rock  1=fire  2=water</param>
    public void Spawn(int team, int lane, int type) {
        if(team == 0) {
            Instantiate(Pixels[type], spawnpointsTeam1[lane].position, Quaternion.identity); 
        } else if(team == 1) {
            Instantiate(Pixels[type], spawnpointsTeam2[lane].position, Quaternion.identity);
        }
    }

    // test
    void Update() {
        if(Input.anyKeyDown) {
            Spawn(Random.Range(0, 2), Random.Range(0, 5), Random.Range(0, 3));
        }
    }
}
