using UnityEngine;
using System.Collections;

public class CameraNormal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    // a coroutine that shakes the camera when hit
    public IEnumerator CameraShake(int big)
    {
        for (int i = 0; i < 4; i++)
        {
            transform.position -= new Vector3(0.2f + big * 0.3f, 0, 0);
            yield return new WaitForSeconds(0.05f);
            transform.position += new Vector3(0.2f + big * 0.3f, 0, 0);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
