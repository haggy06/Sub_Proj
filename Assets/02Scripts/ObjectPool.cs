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

        Transform memberPool = new GameObject().transform;
        memberPool.transform.parent = transform;
        memberPool.name = member.gameObject.name + " Pool";

        Stack<PoolObject> pool;
        if (!objectPool.TryGetValue(member.gameObject.name, out pool)) // 딕셔너리에 알맞는 스택이 있었을 경우 pool에 저장하고, 없었다면 새로 만들어 저장함
        {
            pool = new Stack<PoolObject>();
            objectPool.Add(member.gameObject.name, pool);
        }

        for (int i = 0; i < countPerObj; i++)
        {
            PoolObject newObject = Instantiate(member, memberPool);

            newObject.RememberPool(pool, memberPool);
            newObject.ReturnToPool();
        }
    }
    public PoolObject GetObject(PoolObject member)
    {
        PoolObject outObject;
        if (objectPool.TryGetValue(member.gameObject.name, out Stack<PoolObject> pool))
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
                //Debug.LogError(member.gameObject.name + "의 재고가 다 떨어졌습니다.");
                Transform memberPool = transform.Find(member.gameObject.name + " Pool").transform;

                outObject = Instantiate(member, memberPool);

                outObject.RememberPool(pool, memberPool);
            }

            outObject.ExitFromPool();
        }
        else
        {
            Debug.Log(member.gameObject.name + "이라는 오브젝트가 이 오브젝트풀에 없습니다.");

            MakePool(member);
            objectPool.TryGetValue(member.gameObject.name, out pool);
            
            outObject = pool.Pop();
        }
        return outObject;
    }
    public bool ReturnObject(PoolObject member)
    {
        if (objectPool.TryGetValue(member.gameObject.name, out Stack<PoolObject> pool))
        {
            pool.Push(member);
            return true;
        }
        else
        {
            Debug.LogError(member.gameObject.name + " 오브젝트용 전용 풀이 없습니다.");
            return false;
        }
    }
}
