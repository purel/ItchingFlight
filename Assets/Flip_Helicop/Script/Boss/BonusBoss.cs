using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class BonusBoss : MonoBehaviour {

    Vector3 FirePos;

    // values that define the range of the area that bonus will be spawned
    float YMax = 4.3f;
    float YMin = -7f;
    float XMin = 0;
    float XMax = 15f;

    public bool Begin = false;
	// Use this for initialization
	void Start ()
    {
        
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
	    if(Begin)
        {
            Begin = false;
            StartCoroutine(GenerateFire());
        }
	}
    IEnumerator GenerateFire()
    {
        while (true)
        {
            float time = Random.Range(15f, 30f);
            FirePos = new Vector3(Random.Range(XMin, XMax), Random.Range(YMin, YMax), 0);
            yield return new WaitForSeconds(time);
            int index = Random.Range(0, 4);
            GameObject temp = Instantiate(transform.GetChild(index).gameObject, FirePos, Quaternion.identity) as GameObject;
            temp.SetActive(true);
        }
    }
}
