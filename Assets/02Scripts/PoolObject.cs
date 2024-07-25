using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoolObject : MonoBehaviour
{
    protected Stack<PoolObject> connectedPool;
    protected Transform myPoolTrans;

    [Space(10)]
    [SerializeField]
    protected bool useAutoReturn = true;
    [SerializeField]
    protected float lifeTime = 3f;

    [SerializeField]
    protected bool isReturned = false;
    public bool IsReturned => isReturned;
    public void RememberPool(Stack<PoolObject> connectedPool, Transform myPoolTrans) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.connectedPool = connectedPool;
        this.myPoolTrans = myPoolTrans;
    }

    public virtual void ReturnToPool()
    {
        if (isReturned)
        {
            Debug.LogError("얜 이미 반납된 오브젝트임");
            return;
        }

        print("들어감");
        StopCoroutine("AutoReturn"); // 자동 리턴 취소

        gameObject.SetActive(false);
        isReturned = true;

        transform.parent = myPoolTrans;
        connectedPool.Push(this);
    }
    
    public virtual void ExitFromPool(Transform newParent = null)
    {
        print("꺼내짐");
        StopCoroutine("AutoReturn"); // 자동 리턴 취소

        gameObject.SetActive(true);
        isReturned = false;

        transform.parent = newParent;

        if (useAutoReturn)
            StartCoroutine("AutoReturn"); // 자동 리턴 시작
    }

    private IEnumerator AutoReturn()
    {
        yield return YieldReturn.WaitForSeconds(lifeTime);

        ReturnToPool();
    }
}