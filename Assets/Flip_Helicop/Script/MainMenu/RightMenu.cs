using UnityEngine;
using System.Collections;

public class RightMenu : MonoBehaviour {

    public CanvasGroup CurrentTab = null;
    public CanvasGroup DestTab = null;

    void Set()
    {
        if (CurrentTab != null && DestTab != null)
        {
            CurrentTab.gameObject.SetActive(false);
            DestTab.gameObject.SetActive(true);
            CurrentTab = DestTab;
        }
    }
}
