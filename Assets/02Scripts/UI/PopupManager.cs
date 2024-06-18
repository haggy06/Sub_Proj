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
    #region _Ect Components_
    [Header("Ect Components")]

    [SerializeField]
    private GameObject pause_RetryButton;
    #endregion

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        SceneManager.activeSceneChanged += SceneChanged;

        GameManager.Inst.GameOverEvent += (obstacle) =>
        {
            string[] sheet = GameManager.Inst.GetCauseOfDeth(obstacle.ObstacleID);
            causeOfDeath.text = sheet[0];
            explain.text = sheet[1];

            StartCoroutine("GameOver");
        };
        GameManager.Inst.GameClearEvent += (jewelyClear, timeClear, jumpClear) =>
        {
            StartCoroutine(GameClear(jewelyClear, timeClear, jumpClear));
        };
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

        timeCheck.enabled = GameManager.Inst.Time <= GameManager.Inst.GoalTime; // 목표시간 내로 들어왔을 경우 체크 ON

        yield return YieldReturn.WaitForSeconds(0.75f);

        jumpCheck.enabled = GameManager.Inst.JumpCount <= GameManager.Inst.GoalJumpCount; // 목표 점프 횟수 내로 클리어했을 경우 체크 ON

        yield return YieldReturn.WaitForSeconds(1f);

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

    public void SetClearPopup(int goalTime, int goalJumpCount)
    {
        this.goalTime.text = (goalTime / 60).ToString("D2") + " : " + (goalTime % 60).ToString("D2");
        this.goalJumpCount.text = goalJumpCount.ToString();
    }
    #endregion

    #region _PopupOpen_
    public void PopupOpen(Popup targetPopup)
    {
        popupList[(int)targetPopup].PopupOpen();
    }
    public void PopupClose(Popup targetPopup)
    {
        popupList[(int)targetPopup].PopupClose();
    }
    #endregion
    public void SetRetry(bool isOn)
    {
        pause_RetryButton.SetActive(isOn);
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