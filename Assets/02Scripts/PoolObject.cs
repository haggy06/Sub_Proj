using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PoolObject : MonoBehaviour
{
    private Stack<GameObject> connectedPool;
    private Transform myPoolTrans;
    [SerializeField]
    private float lifeTime = 3f;

    private bool isExit = false;
    public void RememberPool(Stack<GameObject> connectedPool, Transform myPoolTrans) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.connectedPool = connectedPool;
        this.myPoolTrans = myPoolTrans;
    }

    public void ReturnToPool()
    {
        if (isExit)
        {
            transform.parent = myPoolTrans;
            connectedPool.Push(gameObject);

            isExit = false;
            gameObject.SetActive(false);
        }
    }
    
    public void ExitFromPool()
    {
        isExit = true;

        transform.parent = null;

        Invoke("ReturnToPool", lifeTime);
    }
}