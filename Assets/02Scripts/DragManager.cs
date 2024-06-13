using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

[RequireComponent(typeof(Collider2D))]
public class DragManager : MonoBehaviour
{
    [SerializeField]
    private Transform clickPoint;
    [SerializeField]
    private Transform dragPoint;

    [Space(5)]
    [SerializeField, Range(0f, 100f), Tooltip("당겼을 때 100%의 힘이 되는 거리")]
    private float maximumLength = 10f;

    [Header("Drag Status")]

    [SerializeField]
    private bool aiming = false;
    [SerializeField]
    private Vector2 aim = Vector2.zero;
    [SerializeField, Range(0f, 1f)]
    private float powerPercent = 0f;

    public bool Aiming => aiming;
    public Vector2 Aim => aim;
    public float PowerPercent => powerPercent;

    public event Action MouseClickEvent = () => { }; // 이벤트에 빈 람다식을 기본적으로 넣어 둠으로써 NullReferenceException 에러 방지
    public event Action<Vector2, float> MouseDragEvent = (_, _) => { };
    public event Action<Vector2, float> MouseUpEvent = (_, _) => { };

    private void OnMouseDown() // 물체를 눌렀을 경우
    {
        Debug.Log("오브젝트 클릭 시작");

        Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Z 값이 카메라 쪽으로 빨려 들어가는 것을 막기 위해 Vector2에 값을 담았다(Gizmo 안보이는 현상 방지).
        clickPoint.position = clickPos; // Vector3인 transform.position에 Vector2 값을 넣으면 Z 값엔 자동으로 0이 들어간다.

        aiming = true;
        MouseClickEvent.Invoke();
    }
    private void OnMouseDrag() // 마우스 좌클릭을 누르고 있을 경우
    {
        Debug.Log("오브젝트 클릭 중..");

        Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragPoint.position = clickPos;

        aim = -dragPoint.localPosition.normalized; // dragPoint가 clickPoint의 자식이므로 localPosition == 조준 방향이다. -를 붙인 이유는 새총처럼 조준하기 위해서임.
        float dragDistance = MyCalculator.Distance(clickPoint.localPosition, dragPoint.localPosition);//Mathf.Log(Mathf.Pow(dragPoint.localPosition.x, 2) + Mathf.Pow(dragPoint.localPosition.y, 2), 2); // dragPoint부터 clickPoint까지의 거리 계산
        powerPercent = Mathf.Clamp(dragDistance, 0f, maximumLength) / maximumLength; // 거리를 maximumLength를 넘지 않게 조절한 후 백분율로 저장

        MouseDragEvent.Invoke(aim, powerPercent);
    }
    private void OnMouseUp() // 마우스 좌클릭을 뗐을 경우
    {
        Debug.Log("오브젝트 클릭 해제");

        aiming = false;
        MouseUpEvent.Invoke(aim, powerPercent);
    }
}

public static class MyCalculator
{
    public static float Distance(Vector2 pos1, Vector2 pos2)
    {
        /*
        pos2 = pos2 - pos1; // 벡터 평행이동
        return Mathf.Log((pos2.x * pos2.x) + (pos2.y * pos2.y), 2);
        */
        return Mathf.Sqrt(Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2));
    }
}