using UnityEngine;
using System.Collections;

public class misslemovement : MonoBehaviour {

    Vector3 move = Vector3.zero;

    float speed = 25f;
    float offset;

    Animator animController = null;
    bool iscolliding = false;

	// Use this for initialization
	void Start () {
        move.x = speed * 0.02f * 1f;
        animController = GetComponent<Animator>();   
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (name == "missile(Clone)")
            if (iscolliding == false)
                transform.position += move;  
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("margin") || collider.CompareTag("spike") || collider.CompareTag("Switch") || collider.CompareTag("Monster") || collider.CompareTag("Boss"))
            {
                animController.SetTrigger("explode");
                iscolliding = true;
            }
    }
    
    void dissapear()
    {
        Destroy(this.gameObject);
    }
}
