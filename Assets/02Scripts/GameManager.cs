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
        Time.timeScale = 1f;

        StopCoroutine("TimerStart");
        PopupManager.Inst.AllPopupClose();

        if (newScene.buildIndex >= (int)SCENE.StageSelect) // 플레이어가 존재하는 씬일 경우
        {
            PlayerController.Inst.gameObject.SetActive(true);
            PlayerController.Inst.Revive();

            gameStatus = GameStatus.Play;
            IsJewelryGet = false; // 보석 체크 숨김

            if (newScene.buildIndex > (int)SCENE.StageSelect) // 스테이지 내부일 경우
            {
                StartCoroutine("TimerStart");

                EffectManager.Inst.ChangeBGM((ResourceLoader<AudioClip>.ResourceLoad(FolderName.BGM, "Stage")));
            }
            else // 스테이지 선택창일 경우
            {
                EffectManager.Inst.ChangeBGM((ResourceLoader<AudioClip>.ResourceLoad(FolderName.BGM, "Lobby")));
            }
        }
        else // 이외의 씬의 경우
        {
            PlayerController.Inst.gameObject.SetActive(false);

            gameStatus = GameStatus.None;

            if (newScene.buildIndex == (int)SCENE.Title)
                EffectManager.Inst.ChangeBGM((ResourceLoader<AudioClip>.ResourceLoad(FolderName.BGM, "Title")));
        }
    }

    #region _About Setting_
    private Setting setting = null;
    public Setting SettingData
    {
        get
        {
            if (setting == null) // 세팅이 비어있을 경우
            {
                if (!FileSaveLoader<Setting>.TryLoadData("Setting", out setting)) // 저장된 설정이 없을 경우
                {
                    setting = new Setting();
                    FileSaveLoader<Setting>.SaveData("Setting", setting); // 기본 설정을 만든 후 저장
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
    public static event Action LanguageChangeEvent = () => { Debug.Log("언어 바뀜"); };
    public void SetLanguage(Language newLanguage)
    {
        // 딕셔너리들 초기화
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
                Debug.LogError("없는 언어를 가져옴");
                break;
        }

        foreach (Interaction sheet in interactionList) // Axcel 내의 Interaction 정보를 Dictionary로 바꿔 저장
        {
            interactionSheet.Add(sheet.id, sheet.text);
        }
        foreach (CauseOfDeath sheet in causeOfDeathList) // Axcel 내의 CauseOfDeath 정보를 Dictionary로 바꿔 저장
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
        if (interactionSheet.Count < 1)  // 딕셔너리가 비어 있을 경우
        {
            SetLanguage(SettingData.language); // 언어 새로고침
        }

        string value;
        if (!interactionSheet.TryGetValue(textID, out value))
        {
            Debug.LogWarning(textID + "에 해당하는 텍스트가 없음.");
            value = "No Data";
        }

        return value;
    }
    public string[] GetCauseOfDeth(int deathID)
    {
        if (interactionSheet.Count < 1)  // 딕셔너리가 비어 있을 경우
        {
            SetLanguage(SettingData.language); // 언어 새로고침
        }

        string[] value;
        if (!causeOfDeathSheet.TryGetValue(deathID, out value))
        {
            Debug.LogWarning(deathID + "에 해당하는 사인이 없음.");
            value = new string[2];
            value[0] = value[1] = "No Data";
        }

        return value;
    }
    #endregion
    #endregion

    #region _About SaveData_
    private SaveData _saveData;
    private SaveData saveData
    {
        get
        {
            if (_saveData == null) // 세이브 데이터가 비어 있었을 경우
            {
                if (!FileSaveLoader<SaveData>.TryLoadData("GameData", out _saveData))
                { // 세이브 데이터를 불러오는 데 실패했을 경우
                    _saveData = new SaveData();

                    FileSaveLoader<SaveData>.SaveData("GameData", _saveData);
                }
            }

            return _saveData;
        }
    }

    public int StageIndex { get; set; }
    public StageClearInfo CurStageData { get; set; }
    public StageClearInfo GetClearInfo(int stageIndex)
    {
        if (stageIndex > saveData.stageList.Count - 1)
        {
            Debug.LogWarning("StageList를 벗어난 인덱스가 들어옴");

            do
            {
                saveData.stageList.Add(new StageClearInfo());
            }
            while (stageIndex >= saveData.stageList.Count - 1);
        }

        return saveData.stageList[stageIndex];
    }

    public void ResetGameData()
    {
        Debug.Log("데이터 리셋");

        _saveData = new SaveData();
        SaveGameData();
    }
    private void SaveGameData()
    {
        Debug.Log("게임 데이터 저장");
        FileSaveLoader<SaveData>.SaveData("GameData", _saveData);
    }
    #endregion

    private void Start()
    {
        GameClearEvent += GameClear;

        SceneChanged(new Scene(), SceneManager.GetActiveScene());
        SetLanguage(SettingData.language); // 언어 새로고침

        SetBGM(SettingData.bgmVolume);
        SetSFX(SettingData.sfxVolume);
    }


    #region _About Scenemove_
    private SCENE targetScene;
    public SCENE TargetScene => targetScene;
    public void SceneMove(SCENE targetScene)
    {
        Debug.Log(targetScene.ToString() + "으로 이동 준비");

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

    private int _time = 0;
    public int time
    {
        get => _time;
        private set
        {
            _time = value;
            PopupManager.Inst.SetTimer(_time);
            if (_time >= 5999) // 시간이 99분 59초 이상 흘렀을 경우
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
            if (jumpCount >= 999) // 점프 횟수가 999회 이상이 되었을 경우
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

        PlayerController.Inst.transform.position = info.startPosition.position + Vector3.up * 0.6f;
    }
    private IEnumerator TimerStart()
    {
        JumpCount = time = 0;

        yield return YieldReturn.WaitForSeconds(1f); // 시작하자마자 1초가 느는 걸 막기 위함
        do
        {
            time++;

            yield return YieldReturn.WaitForSeconds(1f);
        }
        while (GameManager.Inst.GameStatus == GameStatus.Play); // 플레이 중인 동안 반복
    }
    #endregion

    #region _About GameOver & GameClear_
    [SerializeField]
    private GameStatus gameStatus = GameStatus.None;
    public GameStatus GameStatus => gameStatus;

    public static event Action GameStartEvent = () => Debug.Log("게임 스타트");
    public static event Action<Attack> GameOverEvent = (_) => Debug.Log("게임 오버");
    public static event Action<bool, bool, bool> GameClearEvent = (_, _, _) => Debug.Log("게임 오버");
    private void GameClear(bool jewelyClear, bool timeClear, bool jumpClear)
    {
        StageClearInfo clearInfo = GetClearInfo(StageIndex);

        clearInfo.stageClear = true;
        if (StageIndex == 0) // 첫번째 스테이지, 즉 튜토리얼 스테이지를 깼을 경우
        {
            GetClearInfo(0).stageClear = true;
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
        if (gameStatus == newStatus) // 만약 현재 상태와 새 상태가 같다면 메소드 탈출
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
                EffectManager.Inst.ChangeBGM(null);
                GameOverEvent.Invoke(obstacle);
                break;
            case GameStatus.GameClear:
                GameClearEvent.Invoke(isJewelryGet, _time <= stageInfo.goalTime, jumpCount <= stageInfo.goalJumpCount);
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
public class SaveData
{
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