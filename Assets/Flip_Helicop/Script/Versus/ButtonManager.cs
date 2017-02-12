using UnityEngine;
using System.Collections;

public class ButtonManager : MonoBehaviour {

    public Network NetworkManager;
    public GameObject Pause;
    public bool start = false;
    bool pauseOn = false;

	// Use this for initialization
	void Start ()
    {
        NetworkManager = GameObject.Find("NetworkManager").GetComponent<Network>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && start && !pauseOn)
        {
            Pause.SetActive(true);
            pauseOn = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape) && start && pauseOn)
        {
            Pause.SetActive(false);
            pauseOn = false;
        }
    }
    public void HostButton()
    {
        NetworkManager.ClickStartHost();
    }
    public void ClientButton()
    {
        NetworkManager.ClickStartClient();
    }
    public void BackButton()
    {
        NetworkManager.ClickBack();
    }
    public void EditComplete()
    {
        NetworkManager.GetIPAddress();
    }
    public void ClickResume()
    {
        pauseOn = false;
        Pause.SetActive(false);
    }
}
