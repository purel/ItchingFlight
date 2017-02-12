using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

// this class is a different version of helicopmovement or helicopmovementversus
public class helicopmovementBoss : MonoBehaviour
{
    public float VAccVelocity = 0.2f;
    public float HAccVelocity = 0.2f;

    public Camera mainCamera;
    Animator animController;

    bool didUp = false;
    bool didDown = false;
    bool didLeft = false;
    bool didRight = false;
    public bool ShieldOn = false;
    public bool die = false;

    shootingBoss Shooting = null;
    public GameObject gameOver = null;
    Vector3 HeliPos = Vector3.zero;

    public float YMin = -0.4f;
    public float YMax = 11f;
    public float XMin;
    public float XMax;
    float CameraWidth;

    Vector3 XLimit;
    public int ShieldLife = 0;
    void Start()
    {
        animController = GetComponent<Animator>();
        HeliPos = transform.position;
        CameraWidth = mainCamera.pixelWidth;
        XLimit = new Vector3(CameraWidth, 0, 0);
        Shooting = GameObject.Find("ejactor").GetComponent<shootingBoss>();
    }

    void Update()
    {
        if (die)
        {
            gameOver.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (PlayerPrefs.GetInt("Total") > PlayerPrefs.GetInt("Best"))
                    PlayerPrefs.SetInt("Best", PlayerPrefs.GetInt("Total"));
                shootingBoss.Ult = 10;
                SceneManager.LoadScene(0);
            }
        }
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
            Shooting.Die = true;
        if (didUp)
        {
            transform.position += Vector3.up * VAccVelocity;
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
        XMin = mainCamera.ScreenToWorldPoint(Vector3.zero).x + 1f;
        XMax = mainCamera.ScreenToWorldPoint(XLimit).x - 1f;
        HeliPos = transform.position;
        HeliPos.y = Mathf.Clamp(HeliPos.y, YMin, YMax);
        HeliPos.x = Mathf.Clamp(HeliPos.x, XMin, XMax);
        transform.position = HeliPos;
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Harm" || collider.tag == "Monster")
        {
            StartCoroutine(mainCamera.GetComponent<CameraBoss>().CameraShake(1));
            if (ShieldOn)
            {
                if (collider.name == "rasengan(Clone)")
                {
                    for (int i = 0; i < 3; i++)
                    {
                        ShieldLife--;
                        if (ShieldLife >= 0)
                            transform.GetChild(5).GetChild(ShieldLife).gameObject.SetActive(false);
                        if (ShieldLife == 0)
                        {
                            gameObject.transform.GetChild(6).gameObject.SetActive(false);
                            ShieldOn = false;
                        }
                    }
                }
                else
                {
                    ShieldLife--;
                    if (ShieldLife >= 0)
                        transform.GetChild(5).GetChild(ShieldLife).gameObject.SetActive(false);
                    if (ShieldLife == 0)
                    {
                        gameObject.transform.GetChild(6).gameObject.SetActive(false);
                        ShieldOn = false;
                    }
                }
            }
            else
            {
                die = true;
                Shooting.LaserOff();
                animController.SetTrigger("die");
            }
        }
    }
    void Dead()
    {
        return;
    }
}
