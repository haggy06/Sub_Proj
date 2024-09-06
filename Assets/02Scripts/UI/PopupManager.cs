using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using TMPro;
using System.Runtime.CompilerServices;
using System;

public class PopupManager : Singleton<PopupManager>
{
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        StopCoroutine("GameClear");
        jewelryCheck.enabled = timeCheck.enabled = jumpCheck.enabled = false;
        jewelryStar.enabled = timeStar.enabled = jumpStar.enabled = false;
    }

    [SerializeField, Tooltip("enum.Popup에 맞춰 PopupBase 삽입")]
    private List<PopupBase> popupList = new List<PopupBase>();
    public List<PopupBase> PopupList => popupList;

    #region _Ingame Components_
    [Header("Ingame Components")]

    [SerializeField]
    private Image jewelryGetMark;
    [SerializeField]
    private TextMeshProUGUI timer;
    [SerializeField]
    private TextMeshProUGUI jumpCounter;
    #endregion
    #region _Gameover Components_
    [Header("Gameover Components")]

    [SerializeField]
    private TextMeshProUGUI causeOfDeath;
    [SerializeField]
    private TextMeshProUGUI explain;
    #endregion
    #region _GameClear Components_
    [Header("GameClear Components")]

    [SerializeField]
    private TextMeshProUGUI goalTime;
    [SerializeField]
    private TextMeshProUGUI goalJumpCount;

    [Space(5)]
    [SerializeField]
    private Image jewelryCheck;
    [SerializeField]
    private Image timeCheck;
    [SerializeField]
    private Image jumpCheck;

    [Space(5)]
    [SerializeField]
    private Image jewelryStar;
    [SerializeField]
    private Image timeStar;
    [SerializeField]
    private Image jumpStar;
    #endregion
    #region _Setting Components_
    [Header("Setting Components")]

    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;

    [Space(5)]
    [SerializeField]
    private Slider minimapSlider;

    [Space(5)]
    [SerializeField]
    private Button languageButton;
    #endregion
    #region _Pause Components_
    [Header("Pause Components")]

    [SerializeField]
    private GameObject retryButton;
    [SerializeField]
    private GameObject menuButton;
    #endregion

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        #region _GameManager Event Setting_
        GameManager.GameOverEvent += (obstacle) =>
        {
            string[] sheet = GameManager.Inst.GetCauseOfDeth(obstacle.ObstacleID);
            causeOfDeath.text = sheet[0];
            explain.text = sheet[1];

            StartCoroutine("GameOver");
        };
        GameManager.GameClearEvent += (jewelyClear, timeClear, jumpClear) =>
        {
            StartCoroutine(GameClear(jewelyClear, timeClear, jumpClear));
            SetClearPopup(GameManager.Inst.StageInfo.goalTime, GameManager.Inst.StageInfo.goalJumpCount);
        };
        #endregion
        #region _Setting Event Setting_
        bgmSlider.value = GameManager.Inst.GetBGM();
        sfxSlider.value = GameManager.Inst.GetSFX();

        minimapSlider.value = GameManager.Inst.GetMinimapSize();
        SetMinimapSize(GameManager.Inst.GetMinimapSize());

        bgmSlider.onValueChanged.AddListener((value) => // 배경음 슬라이더 이벤트 삽입
        {
            Debug.Log("Set BGM");
            GameManager.Inst.SetBGM(value);
        });
        sfxSlider.onValueChanged.AddListener((value) => // 효과음 슬라이더 이벤트 삽입
        {
            Debug.Log("Set SFX");
            GameManager.Inst.SetSFX(value);
        });

        minimapSlider.onValueChanged.AddListener((value) => // 미니맵 슬라이더 이벤트 삽입
        {
            SetMinimapSize(value);

            GameManager.Inst.SetMinimapSize(value);
        });

        languageButton.onClick.AddListener(() => // 언어 변경 버튼 이벤트 삽입
        {
            Language language = GameManager.Inst.SettingData.language;
            if (++language > Language.Kor) // 열거형을 벗어났을 경우
            {
                language = 0;
            }

            GameManager.Inst.SetLanguage(language);
        });
        #endregion
    }

    private const float MinimapMaxSize = 0.9f;
    private void SetMinimapSize(float size)
    {
        popupList[(int)Popup.Minimap].transform.localScale = Vector2.one * (size * 0.9f);
    }

    private IEnumerator GameOver()
    {
        popupList[(int)Popup.Screen].PopupClose();
        popupList[(int)Popup.Minimap].PopupClose();
        popupList[(int)Popup.Ingame].PopupClose();

        yield return YieldReturn.WaitForSeconds(1f);

        popupList[(int)Popup.GameOver].PopupOpen();
    }
    private IEnumerator GameClear(bool jewelyClear, bool timeClear, bool jumpClear)
    {
        popupList[(int)Popup.Screen].PopupClose();
        popupList[(int)Popup.Minimap].PopupClose();

        yield return YieldReturn.WaitForSeconds(1f);

        popupList[(int)Popup.GameClear].PopupOpen();

        yield return YieldReturn.WaitForSeconds(0.75f);

        jewelryCheck.enabled = GameManager.Inst.IsJewelryGet; // 보석을 얻었을 경우 체크 ON

        yield return YieldReturn.WaitForSeconds(0.75f);

        timeCheck.enabled = GameManager.Inst.Time <= GameManager.Inst.StageInfo.goalTime; // 목표시간 내로 들어왔을 경우 체크 ON

        yield return YieldReturn.WaitForSeconds(0.75f);

        jumpCheck.enabled = GameManager.Inst.JumpCount <= GameManager.Inst.StageInfo.goalJumpCount; // 목표 점프 횟수 내로 클리어했을 경우 체크 ON

        yield return YieldReturn.WaitForSeconds(1.25f);

        jewelryStar.enabled = jewelryCheck.enabled;
        timeStar.enabled = timeCheck.enabled;
        jumpStar.enabled = jumpCheck.enabled;
    }

    public void AllPopupClose()
    {
        popupList[(int)Popup.Screen].PopupShow(); // 스크린 팝업은 예외적으로 켬.
        popupList[(int)Popup.Minimap].PopupHide();

        popupList[(int)Popup.Ingame].PopupHide();

        popupList[(int)Popup.GameOver].PopupHide();
        popupList[(int)Popup.GameClear].PopupHide();

        popupList[(int)Popup.Pause].PopupHide();
        popupList[(int)Popup.Setting].PopupHide();

        popupList[(int)Popup.Fade].PopupClose(true); // 페이드 팝업은 페이드아웃시켜줌.
    }

    #region _About Ingame UI_
    public void SetTimer(int time)
    {
        timer.text = (time / 60).ToString("D2") + " : " + (time % 60).ToString("D2");
    }
    public void SetJumpCount(int jumpCount)
    {
        jumpCounter.text = jumpCount.ToString();
    }
    public void SetJewelryMark(bool isjewelryGet)
    {
        jewelryGetMark.enabled = isjewelryGet;
    }

    public void SetForStageSelect()
    {
        jewelryGetMark.enabled = false;

        timer.text = "-- : --";
        jumpCounter.text = "-";
    }

    private void SetClearPopup(int goalTime, int goalJumpCount)
    {
        this.goalTime.text = (goalTime / 60).ToString("D2") + " : " + (goalTime % 60).ToString("D2");
        this.goalJumpCount.text = goalJumpCount.ToString();
    }
    #endregion

    #region _PopupOpen/Close_
    public void PopupOpen(Popup targetPopup)
    {
        popupList[(int)targetPopup].PopupOpen();
        if (targetPopup == Popup.Pause)
            PauseEvent.Invoke(true);
    }
    public void PopupClose(Popup targetPopup)
    {
        popupList[(int)targetPopup].PopupClose();
        PauseEvent.Invoke(false);
    }

    public static event Action<bool> PauseEvent = (value) => Time.timeScale = value ? 0f : 1f;
    #endregion
    public void SetPause(bool isOn) // 일시정지 창의 버튼 구조를 수정하는 메소드
    {
        retryButton.SetActive(isOn);
        menuButton.SetActive(isOn && GameManager.Inst.GetClearInfo(0).stageClear);
    }
}

public enum Popup
{
    Screen,
    Minimap,
    Ingame,
    GameOver,
    GameClear,
    Pause,
    Setting,
    Fade,

}