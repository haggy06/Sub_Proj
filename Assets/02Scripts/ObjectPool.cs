using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private int countPerObj = 10;

    [Space(10)]
    [SerializeField]
    private List<PoolObject> poolMembers = new List<PoolObject>();
    private void Awake()
    {
        foreach(PoolObject member in poolMembers)
        {
            MakePool(member, false);
        }
    }

    [SerializeField]
    private Dictionary<string, Stack<PoolObject>> objectPool = new Dictionary<string, Stack<PoolObject>>();
    public void MakePool(PoolObject member, bool newMember = true)
    {
        if (newMember)
            poolMembers.Add(member);

        if (member.TryGetComponent<PoolObject>(out _))
        {
            Transform memberPool = new PoolObject().transform;
            memberPool.transform.parent = transform;
            memberPool.name = member.name + " Pool";

            Stack<PoolObject> pool;
            if (!objectPool.TryGetValue(member.name, out pool)) // ��ųʸ��� �˸´� ������ �־��� ��� pool�� �����ϰ�, �����ٸ� ���� ����� ������
            {
                pool = new Stack<PoolObject>();
                objectPool.Add(member.name, pool);
            }

            for (int i = 0; i < countPerObj; i++)
            {
                PoolObject newObject = Instantiate(member, memberPool);

                newObject.GetComponent<PoolObject>().RememberPool(pool, memberPool);
                newObject.GetComponent<PoolObject>().ReturnToPool();
            }
        }
        else
        {
            Debug.LogError("Ǯ ��� " + member.name + "���� PoolObject �������̽��� �����Ǿ����� ����");
        }
    }
    public bool TryGetObject(PoolObject member, out PoolObject outObject)
    {
        if (objectPool.TryGetValue(member.name, out Stack<PoolObject> pool))
        {
            if (pool.TryPop(out outObject))
            {
                outObject.gameObject.SetActive(true);
                outObject.GetComponent<PoolObject>().ExitFromPool();
            }
            else
            {
                //Debug.LogError(member.name + "�� ��� �� ���������ϴ�.");
                Transform memberPool = transform.Find(member.name + " Pool").transform;

                outObject = Instantiate(member, memberPool);
                outObject.gameObject.SetActive(true);
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
    public bool ReturnObject(PoolObject member)
    {
        if (objectPool.TryGetValue(member.name, out Stack<PoolObject> pool))
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
