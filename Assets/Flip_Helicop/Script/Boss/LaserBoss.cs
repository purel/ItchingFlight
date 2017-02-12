using UnityEngine;
using System.Collections;

public class LaserBoss : MonoBehaviour
{
    public GameObject Explode;
    public GameObject Boss;
    bool IsBurning = false; // a boolean that specifies whether the laser is hitting the boss
    public bool LaserOff = false;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Monster"))
        {
            GameObject ExplodeTemp = Instantiate(Explode, collider.transform.position, Quaternion.identity) as GameObject;
            ExplodeTemp.SetActive(true);
        }
        else if (collider.CompareTag("Boss"))
        {
            IsBurning = true;
            StartCoroutine(LaserBurn());
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.CompareTag("Boss"))
        IsBurning = false;
    }
    // when laser is hitting the boss, decrease the Boss's health continuously
    IEnumerator LaserBurn()
    {
        while (IsBurning)
        {
            StartCoroutine(GetHurt());
            if (LaserOff)
                yield break;
            Boss.GetComponent<Boss>().Health -= 1;
            yield return new WaitForSeconds(0.2f);
        }
    }
    // make the Boss turn red when laser is hitting continuously
    IEnumerator GetHurt()
    {
        if (LaserOff)
            yield break;
        Boss.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (LaserOff)
            yield break;
        Boss.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
