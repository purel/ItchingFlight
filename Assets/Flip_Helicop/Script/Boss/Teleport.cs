using UnityEngine;
using System.Collections;

public class Teleport : MonoBehaviour {

    public GameObject Player;
    public GameObject Boss;
    public AudioClip Sound;
    Vector3 TeleportPos = Vector3.zero;
    Camera MainCamera;
    public bool Back;
    void Start () {
        MainCamera = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    // this function moves boss to the back of the player
    // and moves the boss back to the right side later
    void Transit()
    {
        if (Back == false)
        {
            TeleportPos.y = Player.transform.position.y;
            TeleportPos.x = Player.transform.position.x + Random.Range(-6f, -3f);
            if (TeleportPos.x < MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + 1)
                TeleportPos.x = MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + 1;
            AudioSource.PlayClipAtPoint(Sound, transform.position);
            transform.position = TeleportPos;
        }
        else
        {
            AudioSource.PlayClipAtPoint(Sound, transform.position);
            transform.position = new Vector3(20,0,0);
        }
    }
    void Disappear()
    {
        Boss.transform.position = transform.position;
        Destroy(this.gameObject);
    }
}
