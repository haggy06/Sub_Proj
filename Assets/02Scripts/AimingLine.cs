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
        Debug.Log("������ ���� ����");
        visible = true;

        for (int i = 0; i < transform.childCount; i++)
        {
            float t = (i + 1) * timeInterval; // ���� �� ��ġ ��꿡 �ʿ��� �ð�. i + 1�� ������ i�� �����ϸ� ù ���� ������ ������ ������ �����̴�.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // ��� � : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // �������� � : U = g/2 * t^2 + Vy * t

            Transform dot = transform.GetChild(i);
            dot.localPosition = pos; // ����� ��ġ�� �� �̵�
            if (i > 0 && Physics2D.Raycast(dot.position, transform.GetChild(i - 1).position - dot.position, 1f, 1 << (int)LAYER.Ground))
            {
                Debug.Log((i + 1) + "��° ���� ���� ������");
                Debug.DrawRay(dot.position, transform.GetChild(i - 1).position - dot.position);
                HideAuxiliaryLine(i);
                break;
            }
            dot.GetComponent<SpriteRenderer>().color = dotColor; // �� �÷� ����
        }
    }
}