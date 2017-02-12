using UnityEngine;
using System.Collections;

public class ThunderBall : MonoBehaviour {

    public GameObject Player;
    Vector3 Target = Vector3.zero; // the direction that the thunderball shoud move to
    float speed = 0.3f;

	// Use this for initialization
	void Start () {
        Target = (Player.transform.position - transform.position).normalized * speed;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        transform.position += Target;
	}
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") || collider.CompareTag("margin"))
            Destroy(this.gameObject);
    }
}
