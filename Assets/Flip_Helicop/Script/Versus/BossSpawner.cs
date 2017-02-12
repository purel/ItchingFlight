using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

/* the class that manages boss's spawning and handle game start */
public class BossSpawner : NetworkBehaviour {

    bool respawn = false;
    public bool start = false;
    public bool gameStart = false;

    public GameObject BossPrefab;
    GameObject Boss;
    public GameObject GameStartText; /* The text reminds players of the start of the game */
    public GameObject Frame; /* Boss's health UI frame */

    public Text SpawnTime; /* a counter showing the time left before boss respawn */
    Text Tip; /* a reminder when boss is about to respawn */
    
    int Time = 60; /* time interval of respawning */

    AudioSource Audio;
    public AudioClip BossHowl;

    
    // Use this for initialization
    override public void OnStartClient()
    {
        ClientScene.RegisterPrefab(BossPrefab);
    }
    void Start ()
    {
        if (!isServer)
            GetComponent<BossSpawner>().enabled = false;
        Audio = GetComponent<AudioSource>();
        Tip = GameObject.Find("BossFirstInfo").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        /* once game starts, start coroutines */
        if(gameStart)
        {
            gameStart = false;
            StartCoroutine(FirstSpawn());
            RpcShowStart(); /* show 'Gamestart' text */
        }
	    if(start && !respawn)
        {
            respawn = true;
            RpcFrameOff(); /* turn off health bar */
            StartCoroutine(Tips()); /* show reminder */
            RpcSpawnStart(); /* show counter*/
            StartCoroutine(Spawn());/* start to respawn */
        }
	}
    /* for the first time, wait for 20 seconds before star to spawn */
    IEnumerator FirstSpawn()
    {
        yield return new WaitForSeconds(20);
        start = true;
    }
    /* a coroutine to spawn boss */
    IEnumerator Spawn()
    {
        /* Time is shown in the counter */
        for (int i = 1; i <= Time; i++)
        {
            yield return new WaitForSeconds(1);
            RpcTimeTick(Time - i);
        }
        RpcFrameOn(); /* show Boss's UI health bar */
        Boss = Instantiate(BossPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        NetworkServer.Spawn(Boss);
        StartCoroutine(BossSpawned());
        start = false;
        respawn = false;
        Rpcrespawn(); /* turn off counter */
    }
    /* toggle reminder */
    IEnumerator Tips()
    {
        RpcTipOn();
        yield return new WaitForSeconds(5);
        RpcTipOff();
    }
    IEnumerator BossSpawned()
    {
        RpcBossSpawnedInfo(); /* show text when boss is spawned */
        yield return new WaitForSeconds(5);
        RpcTipOff(); /* turn if off after 5 seconds */
    }
    [ClientRpc]
    void RpcShowStart()
    {
        GameStartText.SetActive(true);
    }
    [ClientRpc]
    void RpcSpawnStart()
    {
        SpawnTime.gameObject.SetActive(true);
        SpawnTime.text = Time.ToString();
    }
    [ClientRpc]
    void Rpcrespawn()
    {
        SpawnTime.gameObject.SetActive(false);
    }
    [ClientRpc]
    void RpcTimeTick(int Time)
    {
        SpawnTime.text = Time.ToString();
    }
    [ClientRpc]
    void RpcTipOn()
    {
        Tip.text = "Boss will arrive in 60 seconds";
    }
    [ClientRpc]
    void RpcTipOff()
    {
        Tip.text = "";
    }
    [ClientRpc]
    void RpcFrameOn()
    {
        Frame.SetActive(true);
    }
    [ClientRpc]
    void RpcFrameOff()
    {
        Frame.SetActive(false);
    }
    [ClientRpc]
    void RpcBossSpawnedInfo()
    {
        Tip.text = "Mr.Flying Carrot has come!!!";
        Audio.clip = BossHowl;
        Audio.Play();
    }
}
