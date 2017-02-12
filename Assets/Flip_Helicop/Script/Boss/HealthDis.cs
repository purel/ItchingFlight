using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthDis : MonoBehaviour {

    public GameObject Boss;
    public Image HealthFill;

    // different sprites have different colors notifying different stages
    public Sprite HealthFill1;
    public Sprite HealthFill2;
    public Sprite HealthFill3;
    public Sprite HealthFill4;

    public Text StageText;
    public bool BossDie = false;

    float MaxHealth;
    float Ratio = 1f;
    float MaxRatio = 0.98f;
    float YScale = 0.88f;
    public int Stage = 1;

    Vector3 RatioV = Vector3.zero; // this vector controls the fill of the health bar on UI

	// Use this for initialization
	void Start ()
    {
        RatioV.y = YScale;
        MaxHealth = Boss.GetComponent<Boss>().Health;
    }

    // Update is called once per frame
    void Update()
    {
        // if the Boss is alive, according to its current health, inform the Boss to transit to a different stage
        // Stage 1: 80% - 100%
        // Stage 2: 60% - 80%
        // Stage 3: 40% - 60%
        // Stage 4: 20% - 40%
        // Stage 5: 0% - 20%
        if (!BossDie)
        {
            Ratio = Boss.GetComponent<Boss>().Health / MaxHealth;
            RatioV.x = Ratio * MaxRatio;
            HealthFill.transform.localScale = RatioV;
            if (Stage < 2 && 120 < Boss.GetComponent<Boss>().Health && Boss.GetComponent<Boss>().Health <= 160)
            {
                Stage = 2;
                Boss.GetComponent<Boss>().Stage = Stage;
                StageText.text = "Stage : " + Stage;
                StageText.gameObject.SetActive(true);
                Boss.GetComponent<Boss>().StageTransiting = true;
                HealthFill.sprite = HealthFill1;
            }
            else if (Stage < 3 && 80 <= Boss.GetComponent<Boss>().Health && Boss.GetComponent<Boss>().Health <= 120)
            {
                Stage = 3;
                Boss.GetComponent<Boss>().Stage = Stage;
                StageText.text = "Stage : " + Stage;
                StageText.gameObject.SetActive(true);
                Boss.GetComponent<Boss>().StageTransiting = true;
                HealthFill.sprite = HealthFill2;
            }
            else if (Stage < 4 && 40 <= Boss.GetComponent<Boss>().Health && Boss.GetComponent<Boss>().Health < 80)
            {
                Stage = 4;
                Boss.GetComponent<Boss>().Stage = Stage;
                StageText.text = "Stage : " + Stage;
                StageText.gameObject.SetActive(true);
                Boss.GetComponent<Boss>().StageTransiting = true;
                HealthFill.sprite = HealthFill3;
            }
            else if (Stage < 5 && Boss.GetComponent<Boss>().Health < 40)
            {
                Stage = 5;
                Boss.GetComponent<Boss>().Stage = Stage;
                StageText.text = "Stage : " + Stage;
                StageText.gameObject.SetActive(true);
                Boss.GetComponent<Boss>().StageTransiting = true;

                HealthFill.sprite = HealthFill4;
            }
            if (Boss.GetComponent<Boss>().Health < 0)
                Boss.GetComponent<Boss>().Health = 0;
        }
    }
}
