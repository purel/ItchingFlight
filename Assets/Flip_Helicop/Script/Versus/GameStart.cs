using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/* the class that hadles the transition between lobby and game */
public class GameStart : NetworkBehaviour {

    Animator TimerAnim;
    Text TimerText;

    GameObject Ready;
    GameObject[] Players;
    GameObject Lobby;
    public GameObject buttonManager;

    public BGM bgmcon;
	// Use this for initialization
	void Start ()
    {
        TimerText = transform.GetChild(0).GetComponent<Text>();
        TimerAnim = transform.GetChild(0).GetComponent<Animator>();
        StartCoroutine(Count());
        Players = GameObject.FindGameObjectsWithTag("Player");
        Ready = transform.parent.gameObject;
        Lobby = GameObject.Find("LobbyBackgroundCanvas");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    /* before game start, count 5 seconds */
    IEnumerator Count()
    {
        int Timer = 5;
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1);
            Timer--;
            TimerText.text = Timer.ToString();
        }
        foreach(GameObject player in Players)
        {
            if (player.GetComponent<helicopmovementVersus>().CheckAuthority())
            {
                player.GetComponent<helicopmovementVersus>().start = true;
                player.GetComponent<shootingversus>().start = true;
            }
        }
        buttonManager.GetComponent<ButtonManager>().start = true;
        bgmcon.start = true;
        Lobby.SetActive(false);
        Ready.SetActive(false);
        gameObject.SetActive(false);
    }
}
