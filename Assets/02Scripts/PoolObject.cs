using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoolObject : MonoBehaviour
{
    protected Stack<GameObject> connectedPool;
    protected Transform myPoolTrans;
    [SerializeField]
    protected float lifeTime = 3f;

    public void RememberPool(Stack<GameObject> connectedPool, Transform myPoolTrans) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.connectedPool = connectedPool;
        this.myPoolTrans = myPoolTrans;
    }

    public virtual void ReturnToPool()
    {
        StopCoroutine("AutoReturn"); // 자동 리턴 취소

        transform.parent = myPoolTrans;
        connectedPool.Push(gameObject);

        gameObject.SetActive(false);
    }
    
    public virtual void ExitFromPool(Transform newParent = null)
    {
        transform.parent = newParent;

        StartCoroutine("AutoReturn"); // 자동 리턴 시작
    }

    private IEnumerator AutoReturn()
    {
        yield return YieldReturn.WaitForSeconds(lifeTime);

        ReturnToPool();
    }
}