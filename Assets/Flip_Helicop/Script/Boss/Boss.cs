using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Boss : MonoBehaviour {

    public GameObject Player;
    public GameObject ThunderBall;
    public GameObject Rasengan;
    public GameObject MagicCircle;
    public GameObject Wraith;
    public GameObject Sharingan;
    public GameObject TeleportGate;
    public GameObject Exploder;
    public GameObject HealthBar;
    public GameObject Win = null; // a boolean that decides the fixation of the camera
    public GameObject WinImg = null; // a visual hint appeared when boss is defeated
    public GameObject GoldTip; // '+2000 Gold' text
    public GameObject Laser;

    public AudioClip TeleSound;
    public AudioClip SummonSound;
    public AudioClip ThunderBallSound;
    public AudioClip SharinganSound;

    public int Health = 200;
    public int Stage; // 5 stages in total, each stage skills the Boss will use are different, a higher stage means a harder combat
    public bool Die;

    float YMin = -7;
    float YMax =5;
    float XMin;
    float XMax;
    float MoveSpeed = 3f;

    bool Moving = true; // a boolean to control the Boss's movement, 0 stands still, 1 moves
    bool up = false;
    bool upr = false;
    bool down = false;
    bool downr = false;
    bool forward = false;
    bool Reverse = false;
    bool Casting = false;
    bool IsTransitting = false; // a boolean that specifies whether the boss is transiting stages, it is used to ensure that the transition is only called once
    bool BeginMoving = false; // a boolean that starts the transition
    public bool Begin = false; // a boolean that specifies whether the combat has started
    public bool Teleporting = false;
    public bool StageTransiting = false;

    // the weight of a skill is used when to decide which skill shoule be used next, weights are dynamically changed with an algorithm
    float A1Weight = 1f;
    float A2Weight = 1f;
    float TeleportWeight = 8f;
    float SummonWeight = 9f;
    float AmaterasuWeight = 10f;

    // the backswing of a skill specifies the time a skill requires to perform right before it starts, it is import for synchronization
    float A1BackSwing = 4;
    float A2BackSwing = 3;
    float TeleportBackSwing = 7.5f;
    float SummonBackSwing = 3;
    float AmaterasuBackSwing = 8;

    Vector3 ThunderBallDis = new Vector3(-2f, 0.15f, 0);
    Vector3 RasenganDis = new Vector3(-4f, 0.4f, 0);
    Vector3 SummonPos = Vector3.zero;

    Camera MainCamera;
    Animator Anim;

    // Use this for initialization
    void Start () {
        Anim = gameObject.GetComponent<Animator>();
        Stage = 1;
        MainCamera = Camera.main;
        transform.position = new Vector3(20, 0, 0);
    }

    // Update is called once per frame
    void Update () {
        // if the health of the Boss reaches 0
        if (Health <= 0)
        {
            MainCamera.gameObject.GetComponent<CameraBoss>().Win = true;
            Player.GetComponent<helicopmovementBoss>().XMin = -34f;
            Player.GetComponent<helicopmovementBoss>().XMax = 34f;
            Laser.GetComponent<LaserBoss>().LaserOff = true;
            WinImg.SetActive(true);
            Win.SetActive(true);
            GoldTip.SetActive(true);
            HealthBar.GetComponent<HealthDis>().BossDie = true;
            Exploder.transform.position = transform.position;
            Exploder.GetComponent<Exploder>().BeginExplosion = true;
            Destroy(this.gameObject);
        }
        // if the fight starts, camera will be fixed
        if(Begin)
        {
            Begin = false;
            BeginMoving = true;
            XMin = MainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).x + 1f;
            XMax = MainCamera.ScreenToWorldPoint(new Vector3(MainCamera.pixelWidth, 0, 0)).x - 1f;
            StartCoroutine(Move());
            StartCoroutine(Stage1());
        }
    }
    void FixedUpdate()
    {
        // according to the outcome of a random pick, move the Boss
        #region Move
        if (!Die && BeginMoving)
        {
            Vector3 Pos = Vector3.zero;
            Pos = transform.position;

            if (up == true && Moving)
                Pos += Vector3.up * Time.deltaTime;
            else if (upr == true && Moving)
            {
                Pos += Vector3.up * Time.deltaTime * MoveSpeed;
                Pos += Vector3.right * Time.deltaTime * MoveSpeed;
            }
            else if (down == true && Moving)
                Pos += Vector3.down * Time.deltaTime * MoveSpeed;
            else if (downr == true && Moving)
            {
                Pos += Vector3.down * Time.deltaTime * MoveSpeed;
                Pos += Vector3.right * Time.deltaTime * MoveSpeed;
            }
            if (forward == true && Moving)
                Pos += Vector3.left * Time.deltaTime * MoveSpeed;
            if (!Teleporting)
                Pos.y = Mathf.Clamp(Pos.y, YMin, YMax);
            Pos.x = Mathf.Clamp(Pos.x, XMin, XMax);
            transform.position = Pos;
        }
        #endregion
        // always faces to the player
        #region Turn
        if (!Reverse && transform.position.x < Player.transform.position.x && !Die)
        {
            Vector3 temp = new Vector3(-2.5f, 2.5f, 0);
            transform.localScale = temp;
            Reverse = true;
        }
        if (Reverse && transform.position.x > Player.transform.position.x && !Die)
        {
            Vector3 temp = new Vector3(2.5f, 2.5f, 0);
            transform.localScale = temp;
            Reverse = false;
        }
        #endregion
        // start to transit stages
        #region Stage
        if (StageTransiting && !IsTransitting)
            StartCoroutine(Transit());
        #endregion
    }
    // a coroutine to pick a random direction every second
    IEnumerator Move()
    {
        while (Moving && !Die)
        {
            yield return new WaitForSeconds(1);
            up = upr = down = downr = forward = false;
            float temp = Random.value;
            if (temp < 0.25f)
                up = true;
            else if (0.25f <= temp && temp < 0.5f)
                upr = true;
            else if (0.5f <= temp && temp < 0.75f)
                down = true;
            else if(temp >= 0.75f)
                downr = true;
            if (Random.value < 0.3f)
                forward = true;
        }
        up = upr = down = downr = forward = false;
        yield break;
        
    }
    // a coroutine that performes thunderball attack
    IEnumerator Attack1()
    {
        Anim.Play("BossAttack"); // thunderball is generated using a animation event which makes the action more coherent, this rule applies to all actions
        yield return new WaitForSeconds(1.5f);
        Anim.Play("BossAttack2");
    }
    // a coroutine that performes Rasengan attack
    IEnumerator Attack2()
    {
        Moving = false; // when casting Rasengan, stop moving
        Anim.Play("BossCastingSpellLaunch");
        yield return new WaitForSeconds(1.5f);
        Anim.Play("BossIdle");
        Moving = true; // recover movement
        StartCoroutine(Move());
    }
    // a coroutine that performes summon
    IEnumerator Summon()
    {
        Moving = false; // when casting summoning, stop moving 
        yield return new WaitForSeconds(1f);
        AudioSource.PlayClipAtPoint(SummonSound, transform.position);
        Anim.Play("BossSummonSpell");
        Moving = true; // recover moving
        StartCoroutine(Move());
        SummonPos.x = Random.Range(1,13);
        SummonPos.y = Random.Range(-7,5);
        yield return new WaitForSeconds(0.4f);
        GameObject temp1 = Instantiate(MagicCircle,SummonPos, Quaternion.identity) as GameObject;
        GameObject temp2 = Instantiate(Wraith,SummonPos, Quaternion.identity) as GameObject;
        temp1.SetActive(true);
        temp2.SetActive(true);
        yield break;
    }
    // a coroutine that cast Amaterasu
    IEnumerator Amaterasu()
    {
        Moving = false; // when casting Amaterasu, stop moving
        Anim.Play("BossCastingSpell");
        for (int i = 0; i < 3; i++)
        {
            GameObject temp = Instantiate(Sharingan, Player.transform.position, Quaternion.identity) as GameObject;
            temp.SetActive(true);
            AudioSource.PlayClipAtPoint(SharinganSound, temp.transform.position);
            yield return new WaitForSeconds(2);
        }
        Anim.Play("BossIdle");
        Moving = true; // recover movement
        StartCoroutine(Move());
        yield return 0;
    }
    // a coroutine that cast teleport with attacks
    IEnumerator Teleport()
    {
        yield return new WaitForSeconds(0.5f);
        Teleporting = true;
        AudioSource.PlayClipAtPoint(TeleSound, transform.position);
        GameObject temp = Instantiate(TeleportGate, transform.position, Quaternion.identity) as GameObject;
        transform.position = new Vector3(transform.position.x, 50f, 0); // put the Boss outside the screen
        temp.SetActive(true);
        temp.GetComponent<Teleport>().Back = false;
        yield return new WaitForSeconds(2);
        Teleporting = false;
        /* after teleport perform either thunderball attack or Rasengan attack */
        if (Random.value<0.5f)
            StartCoroutine(Attack1());
        else
            StartCoroutine(Attack2());
        yield return new WaitForSeconds(3);
        Teleporting = true;
        AudioSource.PlayClipAtPoint(TeleSound, transform.position);
        GameObject temp1 = Instantiate(TeleportGate, transform.position, Quaternion.identity) as GameObject;
        transform.position = new Vector3(0, 13f, 0);
        temp1.SetActive(true);
        temp1.GetComponent<Teleport>().Back = true;
        yield return new WaitForSeconds(2);
        Teleporting = false;
        yield return 0;
    }
    // Stage coroutines are critical for synchronization
    // 'Casting', 'StageTransiting' are synchronization mutex that helps to facilitate the stage transitions
    IEnumerator Stage1()
    {
        while (true)
        {
            Casting = true;
            StartCoroutine(Attack1());
            yield return new WaitForSeconds(A1BackSwing);
            Casting = false;
            if (StageTransiting)
                yield break;
        }
    }
    IEnumerator Stage2()
    {
        int Skill = 0;
        while (true)
        {
            Casting = true;
            Skill=PickSkill(2);
            switch (Skill)
            {
                case 1:
                    StartCoroutine(Attack1());
                    yield return new WaitForSeconds(A1BackSwing);
                    break;
                case 2:
                    StartCoroutine(Attack2());
                    yield return new WaitForSeconds(A2BackSwing);
                    break;
            }
            Casting = false;
            if (StageTransiting)
                yield break;
        }
    }
    IEnumerator Stage3()
    {
        int Skill = 0;
        while (true)
        {
            Casting = true;
            Skill = PickSkill(3);
            switch (Skill)
            {
                case 1:
                    StartCoroutine(Attack1());
                    yield return new WaitForSeconds(A1BackSwing);
                    break;
                case 2:
                    StartCoroutine(Attack2());
                    yield return new WaitForSeconds(A2BackSwing);
                    break;
                case 3:
                    StartCoroutine(Teleport());
                    yield return new WaitForSeconds(TeleportBackSwing);
                    break;
            }
            Casting = false;
            if (StageTransiting)
                yield break;
        }
    }
    IEnumerator Stage4()
    {
        int Skill = 0;
        while (true)
        {
            Casting = true;
            Skill = PickSkill(4);
            switch (Skill)
            {
                case 1:
                    StartCoroutine(Attack1());
                    yield return new WaitForSeconds(A1BackSwing);
                    break;
                case 2:
                    StartCoroutine(Attack2());
                    yield return new WaitForSeconds(A2BackSwing);
                    break;
                case 3:
                    StartCoroutine(Teleport());
                    yield return new WaitForSeconds(TeleportBackSwing);
                    break;
                case 4:
                    StartCoroutine(Summon());
                    yield return new WaitForSeconds(SummonBackSwing);
                    break;
            }
            Casting = false;
            if (StageTransiting)
                yield break;
        }
    }
    IEnumerator Stage5()
    {
        int Skill = 0;
        while (true)
        {
            Casting = true;
            Skill = PickSkill(5);
            switch (Skill)
            {
                case 1:
                    StartCoroutine(Attack1());
                    yield return new WaitForSeconds(A1BackSwing);
                    break;
                case 2:
                    StartCoroutine(Attack2());
                    yield return new WaitForSeconds(A2BackSwing);
                    break;
                case 3:
                    StartCoroutine(Teleport());
                    yield return new WaitForSeconds(TeleportBackSwing);
                    break;
                case 4:
                    StartCoroutine(Summon());
                    yield return new WaitForSeconds(SummonBackSwing);
                    break;
                case 5:
                    StartCoroutine(Amaterasu());
                    yield return new WaitForSeconds(AmaterasuBackSwing);
                    break;
            }
            Casting = false;
            if (StageTransiting)
                yield break;
        }
    }
    // Transit coroutine let Boss transit to a different stage
    IEnumerator Transit()
    {
        switch (Stage)
        {
            case 2:
                IsTransitting = true;
                yield return new WaitUntil(() => !Casting); // a Lambda expression that will wait until 'Casting' is not true
                StageTransiting = false;
                ResetWeight();
                StartCoroutine(Stage2());
                IsTransitting = false;
                yield break;
            case 3:
                IsTransitting = true;
                yield return new WaitUntil(() => !Casting);
                StageTransiting = false;
                ResetWeight();
                StartCoroutine(Stage3());
                IsTransitting = false;
                yield break;
            case 4:
                IsTransitting = true;
                yield return new WaitUntil(() => !Casting);
                StageTransiting = false;
                ResetWeight();
                StartCoroutine(Stage4());
                IsTransitting = false;
                yield break;
            case 5:
                IsTransitting = true;
                yield return new WaitUntil(() => !Casting);
                StageTransiting = false;
                ResetWeight();
                StartCoroutine(Stage5());
                IsTransitting = false;
                yield break;
        }

    }
    // a function that picks next skill
    int PickSkill (int Stage)
    {
        int Skill = 0;
        float prob1 = 0f;
        float prob2 = 0f;
        float prob3 = 0f;
        float prob4 = 0f;
        float Dis = Random.value;
        // according to stage, pick skill
        // the probability of a skill is its weight divided by the sum of all skills' weights
        switch (Stage)
        {
            case 2:
                prob1 = A1Weight / (A1Weight + A2Weight); 
                if (Dis <= prob1)
                {
                    Skill = 1;
                    A2Weight++;
                }
                else
                {
                    Skill = 2;
                    A1Weight++;
                }
                break;
            case 3:
                prob1 = A1Weight / (A1Weight + A2Weight + TeleportWeight);
                prob2 = A2Weight / (A1Weight + A2Weight + TeleportWeight);
                if (Dis <= prob1)
                {
                    Skill = 1;
                    TeleportWeight++;
                }
                else if (prob1 < Dis && Dis <= prob1 + prob2)
                {
                    Skill = 2;
                    TeleportWeight++;
                }
                else
                {
                    Skill = 3;
                    TeleportWeight =1;
                }
                break;
            case 4:
                TeleportWeight = 4;
                prob1 = A1Weight / (A1Weight + A2Weight + TeleportWeight + SummonWeight);
                prob2 = A2Weight / (A1Weight + A2Weight + TeleportWeight + SummonWeight);
                prob3 = TeleportWeight / (A1Weight + A2Weight + TeleportWeight + SummonWeight);
                if (Dis <= prob1)
                {
                    Skill = 1;
                    TeleportWeight++;
                    SummonWeight++;
                }
                else if (prob1 < Dis && Dis <= prob1 + prob2)
                {
                    Skill = 2;
                    TeleportWeight++;
                    SummonWeight++;
                }
                else if (prob1 + prob2 < Dis && Dis <= prob1 + prob2 + prob3)
                {
                    Skill = 3;
                    TeleportWeight = 1;
                }
                else
                {
                    Skill = 4;
                    SummonWeight = 1;
                }
                break;
            case 5:
                TeleportWeight = 4;
                SummonWeight = 4;
                prob1 = A1Weight / (A1Weight + A2Weight + TeleportWeight + SummonWeight + AmaterasuWeight);
                prob2 = A2Weight / (A1Weight + A2Weight + TeleportWeight + SummonWeight + AmaterasuWeight);
                prob3 = TeleportWeight / (A1Weight + A2Weight + TeleportWeight + SummonWeight + AmaterasuWeight);
                prob4 = SummonWeight / (A1Weight + A2Weight + TeleportWeight + SummonWeight+ AmaterasuWeight);
                if (Dis <= prob1)
                {
                    Skill = 1;
                    TeleportWeight++;
                    SummonWeight++;
                    AmaterasuWeight++;
                }
                else if (prob1 < Dis && Dis <= prob1 + prob2)
                {
                    Skill = 2;
                    TeleportWeight++;
                    SummonWeight++;
                    AmaterasuWeight++;
                }
                else if (prob1 + prob2 < Dis && Dis <= prob1 + prob2 + prob3)
                {
                    Skill = 3;
                    TeleportWeight = 1;
                }
                else if(prob1 + prob2 + prob3 < Dis && Dis <= prob1 + prob2 + prob3 + prob4)
                {
                    Skill = 4;
                    SummonWeight = 1;
                }
                else
                {
                    Skill = 5;
                    AmaterasuWeight = 1;
                }
                break;
        }
                return Skill;
    }
    // reset weight so that new skills have a higher chance to be chosen
    void ResetWeight()
    {
        A1Weight = 1;
        A2Weight = 1;
        TeleportWeight = 8;
        SummonWeight = 9;
        AmaterasuWeight = 10;
    }
    // Generate functions are all used as animation events to make actions of the boss more fluent and reasonable
    void GenerateThunderBall()
    {
        if (Reverse)
        {
            GameObject temp = Instantiate(ThunderBall, transform.position - ThunderBallDis, Quaternion.identity) as GameObject;
            temp.SetActive(true);
            AudioSource.PlayClipAtPoint(ThunderBallSound, transform.position);
        }
        else
        {
            GameObject temp = Instantiate(ThunderBall, transform.position + ThunderBallDis, Quaternion.identity) as GameObject;
            temp.SetActive(true);
            AudioSource.PlayClipAtPoint(ThunderBallSound, transform.position);
        }
    }
    void GenerateRasengan()
    {
        if (Reverse)
        {
            GameObject temp = Instantiate(Rasengan, transform.position - RasenganDis, Quaternion.identity) as GameObject;
            temp.transform.localScale = new Vector3(3, 3, 0);
            temp.SetActive(true);
        }
        else
        {
            GameObject temp = Instantiate(Rasengan, transform.position + RasenganDis, Quaternion.identity) as GameObject;
            temp.SetActive(true);
        }
        
    }
    // decrease health when hit by projectiles
    #region Hurt
    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("missile"))
        {
            StartCoroutine(GetHurt());
            if (collider.name == "missile(Clone)")
                Health --;
            else
                Health -= 2;
        }
    }

    // when get hurt, the Boss turns red
    IEnumerator GetHurt()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(0.1f);
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
    #endregion
}
