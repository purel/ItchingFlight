using UnityEngine;
using System.Collections;

// a class used to tune pig's action
public class StarSmall : MonoBehaviour {

    GameObject BigStar;
    GameObject StarSmall1;
    GameObject StarSmall2;
    GameObject StarSmall3;
    GameObject StarSmall4;
    GameObject StarSmall5;
    GameObject StarSmall6;
    // Use this for initialization
    void Start ()
    {
        BigStar = transform.parent.GetChild(0).gameObject;
        StarSmall1 = transform.parent.GetChild(1).gameObject;
        StarSmall2 = transform.parent.GetChild(2).gameObject;
        StarSmall3 = transform.parent.GetChild(3).gameObject;
        StarSmall4 = transform.parent.GetChild(4).gameObject;
        StarSmall5 = transform.parent.GetChild(5).gameObject;
        StarSmall6 = transform.parent.GetChild(6).gameObject;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void InvokeBigStar()
    {
        StarSmall1.SetActive(false);
        StarSmall2.SetActive(false);
        StarSmall3.SetActive(false);
        StarSmall4.SetActive(false);
        StarSmall5.SetActive(false);
        StarSmall6.SetActive(false);
        BigStar.SetActive(true);
        BigStar.GetComponent<Animator>().Play("StarBig");
    }
}
