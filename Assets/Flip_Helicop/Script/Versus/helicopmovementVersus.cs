using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class helicopmovementVersus : NetworkBehaviour
{
    public float VAccVelocity = 0.2f;
    public float HAccVelocity = 0.2f;
    float YMin = -3.2f;
    float YMax = 4.2f;
    float XMin = -49f;
    float XMax = 49f;
    float CMin;
    float CMax;
    float gap;
    float Cpos;
    float SelfHealthBarLen = 573f;
    float SelfHealthBarWid = 44f;

    bool didUp = false;
    bool didDown = false;
    bool didLeft = false;
    bool didRight = false;
    bool inside = false;
    bool Rec = false;
    bool BossReced = false;
    public bool ShieldOn = false;
    public bool die = false;
    public bool TurretDie = false;
    public bool start = false;
    public bool end = false;
    public bool BossRec = false;
    [SyncVar]
    public bool side = false;

    Vector3 HeliPos = Vector3.zero;
    Vector3 BasePos = Vector3.zero;
    Vector3 XLimit;

    GameObject Turret;
    GameObject Selfbar;
    public List<GameObject> Laseremitter = new List<GameObject>();
    public GameObject ReadyList;

    int MaxHealth = 100;
    const int SMaxHealth = 50;
    public int Health = 100;
    public int SHealth = 0;
    public int ShieldLife = 0;
    [SyncVar]
    public int teamnum;

    public Sprite redhealth; /* a red health bar used for dire players */
    Network Manager;
    Camera mainCamera;
    Animator animController;
    public BGM bgmCon;

    public override void OnStartLocalPlayer()
    {
        LocalInitialSetting();
    }
    void Start()
    {
        UniversalInitialSetting();
        ReadyList = GameObject.Find("Ready");
        if (isServer)
        {
            Manager = GameObject.Find("NetworkManager").GetComponent<Network>();
            RpcUpdate(Manager.GetComponent<Network>().radientNum, Manager.GetComponent<Network>().direNum, Manager.GetComponent<Network>().Ready); /* update lobby information */
            RpcRegister();
        }
    }

    #region Lobby
    /* a function that updates all clients with number of both sides' players and ready situation as input parameters */
    /* called when a player gets in the lobby or when information needs to be updated */
    [ClientRpc]
    public void RpcUpdate(int r, int d, bool[] Ready)
    {
        if(!hasAuthority)
            return;
        ReadyList = GameObject.Find("Ready");
        Clean();
        /* according to the number of radient players, update players' icons in the lobby */
        for (int i = 0; i < r; i++)
        {
            ReadyList.transform.GetChild(2).GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
        /* according to the number of dire players, update players' icons in the lobby */
        for (int i = 0; i < d; i++)
        {
            ReadyList.transform.GetChild(4).GetChild(i).GetChild(0).gameObject.SetActive(true);
        }
        /* display local player as a special icon */
        if (side)
        {
            ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(0).gameObject.SetActive(false);
            ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(0).gameObject.SetActive(false);
            ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(2).gameObject.SetActive(true);
        }
        /* update ready icons according to 'Ready' array */
        for (int i = 0; i < 5; i++)
        {
            if (Ready[i])
            {
                ReadyList.transform.GetChild(2).GetChild(i).GetChild(0).gameObject.SetActive(false);
                ReadyList.transform.GetChild(2).GetChild(i).GetChild(2).gameObject.SetActive(false);
                ReadyList.transform.GetChild(2).GetChild(i).GetChild(1).gameObject.SetActive(true);
            }
        }
        for (int i = 5; i < 10; i++)
        {
            if (Ready[i])
            {
                ReadyList.transform.GetChild(4).GetChild(i-5).GetChild(0).gameObject.SetActive(false);
                ReadyList.transform.GetChild(4).GetChild(i-5).GetChild(2).gameObject.SetActive(false);
                ReadyList.transform.GetChild(4).GetChild(i-5).GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    /* decrease team number of all players behind when a player with a lower team number disconnects */
    [ClientRpc]
    public void RpcUpdateTeamnum()
    {
        teamnum--;
    }
    /* reset lobby display */
    void Clean()
    {
        for (int i = 0; i < 5; i++)
        {
            ReadyList.transform.GetChild(2).GetChild(i).GetChild(0).gameObject.SetActive(false);
            ReadyList.transform.GetChild(2).GetChild(i).GetChild(1).gameObject.SetActive(false);
            ReadyList.transform.GetChild(2).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
        for (int i = 0; i < 5; i++)
        {
            ReadyList.transform.GetChild(4).GetChild(i).GetChild(0).gameObject.SetActive(false);
            ReadyList.transform.GetChild(4).GetChild(i).GetChild(1).gameObject.SetActive(false);
            ReadyList.transform.GetChild(4).GetChild(i).GetChild(2).gameObject.SetActive(false);
        }
    }
    /* called when a player connects to display player icon */
    [ClientRpc]
    void RpcRegister()
    {
        ReadyList = GameObject.Find("Ready");
        if (side)
        {
            if (hasAuthority)
            {
                ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(0).gameObject.SetActive(true);
            }
        }
        else
        {
            if (hasAuthority)
            {
                ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(0).gameObject.SetActive(true);
            }
        }
        
    }
    /* ready button function */
    public void GetReady()
    {
        if(isLocalPlayer)
            CmdReady();
    }
    /* called when a player is ready */
    [Command]
    void CmdReady()
    {
        Manager.GetComponent<Network>().Ready[teamnum + (side ? 5 : 0)] = true;
        Manager.GetComponent<Network>().CheckStart();
        RpcReady();
    }
    /* inform all clients a player is ready */
    [ClientRpc]
    void RpcReady()
    {
        if (side)
        {
            ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(0).gameObject.SetActive(false);
            ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(2).gameObject.SetActive(false);
            ReadyList.transform.GetChild(4).GetChild(teamnum).GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(0).gameObject.SetActive(false);
            ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(2).gameObject.SetActive(false);
            ReadyList.transform.GetChild(2).GetChild(teamnum).GetChild(1).gameObject.SetActive(true);
        }
    }
    #endregion

    public bool CheckAuthority()
    {
        return hasAuthority ? true : false;
    }

    void Update()
    {
        /* the server will tell all clients to start a recover buff */
        if(isServer)
        {
            /* start boss's crazy buff */
            if(BossRec && !BossReced && !die)
            {
                BossReced = true;
                StartCoroutine(BossRecover());
            }
            /* if the player is near its base, start to recover */
            if (Vector3.Distance(transform.position, BasePos) <= 5f && !Rec && !die) 
            {
                Rec = true;
                RpcStartRec();
                StartCoroutine(Recover());
            }
            if (Vector3.Distance(transform.position, BasePos) > 5f && Rec || die)
            {
                Rec = false;
                RpcStopRec();
            }
        }
        /* for players without authority or when the games hasn't started or has ended already, manipulation is not allowed */
        if (!isLocalPlayer || end || !start)
            return;
        /* movement controlling */
        if (Input.GetKey(KeyCode.W))
            didUp = true;
        if (Input.GetKey(KeyCode.S))
            didDown = true;
        if (Input.GetKey(KeyCode.A))
            didLeft = true;
        if (Input.GetKey(KeyCode.D))
            didRight = true;
        if (Input.GetKeyUp(KeyCode.W))
            didUp = false;
        if (Input.GetKeyUp(KeyCode.S))
            didDown = false;
        if (Input.GetKeyUp(KeyCode.A))
            didLeft = false;
        if (Input.GetKeyUp(KeyCode.D))
            didRight = false;
        /* inform turret to start shooting */
        if(!TurretDie)
            if (side)
            {
                if (!die && hasAuthority && !inside && transform.position.x <= -12f && transform.position.x >= -27f)
                {
                    CmdEnter();
                    inside = true;
                }
                if (!die && hasAuthority && inside && (transform.position.x > -12f || transform.position.x < -27f))
                {

                    CmdLeave();
                    inside = false;
                }
            }
            else
            {
                if (!die && hasAuthority && !inside && transform.position.x >= 12f && transform.position.x <= 27f)
                {

                    CmdEnter();
                    inside = true;
                }
                if (!die && hasAuthority && inside && (transform.position.x < 12f || transform.position.x > 27f))
                {

                    CmdLeave();
                    inside = false;
                }
            }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;
        Cpos = Mathf.Clamp(transform.position.x + gap, CMin, CMax);
        mainCamera.transform.position = new Vector3(Cpos, 0, -10);
        if (didUp)
        {
            transform.position += Vector3.up * VAccVelocity;
        }
        if (didDown)
        {
            transform.position += Vector3.down * VAccVelocity;
        }
        if (didLeft)
        {
            transform.position += Vector3.left * HAccVelocity;
        }
        if (didRight)
        {
            transform.position += Vector3.right * HAccVelocity;
        }
        HeliPos = transform.position;
        HeliPos.y = Mathf.Clamp(HeliPos.y, YMin, YMax);
        HeliPos.x = Mathf.Clamp(HeliPos.x, XMin, XMax);
        transform.position = HeliPos;
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        /* collision to take damage is decided on the server */
        if (!isServer || die)
            return;
        if (collider.tag == "missile" && side != collider.gameObject.GetComponent<missleversus>().side)
        {
            RpcTakeDamage(2);
        }
        else if(collider.tag == "Monster" || collider.CompareTag("Harm") || collider.tag == "ChiWave" && side != collider.gameObject.GetComponent<VChiWave>().side)
        {
            RpcTakeDamage(10);
        }
        else if (collider.tag == "laser" && side != collider.transform.parent.gameObject.GetComponent<helicopmovementVersus>().side)
        {
            Laseremitter.Add(collider.transform.parent.gameObject);
            collider.transform.parent.gameObject.GetComponent<shootingversus>().Laserhitting.Add(this.gameObject);
            StartCoroutine(Burning(collider.transform.parent.gameObject)); /* start coroutine to continuously take damage */
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (isServer && collider.CompareTag("laser"))
            Laseremitter.Remove(collider.transform.parent.gameObject);
    }
    IEnumerator Burning(GameObject Emitter)
    {
        while (Laseremitter.Contains(Emitter))
        {
            RpcTakeDamage(5);
            yield return new WaitForSeconds(0.2f);
        }
    }
    /* a function to take damage with input parameter specifying the value */
    /* input value is negative when healing */
    [ClientRpc]
    public void RpcTakeDamage(int val)
    {
        if (die)
            return;
        if (val > 0)
            StartCoroutine(GetHurt()); /* a corountine that gives visual hint of taking damage */
        if (ShieldOn && val > 0)
        {
            SHealth -= val; /* compare current shield health to damage */
            /* if damage is more than or equal the current shield life, turn off the shield and the rest damage will be exerted on health later */
            if (SHealth <= 0)
            {
                ShieldOn = false;
                transform.GetChild(1).gameObject.SetActive(false);
                if(isLocalPlayer)
                    Selfbar.transform.parent.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(0, SelfHealthBarWid);
                else
                    transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(0, 2.7f);
                if (SHealth == 0) /* if the damage is equal the shield life, no extra damage to health */
                    return;
                val = -SHealth; /* remaining damage */
            }
            /* shield life is enough to take damage, only shield life will be cost */
            else
            {
                if(isLocalPlayer)
                    Selfbar.transform.parent.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(SelfHealthBarLen * SHealth / SMaxHealth, SelfHealthBarWid);
                else
                    transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(14.7f * SHealth / SMaxHealth, 2.7f);
                return; /* no damage to health */
            } 
        }
        Health -= val; /* deal with remaining damage or healing */
        if (Health >= MaxHealth)
            Health = MaxHealth;
        if (isLocalPlayer)
           Selfbar.GetComponent<RectTransform>().sizeDelta = new Vector2(SelfHealthBarLen * Health / MaxHealth, SelfHealthBarWid);
        else
           transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(14.7f * Health / MaxHealth, 2.7f);
        if (Health <= 0)
        {
            GetComponent<shootingversus>().die = true;
            die = true;
            if (isLocalPlayer)
                CmdLaserDie(); /* clear laser list when dead */
            else
                transform.GetChild(2).gameObject.SetActive(false);
            animController.SetTrigger("die");
        }
    }
    void LocalInitialSetting()
    {
        mainCamera = Camera.main;
        Selfbar = GameObject.Find("SelfBar");
        transform.GetChild(2).gameObject.SetActive(false);
        if (side)
        {
            mainCamera.transform.position = new Vector3(40, 0, 0);
            Selfbar.GetComponent<Image>().sprite = redhealth;
        }
        else
        {
            
            mainCamera.transform.position = new Vector3(-40, 0, 0);
        }
        CMin = -50f + (mainCamera.transform.position.x - mainCamera.ScreenToWorldPoint(Vector3.zero).x);
        CMax = 50f - (mainCamera.transform.position.x - mainCamera.ScreenToWorldPoint(Vector3.zero).x);
        gap = mainCamera.transform.position.x - transform.position.x;
        Selfbar.GetComponent<RectTransform>().sizeDelta = new Vector2(SelfHealthBarLen * Health / MaxHealth, SelfHealthBarWid);
        HeliPos = transform.position;
        bgmCon = GameObject.Find("BGM").GetComponent<BGM>();
    }
    void UniversalInitialSetting()
    {
        if (side)
        {
            Turret = GameObject.Find("RadientTurret");
            transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = redhealth;
            GetComponent<SpriteRenderer>().flipX = true;
            Destroy(transform.GetChild(4).gameObject);
            BasePos = new Vector3(49, 0, 0);
        }
        else
        {
            Turret = GameObject.Find("DireTurret");
            Destroy(transform.GetChild(5).gameObject);
            BasePos = new Vector3(-49, 0, 0);
        }
        animController = GetComponent<Animator>();
    }
    void Dead()
    {
        transform.localScale = new Vector3(0.3f, 0.3f, 0);
        if (hasAuthority)
        {
            CmdLeave();
            CmdStopRec();
        }
        if(isServer)
            StartCoroutine(Respawn());
    }
    IEnumerator GetHurt()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    /* when near fountain, heal 5 health and one chi every second */
    IEnumerator Recover()
    {
        while(Rec)
        {
            RpcTakeDamage(-5);
            RpcUltRec();
            yield return new WaitForSeconds(1);
        }
    }
    /* when in the crazy buff, heal 8 health and one chi every second */
    IEnumerator BossRecover()
    {
        RpcStartBossRec();
        for (int i=0;i<30 && BossRec;i++)
        {
            RpcTakeDamage(-8);
            RpcUltRec();
            yield return new WaitForSeconds(1);
        }
        BossRec = false;
        BossReced = false;
        RpcStopBossRec();
    }

    IEnumerator Respawn()
    {
        /* 10 seconds after death, reset player */
        yield return new WaitForSeconds(10);
        RpcRespawn();
    }
    [ClientRpc]
    void RpcRespawn()
    {
        if (side)
            transform.position = new Vector3(45f, 0, 0);
        else
            transform.position = new Vector3(-45f, 0, 0);
        if (!hasAuthority)
            transform.GetChild(2).gameObject.SetActive(true);
        if (hasAuthority)
            for (int i = 0; i < 10; i++)
            {
                gameObject.GetComponent<shootingversus>().UltIncrease();
            }
        Health = 100;
        die = false;
        GetComponent<shootingversus>().die = false;
        GetComponent<shootingversus>().LaserDie = false;
        animController.Play("HelicopAnim");
        if (isLocalPlayer)
            Selfbar.GetComponent<RectTransform>().sizeDelta = new Vector2(SelfHealthBarLen * Health / MaxHealth, SelfHealthBarWid);
        else
            transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(14.7f * Health / MaxHealth, 2.7f);
        GetComponent<Collider2D>().enabled = true;
    }
    [Command]
    void CmdEnter()
    {
        Turret.GetComponent<Turret>().TurretTargets.AddLast(gameObject);
    }
    [Command]
    void CmdLeave()
    {
        Turret.GetComponent<Turret>().TurretTargets.Remove(gameObject);
    }
    /* clear laser list when dead */
    [Command]
    void CmdLaserDie()
    {
        foreach (GameObject item in Laseremitter)
        {
            item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
        }
        Laseremitter.Clear();
    }
    /* functions controlling recover */
    [Command]
    void CmdStopRec()
    {
        BossRec = false;
        Rec = false;
    }
    [ClientRpc]
    void RpcStartRec()
    {
        transform.GetChild(4).gameObject.SetActive(true);
    }
    [ClientRpc]
    void RpcStopRec()
    {
        transform.GetChild(4).gameObject.SetActive(false);
    }
    [ClientRpc]
    void RpcStartBossRec()
    {
        /* play crazy music when having a crazy buff */
        if(hasAuthority)
            bgmCon.ecstasy = true;
        transform.GetChild(5).gameObject.SetActive(true);
    }
    [ClientRpc]
    void RpcStopBossRec()
    {
        transform.GetChild(5).gameObject.SetActive(false);
    }
    [ClientRpc]
    void RpcUltRec()
    {
        if(hasAuthority)
        {
            GetComponent<shootingversus>().UltIncrease();
        }
    }
}