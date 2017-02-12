using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class NormalEntrance : MonoBehaviour {
    public GameObject Score;
    float total;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player")  && !collider.GetComponent<helicopmovement>().die)
        {
            // save the total score and move to the boss fight scene
            total = score.total;
            PlayerPrefs.SetInt("Total", (int)total);
            SceneManager.LoadScene("BossFight");
        }
    }
}
