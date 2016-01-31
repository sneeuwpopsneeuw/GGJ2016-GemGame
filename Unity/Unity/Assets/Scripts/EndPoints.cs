using UnityEngine;

public class EndPoints : MonoBehaviour {

    public Gem gem;
    [Range(1,2)]
    public int team;

    void OnTriggerEnter2D (Collider2D other) {
        PixelMonster pixelMonster = other.gameObject.GetComponent<PixelMonster>();
        if (pixelMonster != null) {
            if (pixelMonster.team != this.team) {
                Destroy(other.gameObject);
                gem.SpawnCockblocker();
            }
        }
    }
}
