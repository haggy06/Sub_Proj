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

    protected bool isReturned = false;
    public void RememberPool(Stack<PoolObject> connectedPool, Transform myPoolTrans) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.connectedPool = connectedPool;
        this.myPoolTrans = myPoolTrans;
    }

    public virtual void ReturnToPool()
    {
        isReturned = true;
        StopCoroutine("AutoReturn"); // 자동 리턴 취소

        transform.parent = myPoolTrans;
        connectedPool.Push(this);

        gameObject.SetActive(false);
    }
    
    public virtual void ExitFromPool(Transform newParent = null)
    {
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