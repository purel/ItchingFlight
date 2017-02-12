using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackGround : MonoBehaviour {

    Camera cam;
	// Use this for initialization
	void Start ()
    {
        cam = Camera.main;
        if (cam.pixelWidth / cam.pixelHeight > 16 / 9)
            GetComponent<CanvasScaler>().matchWidthOrHeight = 0;
        else
            GetComponent<CanvasScaler>().matchWidthOrHeight = 1;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
