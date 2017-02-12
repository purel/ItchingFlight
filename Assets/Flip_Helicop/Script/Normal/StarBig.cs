using UnityEngine;
using System.Collections;

// a class used for animation event
public class StarBig : MonoBehaviour {

    GameObject FlyingPig;
    GameObject Player;
    Vector2 temp;

	// Use this for initialization
	void Start () {
        FlyingPig = transform.parent.gameObject;
        Player = GameObject.Find("player");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void Dissappear()
    {
        this.gameObject.SetActive(false);
        if (!FlyingPig.GetComponent<FlyingPig>().Die)
        {
            temp = (Player.transform.position - FlyingPig.transform.position).normalized;
            FlyingPig.GetComponent<FlyingPig>().Target = temp;
            FlyingPig.GetComponent<Animator>().Play("FlyingPigAttack");
            FlyingPig.GetComponent<FlyingPig>().Attacking = true;
            FlyingPig.GetComponent<FlyingPig>().Margin = false;
        }
    }
}
