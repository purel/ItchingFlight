using UnityEngine;
using System.Collections;

public class LaserExplode : MonoBehaviour {

    Animator AnimController;
    GameObject Destructible;

	// Use this for initialization
	void Start () {
        AnimController = gameObject.GetComponent<Animator>();
        AnimController.Play("LaserExplode");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter2D(Collider2D collider)
    {
        // Destroy whatever collides with it
        if (collider.CompareTag("Switch") || collider.CompareTag("spike"))
        {
            if (collider.name == "Thunder" || collider.name == "uppillar(Clone)" || collider.name == "downpillar(Clone)")
                Destructible = collider.gameObject;
            else 
                Destructible = collider.transform.parent.gameObject;
         }
        else if(collider.CompareTag("Monster"))
            Destructible = collider.gameObject;
    }
    void Dissapear()
    {
        Destroy(Destructible.gameObject);
        Destroy(this.gameObject);
    }
}
