using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private int countPerObj = 10;

    [Space(10)]
    [SerializeField]
    private List<GameObject> poolMembers = new List<GameObject>();
    private void Awake()
    {
        foreach(GameObject member in poolMembers)
        {
            MakePool(member, false);
        }
    }

    [SerializeField]
    private Dictionary<string, Stack<GameObject>> objectPool = new Dictionary<string, Stack<GameObject>>();
    public void MakePool(GameObject member, bool newMember = true)
    {
        if (newMember)
            poolMembers.Add(member);

        if (member.TryGetComponent<PoolObject>(out _))
        {
            Transform memberPool = new GameObject().transform;
            memberPool.transform.parent = transform;
            memberPool.name = member.name + " Pool";

            Stack<GameObject> pool;
            if (!objectPool.TryGetValue(member.name, out pool)) // ��ųʸ��� �˸´� ������ �־��� ��� pool�� �����ϰ�, �����ٸ� ���� ����� ������
            {
                pool = new Stack<GameObject>();
                objectPool.Add(member.name, pool);
            }

            for (int i = 0; i < countPerObj; i++)
            {
                GameObject newObject = Instantiate(member, memberPool);

                newObject.GetComponent<PoolObject>().RememberPool(pool, memberPool);
                newObject.GetComponent<PoolObject>().ReturnToPool();
            }
        }
        else
        {
            Debug.LogError("Ǯ ��� " + member.name + "���� PoolObject �������̽��� �����Ǿ����� ����");
        }
    }
    public bool TryGetObject(GameObject member, out GameObject outObject)
    {
        if (objectPool.TryGetValue(member.name, out Stack<GameObject> pool))
        {
            if (pool.TryPop(out outObject))
            {
                outObject.SetActive(true);
                outObject.GetComponent<PoolObject>().ExitFromPool();
            }
            else
            {
                //Debug.LogError(member.name + "�� ��� �� ���������ϴ�.");
                Transform memberPool = transform.Find(member.name + " Pool").transform;

                outObject = Instantiate(member, memberPool);
                outObject.SetActive(true);
                outObject.GetComponent<PoolObject>().RememberPool(pool, memberPool);

                outObject.GetComponent<PoolObject>().ExitFromPool();
            }

            return true;
        }
        else
        {
            Debug.LogError(member.name + "�̶�� ������Ʈ�� �� ������ƮǮ�� �����ϴ�.");

            outObject = null;
            return false;
        }
    }
    public bool ReturnObject(GameObject member)
    {
        if (objectPool.TryGetValue(member.name, out Stack<GameObject> pool))
        {
            pool.Push(member);
            return true;
        }
        else
        {
            Debug.LogError(member.name + " ������Ʈ�� ���� Ǯ�� �����ϴ�.");
            return false;
        }
    }
}
