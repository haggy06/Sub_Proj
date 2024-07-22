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
            if (!objectPool.TryGetValue(member.name, out pool)) // 딕셔너리에 알맞는 스택이 있었을 경우 pool에 저장하고, 없었다면 새로 만들어 저장함
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
            Debug.LogError("풀 멤버 " + member.name + "에게 PoolObject 인터페이스가 구현되어있지 않음");
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
                //Debug.LogError(member.name + "의 재고가 다 떨어졌습니다.");
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
            Debug.LogError(member.name + "이라는 오브젝트가 이 오브젝트풀에 없습니다.");

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
            Debug.LogError(member.name + " 오브젝트용 전용 풀이 없습니다.");
            return false;
        }
    }
}
