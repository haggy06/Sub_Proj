using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private AudioMixer audioMixer;
    protected override void SceneChanged(Scene replacedScene, Scene newScene)
    {
        StopCoroutine("TimerStart");
        PopupManager.Inst.AllPopupClose();

        if (newScene.buildIndex >= (int)SCENE.StageSelect) // �÷��̾ �����ϴ� ���� ���
        {
            PlayerController.Inst.gameObject.SetActive(true);
            PlayerController.Inst.Revive();

            gameStatus = GameStatus.Play;
            IsJewelryGet = false; // ���� üũ ����
            PopupManager.Inst.PopupOpen(Popup.Ingame); // �ΰ��� UI ����

            if (newScene.buildIndex > (int)SCENE.StageSelect) // �������� ������ ���
            {
                PopupManager.Inst.SetPause(true);
                PopupManager.Inst.PopupOpen(Popup.Minimap); // �̴ϸ� ����

                StartCoroutine("TimerStart");
            }
            else // �������� ����â�� ���
            {
                PopupManager.Inst.SetPause(false);
                PopupManager.Inst.SetForStageSelect();

                PlayerController.Inst.transform.position = stageInfo.doorPosition;
            }
        }
        else // �̿��� ���� ���
        {
            PlayerController.Inst.gameObject.SetActive(false);

            gameStatus = GameStatus.None;
        }
    }

    #region _About Setting_
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
    private void SaveSetting()
    {
        FileSaveLoader<Setting>.SaveData("Setting", setting);
    }

    public void SetBGM(float value)
    {
        SettingData.bgmVolume = value;
        audioMixer.SetFloat("BGM", Mathf.Log10(SettingData.bgmVolume) * 80f);

        SaveSetting();
    }
    public float GetBGM()
    {
        return SettingData.bgmVolume;
    }

    public void SetSFX(float value)
    {
        SettingData.sfxVolume = value;
        audioMixer.SetFloat("SFX", Mathf.Log10(SettingData.sfxVolume) * 80f);

        SaveSetting();
    }
    public float GetSFX()
    {
        return SettingData.sfxVolume;
    }

    public void SetMinimapSize(float value)
    {
        SettingData.minimapSize = value;

        SaveSetting();
    }
    public float GetMinimapSize()
    {
        return SettingData.minimapSize;
    }

    #region _About Language_
    public static event Action LanguageChangeEvent = () => { Debug.Log("��� �ٲ�"); };
    public void SetLanguage(Language newLanguage)
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
        SaveSetting();

        LanguageChangeEvent.Invoke();
    }

    private Dictionary<int, string> interactionSheet = new Dictionary<int, string>(0);
    private Dictionary<int, string[]> causeOfDeathSheet = new Dictionary<int, string[]>(0);
    public string GetInteractionText(int textID)
    {
        if (interactionSheet.Count < 1)  // ��ųʸ��� ��� ���� ���
        {
            SetLanguage(SettingData.language); // ��� ���ΰ�ħ
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
            SetLanguage(SettingData.language); // ��� ���ΰ�ħ
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
    #endregion

    #region _About ClearData_
    private GameData saveData;
    private GameData gameData
    {
        get
        {
            if (saveData == null) // ���̺� �����Ͱ� ��� �־��� ���
            {
                if (!FileSaveLoader<GameData>.TryLoadData("GameData", out saveData))
                { // ���̺� �����͸� �ҷ����� �� �������� ���
                    saveData = new GameData();

                    FileSaveLoader<GameData>.SaveData("GameData", saveData);
                }
            }

            return saveData;
        }
    }

    public int StageIndex { get; set; }
    public StageClearInfo CurStageData { get; set; }
    public StageClearInfo GetClearInfo(int stageIndex)
    {
        if (stageIndex > gameData.stageList.Count - 1)
        {
            Debug.LogWarning("StageList�� ��� �ε����� ����");

            do
            {
                gameData.stageList.Add(new StageClearInfo());
            }
            while (stageIndex >= gameData.stageList.Count);
        }

        return gameData.stageList[stageIndex];
    }
    public bool GetTutorialClear()
    {
        return gameData.tutorialClear;
    }

    public void ResetGameData()
    {
        Debug.Log("������ ����");

        saveData = new GameData();
        SaveGameData();
    }
    private void SaveGameData()
    {
        Debug.Log("���� ������ ����");
        FileSaveLoader<GameData>.SaveData("GameData", saveData);
    }
    #endregion

    private void Start()
    {
        GameClearEvent += GameClear;

        SceneChanged(new Scene(), SceneManager.GetActiveScene());
        SetLanguage(SettingData.language); // ��� ���ΰ�ħ

        SetBGM(SettingData.bgmVolume);
        SetSFX(SettingData.sfxVolume);
    }


    #region _About Scenemove_
    private SCENE targetScene;
    public SCENE TargetScene => targetScene;
    public void SceneMove(SCENE targetScene)
    {
        Debug.Log(targetScene.ToString() + "���� �̵� �غ�");

        this.targetScene = targetScene;
        PopupManager.Inst.PopupOpen(Popup.Fade);

        StartCoroutine("LoadScene");
    }
    private IEnumerator LoadScene()
    {
        yield return YieldReturn.WaitForSecondsRealtime(PopupManager.Inst.PopupList[(int)Popup.Fade].FadeDuration);

        SceneManager.LoadScene((int)SCENE.Loading);
    }
    #endregion

    #region _About Ingame UI_
    private StageInfo stageInfo;
    public StageInfo StageInfo => stageInfo;

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

    private bool isJewelryGet = false;
    public bool IsJewelryGet
    {
        get => isJewelryGet;
        set
        {
            isJewelryGet = value;
            PopupManager.Inst.SetJewelryMark(isJewelryGet);
        }
    }
    public void SetStageInfo(StageInfo info)
    {
        stageInfo = info;

        PlayerController.Inst.transform.position = info.startPosition;
    }
    private IEnumerator TimerStart()
    {
        JumpCount = Time = 0;

        yield return YieldReturn.WaitForSeconds(1f); // �������ڸ��� 1�ʰ� ���� �� ���� ����
        do
        {
            Time++;

            yield return YieldReturn.WaitForSeconds(1f);
        }
        while (GameManager.Inst.GameStatus == GameStatus.Play); // �÷��� ���� ���� �ݺ�
    }
    #endregion

    #region _About GameOver & GameClear_
    [SerializeField]
    private GameStatus gameStatus = GameStatus.None;
    public GameStatus GameStatus => gameStatus;

    public static event Action GameStartEvent = () => Debug.Log("���� ��ŸƮ");
    public static event Action<Attack> GameOverEvent = (_) => Debug.Log("���� ����");
    public static event Action<bool, bool, bool> GameClearEvent = (_, _, _) => Debug.Log("���� ����");
    private void GameClear(bool jewelyClear, bool timeClear, bool jumpClear)
    {
        StageClearInfo clearInfo = GetClearInfo(StageIndex);

        clearInfo.stageClear = true;
        if (StageIndex == 0) // ù��° ��������, �� Ʃ�丮�� ���������� ���� ���
        {
            gameData.tutorialClear = true;
        }

        if (jewelyClear)
            clearInfo.jewelryClear = true;
        if (timeClear)
            clearInfo.timeClear = true;
        if (jumpClear)
            clearInfo.jumpClear = true;

        if (jewelyClear && timeClear && jumpClear)
            clearInfo.clearAtOnce = true;

        SaveGameData();
    }

    public void ChangeGameStatus(GameStatus newStatus, Attack obstacle = null)
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
                GameClearEvent.Invoke(isJewelryGet, time <= stageInfo.goalTime, jumpCount <= stageInfo.goalJumpCount);
                break;
            default:
                break;
        }
    }
    #endregion
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

[Serializable]
public class Setting
{
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public float minimapSize = 0.75f;

    public Language language = Language.Eng;
}

[Serializable]
public class GameData
{
    public bool tutorialClear = false;

    public List<StageClearInfo> stageList = new List<StageClearInfo>();

    public List<DeathInfo> deathRecord = new List<DeathInfo>();
}

[Serializable]
public class StageClearInfo
{
    public bool stageClear = false;

    public bool jewelryClear = false;
    public bool timeClear = false;
    public bool jumpClear = false;

    public bool clearAtOnce = false;
}
[Serializable]
public class DeathInfo
{
    public bool isDeathBefore = false;
    public int deathCount = 0;
}