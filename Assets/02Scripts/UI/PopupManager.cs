using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.CompilerServices;

public class PopupManager : MonoSingleton<PopupManager>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
    }

    #region _Screen Components_
    [Header("Screen Components")]

    [SerializeField]
    private RawImage screen;
    [SerializeField]
    private RawImage minimap;
    #endregion
    #region _Ingame Components_
    [Header("Ingame Components")]

    [SerializeField]
    private PopupBase ingamePopup;

    [Space(5)]
    [SerializeField]
    private Image JewelyGetMark;
    [SerializeField]
    private TextMeshProUGUI timer;
    [SerializeField]
    private TextMeshProUGUI jumpCounter;
    #endregion
    #region _Gameover Components_
    [Header("Gameover Components")]

    [SerializeField]
    private PopupBase gameoverPopup;

    [Space(5)]
    [SerializeField]
    private TextMeshProUGUI causeOfDeath;
    [SerializeField]
    private TextMeshProUGUI explain;
    #endregion
    #region _GameClear Components_
    [Header("GameClear Components")]

    [SerializeField]
    private PopupBase gameClearPopup;

    [SerializeField]
    private TextMeshProUGUI jumpLimit;
    [SerializeField]
    private TextMeshProUGUI timeLimit;

    [Space(5)]
    [SerializeField]
    private Image JewelyCheck;
    [SerializeField]
    private Image jumpCheck;
    [SerializeField]
    private Image timeCheck;

    [Space(5)]
    [SerializeField]
    private Image JewelyStar;
    [SerializeField]
    private Image jumpStar;
    [SerializeField]
    private Image timeStar;
    #endregion
    #region _Pause Components_
    [Header("Pause Components")]

    [SerializeField]
    private PopupBase pausePopup;
    #endregion
    #region _Setting Components_
    [Header("Setting Components")]

    [SerializeField]
    private PopupBase settingPopup;
    #endregion


    protected override void Awake()
    {
        base.Awake();

        GetComponent<Canvas>().worldCamera = Camera.main;
        AllPopupClose();

        SceneManager.activeSceneChanged += SceneChanged;
    }

    private void AllPopupClose()
    {
        ingamePopup.PopupHide();

        gameoverPopup.PopupHide();
        gameClearPopup.PopupHide();

        pausePopup.PopupHide();
        settingPopup.PopupHide();
    }
    private void SceneChanged(Scene replacedScene, Scene newScene)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        AllPopupClose();

        if (newScene.buildIndex >= (int)SCENE.StageSelect) // 스테이지 선택창 or 스테이지일 경우(스테이지 선택창도 플레이 화면이기 때문에 살짝 분리)
        {
            Debug.Log("플레이 UI 오픈");
            ingamePopup.PopupOpen();
            if (newScene.buildIndex > (int)SCENE.StageSelect) // 스테이지일 경우
            {
                Debug.Log("타이머 시작");
                jumpCounter.text = "0";
                StartCoroutine("TimerStart");
            }
            else // 스테이지 선택창일 경우
            {
                timer.text = "-- : --";
                jumpCounter.text = "-";
            }
        }
    }

    private WaitForSeconds Wait1Sec = new WaitForSeconds(1f);
    #region _About Ingame UI_
    private int time = 0;
    public int Time
    {
        get => time;
        private set
        {
            time = value;
            timer.text = (time / 60).ToString("D2") + " : " + (time % 60).ToString("D2");
            if (time >= 5999) // 시간이 99분 59초 이상 흘렀을 경우
            {

            }
        }
    }

    private int jumpCount = 0;
    public int JumpCount
    {
        get => jumpCount;
        set
        {
            jumpCount = value;
            jumpCounter.text = jumpCount.ToString();
            if (jumpCount >= 999) // 점프 횟수가 999회 이상이 되었을 경우
            {

            }
        }
    }
    private IEnumerator TimerStart()
    {
        jumpCount = time = 0;
        do
        {
            Time++;

            yield return Wait1Sec;
        }
        while (GameManager.Inst.GameStatus == GameStatus.Play); // 플레이 중인 동안 반복
    }
    #endregion

    #region _About Game Over_
    public void GameOverBy(Obstacle obstacle)
    {

    }
    #endregion
}
