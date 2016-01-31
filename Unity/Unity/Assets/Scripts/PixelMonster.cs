using UnityEngine;
using System.Collections;

public class PixelMonster : MonoBehaviour {

    [HideInInspector] public int team = 1;   // 1 of 2
    public string monsterType; // "rock" "paper" "scissors" "cockblocker"
    private Vector3 speedVector = new Vector3(1,0,0);

    private Animator animator;
    private Transform mytransform;
    private bool battle = false;
    private GameObject enemy;
    public bool paused;

    void Awake () {
        animator = GetComponent<Animator>();
        mytransform = gameObject.transform;
    }

    void Start() {
        team = (mytransform.position.x > 0)? 2: 1;
        mytransform.localScale = (mytransform.position.x > 0) ?  new Vector3(-2,2,2) : new Vector3(2, 2, 2);
    }

    void Update()
    {
        if (!paused)
        {
            if (battle)
                return;

            if (team == 1)
                mytransform.position += speedVector * Time.deltaTime;
            else if (team == 2)
                mytransform.position -= speedVector * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D (Collider2D other) {
        animator.SetTrigger("Attack");
        transform.GetChild(0).gameObject.SetActive(true);
        PixelMonster pixelMonster = other.gameObject.GetComponent<PixelMonster>();
        if (pixelMonster != null) {
            if (pixelMonster.team != this.team) {
                pixelMonster.Battle(monsterType, this.gameObject);
                battle = true;
            }
        }
    }

    public void Battle(string attackerType, GameObject attacker) {
        battle = true;
        enemy = attacker;
        StartCoroutine(BattleCheck(monsterType, attackerType));
    }

    IEnumerator BattleCheck (string choice1, string choice2) {
        Debug.Log("BattleCheck");
        yield return new WaitForSeconds(1f);

        if (choice1 == choice2) {
            DestoryLoser(0);
        } else if (choice1 == "rock") {
            if (choice2 == "water") {
                DestoryLoser(1);
            } else {
                DestoryLoser(2);
            }
        } else if (choice1 == "fire") {
            if (choice2 == "rock") {
                DestoryLoser(1);
            } else {
                DestoryLoser(2);
            }
        } else if (choice1 == "water") {
            if (choice2 == "fire") {
                DestoryLoser(1);
            } else {
                DestoryLoser(2);
            }
        }
    }

    private void DestoryLoser (int loser) {
        if (loser == 0) {
            Destroy(gameObject, 0.1f);
            Destroy(enemy, 0.1f);
        } else if (loser == 1) {
            battle = false;
            Destroy(enemy, 0.1f);
        } else if (loser == 1) {
            enemy.GetComponent<PixelMonster>().battle = false;
            Destroy(gameObject, 0.1f);
        }
    }
}
