using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private int countPerObj = 10;

    [Space(10)]
    [SerializeField]
    private PoolObject[] initialMembers;
    public PoolObject[] InitialMembers => initialMembers;
    private void Awake()
    {
        foreach(PoolObject member in initialMembers)
        {
            MakePool(member);
        }
    }

    private Dictionary<string, Stack<PoolObject>> objectPool = new Dictionary<string, Stack<PoolObject>>();
    private Dictionary<string, Transform> containers = new();
    private Transform GetContainer(PoolObject member)
    {
        Transform container;
        if (!containers.TryGetValue(member.name, out container))
        { // �����̳ʰ� ���� ���
            container = new GameObject(member.name + " Pool").transform;
            container.parent = transform;
            containers.Add(member.name, container);
        }
        return container;
    }
    public void MakePool(PoolObject member)
    {
        Stack<PoolObject> pool;
        if (!objectPool.TryGetValue(member.name, out pool)) // ��ųʸ��� �˸´� ������ �־��� ��� pool�� �����ϰ�, �����ٸ� ���� ����� ������
        {
            pool = new Stack<PoolObject>();
            objectPool.Add(member.name, pool);
        }

        Transform memberPool = GetContainer(member);
        for (int i = 0; i < countPerObj; i++)
        {
            PoolObject newObject = Instantiate(member, memberPool);
            newObject.name = member.name;

            newObject.RememberPool(this);
            newObject.ReturnToPool();
        }
    }
    public PoolObject GetObject(PoolObject member)
    {
        PoolObject outObject;
        if (objectPool.TryGetValue(member.name, out Stack<PoolObject> pool))
        {
            POP_AGAIN:

            if (pool.TryPop(out outObject))
            {
                if (!outObject.IsReturned)
                {
                    Debug.LogError("�̹� ����� ������Ʈ�� ����");

                    goto POP_AGAIN; // �ٽ� ������ �ڷ� �ǵ���
                }
            }
            else
            {
                Debug.Log(member.name + "�� ��� �� ������");
                Transform memberPool = GetContainer(member);

                outObject = Instantiate(member, memberPool);
                outObject.name = member.name;

                outObject.RememberPool(this);
            }
        }
        else
        {
            Debug.Log(member.name + "�̶�� ������Ʈ�� �� ������ƮǮ�� �����ϴ�.");

            MakePool(member);
            objectPool.TryGetValue(member.name, out pool);
            
            outObject = pool.Pop();
        }
        return outObject;
    }

    public bool ReturnObject(PoolObject member)
    {
        if (objectPool.TryGetValue(member.name, out Stack<PoolObject> pool))
        {
            pool.Push(member);

            member.transform.parent = GetContainer(member);
            //Debug.Log(member.name + " ������Ʈ �ݳ�)" + pool.TryPeek(out _));
            return true;
        }
        else
        {
            Debug.LogError(member.name + " ������Ʈ�� ���� Ǯ�� �����ϴ�.");
            return false;
        }
    }
}
