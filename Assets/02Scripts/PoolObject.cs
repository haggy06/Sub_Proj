using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoolObject : MonoBehaviour
{
    protected Stack<GameObject> connectedPool;
    protected Transform myPoolTrans;
    [SerializeField]
    protected float lifeTime = 3f;

    protected bool isExit = false;
    public void RememberPool(Stack<GameObject> connectedPool, Transform myPoolTrans) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.connectedPool = connectedPool;
        this.myPoolTrans = myPoolTrans;
    }

    public virtual void ReturnToPool()
    {
        if (isExit)
        {
            transform.parent = myPoolTrans;
            connectedPool.Push(gameObject);

            isExit = false;
            gameObject.SetActive(false);
        }
    }
    
    public virtual void ExitFromPool(Transform newParent = null)
    {
        isExit = true;

        transform.parent = newParent;

        Invoke("ReturnToPool", lifeTime);
    }
}