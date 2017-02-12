using UnityEngine;
using System.Collections;

public class GoldTip : MonoBehaviour {

    // animation event
	void Disappear()
    {
        Destroy(this.gameObject);
    }
}
