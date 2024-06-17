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
    }

    [SerializeField, Tooltip("enum.Popup¿¡ ¸ÂÃç PopupBase »ðÀÔ")]
    private List<PopupBase> popupList = new List<PopupBase>();
    public List<PopupBase> PopupList => popupList;

    #region _Ingame Components_
    [Header("Ingame Components")]

    [SerializeField]
    private Image jewelyGetMark;
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

    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;

        SceneManager.activeSceneChanged += SceneChanged;

        GameManager.Inst.GameOverEvent += (obstacle) =>
        {
            popupList[(int)Popup.Screen].PopupClose();
            popupList[(int)Popup.Minimap].PopupClose();
            popupList[(int)Popup.Ingame].PopupClose();

            string[] sheet = GameManager.Inst.GetCauseOfDeth(obstacle.ObstacleID);
            causeOfDeath.text = sheet[0];
            explain.text = sheet[1];

            Invoke("GameOver", 1f);
        };
        GameManager.Inst.GameClearEvent += () => Invoke("GameClear", 1f);
    }
    private void GameOver()
    {
        popupList[(int)Popup.GameOver].PopupOpen();
    }
    private void GameClear()
    {
        popupList[(int)Popup.GameClear].PopupOpen();
    }

    public void AllPopupClose()
    {
        popupList[(int)Popup.Screen].PopupShow(); // ½ºÅ©¸° ÆË¾÷Àº ¿¹¿ÜÀûÀ¸·Î ÄÔ.
        popupList[(int)Popup.Minimap].PopupHide();

        popupList[(int)Popup.Ingame].PopupHide();

        popupList[(int)Popup.GameOver].PopupHide();
        popupList[(int)Popup.GameClear].PopupHide();

        popupList[(int)Popup.Pause].PopupHide();
        popupList[(int)Popup.Setting].PopupHide();

        popupList[(int)Popup.Fade].PopupClose(true); // ÆäÀÌµå ÆË¾÷Àº ÆäÀÌµå¾Æ¿ô½ÃÄÑÁÜ.
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
    public void SetJewelyMark(bool isJewelyGet)
    {
        jewelyGetMark.enabled = isJewelyGet;
    }

    public void SetForStageSelect()
    {
        jewelyGetMark.enabled = false;

        timer.text = "-- : --";
        jumpCounter.text = "-";
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