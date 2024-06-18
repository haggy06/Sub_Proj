using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingPopup : FadePopup
{
    [SerializeField]
    private AudioMixerGroup bgm;
    [SerializeField]
    private AudioMixerGroup sfx;

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

    protected override void Awake()
    {
        base.Awake();

        languageButton.onClick.AddListener(() => // 언어 변경 버튼 이벤트 삽입
        {
            Language language = GameManager.Inst.SettingData.language;
            if (++language > Language.Kor) // 열거형을 벗어났을 경우
            {
                language = 0;
            }

            GameManager.Inst.LanguageChange(language);
        });
    }
}
