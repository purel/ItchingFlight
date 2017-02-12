using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Network : NetworkManager {

    bool start = false;
    bool found = false;
    public bool[] Ready = new bool[10]; /* keep track of players' readiness */

    public int radientNum = -1; /* radient players' number */
    public int direNum = 0; /* dire players' number */

    List<NetworkConnection> radientcons = new List<NetworkConnection>(); /* radient players' connections */
    List<NetworkConnection> direcons = new List<NetworkConnection>(); /* dire players' connections */

    public GameObject ConnectionButtons;
    public GameObject StopHostButton;
    public GameObject Lobby;
    GameObject Spawner;
    GameObject[] Players;

    public Text IP;

	// Use this for initialization
	void Start ()
    {
        //ConnectionButtons = GameObject.Find("ConnectionButtons");
        //StopHostButton = GameObject.Find("StopHostButton");
        for (int i=0;i<10;i++)
        {
            Ready[i] = false;
        }
    }
	// Update is called once per frame
	void Update ()
    {
        
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        /* once start or full, no new connection is allowed */
        if (start == true || (radientNum == 5 && direNum == 5))
            conn.Disconnect();
        /* this is host's server connection */
        if (radientNum == 0)
        {
            radientNum++;
            return;
        }
        /* this is host's client connection */
        if (radientNum == -1)
        {
            radientNum++;
            radientcons.Add(conn);
            return;
        }
        if (radientNum <= direNum)
        {
            radientNum++;
            radientcons.Add(conn);
        }
        else
        {
            direNum++;
            direcons.Add(conn);
        }
    }
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        /* search the disconnecting client in radient*/
        foreach (NetworkConnection item in radientcons)
        {
            if (conn == item)
            {
                /* disconnection process */
                radientNum--;
                found = true;
                int index = radientcons.IndexOf(conn);
                Players = GameObject.FindGameObjectsWithTag("Player");
                /* decrease team number of all players behind when a player with a lower team number disconnects */
                foreach (GameObject player in Players)
                {
                    if(player.GetComponent<helicopmovementVersus>().side == false && player.GetComponent<helicopmovementVersus>().teamnum > index)
                    {
                        player.GetComponent<helicopmovementVersus>().RpcUpdateTeamnum();
                    }
                }
                int i;
                for(i = index + 1; i < radientNum + 1; i++)
                {
                    Ready[i - 1] = Ready[i];
                    Ready[i] = false;
                }
                /* update current information to all clients */
                foreach (GameObject player in Players)
                {
                    player.GetComponent<helicopmovementVersus>().RpcUpdate(radientNum,direNum,Ready);
                }
                radientcons.Remove(item);
                break;
            }
        }
        if (!found)
        {
            /* search the disconnecting client in dire*/
            foreach (NetworkConnection item in direcons)
            {
                if (conn == item)
                {
                    /* disconnection process */
                    direNum--;
                    int index = direcons.IndexOf(conn);
                    Players = GameObject.FindGameObjectsWithTag("Player");
                    /* decrease team number of all players behind when a player with a lower team number disconnects */
                    foreach (GameObject player in Players)
                    {
                        if (player.GetComponent<helicopmovementVersus>().side == true && player.GetComponent<helicopmovementVersus>().teamnum > index)
                        {
                            player.GetComponent<helicopmovementVersus>().RpcUpdateTeamnum();
                        }
                    }
                    int i;
                    for (i = 5 + index + 1; i < 5 + direNum + 1; i++)
                    {
                        Ready[i - 1] = Ready[i];
                        Ready[i] = false;
                    }
                    /* update current information to all clients */
                    foreach (GameObject player in Players)
                    {
                        player.GetComponent<helicopmovementVersus>().RpcUpdate(radientNum, direNum,Ready);
                    }
                    direcons.Remove(item);
                    break;
                }
            }
        }
        found = false;
        NetworkServer.DestroyPlayersForConnection(conn);
        if(!start)
            CheckStart();
    }
    public override void OnClientConnect(NetworkConnection conn)
    {
        Lobby.SetActive(true);
        ClientScene.Ready(conn);
        ClientScene.AddPlayer(0);
    }
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        foreach(NetworkConnection item in radientcons)
        {
            if(conn == item)
            {
                /* spawn the player in the designated location, set its side and teamnumber */
                GameObject player = (GameObject)Instantiate(playerPrefab, new Vector3(-45, 0, 0), Quaternion.identity);
                player.GetComponent<helicopmovementVersus>().side = false;
                player.GetComponent<helicopmovementVersus>().teamnum = radientcons.IndexOf(conn);
                NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
                return;
            }
        }
        foreach (NetworkConnection item in direcons)
        {
            if(conn == item)
            {
                /* spawn the player in the designated location, set its side and teamnumber */
                GameObject player = (GameObject)Instantiate(playerPrefab, new Vector3(45, 0, 0), Quaternion.identity);
                player.GetComponent<helicopmovementVersus>().side = true;
                player.GetComponent<helicopmovementVersus>().teamnum = direcons.IndexOf(conn); 
                NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
                return;
            }
        }
    }
    public override void OnStopHost()
    {
        for (int i = 0; i < 10; i++)
        {
            Ready[i] = false;
        }
        radientNum = -1;
        direNum = 0;
        radientcons.Clear();
        direcons.Clear();
    }
    public void CheckStart()
    {
        /* check whether all players are ready and at least there are players in two sides */
        if (radientNum == 0 || direNum == 0)
            return;
        for (int i = 0; i < radientNum; i++)
            if (Ready[i] == false)
                return;
        for (int i = 0; i < direNum; i++)
            if (Ready[i + 5] == false)
                return;
        start = true;
        /* start the timer of bossspawner */
        Spawner = GameObject.Find("BossSpawner");
        Spawner.GetComponent<BossSpawner>().gameStart = true;
    }
    /* button functions */
    public void ClickStartHost()
    {
        ConnectionButtons.SetActive(false);
        StopHostButton.SetActive(true);
        StartHost();
    }
    public void ClickStartClient()
    {
        ConnectionButtons.SetActive(false);
        StopHostButton.SetActive(true);
        StartClient();
    }
    public void GetIPAddress()
    {
        networkAddress = IP.text;
    }
    public void ClickStop()
    {
        Lobby.SetActive(false);
        ConnectionButtons.SetActive(true);
        StopHost();
        StopHostButton.SetActive(false);
    }
    public void ClickBack()
    {
        SceneManager.LoadScene(0);
    }
}
