using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
public class Base : NetworkBehaviour
{
    public bool side; /* identify the side it belongs to */
    bool die = false;
    bool Timing = false;

    int Health = 1000;
    int MaxHealth = 1000;
    public int state = 2; // 2: invulnerable    1: can be damaged with half damage      0:take full damage
    float Timer;

    public GameObject Lightning;
    GameObject BGM;
    public List<GameObject> Laseremitter = new List<GameObject>(); /* keep track of laser emitters to decide whether keep getting damage or not */
    GameObject[] Players;

    Image Icon;
    Image Warning;
    public Text Res;
    Animator Anim;
    public BGM bgmcon;

    // Use this for initialization
    void Start()
    {
        BGM = GameObject.Find("BGM");
        Anim = GetComponent<Animator>();
        if (transform.position.x > 0)
        {
            side = true;
            Icon = GameObject.Find("DBf").GetComponent<Image>();
            Warning = GameObject.Find("Warnings").transform.GetChild(3).GetComponent<Image>();
        }
        else
        {
            side = false;
            Icon = GameObject.Find("RBf").GetComponent<Image>();
            Warning = GameObject.Find("Warnings").transform.GetChild(0).GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* Warning Timer */
        if (Timing)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
            {
                Timing = false;
                Warning.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        /* only server takes care of taking damage */
        if (!isServer || state == 2)
            return;
        if (collider.tag == "missile" && side != collider.gameObject.GetComponent<missleversus>().side)
        {
            if (state == 0)
                RpcTakeDamage(2);
            else
                RpcTakeDamage(1);
        }
        else if (collider.tag == "ChiWave" && side != collider.gameObject.GetComponent<VChiWave>().side)
        {
            if (state == 0)
                RpcTakeDamage(10);
            else
                RpcTakeDamage(5);
        }
        else if (collider.tag == "laser" && side != collider.transform.parent.gameObject.GetComponent<helicopmovementVersus>().side)
        {
            /* add laser emitter, start a coroutine to take damage */
            Laseremitter.Add(collider.transform.parent.gameObject);
            collider.transform.parent.gameObject.GetComponent<shootingversus>().Laserhitting.Add(this.gameObject);
            StartCoroutine(Burning(collider.transform.parent.gameObject));
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (isServer && collider.CompareTag("laser"))
        {
            /* delete laser emitter, the coroutine to take damage will stop*/
            collider.transform.parent.gameObject.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
            Laseremitter.Remove(collider.transform.parent.gameObject);
        }
    }

    void StartTiming()
    {
        /* initialze timer or refresh timer*/
        Timer = 3f;
        Timing = true;
        Warning.gameObject.SetActive(true);
    }

    void dissappear()
    {
        /* animation event */
        Warning.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    /* a function that asks clients to take damage */
    [ClientRpc]
    void RpcTakeDamage(int value)
    {
        StartTiming();
        Health -= value;
        Icon.fillAmount = (float)Health / (float)MaxHealth;
        /* boolean 'die' here is used to assure that base only dies once*/
        if (!die && Health <= 0)
        {
            die = true;
            /* Destroy() doesn't trigger exit2d, clear emitters here */
            foreach (GameObject item in Laseremitter)
            {
                item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
            }
            /* return a verdict of the match */
            if (isServer)
            {
                if (side)
                    RpcRadientWin();
                else
                    RpcDireWin();
            }
            Anim.Play("BaseDie");
        }
    }

    [ClientRpc]
    void RpcRadientWin()
    {
        /* stop players' behaviors */
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject item in Players)
        {
            if (item.GetComponent<helicopmovementVersus>().CheckAuthority())
            {
                item.GetComponent<helicopmovementVersus>().end = true;
                item.GetComponent<shootingversus>().end = true;
            }
        }
        /*show results and stop players' controlling */
        Res.text = "Radient Wins!";
        Res.color = new Color32(65, 255, 152, 255);
        if (bgmcon.side == true)
            bgmcon.lose = true;
        else
            bgmcon.win = true;
    }

    [ClientRpc]
    void RpcDireWin()
    {
        /* stop players' behaviors */
        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject item in Players)
        {
            if (item.GetComponent<helicopmovementVersus>().CheckAuthority())
            {
                item.GetComponent<helicopmovementVersus>().end = true;
                item.GetComponent<shootingversus>().end = true;
            }
        }

        /*show results and stop players' controlling */
        Res.text = "Dire Wins!";
        Res.color = new Color32(255, 64, 64, 255);
        if (bgmcon.side == true)
            bgmcon.win = true;
        else
            bgmcon.lose = true;
    }

    IEnumerator Burning(GameObject Emitter)
    {
        /* if emitter is still in keep taking damage*/
        while (Laseremitter.Contains(Emitter))
        {
            if (state == 0)
                RpcTakeDamage(5);
            else
                RpcTakeDamage(2);
            yield return new WaitForSeconds(0.2f);
        }
    }
}