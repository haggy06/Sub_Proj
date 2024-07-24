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
            if (!objectPool.TryGetValue(member.name, out pool)) // 딕셔너리에 알맞는 스택이 있었을 경우 pool에 저장하고, 없었다면 새로 만들어 저장함
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
            Debug.LogError("풀 멤버 " + member.name + "에게 PoolObject 인터페이스가 구현되어있지 않음");
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
                //Debug.LogError(member.name + "의 재고가 다 떨어졌습니다.");
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
            Debug.LogError(member.name + "이라는 오브젝트가 이 오브젝트풀에 없습니다.");

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
            Debug.LogError(member.name + " 오브젝트용 전용 풀이 없습니다.");
            return false;
        }
    }
}
