using UnityEngine;
using System.Collections;

public class CameraBoss : MonoBehaviour {

    public GameObject Boss;
    public GameObject Player;
    public GameObject Starter;
    public GameObject Img;
    public GameObject StageImg;
    public GameObject Bonus;
    public AudioSource BGM;
    Camera cam;
    float Xmin;
    float Xmax;
    float Dis = 0f;
    public bool Win = false;
    bool Moving = false;
    bool Begin = false;
    Vector3 temp = new Vector3(0,0,-14.4f);
	// Use this for initialization
	void Start () {
        Win = true;
        Starter.SetActive(true);
        Img.SetActive(true);
        cam = GetComponent<Camera>();
        Xmin = -35.5f + (cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, 0)) - cam.ScreenToWorldPoint(Vector3.zero)).x;
        Xmax = 35.5f - (cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth / 2, 0, 0)) - cam.ScreenToWorldPoint(Vector3.zero)).x;
        transform.position = new Vector3(Xmin, transform.position.y, transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        // if the Boss is defeated don't fix the camera
        if (Win)
        {
            Win = false;
            Dis = transform.position.x - Player.transform.position.x;
            Moving = true;
        }
    }
    void FixedUpdate()
    {
        if(Moving)
        {
            temp.x = Mathf.Clamp(Player.transform.position.x + Dis,Xmin,Xmax);
            transform.position = temp;
        }
        // if player enters the fighting area, combat starts
        if (transform.position.x >= 8 && !Begin)
        {
            BGM.Play();
            Begin = true;
            Bonus.GetComponent<BonusBoss>().Begin = true;
            Boss.GetComponent<Boss>().Begin = true;
            Player.transform.GetChild(2).gameObject.GetComponent<shootingBoss>().Begin = true;
            Moving = false;
            Vector3 temp = transform.position;
            temp.x = 8;
            transform.position = temp;
            Starter.SetActive(false);
            Img.SetActive(false);
            StageImg.SetActive(true);
        }
    }
    // a coroutine that shakes the camera when player hit
    public IEnumerator CameraShake(int big)
    {
        for (int i = 0; i < 5; i++)
        {
            transform.position -= new Vector3(0.5f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            transform.position += new Vector3(0.5f, 0, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
