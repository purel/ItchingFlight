using UnityEngine;
using System.Collections;

public class FireBall : MonoBehaviour {

    GameObject Player;
    Vector2 Target = Vector2.zero; // player's position
    Vector3 TrueTarget = Vector3.zero;
    public float speed = 0.2f;

	// Use this for initialization
	void Start () {
        Player = GameObject.Find("player");
        Target = Player.transform.position - transform.position;
        Target = Target.normalized;
        TrueTarget.x = Target.x;
        TrueTarget.y = Target.y;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.position += TrueTarget*speed ;
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("margin") || collider.CompareTag("Player") || collider.CompareTag("Shield"))
        {
            Destroy(this.gameObject);
        }

    }
}
