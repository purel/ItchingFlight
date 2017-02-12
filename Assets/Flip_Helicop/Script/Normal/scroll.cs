using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

// this script is run on a rectangular object on the left of the screen which keeps move right at the same speed of the camera
public class scroll : MonoBehaviour {

    public List<GameObject> Panels; // background gameobject

    int numberOfPanels = 0;
    float offsetX;
    public float SmileCloudProb = 0.3f; // the possibility of a smiling cloud

    Vector3 posc = Vector3.zero;
    Vector3 poss = Vector3.zero;

    Transform player;

    void Awake()
    {
        if (Panels != null)
            numberOfPanels = Panels.Count;
        GameObject playerObject = GameObject.Find("ReferencePoint");
        player = playerObject.transform;
        offsetX = transform.position.x - player.position.x;
    }
	void OnTriggerEnter2D(Collider2D collider)
    {
        float width = ((BoxCollider2D)collider).size.x;
        // destroy all obstacles when it's outside the screen on the left
        if (collider.CompareTag("Switch"))
            Destroy(collider.transform.parent.gameObject);
        // destroy all monsters when it's outside the screen on the left
        else if (collider.CompareTag("Harm") || collider.CompareTag("Monster"))
                    Destroy(collider.gameObject);
        // move the background outside the screen on the left to the right of the screen
        // this seamlessly makes the background scrolling with repeated movements
        else if (collider.CompareTag("RandomBackground"))
        {
            posc = collider.transform.position;
            posc.x += Random.Range(50f, 300f);
            collider.transform.position = posc;
            // move the cloud right with a random distance
            if (collider.name == "Cloud")
            {
                // randomly decide whether the cloud will smile or not
                if (Random.value <= SmileCloudProb)
                {
                    collider.gameObject.GetComponent<Renderer>().sortingOrder = 1;
                    collider.transform.GetChild(0).gameObject.GetComponent<Renderer>().sortingOrder = 2;
                }
                else
                {
                    collider.gameObject.GetComponent<Renderer>().sortingOrder = 2;
                    collider.transform.GetChild(0).gameObject.GetComponent<Renderer>().sortingOrder = 1;
                }
            }
        }
        else
        {
            posc = collider.transform.position;
            posc.x += width * numberOfPanels;
            collider.transform.position = posc;
        }
        
    }
    void Update()
    {
        if (player != null)
        {
            poss = transform.position;
            poss.x = player.position.x + offsetX;
            transform.position = poss;
        }
    }
}
