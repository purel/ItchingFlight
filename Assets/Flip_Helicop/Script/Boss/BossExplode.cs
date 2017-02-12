using UnityEngine;
using System.Collections;

public class BossExplode : MonoBehaviour {

	void Disappear ()
    {
        Destroy(this.gameObject);
    }
}
