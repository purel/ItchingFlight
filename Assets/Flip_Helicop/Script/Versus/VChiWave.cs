using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class VChiWave : NetworkBehaviour
{

    public Vector3 move = Vector3.zero;
    float speed = 25f;
    [SyncVar]
    public bool side = false;
    // Use this for initialization
    void Start()
    {
        if (!side)
            move.x = speed * 0.02f * 1f;
        else
        {
            move.x = speed * 0.02f * -1f;
            GetComponent<SpriteRenderer>().flipY = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
            transform.position += move;
    }

}
