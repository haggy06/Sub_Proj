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
    public void HideAuxiliaryLine(int index = 0)
    {
        Color transparent = new Color(0f, 0f, 0f, 0f);
        for (int i = index; i <transform.childCount; i++)
        {
            Transform dot = transform.GetChild(i);
            dot.localPosition = Vector2.zero;
            dot.GetComponent<SpriteRenderer>().color = transparent;
        }
        /*
        foreach (SpriteRenderer sprite in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            sprite.color = transparent;
        }
        */
        visible = index == 0;
    }
    public void SetAuxiliaryLine(float g, Vector2 startSpeed, Color dotColor)
    {
        Debug.Log("보조선 세팅 시작");
        visible = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            float t = (i + 1) * timeInterval; // 현재 점 위치 계산에 필요한 시간. i + 1인 이유는 i로 시작하면 첫 점은 무조건 원점에 찍히기 때문이다.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // 등속 운동 : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // 자유낙하 운동 : U = g/2 * t^2 + Vy * t

            Transform dot = transform.GetChild(i);
            dot.localPosition = pos; // 계산한 위치로 점 이동
            if (i > 0 && Physics2D.Raycast(dot.position, transform.GetChild(i - 1).position - dot.position, 1f, 1 << (int)LAYER.Ground))
            {
                Debug.Log((i + 1) + "번째 점이 벽에 가려짐");
                Debug.DrawRay(dot.position, transform.GetChild(i - 1).position - dot.position);
                HideAuxiliaryLine(i);
                break;
            }
            dot.GetComponent<SpriteRenderer>().color = dotColor; // 점 컬러 변경
        }
    }
}