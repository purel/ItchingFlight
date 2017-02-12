using UnityEngine;
using System.Collections;

public class MagicCircle : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    // an animation event
    void Disappear()
    {
        Destroy(this.gameObject);
    }
}
