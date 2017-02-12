using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class MainMenuManager : MonoBehaviour {

    /* menu components */
    public CanvasGroup CurrentGroup = null;
    public CanvasGroup Title = null;
    public CanvasGroup CurrentTab = null;
    public CanvasGroup RightMenu = null;
    public GameObject Help = null;

    public RightMenu RightControll = null; /* control text shown in right menu */

    public Text HighScoreNum = null;
    public Text ChiText;
    public Text AimText;
    public Text ShieldText;
    public Text LaserText;
    public Text Gold;
    public Text ChiAmount;
    public Text CoolDownAmount;

    /* purchase */
    bool ChiP = false;
    bool AimP = false;
    bool ShieldP = false;
    bool LaserP = false;

    int GoldNum = 0;
    int ChiNum = 0;
    int CoolNum = 0;

    void Start()
    {
        /* load player's record */
        HighScoreNum.text = PlayerPrefs.GetInt("Best").ToString();
        GoldNum = PlayerPrefs.GetInt("Gold");
        Gold.text = "Gold : " + GoldNum;
        ChiNum = PlayerPrefs.GetInt("ChiPotion");
        ChiAmount.text = "You have : " + ChiNum;
        CoolNum = PlayerPrefs.GetInt("CoolDown");
        CoolDownAmount.text = "You have : " + CoolNum;
        ChiP = Convert.ToBoolean(PlayerPrefs.GetInt("ChiP"));
        AimP = Convert.ToBoolean(PlayerPrefs.GetInt("AimP"));
        ShieldP = Convert.ToBoolean(PlayerPrefs.GetInt("ShieldP"));
        LaserP = Convert.ToBoolean(PlayerPrefs.GetInt("LaserP"));

        /* adjust buttons according to purchases */
        if (ChiP)
        {
            ChiText.text = "Purchased";
            ChiText.transform.parent.GetComponent<Button>().interactable = false;
        }
        if (AimP)
        {
            AimText.text = "Purchased";
            AimText.transform.parent.GetComponent<Button>().interactable = false;
        }
        if (ShieldP)
        {
            ShieldText.text = "Purchased";
            ShieldText.transform.parent.GetComponent<Button>().interactable = false;
        }
        if (LaserP)
        {
            LaserText.text = "Purchased";
            LaserText.transform.parent.GetComponent<Button>().interactable = false;
        }
    }
    /* a button function that switch menu components */
	public void SwitchMenu(CanvasGroup Dest)
    {
        CurrentGroup.GetComponent<Animator>().SetTrigger("out");
        Dest.GetComponent<Animator>().SetTrigger("in");
        if (Dest.name == "MainMenu")
            Title.GetComponent<Animator>().SetTrigger("in");
        else
            Title.GetComponent<Animator>().SetTrigger("out");
        /* disable right menu for side menu */
        if (CurrentGroup.name == "SideMenu")
        {
            if (CurrentTab != null)
            {
                RightMenu.GetComponent<Animator>().SetTrigger("back");
                CurrentTab.gameObject.SetActive(false);
            }
            CurrentTab = null;
        }
        CurrentGroup = Dest;
    }
    /* a button function that is called when changing right menu */
    public void SwitchTab(CanvasGroup Tab)
    {
        if (CurrentTab == null)
        {
            RightMenu.GetComponent<Animator>().SetTrigger("in");
            CurrentTab = Tab;
            /* RightControll controlls the content shown */
            RightControll.CurrentTab = CurrentTab;
            Tab.gameObject.SetActive(true);
        }
        else if (CurrentTab != null && Tab != CurrentTab)
        {
            RightMenu.GetComponent<Animator>().SetTrigger("out");
            RightMenu.GetComponent<Animator>().SetTrigger("in");
            CurrentTab = Tab;
            RightControll.DestTab = CurrentTab;
        }
    }

    /* purchase buttons in shop menu */
    public void BuyChiPotion()
    {
        if (GoldNum >= 550)
        {
            GoldNum -= 550;
            PlayerPrefs.SetInt("Gold", GoldNum);
            Gold.text = "Gold : " + GoldNum;
            ChiNum++;
            PlayerPrefs.SetInt("ChiPotion", ChiNum);
            ChiAmount.text = "You have : " + ChiNum;
        }
    }
    public void BuyCoolPotion()
    {
        if (GoldNum >= 850)
        {
            GoldNum -= 850;
            PlayerPrefs.SetInt("Gold", GoldNum);
            Gold.text = "Gold : " + GoldNum;
            CoolNum++;
            PlayerPrefs.SetInt("CoolDown", CoolNum);
            CoolDownAmount.text = "You have : " + CoolNum;
        }
    }
    public void BuyChi()
    {
        if (GoldNum >= 10000)
        {
            GoldNum -= 10000;
            PlayerPrefs.SetInt("Gold", GoldNum);
            Gold.text = "Gold : " + GoldNum;
            ChiP = true;
            PlayerPrefs.SetInt("ChiP", 1);
            ChiText.text = "Purchased";
            ChiText.transform.parent.GetComponent<Button>().interactable = false;
        }
    }
    public void BuyAim()
    {
        if (GoldNum >= 30000)
        {
            GoldNum -= 30000;
            PlayerPrefs.SetInt("Gold", GoldNum);
            Gold.text = "Gold : " + GoldNum;
            AimP = true;
            PlayerPrefs.SetInt("AimP", 1);
            AimText.text = "Purchased";
            AimText.transform.parent.GetComponent<Button>().interactable = false;
        }
    }
    public void BuyShield()
    {
        if (GoldNum >= 50000)
        {
            GoldNum -= 50000;
            PlayerPrefs.SetInt("Gold", GoldNum);
            Gold.text = "Gold : " + GoldNum;
            ShieldP = true;
            PlayerPrefs.SetInt("ShieldP", 1);
            ShieldText.text = "Purchased";
            ShieldText.transform.parent.GetComponent<Button>().interactable = false;
        }
    }
    public void BuyLaser()
    {
        if (GoldNum >= 100000)
        {
            GoldNum -= 100000;
            PlayerPrefs.SetInt("Gold", GoldNum);
            Gold.text = "Gold : " + GoldNum;
            LaserP = true;
            PlayerPrefs.SetInt("LaserP", 1);
            LaserText.text = "Purchased";
            LaserText.transform.parent.GetComponent<Button>().interactable = false;
        }
    }
    /* button functions for mainmenu */
    public void OpenHelp()
    {
        Help.gameObject.SetActive(true);
    }
    public void CloseHelp()
    {
        Help.gameObject.SetActive(false);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Enter()
    {
        SceneManager.LoadScene(1);
    }
    public void EnterVersus()
    {
        SceneManager.LoadScene(3);
    }
}
