using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour
{
    [Space(5)]
    [SerializeField, Range(0f, 1f), Tooltip("몇 초 후의 예상 위치마다 점을 둘 것인지 설정")]
    private float timeInterval = 0.1f;

    private void Awake()
    {
        HideAuxiliaryLine();
    }

    private bool visible = false; // 보조선 표시 여부
    public bool Visible => visible;
    public void HideAuxiliaryLine(int index = 0) // 몇 번째 점부터 끌지
    {
        for (int i = index; i <transform.childCount; i++)
        {
            Transform dot = transform.GetChild(i);
            dot.localPosition = Vector2.zero;
            dot.GetComponent<SpriteRenderer>().color = CustomColor.zero;
        }
        /*
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = transparent;
        }
        */
        visible = (index != 0); // 첫 번째 점부터 껐을 경우 false, 아닐 경우 true
    }
    public void SetAuxiliaryLine(float g, Vector2 startSpeed, Color dotColor)
    {
        Debug.Log("보조선 세팅 시작");
        visible = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            float t = i * timeInterval; // 현재 점 위치 계산에 필요한 시간. i로 시작하면 첫 점은 무조건 원점에 찍히지만 간단하게 조준선을 끊을 수 있게 된다.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // 등속 운동 : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // 자유낙하 운동 : U = g/2 * t^2 + Vy * t

            Transform dot = transform.GetChild(i);
            dot.localPosition = pos; // 계산한 위치로 점 이동
            if (i > 0) // 첫 번째 점(원점)이 아닐 경우
            {
                Transform previousDot = transform.GetChild(i - 1); // 이전 점의 Transform

                // 이전 점과 이번 점 사이에 벽이 있을 경우
                if (Physics2D.Raycast(dot.position, (previousDot.position - dot.position).normalized, MyCalculator.Distance(dot.position, previousDot.position), 1 << (int)LAYER.Ground))
                {
                    //Debug.Log((i + 1) + "번째 점이 벽에 가려짐");
                    Debug.DrawRay(dot.position, previousDot.position - dot.position); // 점 사이에 라인 그려줌
                    HideAuxiliaryLine(i); // 이번 점부터 끝 점까지 숨김
                    break;
                }
            }
            dot.GetComponent<SpriteRenderer>().color = dotColor; // 점 컬러 변경
        }
    }
}