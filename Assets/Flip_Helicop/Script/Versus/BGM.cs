using UnityEngine;
using System.Collections;

/* the class that handles music playing */
public class BGM : MonoBehaviour {

    AudioSource Audio;

    public AudioClip Lobby;
    public AudioClip Tie;
    public AudioClip Winning;
    public AudioClip Losing;
    public AudioClip Ecstasy;
    public AudioClip Win;
    public AudioClip Lose;

    AudioClip last;

    public bool side = false;
    public bool start = false;
    public bool tie = false;
    public bool losing = false;
    public bool winning = false;
    public bool win = false;
    public bool lose = false;
    public bool ecstasy = false;
    public bool end = false;

    GameObject[] Players;
    // Use this for initialization
    void Start ()
    {
        Audio = gameObject.GetComponent<AudioSource>();
        Audio.clip = Lobby;
        Audio.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {
        /* if game starts, check every player's side and start to play music */
        if(start)
        {
            start = false;
            Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in Players)
            {
                if (player.GetComponent<helicopmovementVersus>().CheckAuthority())
                {
                    side = player.GetComponent<helicopmovementVersus>().side;
                }
            }
            Audio.clip = Tie;
            Audio.Play();
        }
        /* when the situations for both sides tie */
        if (tie)
        {
            tie = false;
            Audio.clip = Tie;
            Audio.Play();
        }
        /* when one side is losing, play a music that reminds players of hard time */
        if(losing)
        {
            losing = false;
            Audio.clip = Losing;
            Audio.Play();
        }
        /* when one side is winning, play a encouraging music */
        if(winning)
        {
            winning = false;
            Audio.clip = Winning;
            Audio.Play();
        }
        /* when got Boss's buff, play a overjoyed music */
        if(ecstasy)
        {
            ecstasy = false;
            last = Audio.clip;
            Audio.clip = Ecstasy;
            Audio.Play();
            Invoke("PlayBack", 30);
        }
        /* when one side wins, play a victorious music */
        if(win)
        {
            win = false;
            Audio.clip = Win;
            Audio.Play();
        }
        /* when one side loses, play a lamentable music */
        if(lose)
        {
            lose = false;
            Audio.clip = Lose;
            Audio.Play();
        }
	}
    void PlayBack()
    {
        Audio.clip = last;
        Audio.Play();
    }
}
