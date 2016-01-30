using UnityEngine;
using System.Collections;

public class ObjectPositioner : MonoBehaviour {

    public Vector2 normalizedPosition;
    
	void Update () {
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        
        Camera cam = Camera.main;
        Vector3 min = cam.ScreenToWorldPoint(new Vector3(0, Screen.height)) - new Vector3(-r.bounds.size.x, r.bounds.size.y)/2;
        Vector3 max = cam.ScreenToWorldPoint(new Vector3(Screen.width, 0)) + new Vector3(-r.bounds.size.x, r.bounds.size.y)/2;
        transform.position = new Vector3(Mathf.Lerp(min.x, max.x, normalizedPosition.x), Mathf.Lerp(min.y, max.y, normalizedPosition.y));
	}
}
