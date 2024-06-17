using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageButton : EventButton
{
    protected override void ClickEvent()
    {
        Language language = GameManager.Inst.SettingData.language;
        if (++language > Language.Kor) // 열거형을 벗어났을 경우
        {
            language = 0;
        }

        GameManager.Inst.LanguageChange(language);
    }
}
