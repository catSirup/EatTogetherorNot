using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum LEVEL { LEVEL1, LEVEL2, LEVEL3 }

public class PlayScene : Scene
{
    #region UI
    [SerializeField]
    private GameObject playUI;
    [SerializeField]
    private GameObject menuWindow;
    [SerializeField]
    private GameObject continueWindow;
    #endregion

    public GameObject effct;

    public LEVEL curLevel;
    [SerializeField]
    private Transform[] leftSides;
    [SerializeField]
    private Transform[] rightSides;

    [SerializeField]
    private Food[] foods;
    [SerializeField]
    private List<Food> leftFoods;
    [SerializeField]
    private List<Food> rightFoods;

    [SerializeField]
    private GameObject chooseFrame;

    private bool b_CanMove;
    private float curTime;
    /// <summary>
    /// Drag
    /// </summary>
    private bool b_Touchdown;
    private Vector2 touch_Offset;
    private Vector2 originVec;
    [SerializeField]
    private float correction_DragLength;
    private bool b_Changing;
    private GameObject touchObj;

    [SerializeField]
    private GameObject gauge;
    private bool b_StartGauge;
    private float GaugeTime;
    private bool b_GameOver;

    private float time;

    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Text timerText;
    [SerializeField]
    private Animator pilot;

    [SerializeField]
    private GameObject GameStart;
    [SerializeField]
    private GameObject GameOver;
    [SerializeField]
    private GameObject LevelUp;

    [SerializeField]
    private Text windowScoreText;
    [SerializeField]
    private Text windowTimeText;

    public override void Initialize()
    {
        GameStart.SetActive(false);
        GameOver.SetActive(false);
        LevelUp.SetActive(false);

        playUI.SetActive(true);
        chooseFrame.SetActive(false);
        GaugeTime = 0;
        curTime = 0;
        StartCoroutine(SetFoods());

        b_Touchdown = false;
        b_Changing = false;
        b_StartGauge = false;
        b_CanMove = false;
        gauge.transform.localScale = Vector3.one;
        time = 0;
        SoundManager.soundMgr.audioSource.Stop();
        SoundManager.soundMgr.PlayES("Intro");
        DataManager.curScore = 0;

        pilot.SetBool("GameOver", false);
        b_GameOver = false;

        StartCoroutine(GameStartAnim());

        curLevel = LEVEL.LEVEL1;
    }

    public override void Updated()
    {

        if (!b_GameOver)
        {
            #region Click Block
            if (b_CanMove)
            {
                Ray2D ray = new Ray2D(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null && Input.GetMouseButtonDown(0))
                {
                    if (hit.collider.CompareTag("LeftFood") || hit.collider.CompareTag("RightFood"))
                    {
                        chooseFrame.SetActive(true);
                        b_Touchdown = true;
                        originVec = ray.origin;
                        touchObj = hit.collider.gameObject;
                        chooseFrame.transform.position = hit.collider.transform.position;
                        chooseFrame.transform.localScale = hit.collider.transform.localScale;
                        chooseFrame.GetComponent<SpriteRenderer>().sortingOrder = hit.collider.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
                    }

                    //StartCoroutine(EffectAnim(ray.origin));
                }

                if (touchObj != null)
                    Drag(ref ray, originVec, touchObj.GetComponent<Food>());

                curTime += Time.deltaTime;

                if (curTime > 3.0f)
                {
                    StartCoroutine(DeleteFrontFood());
                    curTime = 0;
                }
            }

            else
            {
                chooseFrame.SetActive(false);
            }
            #endregion
            if (b_StartGauge)
            {
                GaugeTime += Time.deltaTime;
                time += Time.deltaTime;
            }

            #region GameOver
            if (GaugeTime > 50)
            {
                DataManager.Time = time;
                SoundManager.soundMgr.audioSource.Stop();
                SoundManager.soundMgr.PlayES("GameOver");
                GameOver.SetActive(true);
                pilot.SetBool("GameOver", true);
                continueWindow.SetActive(true);
                GameManager.gameMgr.SaveData();
                windowScoreText.text = scoreText.text;
                windowTimeText.text = timerText.text;
                b_GameOver = true;
            }
            #endregion

            if(time > 29.98f && time <= 30)
            {
                StartCoroutine(LevelUPAnim());
            }

            else if (time > 30 && time < 60)
            {
                curLevel = LEVEL.LEVEL2;
            }

            else if(time >= 60 && time <= 60.02f)
            {
                StartCoroutine(LevelUPAnim());
            }

            else if (time > 60.03f && time <= 90)
            {
                curLevel = LEVEL.LEVEL3;
            }

            //if (time > 90 && time <= 120)
            //{
            //    foodRange = 6;
            //}

            if (GaugeTime < 0)
                GaugeTime = 0;

            gauge.transform.localScale = new Vector3(1 - 0.02f * GaugeTime, 1, 1);
            if (gauge.transform.localScale.x < 0)
                gauge.transform.localScale = new Vector3(0, 1, 1);

            scoreText.text = DataManager.curScore.ToString();
            timerText.text = ((int)(time / 60)).ToString("00") + ":" + ((int)(time % 60)).ToString("00");
        }
    }

    public override void Exit()
    {
        playUI.SetActive(false);

        for (int i = 0; i < leftFoods.Count; i++)
            Destroy(leftFoods[i].gameObject);

        for (int i = 0; i < rightFoods.Count; i++)
            Destroy(rightFoods[i].gameObject);

        leftFoods.Clear();
        rightFoods.Clear();

        continueWindow.SetActive(false);
    }

    #region Buttons
    public void Button_Menu()
    {
        menuWindow.SetActive(true);
        SoundManager.soundMgr.PlayES("Click");
        Time.timeScale = 0;
    }
    #endregion

    #region Personal Function
    private IEnumerator EffectAnim(Vector2 pos)
    {
        Debug.Log("A");
        GameObject clone = Instantiate(effct);
        clone.transform.position = pos;
        yield return new WaitForSeconds(0.1f);
        Destroy(clone);
    }
    private IEnumerator GameStartAnim()
    {
        GameStart.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        GameStart.SetActive(false);
    }
    private IEnumerator LevelUPAnim()
    {
        SoundManager.soundMgr.audioSource.volume = 0;
        SoundManager.soundMgr.PlayES("LevelUp");
        LevelUp.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        SoundManager.soundMgr.audioSource.volume = 1;
        LevelUp.SetActive(false);
    }
    private IEnumerator SetFoods()
    {
        float time = 0;

        for (int i = 0; i < 16; i++)
        {
            int rand = Random.Range(0, 3);

            Food clone = GameObject.Instantiate(foods[rand]);
            if (i % 2 == 0)
            {
                clone.transform.position = new Vector3(0.3f, 7.8f);
                clone.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                clone.name = foods[rand].name;
                clone.tag = "LeftFood";
                leftFoods.Add(clone);
            }

            else
            {
                clone.transform.position = new Vector3(-0.3f, 7.8f);
                clone.transform.localScale = new Vector3(-0.4f, 0.4f, 1);
                clone.name = foods[rand].name;
                clone.tag = "RightFood";
                rightFoods.Add(clone);
            }

            clone.GetComponent<SpriteRenderer>().sortingOrder = 20 - i;
        }

        while (true)
        {
            int temp = 0;
            time += Time.deltaTime * 4;

            for (temp = 0; temp < leftFoods.Count; temp++)
            {
                leftFoods[temp].transform.position = Vector3.Lerp(new Vector2(-0.15f, 7.8f), leftSides[temp].position, time - temp);
                leftFoods[temp].transform.localScale = Vector3.Lerp(new Vector3(0.4f, 0.5f, 1), leftSides[temp].localScale, time - temp);
                rightFoods[temp].transform.localScale = Vector3.Lerp(new Vector3(-0.4f, 0.5f, 1), rightSides[temp].localScale, time - temp);
                rightFoods[temp].transform.position = Vector3.Lerp(new Vector2(0.15f, 7.8f), rightSides[temp].position, time - temp);
            }

            if (time > temp)
            {
                b_CanMove = true;
                b_StartGauge = true;
                SoundManager.soundMgr.PlayBGM("GameBGM");
                yield break;
            }

            yield return new WaitForSeconds(0.02f);
        }

    }
    private void Drag(ref Ray2D ray, Vector2 originVec, Food obj)
    {
        if (b_Touchdown)
        {
            touch_Offset = ray.origin - originVec;

            if (touch_Offset.magnitude >= correction_DragLength && Input.GetMouseButtonUp(0))
            {
                // 좌 또는 우 드래그
                if (Mathf.Abs(touch_Offset.x) > Mathf.Abs(touch_Offset.y))
                {
                    if (touch_Offset.x >= 0)
                    {
                        // 오른쪽
                        if (originVec.x < 0 /*&& !obj.b_Touched*/)
                        {
                            StartCoroutine(SwapBlockLR(leftFoods, rightFoods, leftFoods.IndexOf(obj), leftFoods.IndexOf(obj)));
                        }
                    }
                    else
                    {
                        // 왼쪽
                        if (originVec.x > 0 /*&& !obj.b_Touched*/)
                        {
                            StartCoroutine(SwapBlockLR(rightFoods, leftFoods, rightFoods.IndexOf(obj), rightFoods.IndexOf(obj)));
                        }
                    }

                    b_Touchdown = false;
                }

                else if (Mathf.Abs(touch_Offset.x) < Mathf.Abs(touch_Offset.y))
                {
                    if (touch_Offset.y >= 0)
                    {
                        // 위쪽
                        if (originVec.y < 5.9f /*&& !obj.b_Touched*/)
                        {
                            if (originVec.x < 0 && leftFoods.IndexOf(obj) <= 6)
                                StartCoroutine(SwapBlockUD(leftFoods, leftFoods.IndexOf(obj), leftFoods.IndexOf(obj) + 1));
                            else if (originVec.x > 0 && rightFoods.IndexOf(obj) <= 6)
                                StartCoroutine(SwapBlockUD(rightFoods, rightFoods.IndexOf(obj), rightFoods.IndexOf(obj) + 1));
                        }
                    }
                    else
                    {
                        // 아래쪽
                        if (originVec.y > -0.87f /*&& !obj.b_Touched*/)
                        {
                            if (originVec.x < 0 && leftFoods.IndexOf(obj) >= 0)
                                StartCoroutine(SwapBlockUD(leftFoods, leftFoods.IndexOf(obj), leftFoods.IndexOf(obj) - 1));
                            else if (originVec.x > 0 && rightFoods.IndexOf(obj) >= 0)
                                StartCoroutine(SwapBlockUD(rightFoods, rightFoods.IndexOf(obj), rightFoods.IndexOf(obj) - 1));
                        }
                    }

                    b_Touchdown = false;
                }
            }

            else if (touch_Offset.magnitude < correction_DragLength && Input.GetMouseButtonUp(0))
            {
                // 터치

                b_Touchdown = false;
            }
        }
    }
    private IEnumerator SwapBlockLR(List<Food> curList, List<Food> targetList, int curIdx, int targetIdx)
    {
        float time = 0;
        Vector3 curOriginPos = curList[curIdx].transform.position;
        Vector3 targetPos = targetList[targetIdx].transform.position;

        Vector3 curOriginScale = curList[curIdx].transform.localScale;
        Vector3 targetScale = targetList[targetIdx].transform.localScale;

        Food curTempFood;

        b_Changing = true;

        while (b_Changing)
        {
            time += Time.deltaTime * 15;

            curList[curIdx].transform.position = Vector3.Lerp(curOriginPos, targetPos, time);
            curList[curIdx].transform.localScale = Vector3.Lerp(curOriginScale, targetScale, time);

            targetList[targetIdx].transform.position = Vector3.Lerp(targetPos, curOriginPos, time);
            targetList[targetIdx].transform.localScale = Vector3.Lerp(targetScale, curOriginScale, time);


            if (time > 1.0f)
            {
                curTempFood = curList[curIdx];
                curList.Remove(curList[curIdx]);

                curList.Insert(curIdx, targetList[targetIdx]);
                int targetOrder = targetList[targetIdx].GetComponent<SpriteRenderer>().sortingOrder;
                targetList.Remove(targetList[targetIdx]);
                targetList.Insert(targetIdx, curTempFood);

                //targetList[targetIdx].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                b_Changing = false;
            }
           
            yield return null;
        }
    }
    private IEnumerator SwapBlockUD(List<Food> curList, int curIdx, int targetIdx)
    {
        float time = 0;

        Vector3 curOriginPos = curList[curIdx].transform.position;
        Vector3 targetPos = curList[targetIdx].transform.position;

        Vector3 curOriginScale = curList[curIdx].transform.localScale;
        Vector3 targetScale = curList[targetIdx].transform.localScale;

        b_Changing = true;
        while (b_Changing)
        {
            time += Time.deltaTime * 15;

            curList[curIdx].transform.position = Vector3.Lerp(curOriginPos, targetPos, time);
            curList[curIdx].transform.localScale = Vector3.Lerp(curOriginScale, targetScale, time);

            curList[targetIdx].transform.position = Vector3.Lerp(targetPos, curOriginPos, time);
            curList[targetIdx].transform.localScale = Vector3.Lerp(targetScale, curOriginScale, time);

            if (time > 1.0f)
            {
                // 아래
                if (curIdx > targetIdx)
                {
                    Food tempFood = curList[targetIdx];
                    curList.Remove(curList[targetIdx]);
                    curList.Insert(curIdx, tempFood);

                    //curList[targetIdx].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                }
                // 위
                else
                {
                    Food tempFood = curList[curIdx];
                    curList.Remove(curList[curIdx]);
                    curList.Insert(curIdx + 1, tempFood);

                    //curList[targetIdx].GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1);
                }

                b_Changing = false;
            }

            yield return null;
        }
    }
    private IEnumerator DeleteFrontFood()
    {
        chooseFrame.SetActive(false);
        float time = 0;
        float speed = 0;
        b_CanMove = false;

        Vector2[] originPos_Left = new Vector2[20];
        Vector3[] originScale_Left = new Vector3[20];

        Vector2[] originPos_Right = new Vector2[20];
        Vector3[] originScale_Right = new Vector3[20];

        for (int i = 1; i < leftFoods.Count; i++)
        {
            originPos_Left[i] = leftFoods[i].transform.position;
            originScale_Left[i] = leftFoods[i].transform.localScale;
        }

        for (int i = 1; i < rightFoods.Count; i++)
        {
            originPos_Right[i] = rightFoods[i].transform.position;
            originScale_Right[i] = rightFoods[i].transform.localScale;
        }

        int randLeft = 0;
        int randRight = 0;

        switch (curLevel)
        {
            case LEVEL.LEVEL1:
                randLeft = Random.Range(0, 3);
                randRight = Random.Range(0, 3);
                speed = 2.0f;
                break;

            case LEVEL.LEVEL2:
                randLeft = Random.Range(3, 6);
                randRight = Random.Range(3, 6);
                speed = 4.0f;
                break;

            case LEVEL.LEVEL3:
                randLeft = Random.Range(6, 10);
                randRight = Random.Range(6, 10);
                speed = 6.0f;
                break;
        }

        Food leftObj = GameObject.Instantiate(foods[randLeft]);
        Food rightObj = GameObject.Instantiate(foods[randRight]);

        leftObj.transform.position = new Vector3(0.3f, 7.8f);
        leftObj.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        leftObj.name = foods[randLeft].name;
        leftObj.tag = "LeftFood";
        leftFoods.Add(leftObj);

        rightObj.transform.position = new Vector3(-0.3f, 7.8f);
        rightObj.transform.localScale = new Vector3(-0.4f, 0.4f, 1);
        rightObj.name = foods[randRight].name;
        rightObj.tag = "RightFood";
        rightFoods.Add(rightObj);

        while (true)
        {
            time += Time.deltaTime * speed;

            leftFoods[0].transform.position = Vector3.Lerp(leftSides[0].position, new Vector2(-0.15f, -2.26f), time);
            leftFoods[0].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - time);
            LeftSideFoodsMove(originPos_Left, originScale_Left, ref time);

            rightFoods[0].transform.position = Vector3.Lerp(rightSides[0].position, new Vector2(0.15f, -2.26f), time);
            rightFoods[0].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1 - time);
            RightSideFoodsMove(originPos_Right, originScale_Right, ref time);

            if (time > 2.5f)
            {
                SoundManager.soundMgr.PlayES("Normal");
                Food tempLeft = leftFoods[0];
                Food tempRight = rightFoods[0];

                DataManager.curScore += 200;
                CheckFoods(tempLeft.name, tempRight.name);

                if (DataManager.curScore < 0)
                    DataManager.curScore = 0;

                leftFoods.Remove(leftFoods[0]);
                Destroy(tempLeft.gameObject);

                rightFoods.Remove(rightFoods[0]);
                Destroy(tempRight.gameObject);

                b_CanMove = true;
                yield break;
            }


            yield return null;
        }
    }
    private void LeftSideFoodsMove(Vector2[] originPos_Left, Vector3[] originScale_Left, ref float time)
    {
        for (int temp = 1; temp < leftFoods.Count; temp++)
        {
            leftFoods[temp].transform.position = Vector2.Lerp(originPos_Left[temp], leftSides[temp - 1].position, time * 4 - temp);
            leftFoods[temp].transform.localScale = Vector3.Lerp(originScale_Left[temp], leftSides[temp - 1].localScale, time * 4 - temp);

            leftFoods[temp].GetComponent<SpriteRenderer>().sortingOrder = 20 - temp;
        }

    }
    private void RightSideFoodsMove(Vector2[] originPos_Right, Vector3[] originScale_Right, ref float time)
    {
        for (int temp = 1; temp < rightFoods.Count; temp++)
        {
            rightFoods[temp].transform.position = Vector2.Lerp(originPos_Right[temp], rightSides[temp - 1].position, time * 4 - temp);
            rightFoods[temp].transform.localScale = Vector3.Lerp(originScale_Right[temp], rightSides[temp - 1].localScale, time * 4 - temp);

            rightFoods[temp].GetComponent<SpriteRenderer>().sortingOrder = 20 - temp;
        }

        //}
        //private IEnumerator FillLeftSideBlock(int length)
        //{
        //    float time = 0;

        //    for (int i = 0; i < length; i++)
        //    {
        //        int rand = Random.Range(0, foods.Length);
        //        GameObject clone = GameObject.Instantiate(foods[rand]);
        //        clone.transform.position = new Vector3(0.3f, 7.8f);
        //        clone.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        //        clone.name = foods[rand].name;
        //        clone.tag = "LeftFood";
        //        leftFoods.Add(clone);
        //    }

        //    Vector2[] originPos_Left = new Vector2[20];
        //    Vector3[] originScale_Left = new Vector3[20];

        //    for (int i = 1; i < leftFoods.Count; i++)
        //    {
        //        originPos_Left[i] = leftFoods[i].transform.position;
        //        originScale_Left[i] = leftFoods[i].transform.localScale;
        //    }

        //    while (true)
        //    {
        //        time += Time.deltaTime * 1;
        //        LeftSideFoodsMove(originPos_Left, originScale_Left, ref time);
        //        yield return null;
        //        if (time > 1.0f)
        //        {
        //            yield break;
        //        }
        //    }
        //}
        //private IEnumerator FillRightSideBlock(int length)
        //{
        //    float time = 0;

        //    for (int i = 0; i < length; i++)
        //    {
        //        int rand = Random.Range(0, foods.Length);
        //        GameObject clone = GameObject.Instantiate(foods[rand]);
        //        clone.transform.position = new Vector3(-0.3f, 7.8f);
        //        clone.transform.localScale = new Vector3(-0.4f, 0.4f, 1);
        //        clone.name = foods[rand].name;
        //        clone.tag = "RightFood";
        //        leftFoods.Add(clone);
        //    }

        //    Vector2[] originPos_Right = new Vector2[20];
        //    Vector3[] originScale_Right = new Vector3[20];

        //    for (int i = 1; i < rightFoods.Count; i++)
        //    {
        //        originPos_Right[i] = rightFoods[i].transform.position;
        //        originScale_Right[i] = rightFoods[i].transform.localScale;
        //    }

        //    while (true)
        //    {
        //        time += Time.deltaTime * 1;
        //        RightSideFoodsMove(originPos_Right, originScale_Right, ref time);
        //        yield return null;
        //        if (time > 1.0f)
        //        {
        //            yield break;
        //        }
        //    }
        //}
        //#endregion
    }
    private void CheckFoods(string name1, string name2)
    {
        if((name1 == "콩" && name2 == "치즈") || (name1 == "치즈" && name2 == "콩"))
        {
            DataManager.curScore -= 500;
            SoundManager.soundMgr.PlayES("Fail");
            GaugeTime += 5;
        }

        if ((name1 == "콩" && name2 == "시금치") || (name1 == "시금치" && name2 == "콩"))
        {
            DataManager.curScore -= 500;
            SoundManager.soundMgr.PlayES("Fail");
            GaugeTime += 5;
        }

        if ((name1 == "미역" && name2 == "파") || (name1 == "파" && name2 == "미역"))
        {
            DataManager.curScore -= 500;
            SoundManager.soundMgr.PlayES("Fail");
            GaugeTime += 5;
        }

        if ((name1 == "우유" && name2 == "초코") || (name1 == "초코" && name2 == "우유"))
        {
            DataManager.curScore -= 500;
            SoundManager.soundMgr.PlayES("Fail");
            GaugeTime += 5;
        }

        if ((name1 == "콩" && name2 == "시금치") || (name1 == "시금치" && name2 == "콩"))
        {
            DataManager.curScore -= 500;
            SoundManager.soundMgr.PlayES("Fail");
            GaugeTime += 5;
        }


        if ((name1 == "콩" && name2 == "미역") || (name1 == "미역" && name2 == "콩"))
        {
            DataManager.curScore += 1000;
            SoundManager.soundMgr.PlayES("Suc");
            GaugeTime -= 2;
        }

        if ((name1 == "시금치" && name2 == "레몬") || (name1 == "레몬" && name2 == "시금치"))
        {
            DataManager.curScore += 1000;
            SoundManager.soundMgr.PlayES("Suc");
            GaugeTime -= 2;
        }

        if((name1 == "바나나" && name2 == "우유") || (name1 == "우유" && name2 == "바나나"))
        {
            DataManager.curScore += 1000;
            SoundManager.soundMgr.PlayES("Suc");
            GaugeTime -= 2;
        }


    }
    #endregion

}