using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

public class score : MonoBehaviour
{
    public helicopmovement helicop = null;

    public Text Meter;
    public Text Best;
    public Text Gold;
    public Text BlackholeBonus;

    public static float total = 0;
    public static bool isset = false;
    public int CurrentGold = 0;

    public GameObject Right;
    public GameObject GoldTip;

    public Canvas GUI = null;
    public List<AudioClip> GoldSound = null;
    AudioSource Audio;

    void Awake()
    {
        // at the start of the game, load best score and gold amount
        SetBest(PlayerPrefs.GetInt("Best"));
        CurrentGold = PlayerPrefs.GetInt("Gold");
        Gold.text = "Gold : " + CurrentGold;
        // check whether the player just left from the boss fight or not
        if (PlayerPrefs.GetInt("BlackholeBonus") == 1)
        {
            // set a random level
            Right.GetComponent<rightmarginmovement>().Level = PlayerPrefs.GetInt("Level");
            Right.GetComponent<rightmarginmovement>().SetLevel();
            BlackholeBonus.gameObject.SetActive(true);
            total += 800f; // score bonus after the boss fight
            PlayerPrefs.SetInt("BlackholeBonus", 0);

        }
        Audio = gameObject.GetComponent<AudioSource>();
    }
    void FixedUpdate()
    {
        total += (helicop.ForwardSpeed); // keeps adding score as time increases
        SetScore((int)total);
    }

    void SetScore(int score)
    {
        Meter.text = "METER : " + score;
    }
    public void SetBest(int score)
    {
        Best.text = "BEST : " + score;
    }
    public void sethighscore()
    {
        isset = true;
        // check whether the score is higher than the best score
        if(total > PlayerPrefs.GetInt("Best"))
        {
            PlayerPrefs.SetInt("Best", (int)total);
            SetBest(PlayerPrefs.GetInt("Best"));
            total = 0;
        }
    }
    // a function that displays a visual promt after killing a monster and looting the money
    public void EvokeGoldTip(int amount,  Vector3 pos)
    {
        int rand = Random.Range(0, GoldSound.Count);
        Audio.clip = GoldSound[rand];
        Audio.Play();
        CurrentGold += amount;
        GameObject temp = Instantiate(GoldTip, pos, Quaternion.identity) as GameObject;
        temp.gameObject.GetComponent<Text>().text = "+ " + amount + " gold";
        temp.transform.parent = GUI.transform;
        Gold.text = "Gold : " + CurrentGold;
    }
}
