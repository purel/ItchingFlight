using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Wyvern : MonoBehaviour {

    Camera MainCamera;
    Animator Anim;

    GameObject Player;
    GameObject Scores;

    public bool Die = false;
    bool InScreen = false; // a boolean that specifies whether the wyvern is in side the screen
    bool Reverse = false; // check the facing side of the wyvern
    bool Grounded = false;
    bool start = false;
    bool up =false;
    bool upr = false;
    bool down = false;
    bool downr = false;

    int Health = 3;
    float YMax;
    float YMin;
    float XMax;

	// Use this for initialization
	void Start ()
    {
        Anim = gameObject.GetComponent<Animator>();
        YMax = GameObject.Find("player").GetComponent<helicopmovement>().YMax;
        YMin = GameObject.Find("player").GetComponent<helicopmovement>().YMin;
        MainCamera = Camera.main;
        Player = GameObject.Find("player");
        Scores = GameObject.Find("Score");
    }
	
	// Update is called once per frame
	void Update ()
    {
        XMax = MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x - 1f;
    }
    void FixedUpdate()
    {
        // Only when wyvern is inside the screen will it start to attack, otherwise it will stop
        if (InScreen == false && transform.position.x < MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x)
            InScreen = true;
        if (InScreen == true && transform.position.x < MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x)
            InScreen = false;
        if (!Reverse && transform.position.x < Player.transform.position.x && !Die)
        {
            Vector3 temp = new Vector3(-2, 2, 0);
            transform.localScale = temp;
            Reverse = true;
        }
        if (Reverse && transform.position.x > Player.transform.position.x && !Die)
        {
            Vector3 temp = new Vector3(2, 2, 0);
            transform.localScale = temp;
            Reverse = false;
        }
        if (!Die && InScreen && !start)
        {
            start = true;
            StartCoroutine(Attack());
        }
        // start to move when wyvern is inside the screen
        #region Move
        if(!Die && InScreen)
        {
            Vector3 Pos = Vector3.zero;
            Pos = transform.position;

            if (up == true)
                Pos += Vector3.up * Time.deltaTime ;
            else if (upr == true)
            {
                Pos += Vector3.up * Time.deltaTime ;
                Pos += Vector3.right * Time.deltaTime *2f ;
            }
            else if (down == true)
                Pos += Vector3.down * Time.deltaTime ;
            else if (downr == true)
            {
                Pos += Vector3.down * Time.deltaTime ;
                Pos += Vector3.right * Time.deltaTime *2f;
            }
            Pos.y = Mathf.Clamp(Pos.y, YMin, YMax);
            Pos.x = Mathf.Clamp(Pos.x, 0, XMax);
            transform.position = Pos;
        }
        #endregion
        if (Die && !Grounded)
        {
            transform.position += Vector3.down * Time.deltaTime * 8f;
            if (transform.position.y <= -1.5f)
            {
                Grounded = true;
                Anim.Play("WyvernLie");
            }
        }
        if (Grounded && transform.position.x < MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x-5)
            Destroy(this.gameObject);
    }
    IEnumerator Attack()
    {
        while (InScreen)
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(Move());
            yield return new WaitForSeconds(1);
            Anim.SetTrigger("Attack");
            float angle = Mathf.Rad2Deg*Mathf.Atan2((transform.position.y - Player.transform.position.y) , (transform.position.x - Player.transform.position.x)) + 90f;
            if (!Die)
            {
                GameObject tempFireBall = Instantiate(transform.GetChild(0).gameObject, transform.GetChild(0).position, Quaternion.Euler(0, 0, angle)) as GameObject;
                tempFireBall.SetActive(true);
            }
            if (Die || !InScreen)
                yield break;
        }
        yield return 0;
    }
    IEnumerator Move()
    {
        if (!Die && InScreen)
        {
            up = upr = down = downr = false;
            float temp = Random.value;
            if (temp < 0.25f)
                up = true;
            else if (0.25f <= temp && temp < 0.5f)
                upr = true;
            else if (0.5f <= temp && temp < 0.75f)
                down = true;
            else
                downr = true;
        }
        yield return 0;
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        // only take damage when inside the screen
        if(collider.CompareTag("missile") && InScreen)
        {
            Health--;
            if(Health >=0)
            transform.GetChild(1).GetChild(Health).gameObject.SetActive(false);
            if(collider.name == "ChiWave(Clone)")
            {
                Health--;
                if (Health >= 0)
                    transform.GetChild(1).GetChild(Health).gameObject.SetActive(false);
            }
            Anim.SetTrigger("Hurt");
            if(Health <=0)
            {
                Die = true;
                int temp = (int)Random.Range(30f, 55f);
                Scores.GetComponent<score>().EvokeGoldTip(temp, Camera.main.WorldToScreenPoint(transform.position));
                Anim.Play("WyvernDie");
            }
        }
    }

}
