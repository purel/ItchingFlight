using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {
    public GameObject Explode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Switch") || collider.CompareTag("spike") || collider.CompareTag("Monster"))
        {
            GameObject ExplodeTemp = Instantiate(Explode, collider.transform.position, Quaternion.identity) as GameObject;
            ExplodeTemp.SetActive(true);
        }
    }
}
