using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Exploder : MonoBehaviour {

    public bool BeginExplosion = false;
    public GameObject Explode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (BeginExplosion == true)
        {
            
            StartCoroutine(Explosion());
            BeginExplosion = false;
        }
	}
    // this coroutine generates random explosions after the boss is defeated
    IEnumerator Explosion()
    {

        GameObject temp1 = Instantiate(Explode, transform.position, Quaternion.identity) as GameObject;
        temp1.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        int number = Random.Range(6, 10);
        for (int i = 0; i < number; i++)
        {
            GameObject temp2 = Instantiate(Explode, transform.position + new Vector3(Random.Range(0.5f,1.8f), Random.Range(0.5f, 1.8f),0),Quaternion.identity) as GameObject;
            float scale = Random.Range(1f, 3f);
            temp2.transform.localScale = new Vector3(scale, scale, 0);
            temp2.SetActive(true);
            float time = Random.value;
            yield return new WaitForSeconds(time);
        }
        yield break;
    }
}
