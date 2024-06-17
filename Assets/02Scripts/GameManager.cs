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

        if (newScene.buildIndex >= (int)SCENE.StageSelect) // 플레이어가 존재하는 씬일 경우
        {
            gameStatus = GameStatus.Play;
            IsJewelyGet = false; // 보석 체크 숨김
            PopupManager.Inst.PopupOpen(Popup.Ingame); // 인게임 UI 오픈

            if (newScene.buildIndex > (int)SCENE.StageSelect) // 스테이지 내부일 경우
            {
                PopupManager.Inst.SetRetry(true);
                PopupManager.Inst.PopupOpen(Popup.Minimap); // 미니맵 오픈

                StartCoroutine("TimerStart");
            }
            else // 스테이지 선택창일 경우
            {
                PopupManager.Inst.SetRetry(false);
                PopupManager.Inst.SetForStageSelect();
            }
        }
        else // 이외의 씬의 경우
        {
            gameStatus = GameStatus.None;
        }
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

    private void Start()
    {
        print(instance.gameObject.name + ", " + instance.transform.position);
        SceneChanged(new Scene(), SceneManager.GetActiveScene());
        LanguageChange(SettingData.language); // 언어 새로고침
    }

    #region _About Language_
    public event Action LanguageChangeEvent = ()=> { Debug.Log("언어 바뀜"); };
    public void LanguageChange(Language newLanguage)
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
        FileSaveLoader<Setting>.SaveData("Setting", setting);

        LanguageChangeEvent.Invoke();
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

    private Dictionary<int, string> interactionSheet = new Dictionary<int, string>(0);
    private Dictionary<int, string[]> causeOfDeathSheet = new Dictionary<int, string[]>(0);
    public string GetInteractionText(int textID)
    {
        if (interactionSheet.Count < 1)  // 딕셔너리가 비어 있을 경우
        {
            LanguageChange(SettingData.language); // 언어 새로고침
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
            LanguageChange(SettingData.language); // 언어 새로고침
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
            PopupManager.Inst.SetJumpCount(jumpCount);
            if (jumpCount >= 999) // 점프 횟수가 999회 이상이 되었을 경우
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
        while (GameManager.Inst.GameStatus == GameStatus.Play); // 플레이 중인 동안 반복
    }
    #endregion

    [SerializeField]
    private GameStatus gameStatus = GameStatus.None;
    public GameStatus GameStatus => gameStatus;


    public event Action GameStartEvent = () => { Debug.Log("게임 스타트"); };
    public event Action<Obstacle> GameOverEvent = (_) => { Debug.Log("게임 오버"); };
    public event Action GameClearEvent = () => { Debug.Log("게임 클리어"); };
    public void ChangeGameStatus(GameStatus newStatus, Obstacle obstacle = null)
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