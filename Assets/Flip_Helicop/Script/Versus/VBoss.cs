using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Collections.Generic;

public class VBoss : NetworkBehaviour
{
    /* booleans to decide which direction the boss should move */
    bool up = false;
    bool down = false;
    bool right = false;
    bool left = false;
    bool Moving = true;
    bool Die = false;

    float MoveSpeed = 3f;
    float YMin = -3.2f;
    float YMax = 4f;
    float XMin = -8f;
    float XMax = 8f;
    float HealthLen = 6.04f; /* Health UI length */
    float HealthHight = 0.31f; /* Health UI width */

    int Health = 500;
    const int MaxHealth = 500;

    public GameObject FireBall;
    GameObject Spawner;
    public List<GameObject> Laseremitter = new List<GameObject>(); /* keep track of laser emitters to decide whether keep getting damage or not */
    GameObject[] Players = new GameObject[10];

    Animator Anim;
    Text LastHit;
    Image HealthBar;

    // Use this for initialization
    void Start()
    {
        Anim = GetComponent<Animator>();
        LastHit = GameObject.Find("LastHit").GetComponent<Text>();
        HealthBar = GameObject.Find("BossHealthbar").GetComponent<Image>();
        HealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(HealthLen * Health / MaxHealth, HealthHight);
        if (isServer)
        {
            StartCoroutine(Move());
            StartCoroutine(Attack());
            Spawner = GameObject.Find("BossSpawner");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        /* make boss move in FixedUpdate() */
        #region Move
        if (!Die)
        {
            Vector3 Pos = Vector3.zero;
            Pos = transform.position;
            if (up == true && Moving)
                Pos += Vector3.up * Time.deltaTime;
            else if (down == true && Moving)
                Pos += Vector3.down * Time.deltaTime * MoveSpeed;
            if (left == true && Moving)
                Pos += Vector3.left * Time.deltaTime * MoveSpeed;
            else if (right == true && Moving)
                Pos += Vector3.right * Time.deltaTime * MoveSpeed;
            Pos.y = Mathf.Clamp(Pos.y, YMin, YMax);
            Pos.x = Mathf.Clamp(Pos.x, XMin, XMax);
            transform.position = Pos;
        }
        #endregion
    }
    IEnumerator Attack()
    {
        /* an attack coroutine that loops till the boss die */
        while(!Die)
        {
            float temp = Random.value;
            /* randomly choose one way to attack */
            if (!Die)
            {
                if (temp <= 0.5f)
                {
                    RpcFireOneCircle();
                    yield return new WaitForSeconds(4);
                }
                else
                {
                    RpcFireWave();
                    yield return new WaitForSeconds(12);
                }
            }
        }
    }
    IEnumerator Move()
    {
        /* Randomly pick a direction for the boss to move */
        while (Moving && !Die)
        {
            yield return new WaitForSeconds(1);
            up = down = left = right = false;
            float temp = Random.value;
            if (temp < 0.33f)
                up = true;
            else if (0.33f <= temp && temp < 0.66f)
                down = true;
            temp = Random.value;
            if (temp < 0.33f)
                left = true;
            else if (0.33f <= temp && temp < 0.66f)
                right = true;
        }
        up = down = left = right = false;
        yield break;
    }

    
    void OnTriggerEnter2D(Collider2D collider)
    {
        /* only server takes care of taking damage */
        if (!isServer)
            return;
        if(collider.CompareTag("missile"))
        {
            /* boolean 'Die' here is to stricly ensure that the last hit only occurs once */
            if(Health <= 2 && !Die)
            {
                Die = true;
                foreach (GameObject item in Laseremitter)
                {
                    item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
                }
                Laseremitter.Clear();
                /* check the side of the last hit and accordingly grant the buff */
                if (collider.GetComponent<missleversus>().side)
                {
                    RpcTakeDamage(2, 2);
                    GrantBuff(true);
                }
                else if (!collider.GetComponent<missleversus>().side)
                {
                    RpcTakeDamage(2, 1);
                    GrantBuff(false);
                }
            }
            else
                RpcTakeDamage(2, 0);
        }
        else if(collider.tag == "ChiWave")
        {
            if (Health <= 10 && !Die)
            {
                Die = true;
                foreach (GameObject item in Laseremitter)
                {
                    item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
                }
                Laseremitter.Clear();
                if (collider.GetComponent<VChiWave>().side)
                {
                    RpcTakeDamage(10, 2);
                    GrantBuff(true);
                }
                else if (!collider.GetComponent<VChiWave>().side)
                {
                    RpcTakeDamage(10, 1);
                    GrantBuff(false);
                }
            }
            else
                RpcTakeDamage(10, 0);
        }
        else if (collider.tag == "laser")
        {
            Laseremitter.Add(collider.transform.parent.gameObject);
            collider.transform.parent.gameObject.GetComponent<shootingversus>().Laserhitting.Add(this.gameObject);
            StartCoroutine(Burning(collider.transform.parent.gameObject));
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (isServer && collider.CompareTag("laser"))
            Laseremitter.Remove(collider.transform.parent.gameObject);
    }
    void GrantBuff(bool side)
    {
        Players = GameObject.FindGameObjectsWithTag("Player");
        /* Find all players from the side that gave last hit and trigger the buff */
        if (side)
            foreach (GameObject item in Players)
            {
                if (item.GetComponent<helicopmovementVersus>().side)
                {
                    item.GetComponent<helicopmovementVersus>().BossRec = true;
                }
            }
        else
            foreach (GameObject item in Players)
            {
                if (!item.GetComponent<helicopmovementVersus>().side)
                {
                    item.GetComponent<helicopmovementVersus>().BossRec = true;
                }
            }
    }

    [ClientRpc]
    void RpcTakeDamage(int value,int res)
    {
        StartCoroutine(GetHurt());
        if (res == 0)
            Health -= value;
        else if(res == 2)
        {
            Health -= value;
            BossDie();
            LastHit.text = "Dire got last hit!";
            LastHit.color = new Color32(255, 64, 64, 255);
            LastHit.GetComponent<Animator>().Play("LastHit");
        }
        else
        {
            Health -= value;
            BossDie();
            LastHit.text = "Radient got last hit!";
            LastHit.color = new Color32(65, 255, 152, 255);
            LastHit.GetComponent<Animator>().Play("LastHit");
        }
        HealthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(HealthLen * Health / MaxHealth, HealthHight);
    }
    /* called on all clients to create bullets */
    [ClientRpc]
    void RpcFireOneCircle()
    {
        StartCoroutine(OneCircleAnim());
        for (int i = 0; i < 24; i++)
        {
            GameObject temp = Instantiate(FireBall, transform.position, Quaternion.Euler(0, 0, 180 + i * 360f / 24)) as GameObject;
            Vector2 dir = new Vector2(0.15f * Mathf.Cos(i * 360f / 24 * Mathf.Deg2Rad), 0.15f * Mathf.Sin(i * 360f / 24 * Mathf.Deg2Rad));
            temp.GetComponent<BossFireBall>().move = new Vector3(dir.x, dir.y, 0);
            temp.SetActive(true);
            Destroy(temp, 3f);
        }
    }
    /* called on all clients to create bullets */
    [ClientRpc]
    void RpcFireWave()
    {
        StartCoroutine(FireWave());
    }
    /* the coroutine to shoot bullets as a circle */
    IEnumerator OneCircleAnim()
    {
        Anim.Play("VBossAttack");
        yield return new WaitForSeconds(0.5f);
        if (!Die)
            Anim.Play("VBossIdle");
    }
    /* the coroutine to shoot bulles continuously */
    IEnumerator FireWave()
    {
        Anim.Play("VBossAttack");
        /* outer loop 3 times */
        for(int i = 0;i<3;i++)
        {
            /* generate 26 bullets from bottom to up for both left side and right side */
            for(int j = 0;j<13;j++)
            {
                GameObject temp = Instantiate(FireBall, transform.position + Vector3.right * 0.4f + Vector3.down * 0.4f, Quaternion.Euler(0, 0, 240 - j * 10)) as GameObject;
                Vector2 dir = new Vector2(0.1f * Mathf.Cos((60 - j * 10) * Mathf.Deg2Rad), 0.1f * Mathf.Sin((60 - j * 10) * Mathf.Deg2Rad));
                temp.GetComponent<BossFireBall>().move = new Vector3(dir.x, dir.y, 0);
                temp.SetActive(true);
                Destroy(temp, 3f);
                GameObject temp1 = Instantiate(FireBall, transform.position + Vector3.right * 0.4f + Vector3.down * 0.4f, Quaternion.Euler(0, 0, -60 + j * 10)) as GameObject;
                Vector2 dir1 = new Vector2(-0.1f * Mathf.Cos((60 - j * 10) * Mathf.Deg2Rad), 0.1f * Mathf.Sin((60 - j * 10) * Mathf.Deg2Rad));
                temp1.GetComponent<BossFireBall>().move = new Vector3(dir1.x, dir1.y, 0);
                temp1.SetActive(true);
                Destroy(temp1, 3f);
                yield return new WaitForSeconds(0.1f);
            }
            /* generate 26 bullets from up to bottom for both left side and right side */
            for (int j = 0; j < 13; j++)
            {
                GameObject temp = Instantiate(FireBall, transform.position + Vector3.right * 0.4f + Vector3.down * 0.4f, Quaternion.Euler(0, 0, 120 + j * 10)) as GameObject;
                Vector2 dir = new Vector2(-0.1f * Mathf.Cos((120 + j * 10) * Mathf.Deg2Rad), -0.1f * Mathf.Sin((120 + j * 10) * Mathf.Deg2Rad));
                temp.GetComponent<BossFireBall>().move = new Vector3(dir.x, dir.y, 0);
                temp.SetActive(true);
                Destroy(temp, 3f);
                GameObject temp1 = Instantiate(FireBall, transform.position + Vector3.right * 0.4f + Vector3.down * 0.4f, Quaternion.Euler(0, 0, 60 - j * 10)) as GameObject;
                Vector2 dir1 = new Vector2(0.1f * Mathf.Cos((120 + j * 10) * Mathf.Deg2Rad), -0.1f * Mathf.Sin((120 + j * 10) * Mathf.Deg2Rad));
                temp1.GetComponent<BossFireBall>().move = new Vector3(dir1.x, dir1.y, 0);
                temp1.SetActive(true);
                Destroy(temp1, 3f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        if (!Die)
            Anim.Play("VBossIdle");
    }
    /* appear red then hurt */
    IEnumerator GetHurt()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    /* a coroutine to decrease health according to 'Laseremitter' */
    IEnumerator Burning(GameObject Emitter)
    {
        while (Laseremitter.Contains(Emitter))
        {
            if (Health <= 5 && !Die)
            {
                Die = true;
                foreach (GameObject item in Laseremitter)
                {
                    item.GetComponent<shootingversus>().Laserhitting.Remove(this.gameObject);
                }
                Laseremitter.Clear();
                if (Emitter.GetComponent<helicopmovementVersus>().side)
                {
                    RpcTakeDamage(5, 2);
                    GrantBuff(true);
                }
                else if (!Emitter.GetComponent<helicopmovementVersus>().side)
                {
                    RpcTakeDamage(5, 1);
                    GrantBuff(false);
                }
            }
            else
                RpcTakeDamage(5, 0);
            yield return new WaitForSeconds(0.2f);
        }
    }
    /* triggered when animation is played */
    void dissapear()
    {
        if(isServer)
            Spawner.GetComponent<BossSpawner>().start = true;
        Destroy(this.gameObject);
    }
    void BossDie()
    {
        Anim.SetTrigger("Die");
    }
}
