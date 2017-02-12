using UnityEngine;
using System.Collections;

public class Wraith : MonoBehaviour {

    int Health = 5;
    Animator Anim;
    bool Die = false;

	// Use this for initialization
	void Start () {
        Anim = gameObject.GetComponent<Animator>();
        StartCoroutine(Attack());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("missile"))
        {
            StartCoroutine(GetHurt());
            Health--;
            if (Health >= 0)
                transform.GetChild(1).GetChild(Health).gameObject.SetActive(false);
            if (collider.name == "ChiWave(Clone)")
            {
                Health--;
                if (Health >= 0)
                    transform.GetChild(1).GetChild(Health).gameObject.SetActive(false);
            }
            if (Health <= 0)
            {
                Die = true;
                Anim.Play("WraithDie");
            }
        }
    }

    // when hit turns red
    IEnumerator GetHurt()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }

    // a coroutine that lets wrath summon a blackhole drawing the player
    IEnumerator Attack()
    {
        while (!Die)
        {
            yield return new WaitForSeconds(2);
            if (!Die)
            {
                Anim.Play("WraithAttack");
                transform.GetChild(0).gameObject.SetActive(true); // make blackhole appear
                yield return new WaitForSeconds(3);
                Anim.Play("WraithIdle");
                transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }
    void Dissapear()
    {
        Destroy(this.gameObject);
    }
}
