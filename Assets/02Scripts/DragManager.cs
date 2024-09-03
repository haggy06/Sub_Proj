using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class DragManager : MonoBehaviour
{
    [SerializeField]
    private Transform clickPoint;
    [SerializeField]
    private Transform dragPoint;

    [Space(5)]
    [SerializeField, Range(0f, 100f), Tooltip("����� �� 100%�� ���� �Ǵ� �Ÿ�")]
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

    public static event Action MouseClickEvent = () => { }; // �̺�Ʈ�� �� ���ٽ��� �⺻������ �־� �����ν� NullReferenceException ���� ����
    public static event Action<Vector2, float> MouseDragEvent = (_, _) => { };
    public static event Action<Vector2, float> MouseUpEvent = (_, _) => { };

    private void OnMouseDown() // ��ü�� ������ ���
    {
        Debug.Log("������Ʈ Ŭ�� ����");

        Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Z ���� ī�޶� ������ ���� ���� ���� ���� ���� Vector2�� ���� ��Ҵ�(Gizmo �Ⱥ��̴� ���� ����).
        clickPoint.position = clickPos; // Vector3�� transform.position�� Vector2 ���� ������ Z ���� �ڵ����� 0�� ����.

        aiming = true;
        MouseClickEvent.Invoke();
    }
    private void OnMouseDrag() // ���콺 ��Ŭ���� ������ ���� ���
    {
        Debug.Log("������Ʈ Ŭ�� ��..");

        Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        dragPoint.position = clickPos;

        aim = -dragPoint.localPosition.normalized; // dragPoint�� clickPoint�� �ڽ��̹Ƿ� localPosition == ���� �����̴�. -�� ���� ������ ����ó�� �����ϱ� ���ؼ���.
        float dragDistance = MyCalculator.Distance(clickPoint.localPosition, dragPoint.localPosition);//Mathf.Log(Mathf.Pow(dragPoint.localPosition.x, 2) + Mathf.Pow(dragPoint.localPosition.y, 2), 2); // dragPoint���� clickPoint������ �Ÿ� ���
        powerPercent = Mathf.Clamp(dragDistance, 0f, maximumLength) / maximumLength; // �Ÿ��� maximumLength�� ���� �ʰ� ������ �� ������� ����

        MouseDragEvent.Invoke(aim, powerPercent);
    }
    private void OnMouseUp() // ���콺 ��Ŭ���� ���� ���
    {
        Debug.Log("������Ʈ Ŭ�� ����");

        aiming = false;
        MouseUpEvent.Invoke(aim, powerPercent);
    }
}

public static class MyCalculator
{
    public static float Distance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Sqrt(Mathf.Pow(pos1.x - pos2.x, 2) + Mathf.Pow(pos1.y - pos2.y, 2));
    }

    public static Vector2 Deg2Vec(float deg)
    {
        return new Vector2(Mathf.Cos(deg * Mathf.Deg2Rad), Mathf.Sin(deg * Mathf.Deg2Rad));
    }
    public static float Vec2Deg(Vector2 vec)
    {
        vec = vec.normalized;
        return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
    }
}