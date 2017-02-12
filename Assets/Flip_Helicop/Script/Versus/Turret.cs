using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class Turret : NetworkBehaviour {

    bool Timing = false;
    bool Firing = false;
    bool die = false;
    public bool side; /* identify which side it belongs to */

    public GameObject missile;
    public LinkedList<GameObject> TurretTargets = new LinkedList<GameObject>(); /* keep track of targets */
    public List<GameObject> Laseremitter = new List<GameObject>(); /* keep track of laser emitters to decide whether keep getting damage or not */

    Vector3 FirstTarget;
    
    float speed = 0.2f; /* speed of bullets */
    float Timer;
    int MaxHealth = 300;
    int Health;

    Image HealthIcon = null;
    Image Warning;
    Animator Anim;
    public AudioClip MissileSound = null;

    // Use this for initialization
    void Start ()
    {
        Health = MaxHealth;
        if (transform.position.x > 0)
        {
            side = true;
            HealthIcon = GameObject.Find("DT1f").GetComponent<Image>();
            Warning = GameObject.Find("Warnings").transform.GetChild(4).GetComponent<Image>();
        }
        else
        {
            side = false;
            HealthIcon = GameObject.Find("RT1f").GetComponent<Image>();
            Warning = GameObject.Find("Warnings").transform.GetChild(1).GetComponent<Image>();
        }
        Anim = transform.GetChild(0).GetComponent<Animator>();
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
    void FixedUpdate()
    {
        /* only server triggers firing */
        if (!isServer)
            return;
        if(TurretTargets.Count != 0)
        {
            /* pick the first target of all targets */
            FirstTarget = TurretTargets.First.Value.transform.position;
            /* if is not firing, start a coroutine to fire */
            if (!Firing)
                StartCoroutine(Fire());
            /* make the turret face the target */
            transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(transform.position.x - FirstTarget.x, transform.position.y - FirstTarget.y) * - Mathf.Rad2Deg);
        }
    }
    IEnumerator Fire()
    {
        Firing = true;
        RpcFireAnim();
        /* loop till no target left */
        while (TurretTargets.Count != 0)
        {
            Vector3 pos = new Vector3(transform.position.x - Mathf.Sin(-transform.rotation.eulerAngles.z * Mathf.Deg2Rad), transform.position.y - Mathf.Cos(-transform.rotation.eulerAngles.z * Mathf.Deg2Rad), 0);
            Vector2 dir = new Vector2(FirstTarget.x - transform.position.x, FirstTarget.y - transform.position.y);
            dir = dir.normalized * speed;
            Vector3 des = new Vector3(dir.x, dir.y, 0);
            /* ask clients to fire with two parameters: pos(bullet's initial position) / des(target's position) */
            RpcFire(pos, des);
            yield return new WaitForSeconds(0.2f);
        }
        Firing = false;
        RpcStopFireAnim();
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        /* only server takes care of taking damage */
        if (!isServer)
            return;
        if (collider.tag == "missile" && side != collider.gameObject.GetComponent<missleversus>().side)
        {
            RpcTakeDamage(2);
        }
        else if (collider.tag == "ChiWave" && side != collider.gameObject.GetComponent<VChiWave>().side)
        {
            RpcTakeDamage(10);
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
    IEnumerator Burning(GameObject Emitter)
    {
        /* if emitter is still in keep taking damage*/
        while (Laseremitter.Contains(Emitter))
        {
             RpcTakeDamage(5);
            yield return new WaitForSeconds(0.2f);
        }
    }
    /* a function that asks clients to take damage */
    [ClientRpc]
    void RpcTakeDamage(int value)
    {
        StartTiming();
        Health -= value;
        HealthIcon.fillAmount = (float)Health / (float)MaxHealth;
        /* boolean 'die' here is used to assure that base only dies once*/
        if (!die && Health <= 0)
        {
            die = true;
            /* inform players that turret is dead */
            foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
                if(item.GetComponent<helicopmovementVersus>().side != side)
                    item.GetComponent<helicopmovementVersus>().TurretDie = true;
            foreach (GameObject item in Laseremitter)
            {
                item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
            }
            StartCoroutine(dissappear());
        }
     }
    [ClientRpc]
    void RpcFireAnim()
    {
        transform.GetChild(0).GetComponent<Animator>().Play("TurretFire");
    }
    [ClientRpc]
    void RpcStopFireAnim()
    {
        transform.GetChild(0).GetComponent<Animator>().Play("Turret");
    }
    [ClientRpc]
    void RpcFire(Vector3 pos, Vector3 des)
    {
        GameObject tempmis;
        AudioSource.PlayClipAtPoint(MissileSound, transform.position);
        tempmis = Instantiate(missile, pos, Quaternion.Euler(0, 0, -90 + transform.rotation.eulerAngles.z)) as GameObject;
        tempmis.GetComponent<missleversus>().Turret = true;
        tempmis.GetComponent<missleversus>().side = side;
        tempmis.GetComponent<missleversus>().move = des;
        tempmis.SetActive(true);
        Destroy(tempmis, 2.0f);
    }
    IEnumerator dissappear()
    {
        Anim.Play("TurretDie");
        yield return new WaitForSeconds(0.8f);
        Destroy(gameObject);
    }
}
