using UnityEngine;
using System.Collections;

// This class is only for laser hitting Wrath
public class LaserExplodeBoss : MonoBehaviour
{

    Animator AnimController;
    GameObject Destructible =null;
    // Use this for initialization
    void Start()
    {
        AnimController = gameObject.GetComponent<Animator>();
        AnimController.Play("LaserExplode");
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Monster"))
            Destructible = collider.gameObject;
    }
    void Dissapear()
    {
        Destroy(Destructible.gameObject);
        Destroy(this.gameObject);
    }
}
