using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using System.IO;
using static Unity.VisualScripting.Icons;

public class GameManager : MonoSingleton<GameManager>
{
    protected override void SetInstanceToThis()
    {
        instance = this;
    }

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

    protected override void Awake()
    {
        base.Awake();

        SceneManager.activeSceneChanged += SceneChanged;
    }

    #region _About Language_
    public void LanguageChange(Language newLanguage)
    {
        interactionSheet.Clear(); // 딕셔너리들 초기화
        causeOfDeathSheet.Clear();

        List<Interaction> interactionList= null;
        List<CauseOfDeath> causeOfDeathList = null;

        switch (newLanguage)
        {
            case Language.Eng:
                Eng sheet_E = Resources.Load<Eng>(Path.Combine("Language", newLanguage.ToString()));
                interactionList = sheet_E.interaction;
                causeOfDeathList = sheet_E.causeOfDeath;
                break;
            case Language.Kor:
                Kor sheet_K = Resources.Load<Kor>(Path.Combine("Language", newLanguage.ToString()));
                interactionList= sheet_K.interaction;
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
        /*
        dynamic language; // 호환성 이슈로 다룰 수 없을 듯
        switch (newLanguage)
        {
            case Language.Eng:
                language = Resources.Load<Eng>(Path.Combine("Language", newLanguage.ToString()));
                break;
            case Language.Kor:
                language = Resources.Load<Kor>(Path.Combine("Language", newLanguage.ToString()));
                break;
            default:
                Debug.LogError("없는 언어를 가져옴");
                language = Resources.Load<Eng>(Path.Combine("Language", newLanguage.ToString()));
                break;
        }

        foreach (Interaction sheet in language.interaction) // Axcel 내의 Interaction 정보를 Dictionary로 바꿔 저장
        {
            interactionSheet.Add(sheet.id, sheet.text);
        }
        foreach (CauseOfDeath sheet in language.interaction) // Axcel 내의 CauseOfDeath 정보를 Dictionary로 바꿔 저장
        {
            string[] data = { sheet.name, sheet.explain };
            causeOfDeathSheet.Add(sheet.id, data);
        }
        */
    }

    private Dictionary<int, string> interactionSheet = null;
    private Dictionary<int, string[]> causeOfDeathSheet = null;
    public string GetInteractionText(int textID)
    {
        if (interactionSheet == null)  // 딕셔너리가 비어 있을 경우
        {
            LanguageChange(SettingData.language); // 언어 새로고침
        }

        string value = "No Data";
        interactionSheet.TryGetValue(textID, out value);

        return value;
    }
    public string[] GetCauseOfDeth(int deathID)
    {
        if (interactionSheet == null)  // 딕셔너리가 비어 있을 경우
        {
            LanguageChange(SettingData.language); // 언어 새로고침
        }

        string[] value = {"No Data", "No Data" };
        causeOfDeathSheet.TryGetValue(deathID, out value);

        return value;
    }
    #endregion

    #region __About Scenemove_
    private SCENE targetScene;
    public SCENE TargetScene => targetScene;
    public void SceneMove(SCENE targetScene)
    {
        this.targetScene = targetScene;
        SceneManager.LoadScene((int)SCENE.Loading);
    }
    #endregion

    [SerializeField]
    private GameStatus gameStatus = GameStatus.None;
    public GameStatus GameStatus => gameStatus;

    private void SceneChanged(Scene replacedScene, Scene newScene)
    {
        if (newScene.buildIndex >= (int)SCENE.StageSelect) // 플레이어가 존재하는 씬일 경우
        {
            gameStatus = GameStatus.Play;
        }
        else // 이외의 씬의 경우
        {
            gameStatus = GameStatus.None;
        }
    }

    public event Action GameStartEvent = () => { Debug.Log("게임 스타트"); };
    public event Action GameOverEvent = () => { Debug.Log("게임 오버"); };
    public event Action GameClearEvent = () => { Debug.Log("게임 클리어"); };
    public void ChangeGameStatus(GameStatus newStatus)
    {
        gameStatus = newStatus;

        switch (gameStatus)
        {
            case GameStatus.Play:
                GameStartEvent.Invoke();
                break;
            case GameStatus.GameOver:
                GameOverEvent.Invoke();
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

public struct StageInfo
{
    public int targetTime;
    public int targetJumpCount;

    public StageInfo(int targetTime = 60, int targetJumpCount = 5)
    {
        this.targetTime = targetTime;
        this.targetJumpCount = targetJumpCount;
    }
}

public class Setting
{
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;

    public Language language = Language.Eng;

    public float minimapSize = 1f;
}