using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random; 

public class helicopmovement : MonoBehaviour {

    public float VAccVelocity = 0.2f; // vertical speed
    public float HAccVelocity = 0.2f; // horizontal speed
    public float ForwardSpeed = 0.1f; // forward speed
    public float YMin = -0.4f; // values constraining the moving space of player
    public float YMax = 11f;
    public float XMin;
    public float XMax;
    float CameraWidth;

    public score scores = null;
    public Camera mainCamera;
    Animator animController;
    int NNum;
    public int ShieldLife = 0; // maximum 3

    bool didUp = false; // booleans to control movement
    bool didDown = false;
    bool didLeft = false;
    bool didRight = false;
    public bool ShieldOn = false;
    public bool die = false;

    shooting Shooting = null;
    public GameObject gameOver= null;
    public AudioClip HurtSound;
    public AudioSource RFPointAudio;
    Vector3 HeliPos = Vector3.zero;
    Vector3 XLimit;

	void Start ()
    {
        animController = GetComponent<Animator>();
        HeliPos = transform.position;
        CameraWidth = mainCamera.pixelWidth;
        XLimit = new Vector3(CameraWidth, 0, 0);
        Shooting = GameObject.Find("ejactor").GetComponent<shooting>() ;
        NNum = RFPointAudio.gameObject.GetComponent<ReferencePoint>().NBGM.Count;
    }
	
	void Update ()
    {
        if(die)
        {
            // if the player dies, store the gold and score
            PlayerPrefs.SetInt("Gold", scores.CurrentGold);
            // ask class 'score' to set score
            if (!score.isset && scores != null)
                scores.sethighscore();
            gameOver.SetActive(true);
            // after death, press space to restart
            if(Input.GetKeyDown(KeyCode.Space))
            {
                score.total = 0;
                score.isset = false;
                shooting.Ult = 10;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        // monitor keystrokes
        if (Input.GetKey(KeyCode.W) && die == false)
             didUp = true;
        if (Input.GetKey(KeyCode.S) && die == false)
            didDown = true;
        if (Input.GetKey(KeyCode.A) && die == false)     
            didLeft = true;
        if (Input.GetKey(KeyCode.D) && die == false)
            didRight = true;
        if (Input.GetKeyUp(KeyCode.W) && die == false)
            didUp = false;
        if (Input.GetKeyUp(KeyCode.S) && die == false)
            didDown = false;
        if (Input.GetKeyUp(KeyCode.A) && die == false)
            didLeft = false;
        if (Input.GetKeyUp(KeyCode.D) && die == false)
            didRight = false;
    }

    void FixedUpdate()
    {
        if (die == true)
        {
            ForwardSpeed = 0f;
            Shooting.Die = true;
        }
        // move player in FixedUpdate()
        transform.position += Vector3.right * ForwardSpeed;
        if (didUp)
        {
            transform.position += Vector3.up*VAccVelocity;
        }
        if (didDown)
        {
            transform.position += Vector3.down * VAccVelocity;
        }
        if (didLeft)
        {
            transform.position += Vector3.left * HAccVelocity;
        }
        if (didRight)
        {
            transform.position += Vector3.right * HAccVelocity;
        }
        // limit the range of movement
        XMin = mainCamera.ScreenToWorldPoint(Vector3.zero).x+1f;
        XMax = mainCamera.ScreenToWorldPoint(XLimit).x-1f;
        HeliPos = transform.position;
        HeliPos.y = Mathf.Clamp(HeliPos.y, YMin, YMax);
        HeliPos.x = Mathf.Clamp(HeliPos.x, XMin, XMax);
        transform.position = HeliPos;
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        // if hit by a obstacle, die immediately
        if (collider.tag == "spike")
        {
            AudioSource.PlayClipAtPoint(HurtSound, transform.position);
            StartCoroutine(mainCamera.gameObject.GetComponent<CameraNormal>().CameraShake(1));
            ShieldOff();
            Shooting.LaserOff();
            die = true;
            animController.SetTrigger("die");
        }
        // if hit by a monster shield life decrease one or die
        else if (collider.tag == "Harm" || collider.tag == "Monster")
        {
            AudioSource.PlayClipAtPoint(HurtSound, transform.position);
            StartCoroutine(mainCamera.gameObject.GetComponent<CameraNormal>().CameraShake(1));
            if (ShieldOn)
            {
                ShieldLife--;
                if (ShieldLife >= 0)
                    transform.GetChild(5).GetChild(ShieldLife).gameObject.SetActive(false);
                if (ShieldLife == 0)
                {
                    ShieldOff();
                }
            }
            else
            {
                die = true;
                ShieldOff();
                Shooting.LaserOff();
                animController.SetTrigger("die");
            }
        }
     }
    // a function that turns off the shield
    void ShieldOff()
    {
        gameObject.transform.GetChild(6).gameObject.SetActive(false);
        ShieldOn = false;
        int temp = Random.Range(0, NNum);
        RFPointAudio.Stop();
        RFPointAudio.clip = RFPointAudio.GetComponent<ReferencePoint>().NBGM[temp];
        RFPointAudio.Play();
    }
}
