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
        { // 컨테이너가 없을 경우
            container = new GameObject(member.name + " Pool").transform;
            container.parent = transform;
            containers.Add(member.name, container);
        }
        return container;
    }
    public void MakePool(PoolObject member)
    {
        Stack<PoolObject> pool;
        if (!objectPool.TryGetValue(member.name, out pool)) // 딕셔너리에 알맞는 스택이 있었을 경우 pool에 저장하고, 없었다면 새로 만들어 저장함
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
                    Debug.LogError("이미 대출된 오브젝트를 꺼냄");

                    goto POP_AGAIN; // 다시 꺼내러 뒤로 되돌림
                }
            }
            else
            {
                Debug.Log(member.name + "의 재고가 다 떨어짐");
                Transform memberPool = GetContainer(member);

                outObject = Instantiate(member, memberPool);
                outObject.name = member.name;

                outObject.RememberPool(this);
            }
        }
        else
        {
            Debug.Log(member.name + "이라는 오브젝트가 이 오브젝트풀에 없습니다.");

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
            //Debug.Log(member.name + " 오브젝트 반납)" + pool.TryPeek(out _));
            return true;
        }
        else
        {
            Debug.LogError(member.name + " 오브젝트용 전용 풀이 없습니다.");
            return false;
        }
    }
}
