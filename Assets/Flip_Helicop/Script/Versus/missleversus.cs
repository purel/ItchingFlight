using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class missleversus : NetworkBehaviour
{

    public Vector3 move = Vector3.zero;
    float speed = 25f;
    Animator animController = null;
    bool iscolliding = false;
    public bool Turret = false;
    [SyncVar]
    public bool side = false;
    // Use this for initialization
    void Start()
    {
        if (!Turret)
        {
            if (!side)
                move.x = speed * 0.02f * 1f;
            else
            {
                move.x = speed * 0.02f * -1f;
                GetComponent<SpriteRenderer>().flipX = true;
            }
        }
        animController = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!iscolliding)
            transform.position += move;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Monster") || collider.CompareTag("margin") && collider.gameObject.GetComponent<Turret>().side != side || (collider.CompareTag("Player") && collider.gameObject.GetComponent<helicopmovementVersus>().side != side) || collider.CompareTag("Lightning") && collider.gameObject.GetComponent<Lightning>().side != side || collider.CompareTag("spike") && collider.gameObject.GetComponent<Tower>().side != side || collider.CompareTag("Base") && collider.gameObject.GetComponent<Base>().side != side)
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
