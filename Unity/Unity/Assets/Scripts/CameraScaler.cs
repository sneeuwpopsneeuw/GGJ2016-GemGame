using UnityEngine;
using System.Collections;

public class CameraScaler : MonoBehaviour
{
    public float halfWidth = 50;

    // Use this for initialization
    void Update()
    {

        Camera.main.orthographicSize = halfWidth / Screen.width * Screen.height;

    }
}
