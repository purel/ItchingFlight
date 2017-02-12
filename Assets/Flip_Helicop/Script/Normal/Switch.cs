using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Switch: MonoBehaviour {

    Animator animControl;
    Transform Thunder;

	void Start () {
        animControl = GetComponent<Animator>();
        Thunder = transform.parent.FindChild("Thunder");
	}
	
	void Update () {
	    
	}
     void OnTriggerEnter2D(Collider2D collider)
    {
        // when hit by bullet, switch thunder
        if (collider.CompareTag("missile"))
        {
            animControl.SetTrigger("press");
            if (Thunder.gameObject.activeSelf)
                Thunder.gameObject.SetActive(false);
            else
                Thunder.gameObject.SetActive(true);
        }
    }
}
