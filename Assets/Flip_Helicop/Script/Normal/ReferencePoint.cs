using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class ReferencePoint : MonoBehaviour {

    GameObject Heli = null;

    public float ForwardSpeed = 0f;
    int temp;

    AudioSource Audio;
    public List<AudioClip> SBGM = null; // music for the situation when the shield is on
    public List<AudioClip> NBGM = null; // music for the situation when the shield is not on
    public CanvasGroup Pause = null;

    bool pause = false;
    bool shootingpause = false;
    bool coolDown = false;

	// Use this for initialization
	void Start ()
    {
        Heli = GameObject.Find("player");
        ForwardSpeed = Heli.GetComponent<helicopmovement>().ForwardSpeed;
        Audio = gameObject.GetComponent<AudioSource>();
        temp = Random.Range(0, NBGM.Count); // randomly choose a song
        Audio.clip = NBGM[temp];
        Audio.Play();
        transform.GetChild(1).position = Camera.main.ScreenToWorldPoint(new Vector3(0, Camera.main.pixelHeight/2f, 0)) - new Vector3(0.35f, 0, 0); // a vertical bar that is used to limit pig's movement
        transform.GetChild(2).position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight / 2f, 0)) + new Vector3(0.35f, 0, 0); // a vertical bar that is used to limit pig's movement
    }
	
	// Update is called once per frame
    void Update()
    {
        // if 'Esc' is pressed the game will pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pause == true)
            {
                Time.timeScale = 1; // recover to the normal
                GameObject.Find("ejactor").GetComponent<shooting>().paused = false;
                pause = false;
                Pause.gameObject.SetActive(false);
            }
            else if (pause == false && !coolDown)
            {
                Time.timeScale = 0; // set timescale to 0 to pause
                GameObject.Find("ejactor").GetComponent<shooting>().paused = true;
                shootingpause = true;
                coolDown = true;
                pause = true;
                Pause.gameObject.SetActive(true);
                StartCoroutine(CoolDown());
            }
        }
    }
	void FixedUpdate () {
        transform.position += Vector3.right * ForwardSpeed;
	}
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(2);
        coolDown = false;
    }
    public void BackToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
    public void Resume()
    {
        Time.timeScale = 1;
        GameObject.Find("ejactor").GetComponent<shooting>().paused = false;
        pause = false;
        Pause.gameObject.SetActive(false);
    }
}
