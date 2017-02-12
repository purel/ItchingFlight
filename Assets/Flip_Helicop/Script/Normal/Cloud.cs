using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour {
    public AudioClip Sound;
    public AudioSource RFPointAudio;
    int SNum;
    int temp;

	// Use this for initialization
	void Start () {
        SNum = RFPointAudio.gameObject.GetComponent<ReferencePoint>().SBGM.Count;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        // if player collides with a smiling cloud
        if(collider.CompareTag("Player") && gameObject.GetComponent<Renderer>().sortingOrder == 1)
        {
            // if the shield is not on, turn on the shield
            if (collider.gameObject.GetComponent<helicopmovement>().ShieldOn == false)
            {
                temp = Random.Range(0, SNum);
                RFPointAudio.Stop();
                RFPointAudio.clip = RFPointAudio.GetComponent<ReferencePoint>().SBGM[temp];
                RFPointAudio.Play();
            }
            AudioSource.PlayClipAtPoint(Sound, transform.position);
            this.gameObject.GetComponent<SpriteRenderer>().sortingOrder = 2; // hide the smile
            this.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            collider.GetComponent<helicopmovement>().ShieldLife = 3; // refill shield life
            collider.GetComponent<helicopmovement>().ShieldOn = true;
            collider.transform.GetChild(6).gameObject.SetActive(true);
            collider.transform.GetChild(5).GetChild(0).gameObject.SetActive(true); // display shield icons
            collider.transform.GetChild(5).GetChild(1).gameObject.SetActive(true);
            collider.transform.GetChild(5).GetChild(2).gameObject.SetActive(true);
        }
    }
}
