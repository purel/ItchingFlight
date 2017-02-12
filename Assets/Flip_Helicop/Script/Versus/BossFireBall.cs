using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BossFireBall : NetworkBehaviour
{

    public Vector3 move = Vector3.zero;
    Animator animController = null;
    bool iscolliding = false;
    // Use this for initialization
    void Start()
    {
        animController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!iscolliding)
            transform.position += move;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            animController.Play("FireBallDie");
            iscolliding = true;
        }
    }

    void dissapear()
    {
        Destroy(this.gameObject);
    }
}
