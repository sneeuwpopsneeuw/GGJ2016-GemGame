using UnityEngine;

public class DisableAfter : MonoBehaviour {

    public float waitTime = 1;
    private float startTime;


    void Start() {
        startTime = Time.time;
    }

	void Update () {
	    if(Time.time > startTime + waitTime) {
            gameObject.SetActive(false);
        }
	}
}
