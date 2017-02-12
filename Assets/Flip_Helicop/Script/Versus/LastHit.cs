using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LastHit : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void Life()
    {
        gameObject.GetComponent<Text>().text = "";
    }
}
