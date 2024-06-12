using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [Space(5)]
    [SerializeField, Range(0f, 1f), Tooltip("몇 초 후의 예상 위치마다 점을 둘 것인지 설정")]
    private float timeInterval = 0.1f;

    private void Awake()
    {
        HideAuxiliaryLine();

        DragManager.MouseDragEvent += SetAuxiliaryLine;
        DragManager.MouseUpEvent += (_, _) => HideAuxiliaryLine();
    }

    private bool visible = false; // 보조선 표시 여부
    private void HideAuxiliaryLine()
    {
        Color transparent = new Color(0f, 0f, 0f, 0f);
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = transparent;
        }
        visible = false;
    }
    private void SetAuxiliaryLine(Vector2 aim, float powerPercent)
    {

        if (!player.IsGround) // 착지해 있지 않을 경우 실행 중지
        {
            if (visible) // 보조선이 보일 경우 숨겨줌
            {
                HideAuxiliaryLine();
            }
            return;
        }
        Debug.Log("보조선 세팅 시작");
        visible = true;

        float g = Physics2D.gravity.y * PlayerController.Inst.Rigid2D.gravityScale; // 중력 = 공통 중력 세기 * 대상의 중력 세기
        Vector2 startSpeed = aim * powerPercent * PlayerController.Inst.JumpPower; // 초기 속도 = 조준 각도 * 세기 퍼센트 * 최대 점프 파워
        Color dotColor = new Color(1f, 1f - powerPercent, 1f - powerPercent, 0.75f); // 점의 컬러 저장. 셀수록 점들이 빨개진다.

        for (int i = 0; i < transform.childCount; i++)
        {
            float t = (i + 1) * timeInterval; // 현재 점 위치 계산에 필요한 시간. i + 1인 이유는 i로 시작하면 첫 점은 무조건 원점에 찍히기 때문이다.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // 등속 운동 : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // 자유낙하 운동 : U = g/2 * t^2 + Vy * t

            Transform dot = transform.GetChild(i);
            dot.localPosition = pos; // 계산한 위치로 점 이동
            dot.GetComponent<SpriteRenderer>().color = dotColor; // 점 컬러 변경
        }
    }
}