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
        HideAuxiliaryLine();
    }

    private bool visible = false; // ������ ǥ�� ����
    public bool Visible => visible;
    public void HideAuxiliaryLine(int index = 0) // �� ��° ������ ����
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
        visible = (index != 0); // ù ��° ������ ���� ��� false, �ƴ� ��� true
    }
    public void SetAuxiliaryLine(float g, Vector2 startSpeed, Color dotColor)
    {
        Debug.Log("������ ���� ����");
        visible = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            float t = i * timeInterval; // ���� �� ��ġ ��꿡 �ʿ��� �ð�. i�� �����ϸ� ù ���� ������ ������ �������� �����ϰ� ���ؼ��� ���� �� �ְ� �ȴ�.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // ��� � : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // �������� � : U = g/2 * t^2 + Vy * t

            Transform dot = transform.GetChild(i);
            dot.localPosition = pos; // ����� ��ġ�� �� �̵�
            if (i > 0) // ù ��° ��(����)�� �ƴ� ���
            {
                Transform previousDot = transform.GetChild(i - 1); // ���� ���� Transform

                // ���� ���� �̹� �� ���̿� ���� ���� ���
                if (Physics2D.Raycast(dot.position, (previousDot.position - dot.position).normalized, MyCalculator.Distance(dot.position, previousDot.position), 1 << (int)LAYER.Ground))
                {
                    //Debug.Log((i + 1) + "��° ���� ���� ������");
                    Debug.DrawRay(dot.position, previousDot.position - dot.position); // �� ���̿� ���� �׷���
                    HideAuxiliaryLine(i); // �̹� ������ �� ������ ����
                    break;
                }
            }
            dot.GetComponent<SpriteRenderer>().color = dotColor; // �� �÷� ����
        }
    }
}