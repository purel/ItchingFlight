using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

// this class is a different version of shooting or shootingversus
public class shootingBoss : MonoBehaviour
{

    public static int Ult = 10;

    GameObject Laser = null;
    GameObject CShadow;
    GameObject LShadow;
    GameObject TShadow;
    GameObject SShadow;
    GameObject TempAim;
    public GameObject ChiWaveIcon;
    public GameObject LaserIcon;
    public GameObject TargetIcon;
    public GameObject ShieldSkillIcon;
    public GameObject Aim;
    public GameObject Boss;
    public GameObject missile;
    public GameObject ChiWave;
    public GameObject ChiPotionC;
    public GameObject CoolC;
    public List<GameObject> Fires = new List<GameObject>(10);

    Vector3 start;
    float angle;

    public float speed = 1f;
    public float angledis = 5f;

    public bool Begin = false;
    bool IsChiWaveCoolDown = false;
    bool IsTargetCoolDown = false;
    bool IsShieldCoolDown = false;
    bool Laseron = false;
    bool Aiming = false;
    public bool Die = false;

    public AudioClip ShieldSound;
    public AudioClip MissileSound = null;
    public AudioClip SniperSound = null;
    public AudioClip LaserSound = null;



    public Text CTimer;
    public Text TTimer;
    public Text STimer;

    float Ctimer;
    float Ttimer;
    float Stimer;

    bool ChiWavePurchased;
    bool LaserPurchased;
    bool AimPurchased;
    bool ShieldPurchased;
    bool IsChiPotionC = false;
    bool IsCoolC = false;

    int ChiPotionAmount = 0;
    int CoolAmount = 0;

    // Use this for initialization
    void Start()
    {
        CShadow = ChiWaveIcon.transform.GetChild(0).gameObject;
        LShadow = LaserIcon.transform.GetChild(0).gameObject;
        TShadow = TargetIcon.transform.GetChild(0).gameObject;
        SShadow = ShieldSkillIcon.transform.GetChild(0).gameObject;
        Laser = transform.GetChild(0).gameObject;
        Ult = 10;
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
        ChiPotionAmount = PlayerPrefs.GetInt("ChiPotion");
        CoolAmount = PlayerPrefs.GetInt("CoolDown");
        ChiPotionC.transform.parent.parent.GetComponent<Text>().text = "X " + ChiPotionAmount;
        CoolC.transform.parent.parent.GetComponent<Text>().text = "X " + CoolAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Die == false && Begin)
        {
            AudioSource.PlayClipAtPoint(MissileSound, transform.position);
            GameObject tempmis = Instantiate(missile, transform.position, Quaternion.identity) as GameObject;
            tempmis.SetActive(true);
        }
        if (Input.GetButtonDown("Fire2") && !IsChiWaveCoolDown && Ult >= 1 && Die == false && Begin)
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
        if (Input.GetKeyDown(KeyCode.E) && !IsTargetCoolDown && Ult >= 2 && Die == false && Begin)
        {
            TShadow.SetActive(true);
            TTimer.gameObject.SetActive(true);
            TShadow.GetComponent<Animator>().Play("CoolDown");
            UltDecrease();
            UltDecrease();
            IsTargetCoolDown = true;
            StartCoroutine(TargetCoolDown());
            StartCoroutine(Target());
        }
        if (Input.GetKeyDown(KeyCode.Q) && Die == false && Begin)
        {
            if (!Laseron && Ult >= 3)
            {
                UltDecrease();
                Laser.SetActive(true);
                Laseron = true;
                StartCoroutine(LaserBurn());
                LShadow.SetActive(true);
                AudioSource.PlayClipAtPoint(LaserSound, transform.position);
            }
            else if (Laseron)
            {
                LaserOff();
                if(!transform.GetChild(0).GetComponent<LaserBoss>().LaserOff)
                    Boss.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && !IsShieldCoolDown && Ult >= 3 && Die == false && Begin)
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
            transform.parent.gameObject.GetComponent<helicopmovementBoss>().ShieldLife = 3;
            transform.parent.gameObject.GetComponent<helicopmovementBoss>().ShieldOn = true;
            transform.parent.gameObject.transform.GetChild(6).gameObject.SetActive(true);
            transform.parent.gameObject.transform.GetChild(5).GetChild(0).gameObject.SetActive(true);
            transform.parent.gameObject.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
            transform.parent.gameObject.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && Die == false && ChiPotionAmount >= 1 && !IsChiPotionC && Begin)
        {
            IsChiPotionC = true;
            ChiPotionAmount--;
            PlayerPrefs.SetInt("ChiPotion", ChiPotionAmount);
            ChiPotionC.transform.parent.parent.GetComponent<Text>().text = "X " + ChiPotionAmount;
            Ult = 10;
            for (int i = 0; i < 10; i++)
                Fires[i].SetActive(true);
            ChiPotionC.transform.parent.GetComponent<Animator>().Play("ChiPotionC");
            StartCoroutine(ChiPotionReady());
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && Die == false && CoolAmount >= 1 && !IsCoolC && Begin)
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
            TempAim.transform.position = Boss.transform.position;
    }

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
    IEnumerator Target()
    {
        TempAim = Instantiate(Aim, Boss.transform.position, Quaternion.identity) as GameObject;
        TempAim.SetActive(true);
        Aiming = true;
        yield return new WaitForSeconds(1.5f);
        AudioSource.PlayClipAtPoint(SniperSound, transform.position);
        Aiming = false;
        Destroy(TempAim.gameObject);
        GameObject tempmis1 = Instantiate(missile, TempAim.transform.position, Quaternion.identity) as GameObject;
        tempmis1.SetActive(true);
        Boss.GetComponent<Boss>().Health -= 4;
    }
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
                    LaserOff();
                }
            }
            else
                yield break;
        }
    }

    public void UltIncrease()
    {
        if(Ult<10)
            Ult += 1;
        Fires[shootingBoss.Ult - 1].SetActive(true);
    }
    public void UltDecrease()
    {
        if(Ult>=1)
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
