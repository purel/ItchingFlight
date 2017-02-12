using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BonusBossSmall : MonoBehaviour
{

    public int UltBonus; // For different type of fires, the amount of Chi is different. Values are set in the Unity inspector

    GameObject Ejactor;
    GameObject BonusTip = null; // the text that will appear to give a visual hint of the number of Chi gained
    GameObject Player;

    public AudioClip Sound = null;
    public float volume = 0.5f;
    public Color32 Tipcolor; // The color for the text, the color is same as the fire color, each color is set in the Unity inspector

    
    // Use this for initialization
    void Start()
    {
        Player = GameObject.Find("player");
        Ejactor = GameObject.Find("ejactor");
        BonusTip = GameObject.Find("BonusTip");
        BonusTip.gameObject.transform.position = Camera.main.WorldToScreenPoint(Player.transform.position) + Vector3.left * 100;
    }

    // Update is called once per frame
    void Update()
    {
        BonusTip.gameObject.transform.position = Camera.main.WorldToScreenPoint(Player.transform.position) + Vector3.left * 100;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        
        if (collider.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(Sound, transform.position, volume);
            BonusTip.GetComponent<Text>().color = Tipcolor;
            BonusTip.GetComponent<Text>().text = "+" + UltBonus + " Chi";
            BonusTip.GetComponent<Animator>().Play("BonusTip");
            for (int i = 0; i < UltBonus; i++)
            {
                if (shootingBoss.Ult != 10)
                    Ejactor.GetComponent<shootingBoss>().UltIncrease();
            }
            Destroy(this.gameObject);
        }
    }
}
