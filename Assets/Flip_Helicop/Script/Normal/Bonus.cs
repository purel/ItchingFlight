using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bonus : MonoBehaviour {

    public int UltBonus;
    public float volume = 0.5f;

    public Color32 Tipcolor;
    shooting Bonusshooting;
    GameObject Player;
    GameObject BonusTip = null;

    public AudioClip Sound = null;

    // Use this for initialization
    void Start () {
        Bonusshooting = GameObject.Find("ejactor").GetComponent<shooting>();
        Player = GameObject.Find("player");
        BonusTip = GameObject.Find("BonusTip");
        BonusTip.gameObject.transform.position = Camera.main.WorldToScreenPoint(Player.transform.position) + Vector3.left * 100;
    }
	
	// Update is called once per frame
	void Update () {
        BonusTip.gameObject.transform.position = Camera.main.WorldToScreenPoint(Player.transform.position) + Vector3.left * 100;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // if player collides with the bonus, add chi and show a visual prompt
        if (collider.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(Sound, transform.position, volume);
            BonusTip.GetComponent<Text>().color = Tipcolor;
            BonusTip.GetComponent<Text>().text = "+" + UltBonus + " Chi";
            BonusTip.GetComponent<Animator>().Play("BonusTip");
            for (int i = 0; i < UltBonus; i++)
            {
                if (shooting.Ult != 10)
                    Bonusshooting.UltIncrease();
            }
            Destroy(this.gameObject);
        }
    }
}
