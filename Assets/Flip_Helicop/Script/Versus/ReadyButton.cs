using UnityEngine;
using System.Collections;

public class ReadyButton : MonoBehaviour {

    GameObject[] Players;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void GetReady()
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in Players)
        {
            player.GetComponent<helicopmovementVersus>().GetReady();
        }
    }
}
