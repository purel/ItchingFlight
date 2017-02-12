using UnityEngine;
using System.Collections;

public class StageGui : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    // animation event
    void Disappear()
    {
        this.gameObject.SetActive(false);
    }
}
