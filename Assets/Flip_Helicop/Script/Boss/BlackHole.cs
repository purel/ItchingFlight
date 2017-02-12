using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {

    public GameObject Player;
    Vector3 Draw = Vector3.zero; // the vector that defines the movement of the drew player
    float drawSpeed = 0.28f; // drawing speed
    bool IsInside = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Draw = ((transform.position - Player.transform.position).normalized) * drawSpeed;
        Player.transform.position += Draw;
	}
    // a coroutine that keeps damaging the player when it's inside
    IEnumerator BlackHoleDraw()
    {
        while (IsInside)
        {
            yield return new WaitForSeconds(0.8f);
            if (Player.GetComponent<helicopmovementBoss>().ShieldOn)
            {
                Player.GetComponent<helicopmovementBoss>().ShieldLife--;
                if (Player.GetComponent<helicopmovementBoss>().ShieldLife >= 0)
                    Player.GetComponent<helicopmovementBoss>().transform.GetChild(5).GetChild(Player.GetComponent<helicopmovementBoss>().ShieldLife).gameObject.SetActive(false);
                if (Player.GetComponent<helicopmovementBoss>().ShieldLife == 0)
                {
                    Player.GetComponent<helicopmovementBoss>().transform.GetChild(6).gameObject.SetActive(false);
                    Player.GetComponent<helicopmovementBoss>().ShieldOn = false;
                }
            }
            // if player has no shield on, then let the player die
            else
            {
                Player.GetComponent<helicopmovementBoss>().die = true;
                Player.GetComponent <Animator>().SetTrigger("die");
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            IsInside = true;
            StartCoroutine(BlackHoleDraw());
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
            IsInside = false;
    }
}
