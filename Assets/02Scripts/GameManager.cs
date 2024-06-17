using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoSingleton<GameManager>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
    }
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        StopCoroutine("TimerStart");
        PopupManager.Inst.AllPopupClose();

        if (newScene.buildIndex >= (int)SCENE.StageSelect) // �÷��̾ �����ϴ� ���� ���
        {
            gameStatus = GameStatus.Play;
            IsJewelyGet = false; // ���� üũ ����
            PopupManager.Inst.PopupOpen(Popup.Ingame); // �ΰ��� UI ����

            if (newScene.buildIndex > (int)SCENE.StageSelect) // �������� ������ ���
            {
                PopupManager.Inst.SetRetry(true);
                PopupManager.Inst.PopupOpen(Popup.Minimap); // �̴ϸ� ����

                StartCoroutine("TimerStart");
            }
            else // �������� ����â�� ���
            {
                PopupManager.Inst.SetRetry(false);
                PopupManager.Inst.SetForStageSelect();
            }
        }
        else // �̿��� ���� ���
        {
            gameStatus = GameStatus.None;
        }
    }

    private Setting setting = null;
    public Setting SettingData
    {
        get
        {
            if (setting == null) // ������ ������� ���
            {
                if (!FileSaveLoader<Setting>.TryLoadData("Setting", out setting)) // ����� ������ ���� ���
                {
                    setting = new Setting();
                    FileSaveLoader<Setting>.SaveData("Setting", setting); // �⺻ ������ ���� �� ����
                }
            }

            return setting;
        }
    }

    private void Start()
    {
        print(instance.gameObject.name + ", " + instance.transform.position);
        SceneChanged(new Scene(), SceneManager.GetActiveScene());
        LanguageChange(SettingData.language); // ��� ���ΰ�ħ
    }

    #region _About Language_
    public event Action LanguageChangeEvent = ()=> { Debug.Log("��� �ٲ�"); };
    public void LanguageChange(Language newLanguage)
    {
        // ��ųʸ��� �ʱ�ȭ
        interactionSheet.Clear();
        causeOfDeathSheet.Clear();

        List<Interaction> interactionList = null;
        List<CauseOfDeath> causeOfDeathList = null;

        switch (newLanguage)
        {
            case Language.Eng:
                Eng sheet_E = Resources.Load<Eng>(Path.Combine("Language", "Eng"));
                interactionList = sheet_E.interaction;
                causeOfDeathList = sheet_E.causeOfDeath;
                break;
            case Language.Kor:
                Kor sheet_K = Resources.Load<Kor>(Path.Combine("Language", "Kor"));
                interactionList = sheet_K.interaction;
                causeOfDeathList = sheet_K.causeOfDeath;
                break;
            default:
                Debug.LogError("���� �� ������");
                break;
        }

        foreach (Interaction sheet in interactionList) // Axcel ���� Interaction ������ Dictionary�� �ٲ� ����
        {
            interactionSheet.Add(sheet.id, sheet.text);
        }
        foreach (CauseOfDeath sheet in causeOfDeathList) // Axcel ���� CauseOfDeath ������ Dictionary�� �ٲ� ����
        {
            string[] data = { sheet.name, sheet.explain };
            causeOfDeathSheet.Add(sheet.id, data);
        }

        SettingData.language = newLanguage;
        FileSaveLoader<Setting>.SaveData("Setting", setting);

        LanguageChangeEvent.Invoke();
        /*
        dynamic language; // ȣȯ�� �̽��� �ٷ� �� ���� ��
        switch (newLanguage)
        {
            case Language.Eng:
                language = Resources.Load<Eng>(Path.Combine("Language", newLanguage.ToString()));
                break;
            case Language.Kor:
                language = Resources.Load<Kor>(Path.Combine("Language", newLanguage.ToString()));
                break;
            default:
                Debug.LogError("���� �� ������");
                language = Resources.Load<Eng>(Path.Combine("Language", newLanguage.ToString()));
                break;
        }

        foreach (Interaction sheet in language.interaction) // Axcel ���� Interaction ������ Dictionary�� �ٲ� ����
        {
            interactionSheet.Add(sheet.id, sheet.text);
        }
        foreach (CauseOfDeath sheet in language.interaction) // Axcel ���� CauseOfDeath ������ Dictionary�� �ٲ� ����
        {
            string[] data = { sheet.name, sheet.explain };
            causeOfDeathSheet.Add(sheet.id, data);
        }
        */
    }

    private Dictionary<int, string> interactionSheet = new Dictionary<int, string>(0);
    private Dictionary<int, string[]> causeOfDeathSheet = new Dictionary<int, string[]>(0);
    public string GetInteractionText(int textID)
    {
        if (interactionSheet.Count < 1)  // ��ųʸ��� ��� ���� ���
        {
            LanguageChange(SettingData.language); // ��� ���ΰ�ħ
        }

        string value;
        if (!interactionSheet.TryGetValue(textID, out value))
        {
            Debug.LogWarning(textID + "�� �ش��ϴ� �ؽ�Ʈ�� ����.");
            value = "No Data";
        }

        return value;
    }
    public string[] GetCauseOfDeth(int deathID)
    {
        if (interactionSheet.Count < 1)  // ��ųʸ��� ��� ���� ���
        {
            LanguageChange(SettingData.language); // ��� ���ΰ�ħ
        }

        string[] value;
        if (!causeOfDeathSheet.TryGetValue(deathID, out value))
        {
            Debug.LogWarning(deathID + "�� �ش��ϴ� ������ ����.");
            value = new string[2];
            value[0] = value[1] = "No Data";
        }

        return value;
    }
    #endregion

    #region _About Scenemove_
    private SCENE targetScene;
    public SCENE TargetScene => targetScene;
    public void SceneMove(SCENE targetScene)
    {
        this.targetScene = targetScene;
        PopupManager.Inst.PopupOpen(Popup.Fade);
        Invoke("LoadScene", PopupManager.Inst.PopupList[(int)Popup.Fade].FadeDuration);
    }
    private void LoadScene()
    {
        SceneManager.LoadScene((int)SCENE.Loading);
    }
    #endregion

    private WaitForSeconds Wait1Sec = new WaitForSeconds(1f);
    #region _About Ingame UI_
    private int time = 0;
    public int Time
    {
        get => time;
        private set
        {
            time = value;
            PopupManager.Inst.SetTimer(time);
            if (time >= 5999) // �ð��� 99�� 59�� �̻� �귶�� ���
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
            PopupManager.Inst.SetJumpCount(jumpCount);
            if (jumpCount >= 999) // ���� Ƚ���� 999ȸ �̻��� �Ǿ��� ���
            {

            }
        }
    }

    private bool isJewelyGet = false;
    public bool IsJewelyGet
    {
        get => isJewelyGet;
        set
        {
            isJewelyGet = value;
            PopupManager.Inst.SetJewelyMark(isJewelyGet);
            /*
            if (isJewelyGet)
            {
                JewelyGetMark.enabled = true;
            }
            else
            {
                JewelyGetMark.enabled = false;
            }
            */
        }
    }
    private IEnumerator TimerStart()
    {
        JumpCount = Time = 0;
        do
        {
            Time++;

            yield return Wait1Sec;
        }
        while (GameManager.Inst.GameStatus == GameStatus.Play); // �÷��� ���� ���� �ݺ�
    }
    #endregion

    [SerializeField]
    private GameStatus gameStatus = GameStatus.None;
    public GameStatus GameStatus => gameStatus;


    public event Action GameStartEvent = () => { Debug.Log("���� ��ŸƮ"); };
    public event Action<Obstacle> GameOverEvent = (_) => { Debug.Log("���� ����"); };
    public event Action GameClearEvent = () => { Debug.Log("���� Ŭ����"); };
    public void ChangeGameStatus(GameStatus newStatus, Obstacle obstacle = null)
    {
        if (gameStatus == newStatus) // ���� ���� ���¿� �� ���°� ���ٸ� �޼ҵ� Ż��
        {
            return;
        }

        gameStatus = newStatus;

        switch (gameStatus)
        {
            case GameStatus.Play:
                GameStartEvent.Invoke();
                break;
            case GameStatus.GameOver:
                GameOverEvent.Invoke(obstacle);
                break;
            case GameStatus.GameClear:
                GameClearEvent.Invoke();
                break;
            default:
                break;
        }
    }
}
public enum Language
{
    Eng, 
    Kor,

}

public enum GameStatus
{
    None,
    Play,
    GameOver,
    GameClear,

}

public class Setting
{
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public Language language = Language.Eng;

    public float minimapSize = 1f;
}