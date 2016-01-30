using UnityEngine;

public class Pixel : MonoBehaviour {

    public string Type; // "rock" "paper" "scissors" "cockblocker"
    public int team;   // 1 of 2
    private Vector3 speedVector = Vector3.zero;

    //private Animation animation;
    private Transform mytransform;
    private RaycastHit hit;

    void Awake () {
        //animation = GetComponent<Animation>();
        mytransform = gameObject.transform;
    }

    void Update () {
        if (team == 1)
            mytransform.position += speedVector;
        else if (team == 2)
            mytransform.position -= speedVector;


        if (Physics.Raycast(mytransform.transform.position, mytransform.forward, out hit, 2, LayerMask.NameToLayer("Object"))) {
            string enemyType = hit.transform.GetComponent<Pixel>().Type;
            DestoryLoser(CheckType(Type, enemyType));
        }
    }

    public int CheckType (string choice1, string choice2) {
        if (choice1 == choice2) {
            return 0;
        } else if (choice1 == "rock") {
            if (choice2 == "water") {
                return 1;
            } else {
                return 2;
            }
        } else if (choice1 == "fire") {
            if (choice2 == "rock") {
                return 1;
            } else {
                return 2;
            }
        } else if (choice1 == "water") {
            if (choice2 == "fire") {
                return 1;
            } else {
                return 2;
            }
        } else return 69;
    }

    public void DestoryLoser (int loser) {
        if (loser == 0) {
            Destroy(gameObject, 0.1f);
            Destroy(hit.transform.gameObject, 0.1f);
        } else if (loser == 1) {
            Destroy(hit.transform.gameObject, 0.1f);
        } else if (loser == 1) {
            Destroy(gameObject, 0.1f);
        }
    }
}
