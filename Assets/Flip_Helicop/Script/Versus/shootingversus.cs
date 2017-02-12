using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Networking;

public class shootingversus : NetworkBehaviour
{

    public static int Ult = 10; /* chi amount */
    int ChiPotionAmount = 0;
    int CoolAmount = 0;

    bool ChiWavePurchased;
    bool LaserPurchased;
    bool AimPurchased;
    bool ShieldPurchased;
    bool IsChiWaveCoolDown = false;
    bool IsTargetCoolDown = false;
    bool IsShieldCoolDown = false;
    bool Laseron = false;
    bool Aiming = false;
    bool IsChiPotionC = false;
    bool IsCoolC = false;
    public bool LaserDie = false;
    public bool side = false; /* side it belong to */
    public bool die = false;
    public bool end = false; /* end of game */
    public bool start = false; /* start of game */

    public GameObject missile;
    public GameObject ChiWave;
    public GameObject ChiWaveIcon;
    public GameObject LaserIcon;
    public GameObject TargetIcon;
    public GameObject ShieldSkillIcon;
    public GameObject Aim;
    public GameObject ChiPotionC;
    public GameObject CoolC;
    public GameObject Fires;
    public GameObject Laser;
    GameObject CShadow;
    GameObject LShadow;
    GameObject TShadow;
    GameObject SShadow;
    GameObject TempAim;
    GameObject TargetedEnemy;
    GameObject[] Enemies = new GameObject[10]; /* an array that helps with sniper shoot */
    public List<GameObject> Laserhitting = new List<GameObject>(); /* a list keeping track of gameobjects the laser is hitting */

    public AudioClip ShieldSound;
    public AudioClip MissileSound = null;
    public AudioClip SniperSound = null;
    public AudioClip ChiSound = null;

    Text CTimer;
    Text TTimer;
    Text STimer;
    float Ctimer;
    float Ttimer;
    float Stimer;

    int SHealth = 0;
    int SMaxHealth = 50;
    float SelfHealthBarLen = 573f;
    float SelfHealthBarWid = 44f;

    Image SelfShieldBar;

    // Use this for initialization
    void Start()
    {
        ShootingInitialize();    
    }
    // Update is called once per frame
    void Update()
    {
        /* only localplayer has authority to control */
        if (!isLocalPlayer || end || !start)
            return;
        if (die && Laseron && !LaserDie)
        {
            CmdLaserOff();
            LaserDie = true;
        }
        /* shoot bullet */
        if (Input.GetButtonDown("Fire1") && !die)
        {
            AudioSource.PlayClipAtPoint(MissileSound, transform.position);
            CmdFiremissile();
        }
        /* shoot Chi wave */
        if (Input.GetButtonDown("Fire2") && !IsChiWaveCoolDown && Ult >= 1 && !die)
        {
            AudioSource.PlayClipAtPoint(ChiSound, transform.position);
            CShadow.SetActive(true);
            CTimer.gameObject.SetActive(true);
            CShadow.GetComponent<Animator>().Play("CoolDown");
            UltDecrease();
            IsChiWaveCoolDown = true;
            StartCoroutine(ChiWaveCoolDown());
            CmdFireChi();
        }
        /* Sniper shoot */
        if (Input.GetKeyDown(KeyCode.E) && !IsTargetCoolDown && Ult >= 2 && !die)
        {
            TShadow.SetActive(true);
            TTimer.gameObject.SetActive(true);
            TShadow.GetComponent<Animator>().Play("CoolDown");
            UltDecrease();
            UltDecrease();
            IsTargetCoolDown = true;
            StartCoroutine(TargetCoolDown());
            CmdFireAim();
        }
        /* Toggle laser */
        if (Input.GetKeyDown(KeyCode.Q) && LaserPurchased && !die)
        {
            if (!Laseron && Ult >= 3)
            {
                UltDecrease();
                CmdLaserOn();
                Laseron = true;
                StartCoroutine(LaserBurn());
                LShadow.SetActive(true);
            }
            else if (Laseron)
            {
                CmdLaserOff();
                Laseron = false;
                LShadow.SetActive(false);
            }
        }
        /* turn on shield */
        if (Input.GetKeyDown(KeyCode.F) && !IsShieldCoolDown && Ult >= 3 && !die)
        {
            AudioSource.PlayClipAtPoint(ShieldSound, transform.position);
            SShadow.SetActive(true);
            STimer.gameObject.SetActive(true);
            SShadow.GetComponent<Animator>().Play("ShieldCoolDown");
            UltDecrease();
            UltDecrease();
            UltDecrease();
            IsShieldCoolDown = true;
            StartCoroutine(ShieldCoolDown());
            CmdShield();
        }
        /* take chi potion */
        if (Input.GetKeyDown(KeyCode.Alpha1) && ChiPotionAmount >= 1 && !IsChiPotionC && !die)
        {
            IsChiPotionC = true;
            ChiPotionAmount--;
            PlayerPrefs.SetInt("ChiPotion", ChiPotionAmount);
            ChiPotionC.transform.parent.parent.GetComponent<Text>().text = "X " + ChiPotionAmount;
            Ult = 10;
            for (int i = 0; i < 10; i++)
                Fires.transform.GetChild(i).gameObject.SetActive(true);
            ChiPotionC.transform.parent.GetComponent<Animator>().Play("ChiPotionC");
            StartCoroutine(ChiPotionReady());
        }
        /* take cool down potion */
        if (Input.GetKeyDown(KeyCode.Alpha2) && CoolAmount >= 1 && !IsCoolC && !die)
        {
            IsCoolC = true;
            CoolAmount--;
            PlayerPrefs.SetInt("CoolDown", CoolAmount);
            CoolC.transform.parent.parent.GetComponent<Text>().text = "X " + CoolAmount;
            if (ChiWavePurchased)
            {
                IsChiWaveCoolDown = false;
                CShadow.SetActive(false);
                CTimer.gameObject.SetActive(false);
            }
            if (AimPurchased)
            {
                IsTargetCoolDown = false;
                TShadow.SetActive(false);
                TTimer.gameObject.SetActive(false);
            }
            if (ShieldPurchased)
            {
                IsShieldCoolDown = false;
                SShadow.SetActive(false);
                STimer.gameObject.SetActive(false);
            }
            CoolC.transform.parent.GetComponent<Animator>().Play("CoolC");
            StartCoroutine(CoolReady());
        }
        /* display cool down timing */
        if (IsChiWaveCoolDown == true)
        {
            Ctimer -= Time.deltaTime;
            CTimer.text = Ctimer.ToString("F2");
        }
        if (IsTargetCoolDown == true)
        {
            Ttimer -= Time.deltaTime;
            TTimer.text = Ttimer.ToString("F2");
        }
        if (IsShieldCoolDown == true)
        {
            Stimer -= Time.deltaTime;
            STimer.text = Stimer.ToString("F2");
        }
        
    }

    void FixedUpdate()
    {
        /* keep sniper shoot sign following the target */
        if (Aiming)
        {
            TempAim.transform.position = TargetedEnemy.transform.position;
        }
    }

    /* Chi increase function */
    public void UltIncrease()
    {
        if (Ult < 10)
        {
            Ult += 1;
            Fires.transform.GetChild(Ult - 1).gameObject.SetActive(true);
        }
    }
    /* Chi decrease function */
    public void UltDecrease()
    {
        Ult -= 1;
        Fires.transform.GetChild(Ult).gameObject.SetActive(false);
    }

    #region cooldown
    /* IEnumerators that control cool down of skills */
    IEnumerator ChiWaveCoolDown()
    {
        Ctimer = 5f;
        yield return new WaitForSeconds(5);
        IsChiWaveCoolDown = false;
        CShadow.SetActive(false);
        CTimer.gameObject.SetActive(false);
    }
    IEnumerator TargetCoolDown()
    {
        Ttimer = 12f;
        yield return new WaitForSeconds(12);
        IsTargetCoolDown = false;
        TShadow.SetActive(false);
        TTimer.gameObject.SetActive(false);
    }
    IEnumerator ShieldCoolDown()
    {
        Stimer = 10f;
        yield return new WaitForSeconds(10);
        IsShieldCoolDown = false;
        SShadow.SetActive(false);
        STimer.gameObject.SetActive(false);
    }
    IEnumerator ChiPotionReady()
    {
        yield return new WaitForSeconds(30);
        IsChiPotionC = false;
    }
    IEnumerator CoolReady()
    {
        yield return new WaitForSeconds(40);
        IsCoolC = false;
    }
    #endregion
    
    /* functions called on the server to instantiate corresponding projectiles */
    [Command]
    void CmdFiremissile()
    {
        GameObject tempmis;
        if(side)
        {
            tempmis = Instantiate(missile, transform.position + Vector3.left, Quaternion.identity) as GameObject;
            tempmis.GetComponent<missleversus>().side = true;
        }
        else
            tempmis = Instantiate(missile, transform.position, Quaternion.identity) as GameObject;
        tempmis.SetActive(true);
        NetworkServer.Spawn(tempmis);
        /* bullet will be destroyed in 0.7 seconds */
        Destroy(tempmis, 0.7f);
    }

    [Command]
    void CmdFireChi()
    {
        GameObject tempmis;
        if (side)
        {
            tempmis = Instantiate(ChiWave, transform.position + Vector3.left, Quaternion.Euler(0,0,90)) as GameObject;
            tempmis.GetComponent<VChiWave>().side = true;
        }
        else
            tempmis = Instantiate(ChiWave, transform.position, Quaternion.Euler(0, 0, 90)) as GameObject;
        tempmis.SetActive(true);
        NetworkServer.Spawn(tempmis);
        Destroy(tempmis, 2.0f);
    }

    #region laser
    /* called on the server to let clients turn on laser */
    [Command]
    void CmdLaserOn()
    {
        RpcLaserOn();
    }

    [Command]
    void CmdLaserOff()
    {
        /* remove this gameobject from laseremitter lists of gameobjects being hitted */
        foreach(GameObject item in Laserhitting)
        {
            if (item.CompareTag("Monster"))
                item.GetComponent<VBoss>().Laseremitter.Remove(this.gameObject);
            else if (item.CompareTag("margin"))
                item.GetComponent<Turret>().Laseremitter.Remove(this.gameObject);
            else if (item.CompareTag("spike"))
                item.GetComponent<Tower>().Laseremitter.Remove(this.gameObject);
            else if (item.CompareTag("Player"))
                item.GetComponent<helicopmovementVersus>().Laseremitter.Remove(this.gameObject);
            else if (item.CompareTag("Base"))
                item.GetComponent<Base>().Laseremitter.Remove(this.gameObject);
        }
        /* clear all the gameobjects in the list the laser is hitting  */
        Laserhitting.Clear();
        RpcLaserOff();
    }

    /* called on all clients to turn on laser */
    [ClientRpc]
    void RpcLaserOn()
    {
        if (side)
        {
            Laser.GetComponent<SpriteRenderer>().flipX = true;
            Laser.transform.localPosition = new Vector3(-8, -0.3f, 0);
        }
        Laser.SetActive(true);
    }

    /* called on all clients to turn off laser */
    [ClientRpc]
    void RpcLaserOff()
    {
        Laser.SetActive(false);
        if (hasAuthority)
        {
            Laseron = false;
            LShadow.SetActive(false);
        }
     }
    IEnumerator LaserBurn()
    {
        while (true)
        {
            /* while laser on keep decrease chi */
            if (Laseron)
            {
                UltDecrease();
                if (Ult == 0)
                {
                    CmdLaserOff();
                    Laseron = false;
                    LShadow.SetActive(false);
                }
                yield return new WaitForSeconds(1);
            }
            else
                yield break;
        }
    }
    #endregion
    #region Aim
    [Command]
    void CmdFireAim()
    {
        Enemies = GameObject.FindGameObjectsWithTag("Player");
        int index = 0;
        int temp = 0;
        int min = 101;
        /* choose the enemy with lowest health as target on the server */
        foreach (GameObject item in Enemies)
        {
            if (side != item.GetComponent<helicopmovementVersus>().side && !item.GetComponent<helicopmovementVersus>().die)
            {
                temp = item.GetComponent<helicopmovementVersus>().Health;
                if (temp < min)
                {
                    min = temp;
                    index = Array.IndexOf(Enemies, item);
                }
            }
        }
        TargetedEnemy = Enemies[index];
        RpcFireAim(TargetedEnemy); /* a coroutine that runs in all clients to spawn the sniper aiming animation */
        StartCoroutine(TakeAimDamage()); /* a coroutine that makes target take damage after amount of time */
        Array.Clear(Enemies, 0, Enemies.Length);
    }
    [ClientRpc]
    void RpcFireAim(GameObject TargetEnemy)
    {
        StartCoroutine(Target(TargetEnemy));
    }
    /* the coroutine that runs on every client to creat the sniper aim */
    IEnumerator Target(GameObject TargetEnemy)
    {
        TempAim = Instantiate(Aim, TargetEnemy.transform.position, Quaternion.identity) as GameObject;
        TempAim.SetActive(true);
        TargetedEnemy = TargetEnemy;
        Aiming = true;
        yield return new WaitForSeconds(1.5f);
        AudioSource.PlayClipAtPoint(SniperSound, transform.position);
        Aiming = false;
        Destroy(TempAim.gameObject);

    }
    IEnumerator TakeAimDamage()
    {
        yield return new WaitForSeconds(1.5f);
        TargetedEnemy.GetComponent<helicopmovementVersus>().RpcTakeDamage(25);
    }
    #endregion
    #region Shield
    [Command]
    void CmdShield()
    {
        RpcShield();
    }
    /* called on all clients to turn on shield */
    [ClientRpc]
    void RpcShield()
    {
        gameObject.GetComponent<helicopmovementVersus>().ShieldOn = true;
        gameObject.transform.GetChild(1).gameObject.SetActive(true);
        gameObject.GetComponent<helicopmovementVersus>().SHealth = 50;
        SHealth = 50;
        if (isLocalPlayer)
            SelfShieldBar.GetComponent<RectTransform>().sizeDelta = new Vector2(SelfHealthBarLen * SHealth / SMaxHealth, SelfHealthBarWid);
        else
            transform.GetChild(3).GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(14.7f * SHealth / SMaxHealth, 2.7f);
        
    }
    #endregion

    void ShootingInitialize()
    {
        /* Find related objectes */
        SelfShieldBar = GameObject.Find("SelfShieldBar").GetComponent<Image>();
        side = GetComponent<helicopmovementVersus>().side;
        Fires = GameObject.Find("Fire");
        ChiWaveIcon = GameObject.Find("ChiWaveIcon");
        LaserIcon = GameObject.Find("LaserIcon");
        TargetIcon = GameObject.Find("TargetIcon");
        ShieldSkillIcon = GameObject.Find("ShieldSkillIcon");
        ChiPotionC = GameObject.Find("ChiPotionAmount").transform.GetChild(0).GetChild(0).gameObject;
        CoolC = GameObject.Find("CoolAmount").transform.GetChild(0).GetChild(0).gameObject;
        CTimer = ChiWaveIcon.transform.GetChild(1).gameObject.GetComponent<Text>();
        TTimer = TargetIcon.transform.GetChild(1).gameObject.GetComponent<Text>();
        STimer = ShieldSkillIcon.transform.GetChild(1).gameObject.GetComponent<Text>();
        CShadow = ChiWaveIcon.transform.GetChild(0).gameObject;
        LShadow = LaserIcon.transform.GetChild(0).gameObject;
        TShadow = TargetIcon.transform.GetChild(0).gameObject;
        SShadow = ShieldSkillIcon.transform.GetChild(0).gameObject;

        Ult = 10;

        /* check the purchases of skills */
        ChiWavePurchased = Convert.ToBoolean(PlayerPrefs.GetInt("ChiP"));
        LaserPurchased = Convert.ToBoolean(PlayerPrefs.GetInt("LaserP"));
        AimPurchased = Convert.ToBoolean(PlayerPrefs.GetInt("AimP"));
        ShieldPurchased = Convert.ToBoolean(PlayerPrefs.GetInt("LaserP"));
        if (!ChiWavePurchased)
        {
            CShadow.SetActive(true);
            CShadow.GetComponent<Animator>().Play("CNotPurchased");
            IsChiWaveCoolDown = true;
        }
        if (!LaserPurchased)
            LShadow.SetActive(true);
        if (!AimPurchased)
        {
            TShadow.SetActive(true);
            TShadow.GetComponent<Animator>().Play("TNotPurchased");
            IsTargetCoolDown = true;
        }
        if (!ShieldPurchased)
        {
            SShadow.SetActive(true);
            SShadow.GetComponent<Animator>().Play("SNotpurchased");
            IsShieldCoolDown = true;
        }

        /* check the purchases of potions*/
        ChiPotionAmount = PlayerPrefs.GetInt("ChiPotion");
        CoolAmount = PlayerPrefs.GetInt("CoolDown");
        ChiPotionC.transform.parent.parent.GetComponent<Text>().text = "X " + ChiPotionAmount;
        CoolC.transform.parent.parent.GetComponent<Text>().text = "X " + CoolAmount;
    }
}