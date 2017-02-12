using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// this class controls the instantiation of objects
public class rightmarginmovement : MonoBehaviour
{
    public GameObject Player;
    public GameObject uppillar;
    public GameObject downpillar;
    public GameObject Dragon;
    public GameObject Pig;
    public GameObject Bonus;
    public GameObject RFPoint;
    public List<GameObject> sky;

    float StartPos;
    float offsetX;
    float upy;
    float downy;
    float updis = 0f;
    float downdis = 0;
    float mindis = 20f;
    float lastup = 0f; // track the last up pillar that was instantiated 
    float lastdown = 0f; // track the last down pillar that was instantiated 
    float firstmin = 70f; // set the first pillar's x coordinate
    float direct = -1f;
    public float min = 0f;
    public float max = 0f;
    /* all possibilities are dynamically changed when level increases or decreases */
    public float thunderprob = 0.2f; // possibility of the situation where a thunder is on for a new pillar
    public float PillarProb = 0.2f; // possibility of the instantiation of pillars
    public float PigProb = 0.3f; // possibility of the instantiation of pigs
    public float BlackHoleProb = 0.15f; // possibility of the instantiation of blackholes near the pillar
    public float BonusProb = 0.8f; // possibility of the instantiation of bonus

    Vector3 uppos = Vector3.zero;
    Vector3 downpos = Vector3.zero;

    Quaternion downpillarAngle = Quaternion.Euler(0, 0, 180f);

    public int Level=1; // Level : 1-5, a higher level is harder
    public Text LevelText;
    public Animator LevelAnim;

    // color of sky will be changed when level rises as time passes
    Color[] colors = new Color[5] { new Color32(255,255,255,255), new Color32(36, 255, 191,255), new Color32(70, 238, 78,255), new Color32(255, 36, 194,255), new Color32(255, 0, 0,255)};

    // Use this for initialization
    void Start()
    {
        offsetX = transform.position.x - Player.transform.position.x;
        upy = 9.32f;
        downy = 1.19f;
        uppos.y = upy;
        downpos.y = downy;
        StartPos = Player.transform.position.x;
        Level = PlayerPrefs.GetInt("Level")==0?1: PlayerPrefs.GetInt("Level"); // check whether it is the start of the game
        SetLevel();
        PlayerPrefs.SetInt("Level",1);
        StartCoroutine(Generate());
    }

    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = Player.transform.position.x + offsetX;
        transform.position = pos;
        if (Player.transform.position.x - StartPos >= 300)
        {
            StartPos = Player.transform.position.x;
            IncreaseLevel();
        }
    }
    // this function sets difficulties according to the current level
    public void SetLevel()
    {
        LevelText.text = "Level : " + Level; // show visual prompt
        /* set difficult */
        PillarProb = 0.2f * Level;
        BonusProb = 0.8f - Level * 0.15f;
        thunderprob = Level * 0.2f;
        Player.GetComponent<helicopmovement>().ForwardSpeed = 0.1f + (Level-1) * 0.025f;
        RFPoint.GetComponent<ReferencePoint>().ForwardSpeed = 0.1f + (Level - 1) * 0.025f;
        /* change the color of the sky according to the level */
        foreach (GameObject item in sky)
        {
            item.GetComponent<SpriteRenderer>().color = colors[Level - 1];
        }
        LevelAnim.Play("Level");
    }
    void IncreaseLevel()
    {
        if (Level == 5)
            return;
        Level++;
        SetLevel();
    }
    // the coroutine that takes care of instantiation of objects
    IEnumerator Generate()
    {
        // keeps generating through the whole game
        while (true)
        {
#region GeneratePillars
            if (Random.Range(0f, 1f) <= 0.5f)
                direct = -1f;
            else
                direct = 1f;
            updis = direct * Random.Range(min, max);
            downdis = direct * Random.Range(min, max);
            uppos.x = transform.position.x + updis;
            downpos.x = transform.position.x + downdis;
            /* ensure the first pillar is right to a specified x coordinate */
            if (uppos.x < firstmin)
                uppos.x = firstmin;
            if (downpos.x < firstmin)
                downpos.x = firstmin;
            /* ensure that the new pillar will not collide with the last pillar */
            if (uppos.x - lastup < mindis)
                uppos.x = lastup + mindis;
            if (uppos.x - lastdown < mindis)
                uppos.x = lastdown + mindis;
            /* ensure that the up pillar will not collide with the down pillar */
            if (downpos.x - lastup < mindis)
                downpos.x = lastup + mindis;
            if (downpos.x - lastdown < mindis)
                downpos.x = lastdown + mindis;
            if (Mathf.Abs(uppos.x - downpos.x) < mindis)
                if (direct == -1f)
                    downpos.x = uppos.x + mindis;
                else
                    uppos.x = downpos.x + mindis;
            lastup = uppos.x;
            lastdown = downpos.x;
            /* randomly decide whether an up pillar should be instantiated */
            if (Random.value <= PillarProb)
            {
                GameObject tempp = (GameObject)Instantiate(uppillar, uppos, Quaternion.identity);
                tempp.SetActive(true);
                if (Random.value < thunderprob)
                    tempp.transform.FindChild("Thunder").gameObject.SetActive(true);
                if(Random.value < BlackHoleProb)
                {
                    Vector3 BPos = Vector3.zero;
                    BPos.x = Random.Range(2f,5f);
                    BPos.y = Random.Range(-2f,2.5f);
                    tempp.transform.FindChild("BlackHole").transform.localPosition = BPos;
                    tempp.transform.FindChild("BlackHole").gameObject.SetActive(true);
                    tempp.transform.FindChild("BlackHole").parent = null;
                }
            }
            /* randomly decide whether a down pillar should be instantiated */
            if (Random.value <= PillarProb)
            {
                GameObject tempp = (GameObject)Instantiate(downpillar, downpos, downpillarAngle);
                tempp.SetActive(true);
                if (Random.Range(0f, 1f) < thunderprob)
                    tempp.transform.FindChild("Thunder").gameObject.SetActive(true);
                if (Random.value < BlackHoleProb)
                {
                    Vector3 BPos = Vector3.zero;
                    BPos.x = Random.Range(-5f, -2f);
                    BPos.y = Random.Range(0f, 3f);
                    tempp.transform.FindChild("BlackHole").transform.localPosition = BPos;
                    tempp.transform.FindChild("BlackHole").gameObject.SetActive(true);
                    tempp.transform.FindChild("BlackHole").parent = null;
                }
            }
            #endregion
            #region GenerateMonsters
            // decide whether to generate pig or wyvern
            if(Random.value<=PigProb)
                GeneratePig();
            else
                GenerateWyvern(); 
            // generate twice if it's level 4
            if(Level >= 4)
            {
                if (Random.value <= PigProb)
                    GeneratePig();
                else
                    GenerateWyvern();
            }
            #endregion
            #region GenerateBonus
            // decide whether to generate bonus
            if(Random.value<BonusProb)
            {
                float temp = Random.value;
                int index = (int)(temp * 5);
                if (index == 5)
                    index = 4;
                if(Random.value<0.5f) //up
                {
                    Vector3 Pos = new Vector3(transform.position.x, Random.Range(9f, 11.5f), 0);
                    GameObject tempf = Instantiate(Bonus.transform.GetChild(index).gameObject, Pos, Quaternion.identity) as GameObject;
                    tempf.SetActive(true);
                }
                else //down
                {
                    Vector3 Pos = new Vector3(transform.position.x, Random.Range(-1f, 2f), 0);
                    GameObject tempf = Instantiate(Bonus.transform.GetChild(index).gameObject, Pos, Quaternion.identity) as GameObject;
                    tempf.SetActive(true);
                }
            }
        #endregion
            yield return new WaitForSeconds(5);
        }
    }
    // a function to instantiate pig
    void GeneratePig()
    {
            if (Level == 1)
            {
                if (Random.value < 0.5f)
                {
                    float tempr = Random.Range(0, 5f);
                    GameObject tempd = Instantiate(Pig, transform.position + Vector3.up * tempr, Quaternion.identity) as GameObject;
                    tempd.SetActive(true);
                }
                else
                {
                    float tempr = Random.Range(0, 5f);
                    GameObject tempd = Instantiate(Pig, transform.position + Vector3.down * tempr, Quaternion.identity) as GameObject;
                    tempd.SetActive(true);
                }
            }
            else
            {
                float tempr = Random.Range(0, 5f);
                GameObject tempd1 = Instantiate(Pig, transform.position + Vector3.up * tempr, Quaternion.identity) as GameObject;
                tempd1.SetActive(true);
                tempr = Random.Range(0, 5f);
                GameObject tempd2 = Instantiate(Pig, transform.position + Vector3.down * tempr, Quaternion.identity) as GameObject;
                tempd2.SetActive(true);
            }
    }
    // a function to instantiate wyvern
    void GenerateWyvern()
    {
        if (Level < 3)
        {
            if (Random.value < 0.5f)
            {
                float tempr = Random.Range(0, 5f);
                GameObject tempd = Instantiate(Dragon, transform.position + Vector3.up * tempr, Quaternion.identity) as GameObject;
                tempd.SetActive(true);
            }
            else
            {
                float tempr = Random.Range(0, 5f);
                GameObject tempd = Instantiate(Dragon, transform.position + Vector3.down * tempr, Quaternion.identity) as GameObject;
                tempd.SetActive(true);
            }
        }
        else
        {
            float tempr = Random.Range(0, 5f);
            GameObject tempd1 = Instantiate(Dragon, transform.position + Vector3.up * tempr, Quaternion.identity) as GameObject;
            tempd1.SetActive(true);
            tempr = Random.Range(0, 5f);
            GameObject tempd2 = Instantiate(Dragon, transform.position + Vector3.down * tempr, Quaternion.identity) as GameObject;
            tempd2.SetActive(true);
        }
    }
}
