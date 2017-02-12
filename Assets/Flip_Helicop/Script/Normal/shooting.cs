using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class shooting : MonoBehaviour {

    public static int Ult = 10; // Chi amount

    float Ctimer; // cooldown timers
    float Ttimer;
    float Stimer;

    public GameObject missile; // projectiles game objects
    public GameObject ChiWave;
    public GameObject ChiWaveIcon; // skill icons
    public GameObject LaserIcon;
    public GameObject TargetIcon;
    public GameObject ShieldSkillIcon;
    public GameObject Aim;
    public AudioSource RFPointAudio;
    public GameObject ChiPotionC;
    public GameObject CoolC;
    GameObject CShadow; // the shadow appears on the skill icon when it is recovering
    GameObject LShadow;
    GameObject TShadow;
    GameObject SShadow;
    GameObject TempAim;
    GameObject TargetMonster;
    GameObject Laser = null;
    GameObject[] Enemies = new GameObject[10];
    public List<GameObject> Fires = new List<GameObject>(10);

    bool IsChiWaveCoolDown = false;
    bool IsTargetCoolDown = false;
    bool IsShieldCoolDown = false;
    bool Laseron = false;
    bool Aiming = false;
    bool IsChiPotionC = false;
    bool IsCoolC = false;
    bool ChiWavePurchased;
    bool LaserPurchased;
    bool AimPurchased;
    bool ShieldPurchased;
    public bool paused = false; // true if paused false if not paused
    public bool Die = false;

    int ChiPotionAmount = 0;
    int CoolAmount = 0;
    int SNum;

    public AudioClip ShieldSound;
    public AudioClip MissileSound = null;
    public AudioClip SniperSound = null;

    public Text CTimer;
    public Text TTimer;
    public Text STimer;

    // Use this for initialization
    void Start ()
    {
        CShadow = ChiWaveIcon.transform.GetChild(0).gameObject;
        LShadow = LaserIcon.transform.GetChild(0).gameObject;
        TShadow = TargetIcon.transform.GetChild(0).gameObject;
        SShadow = ShieldSkillIcon.transform.GetChild(0).gameObject;
        Laser = transform.GetChild(0).gameObject;
        SNum = RFPointAudio.gameObject.GetComponent<ReferencePoint>().SBGM.Count;
        Ult = 10;
        // load the purchases from playerprefs
        ChiWavePurchased = Convert.ToBoolean(PlayerPrefs.GetInt("ChiP"));
        LaserPurchased = Convert.ToBoolean(PlayerPrefs.GetInt("LaserP"));
        AimPurchased = Convert.ToBoolean(PlayerPrefs.GetInt("AimP"));
        ShieldPurchased = Convert.ToBoolean(PlayerPrefs.GetInt("LaserP"));
        // Check the purchases and enable or disable skills
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
        ChiPotionAmount = PlayerPrefs.GetInt("ChiPotion");
        CoolAmount = PlayerPrefs.GetInt("CoolDown");
        ChiPotionC.transform.parent.parent.GetComponent<Text>().text = "X " + ChiPotionAmount;
        CoolC.transform.parent.parent.GetComponent<Text>().text = "X " + CoolAmount;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (paused)
            return;
        // fire a bullet with mouse left-click
        if (Input.GetButtonDown("Fire1") && Die == false)
        {
            AudioSource.PlayClipAtPoint(MissileSound, transform.position);
            GameObject tempmis= Instantiate(missile, transform.position, Quaternion.identity) as GameObject;
            tempmis.SetActive(true);
        }
        // fire a chi wave with mouse right-click
        if(Input.GetButtonDown("Fire2") && !IsChiWaveCoolDown && Ult >=1 && Die == false)
        {
            CShadow.SetActive(true);
            CTimer.gameObject.SetActive(true);
            CShadow.GetComponent<Animator>().Play("CoolDown");
            UltDecrease();
            IsChiWaveCoolDown = true;
            StartCoroutine(ChiWaveCoolDown());
            GameObject TempChi = Instantiate(ChiWave, transform.position, Quaternion.Euler(0f, 0f, 90f)) as GameObject;
            TempChi.SetActive(true);
        }
        // fire a sniper shoot with key 'E'
        if(Input.GetKeyDown(KeyCode.E) && !IsTargetCoolDown && Ult >= 2 && Die == false)
        {
            TShadow.SetActive(true);
            TTimer.gameObject.SetActive(true);
            TShadow.GetComponent<Animator>().Play("CoolDown");
            UltDecrease();
            UltDecrease();
            IsTargetCoolDown = true;
            StartCoroutine(TargetCoolDown());
            Enemies = GameObject.FindGameObjectsWithTag("Monster"); // Find all monsters
            StartCoroutine(Target());
        }
        // toggle laser with key 'Q'
        if (Input.GetKeyDown(KeyCode.Q) && Die == false && LaserPurchased)
        {
            if (!Laseron && Ult >= 3)
            {
                UltDecrease();
                Laser.SetActive(true);
                Laseron = true;
                StartCoroutine(LaserBurn());
                LShadow.SetActive(true);
            }
            else if(Laseron)
            {
                LaserOff();
            }
        }
        // turn on the shield with key 'F'
        if (Input.GetKeyDown(KeyCode.F) && !IsShieldCoolDown && Ult >= 3 && Die == false)
        {
            if (transform.parent.gameObject.GetComponent<helicopmovement>().ShieldOn == false)
            {
                int temp = Random.Range(0, SNum);
                RFPointAudio.Stop();
                RFPointAudio.clip = RFPointAudio.GetComponent<ReferencePoint>().SBGM[temp];
                RFPointAudio.Play();
            }
            AudioSource.PlayClipAtPoint(ShieldSound, transform.position);
            SShadow.SetActive(true);
            STimer.gameObject.SetActive(true);
            SShadow.GetComponent<Animator>().Play("ShieldCoolDown");
            UltDecrease();
            UltDecrease();
            UltDecrease();
            IsShieldCoolDown = true;
            StartCoroutine(ShieldCoolDown());
            transform.parent.gameObject.GetComponent<helicopmovement>().ShieldLife = 3;
            transform.parent.gameObject.GetComponent<helicopmovement>().ShieldOn = true;
            /* enable the shield object and three shield visual icons above the player */
            transform.parent.gameObject.transform.GetChild(6).gameObject.SetActive(true);
            transform.parent.gameObject.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
            transform.parent.gameObject.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
            transform.parent.gameObject.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
        }
        // take chi potion with key '1'
        if (Input.GetKeyDown(KeyCode.Alpha1) && Die == false && ChiPotionAmount >= 1 && !IsChiPotionC)
        {
            IsChiPotionC = true;
            ChiPotionAmount--;
            PlayerPrefs.SetInt("ChiPotion", ChiPotionAmount);
            ChiPotionC.transform.parent.parent.GetComponent<Text>().text = "X " + ChiPotionAmount;
            Ult = 10; // recover chi to 10
            for (int i = 0; i < 10; i++)
                Fires[i].SetActive(true);
            ChiPotionC.transform.parent.GetComponent<Animator>().Play("ChiPotionC");
            StartCoroutine(ChiPotionReady());
        }
        // take cool down potion with key '2'
        if (Input.GetKeyDown(KeyCode.Alpha2) && Die == false && CoolAmount >= 1 && !IsCoolC)
        {
            IsCoolC = true;
            CoolAmount--;
            PlayerPrefs.SetInt("CoolDown", CoolAmount);
            CoolC.transform.parent.parent.GetComponent<Text>().text = "X " + CoolAmount;
            // reset all skills' cooldowns
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
        // display the remaining seconds of the cooldown
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
        if (Aiming)
            TempAim.transform.position = TargetMonster.transform.position;
    }

    // coroutines that take care of the cooldown
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
        Ttimer = 8f;
        yield return new WaitForSeconds(8);
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
    // the coroutine that inplement sniper shoot
    IEnumerator Target()
    {
        float min = 5000f;
        float temp = 0f;
        int index = 0;
        // find the nearest monster to the player
        foreach (GameObject item in Enemies)
        {
            if (item.transform.position.x > transform.position.x)
            {
                temp = item.transform.position.x - transform.position.x;
                if (temp < min)
                {
                    min = temp;
                    index = System.Array.IndexOf(Enemies,item);
                }
            }   
        }
        TargetMonster = Enemies[index];
        TempAim = Instantiate(Aim, TargetMonster.transform.position, Quaternion.identity) as GameObject;
        TempAim.SetActive(true);
        Aiming = true;
        yield return new WaitForSeconds(1.5f);
        AudioSource.PlayClipAtPoint(SniperSound, transform.position);
        Aiming = false;
        Destroy(TempAim.gameObject);
        // generate three bullets at the destination
        GameObject tempmis1 = Instantiate(missile, TempAim.transform.position, Quaternion.identity) as GameObject;
        tempmis1.SetActive(true);
        GameObject tempmis2 = Instantiate(missile, TempAim.transform.position, Quaternion.identity) as GameObject;
        tempmis2.SetActive(true);
        GameObject tempmis3 = Instantiate(missile, TempAim.transform.position, Quaternion.identity) as GameObject;
        tempmis3.SetActive(true);
        System.Array.Clear(Enemies,0,Enemies.Length);
    }
    // when laser on, decrease chi every second
    IEnumerator LaserBurn()
    {
        while (true)
        {
            if (Laseron)
            {
                yield return new WaitForSeconds(1);
                UltDecrease();
                if (Ult == 0)
                {
                    Laser.SetActive(false);
                    Laseron = false;
                    LShadow.SetActive(false);
                }
            }
            else
                yield break;
        }
    }

    public void UltIncrease()
    {
        Ult += 1;
        Fires[Ult - 1].SetActive(true);
    }
    public void UltDecrease()
    {
        Ult -= 1;
        Fires[Ult].SetActive(false);
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
    public void LaserOff()
    {
        Laser.SetActive(false);
        Laseron = false;
        LShadow.SetActive(false);
    }
}
