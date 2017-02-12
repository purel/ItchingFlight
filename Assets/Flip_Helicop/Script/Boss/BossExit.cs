using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class BossExit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player") && collider.GetComponent<helicopmovementBoss>().die == false)
        {
            PlayerPrefs.SetInt("BlackholeBonus", 1);
            int temp = Random.Range(1, 6);
            PlayerPrefs.SetInt("Level", temp);
            int gold = PlayerPrefs.GetInt("Gold");
            gold += 200;
            PlayerPrefs.SetInt("Gold", gold);
            SceneManager.LoadScene("Normal");
        }
    }
}
