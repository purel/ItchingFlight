using UnityEngine;
using System.Collections;

public class ChiWave : MonoBehaviour {

    public float speed = 0.6f;
    bool iscolliding = false;
    Animator animController;
	// Use this for initialization
	void Start () {
        animController = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void FixedUpdate()
    {
        if (iscolliding == false)
            transform.position += Vector3.right*speed;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("margin") || collider.CompareTag("spike") || collider.CompareTag("Switch"))
        {
            animController.SetTrigger("explode");
            iscolliding = true;
        }
    }
    void Dissapear()
    {
        Destroy(this.gameObject);
    }
}
