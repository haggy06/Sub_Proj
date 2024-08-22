using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour
{
    [Space(5)]
    [SerializeField, Range(0f, 1f), Tooltip("�� �� ���� ���� ��ġ���� ���� �� ������ ����")]
    private float timeInterval = 0.1f;

    private void Awake()
    {
        HideAimingLine();
    }

    private bool visible = false; // ������ ǥ�� ����
    public bool Visible => visible;

    [SerializeField]
    private AimingDot[] dotArray;
    public void HideAimingLine(int index = 0) // �� ��° ������ ����
    {
        for (int i = index; i < dotArray.Length; i++)
        {
            dotArray[i].OFF();
        }

        visible = (index != 0); // ù ��° ������ ���� ��� false, �ƴ� ��� true
    }
    public void SetAuxiliaryLine(float g, Vector2 startSpeed, Color dotColor)
    {
        Debug.Log("������ ���� ����");
        visible = true;

        for (int i = 0; i < dotArray.Length; i++)
        {
            float t = i * timeInterval; // ���� �� ��ġ ��꿡 �ʿ��� �ð�. i�� �����ϸ� ù ���� ������ ������ �������� �����ϰ� ���ؼ��� ���� �� �ְ� �ȴ�.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // ��� � : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // �������� � : U = g/2 * t^2 + Vy * t

            dotArray[i].SetPosition(pos, dotColor);

            if (i > 0) // ù ��° ��(����)�� �ƴ� ���
            {
                Transform dot = dotArray[i].transform; // ���� ���� transform
                Transform previousDot = dotArray[i - 1].transform; // ���� ���� Transform

                // ���� ���� �̹� �� ���̿� ���� ���� ���
                if (Physics2D.Raycast(dot.position, (previousDot.position - dot.position).normalized, MyCalculator.Distance(dot.position, previousDot.position), 1 << (int)LAYER.Ground))
                {
                    Debug.DrawRay(dot.position, previousDot.position - dot.position); // �� ���̿� ���� �׷���
                    HideAimingLine(i); // �̹� ������ �� ������ ����
                    break;
                }
            }

        }
    }
}