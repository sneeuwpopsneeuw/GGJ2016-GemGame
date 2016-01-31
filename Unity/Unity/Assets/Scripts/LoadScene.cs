using UnityEngine;
using System.Collections;

public class LoadScene : MonoBehaviour {
    public void LoadLevel(string name)
    {
        Application.LoadLevel(name);
    }
}
