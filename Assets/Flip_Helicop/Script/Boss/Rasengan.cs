using UnityEngine;
using System.Collections;

public class Rasengan : MonoBehaviour {

    bool Setoff = false; // a boolean that specifies whether the Rasengan is being generated or is moving
    bool Direction = false;

    public AudioClip LaunchSound;
    public AudioClip SetOffSound;

    float speed = 1f;

    Animator Anim;

	// Use this for initialization
	void Start () {
        if (transform.localScale.x == 3)
            Direction = true;
        StartCoroutine(Launch());
        Anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Setoff == true && Direction == false)
            transform.position += Vector3.left * speed;
        else if(Setoff == true && Direction == true)
            transform.position += Vector3.right * speed;
    }
    // play different sound effect for two stages of Rasengan
    IEnumerator Launch()
    {
        AudioSource.PlayClipAtPoint(LaunchSound, transform.position);
        yield return new WaitForSeconds(0.7f);
        AudioSource.PlayClipAtPoint(SetOffSound, transform.position);
        yield return new WaitForSeconds(0.3f);
        Setoff = true;
        
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("margin"))
            Destroy(this.gameObject);
        else if (collider.CompareTag("Player"))
        {
            Setoff = false;
            Anim.Play("RasanEnd");
        }
    }
    void Disappear()
    {
        Destroy(this.gameObject);

    }
}
