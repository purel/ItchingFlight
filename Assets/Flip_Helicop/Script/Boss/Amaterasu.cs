using UnityEngine;
using System.Collections;

public class Amaterasu : MonoBehaviour {
    public GameObject Player;
    Animator Anim;
	// Use this for initialization
	void Start () {
        Anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
     void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Anim.Play("AmaterasuOut");
        }
    }
    void Disappear()
    {
        Destroy(this.gameObject);
    }
}
