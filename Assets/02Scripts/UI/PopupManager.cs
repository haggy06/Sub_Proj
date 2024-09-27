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
        AllPopupClose();
        GetComponent<Canvas>().worldCamera = Camera.main;

        StopCoroutine("GameClear");
        jewelryCheck.enabled = timeCheck.enabled = jumpCheck.enabled = false;
        jewelryStar.enabled = timeStar.enabled = jumpStar.enabled = false;

        if (newScene.buildIndex >= (int)SCENE.StageSelect) // �÷��̾ �����ϴ� ���� ���
        {
            PopupOpen(Popup.Ingame); // �ΰ��� UI ����

            if (newScene.buildIndex > (int)SCENE.StageSelect) // �������� ������ ���
            {
                SetPause(true);
                PopupOpen(Popup.Minimap); // �̴ϸ� ����
            }
            else // �������� ����â�� ���
            {
                SetPause(false);
                SetForStageSelect();
            }
        }
        else if (newScene.buildIndex >= (int)SCENE.Title)
        {
            popupList[(int)Popup.Title].PopupShow();
        }
    }

    [SerializeField, Tooltip("enum.Popup�� ���� PopupBase ����")]
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

        SceneChanged(new Scene(), SceneManager.GetActiveScene());

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

        bgmSlider.onValueChanged.AddListener((value) => // ����� �����̴� �̺�Ʈ ����
        {
            Debug.Log("Set BGM");
            GameManager.Inst.SetBGM(value);
        });
        sfxSlider.onValueChanged.AddListener((value) => // ȿ���� �����̴� �̺�Ʈ ����
        {
            Debug.Log("Set SFX");
            GameManager.Inst.SetSFX(value);
        });

        minimapSlider.onValueChanged.AddListener((value) => // �̴ϸ� �����̴� �̺�Ʈ ����
        {
            SetMinimapSize(value);

            GameManager.Inst.SetMinimapSize(value);
        });

        languageButton.onClick.AddListener(() => // ��� ���� ��ư �̺�Ʈ ����
        {
            Language language = GameManager.Inst.SettingData.language;
            if (++language > Language.Kor) // �������� ����� ���
            {
                language = 0;
            }

            GameManager.Inst.SetLanguage(language);
        });
        #endregion
    }
    public void ButtonClickSound()
    {
        EffectManager.Inst.PlaySFX(ResourceLoader<AudioClip>.ResourceLoad(FolderName.Ect, "Button"));
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

        jewelryStar.enabled = jewelryCheck.enabled = timeStar.enabled = timeCheck.enabled = jumpStar.enabled = jumpCheck.enabled = false;
        popupList[(int)Popup.GameClear].PopupOpen();

        AudioClip clip = ResourceLoader<AudioClip>.ResourceLoad(FolderName.Ect, "Stamp");

        if (GameManager.Inst.IsJewelryGet) // ������ ����� ��� 
        {
            yield return YieldReturn.WaitForSeconds(0.75f);

            jewelryCheck.enabled = true;
            EffectManager.Inst.PlaySFX(clip);
        }

        if (GameManager.Inst.time <= GameManager.Inst.StageInfo.goalTime) // ��ǥ�ð� ���� ������ ���
        {
            yield return YieldReturn.WaitForSeconds(0.75f);

            timeCheck.enabled = true;
            EffectManager.Inst.PlaySFX(clip);
        }

        if (GameManager.Inst.JumpCount <= GameManager.Inst.StageInfo.goalJumpCount) // ��ǥ ���� Ƚ�� ���� Ŭ�������� ���
        {
            yield return YieldReturn.WaitForSeconds(0.75f);

            jumpCheck.enabled = true;
            EffectManager.Inst.PlaySFX(clip);
        }

        if (jewelryCheck.enabled || timeCheck.enabled || jumpCheck.enabled)
        {
            yield return YieldReturn.WaitForSeconds(1.25f);

            jewelryStar.enabled = jewelryCheck.enabled;
            timeStar.enabled = timeCheck.enabled;
            jumpStar.enabled = jumpCheck.enabled;

            EffectManager.Inst.PlaySFX(clip);
        }
    }

    public void AllPopupClose()
    {
        popupList[(int)Popup.Title].PopupHide();
        popupList[(int)Popup.Screen].PopupShow(); // ��ũ�� �˾��� ���������� ��.
        popupList[(int)Popup.Minimap].PopupHide();

        popupList[(int)Popup.Ingame].PopupHide();

        popupList[(int)Popup.GameOver].PopupHide();
        popupList[(int)Popup.GameClear].PopupHide();

        popupList[(int)Popup.Pause].PopupHide();
        popupList[(int)Popup.Setting].PopupHide();

        popupList[(int)Popup.Fade].PopupClose(true); // ���̵� �˾��� ���̵�ƿ�������.
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

    private void SetForStageSelect()
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
        {
            PauseEvent.Invoke(true);
            ButtonClickSound(); // �̋��� �Ͻ����� ������ ��ư Ŭ������ �� �� ������ �������Ͻ����� �� �ѹ� �� ���
        }
    }
    public void PopupClose(Popup targetPopup)
    {
        popupList[(int)targetPopup].PopupClose();
        if (targetPopup == Popup.Pause)
            PauseEvent.Invoke(false);
    }

    public static event Action<bool> PauseEvent = (value) => Time.timeScale = value ? 0f : 1f;
    #endregion
    private void SetPause(bool isOn) // �Ͻ����� â�� ��ư ������ �����ϴ� �޼ҵ�
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
    Title,

    Fade
}