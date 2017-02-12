using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Lightning : NetworkBehaviour {

    public bool side;
    public bool up = true;
    public bool down = true;
    public bool damage = false;
    GameObject Base;
    GameObject OtherBase;
    public BGM bgmcon;
	// Use this for initialization
	void Start ()
    {
        if (transform.position.x > 0)
        {
            side = true;
            Base = GameObject.Find("DireBase");
            OtherBase = GameObject.Find("RadientBase");
        }
        else
        {
            side = false;
            Base = GameObject.Find("RadientBase");
            OtherBase = GameObject.Find("DireBase");
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        /* triggered when a tower is destroyed */
        if (damage)
        {
            damage = false;
            /* check side */
            if (side)
            {
                /* check which tower is left */
                if (up)
                {
                    oneupD(); // change the appearance of the lightning 
                    Base.GetComponent<Base>().state = 1; // change base's status
                }
                else if (down)
                {
                    onedownD();
                    Base.GetComponent<Base>().state = 1;
                }
                /* if no tower is left */
                else
                {
                    Base.GetComponent<Base>().state = 0; // change base's status
                    /* change the background music */
                    if(bgmcon.side == true)
                    {
                        if (OtherBase.GetComponent<Base>().state == 0)
                            bgmcon.tie = true;
                        else
                            bgmcon.losing = true;
                    }
                    else
                    {
                        if (OtherBase.GetComponent<Base>().state == 0)
                            bgmcon.tie = true;
                        else
                            bgmcon.winning = true;

                    }
                    Destroy(this.gameObject);
                }
            }
            else
            {
                if (up)
                {
                    oneupR();
                    Base.GetComponent<Base>().state = 1;
                }
                else if (down)
                {
                    onedownR();
                    Base.GetComponent<Base>().state = 1;
                }
                else
                {
                    Base.GetComponent<Base>().state = 0;
                    if (bgmcon.side == false)
                    {
                        if (OtherBase.GetComponent<Base>().state == 0)
                            bgmcon.tie = true;
                        else
                            bgmcon.losing = true;
                    }
                    else
                    {
                        if (OtherBase.GetComponent<Base>().state == 0)
                            bgmcon.tie = true;
                        else
                            bgmcon.winning = true;

                    }
                    Destroy(this.gameObject);
                }
            }
        }
	}
    /* functions to change the appearance of the lightning */
    void oneupD()
    {
        transform.eulerAngles = new Vector3(0, 0, 19);
        transform.localScale = new Vector3(0.85f, 0.85f, 1);
    }
    void onedownD()
    {
        transform.position = new Vector3(48, -2.8f, 0);
        transform.eulerAngles = new Vector3(0, 0, 161);
        transform.localScale = new Vector3(0.85f, 0.85f, 1);
    }
    void oneupR()
    {
        transform.eulerAngles = new Vector3(0, 0, -19);
        transform.localScale = new Vector3(-0.85f, 0.85f, 1);
    }
    void onedownR()
    {
        transform.position = new Vector3(-48, -2.8f, 0);
        transform.eulerAngles = new Vector3(0, 0, -161);
        transform.localScale = new Vector3(-0.85f, 0.85f, 1);
    }
}
