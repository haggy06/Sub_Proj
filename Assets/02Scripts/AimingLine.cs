using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [Space(5)]
    [SerializeField, Range(0f, 1f), Tooltip("�� �� ���� ���� ��ġ���� ���� �� ������ ����")]
    private float timeInterval = 0.1f;

    private void Awake()
    {
        HideAuxiliaryLine();

        DragManager.MouseDragEvent += SetAuxiliaryLine;
        DragManager.MouseUpEvent += (_, _) => HideAuxiliaryLine();
    }

    private bool visible = false; // ������ ǥ�� ����
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

        if (!player.IsGround) // ������ ���� ���� ��� ���� ����
        {
            if (visible) // �������� ���� ��� ������
            {
                HideAuxiliaryLine();
            }
            return;
        }
        Debug.Log("������ ���� ����");
        visible = true;

        float g = Physics2D.gravity.y * PlayerController.Inst.Rigid2D.gravityScale; // �߷� = ���� �߷� ���� * ����� �߷� ����
        Vector2 startSpeed = aim * powerPercent * PlayerController.Inst.JumpPower; // �ʱ� �ӵ� = ���� ���� * ���� �ۼ�Ʈ * �ִ� ���� �Ŀ�
        Color dotColor = new Color(1f, 1f - powerPercent, 1f - powerPercent, 0.75f); // ���� �÷� ����. ������ ������ ��������.

        for (int i = 0; i < transform.childCount; i++)
        {
            float t = (i + 1) * timeInterval; // ���� �� ��ġ ��꿡 �ʿ��� �ð�. i + 1�� ������ i�� �����ϸ� ù ���� ������ ������ ������ �����̴�.
            Vector2 pos = Vector2.zero;
            pos.x = startSpeed.x * t; // ��� � : U = Vx * t
            pos.y = (g / 2f * (t * t)) + (startSpeed.y * t); // �������� � : U = g/2 * t^2 + Vy * t

            Transform dot = transform.GetChild(i);
            dot.localPosition = pos; // ����� ��ġ�� �� �̵�
            dot.GetComponent<SpriteRenderer>().color = dotColor; // �� �÷� ����
        }
    }
}