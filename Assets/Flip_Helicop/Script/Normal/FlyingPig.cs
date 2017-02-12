using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class FlyingPig : MonoBehaviour
{
    Camera MainCamera;
    GameObject Player;
    Animator Anim;
    public Vector3 Target; // the position of player

    // visual objects for the skill
    public GameObject StarSmall1;
    public GameObject StarSmall2;
    public GameObject StarSmall3;
    public GameObject StarSmall4;
    public GameObject StarSmall5;
    public GameObject StarSmall6;
    public GameObject Heart; // health object
    GameObject Scores;

    int Health = 1;
    Vector3 HeartDis;

    public bool Die = false;
    bool InScreen = false;
    bool Reverse = false;
    bool Started = false;
    bool Grounded = false;
    public bool Margin = false;
    public bool Attacking = false;

    float YMax=12f; // values constrain pig's moving range
    float YMin =-1.5f;
    float XMax;
    float speed = 0.4f;

    // Use this for initialization
    void Start()
    {
        Anim = gameObject.GetComponent<Animator>();
        HeartDis = Heart.transform.position - transform.position;
        MainCamera = Camera.main;
        Player = GameObject.Find("player");
        Scores = GameObject.Find("Score");
    }

    // Update is called once per frame
    void Update()
    {
        XMax = MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x - 1f;
    }
    void FixedUpdate()
    {
        Heart.transform.position = transform.position + HeartDis;
        if (InScreen == false && transform.position.x < MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x)
            InScreen = true;
        // if it at the edge of the screen keep it inside the screen
        if (InScreen && !Die && (transform.position.x <= MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + 1f || transform.position.x >= MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x))
        {
            Vector3 temp = transform.position;
            temp.x = Mathf.Clamp(temp.x, MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x, XMax);
            transform.position = temp;
         }
        if (transform.position.y <= YMin || transform.position.y>=YMax)
        {
            Vector3 temp = transform.position;
            temp.y = Mathf.Clamp(temp.y, YMin, YMax);
            transform.position = temp;
        }

        if (!Die && InScreen && !Started)
        {
            // start to attack
            StartCoroutine(Attack());
            Started = true;
        }
        if (Attacking && !Die)
            transform.position += Target * speed;
        if (Die && !Grounded)
        {
            transform.position += Vector3.down * Time.deltaTime * 8f;
            if (transform.position.y <= -1.5f)
                Grounded = true;
        }
        if (Grounded && transform.position.x < MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x - 5)
            Destroy(this.gameObject);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("missile") && InScreen)
        {
            Health--;
            if (Health >= 0)
                Heart.SetActive(false);
            if (Health == 0)
            {
                Die = true;
                int temp = (int)Random.Range(30f,55f);
                Scores.GetComponent<score>().EvokeGoldTip(temp, Camera.main.WorldToScreenPoint(transform.position));
                Anim.Play("FlyingPigDie");
            }
        }
        // if the pig hit the edge of the screen, start to attack again
        if(collider.CompareTag("Fence") || (collider.CompareTag("margin") && collider.name != "rightMargin" && collider.name != "rightMarginup" && collider.name != "rightMargindown"))
        {
            if (!Die && !Margin)
            {
                Margin = true;
                Attacking = false;
                Anim.Play("FlyingPig");
                StartCoroutine(Attack()); // attack again
            }
        }
    }
    IEnumerator Attack()
    {
        // start the skill animation
        StarSmall1.SetActive(true);
        StarSmall2.SetActive(true);
        StarSmall3.SetActive(true);
        StarSmall4.SetActive(true);
        StarSmall5.SetActive(true);
        StarSmall6.SetActive(true);
        StarSmall1.GetComponent<Animator>().SetTrigger("Inhale");
        StarSmall2.GetComponent<Animator>().SetTrigger("Inhale");
        StarSmall3.GetComponent<Animator>().SetTrigger("Inhale");
        StarSmall4.GetComponent<Animator>().SetTrigger("Inhale");
        StarSmall5.GetComponent<Animator>().SetTrigger("Inhale");
        StarSmall6.GetComponent<Animator>().SetTrigger("Inhale");
        // always face to the player
        #region turn
    if (!Reverse && transform.position.x < Player.transform.position.x && !Die)
        {
            Vector3 temp = new Vector3(2, 2, 0);
            transform.localScale = temp;
            Reverse = true;
        }
        if (Reverse && transform.position.x > Player.transform.position.x && !Die)
        {
            Vector3 temp = new Vector3(-2, 2, 0);
            transform.localScale = temp;
            Reverse = false;
        }
        #endregion
        if (Die)
            yield return 0;
    }
}
