using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
public class Tower : NetworkBehaviour {

    public bool side; /* identify the side it belongs to */
    public bool pos; /* identify the tower's location */
    bool die = false;
    bool Timing = false;

    int Health = 500;
    int MaxHealth = 500;
    float Timer;

    public GameObject Lightning;
    public List<GameObject> Laseremitter = new List<GameObject>(); /* keep track of laser emitters to decide whether keep getting damage or not */

    Image Icon;
    Image Warning;
    Animator Anim;

    // Use this for initialization
    void Start ()
    {
        Anim = GetComponent<Animator>();
        /* find the corresponding tower gameobject*/
        if (transform.position.x > 0)
        {
            side = true;
            Warning = GameObject.Find("Warnings").transform.GetChild(5).GetComponent<Image>();
            if (transform.position.y > 0)
            {
                pos = true;
                Icon = GameObject.Find("DT21f").GetComponent<Image>();
            }
            else
            {
                pos = false;
                Icon = GameObject.Find("DT22f").GetComponent<Image>();
            }
        }
        else
        {
            side = false;
            Warning = GameObject.Find("Warnings").transform.GetChild(2).GetComponent<Image>();
            if (transform.position.y > 0)
            {
                pos = true;
                Icon = GameObject.Find("RT21f").GetComponent<Image>();
            }
            else
            {
                pos = false;
                Icon = GameObject.Find("RT22f").GetComponent<Image>();
            }
        }
        
	}
	
	// Update is called once per frame
	void Update ()
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
        if (!isServer)
            return;
        if (collider.tag == "missile" && side != collider.gameObject.GetComponent<missleversus>().side)
            RpcTakeDamage(2);
        else if(collider.tag == "ChiWave" && side != collider.gameObject.GetComponent<VChiWave>().side)
            RpcTakeDamage(10);
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
    void Die()
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
            /* tell lightning to change its form */
            if(pos)
                Lightning.GetComponent<Lightning>().up = false;
            else
                Lightning.GetComponent<Lightning>().down = false;
            Lightning.GetComponent<Lightning>().damage = true;
            /* Destroy() doesn't trigger exit2d, clear emitters here */
            foreach (GameObject item in Laseremitter)
            {
                item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
            }
            
            Anim.Play("TowerDie");
        }
    }
    IEnumerator Burning(GameObject Emitter)
    {
        /* if emitter is still in keep taking damage*/
        while (Laseremitter.Contains(Emitter))
        {
            RpcTakeDamage(5);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
