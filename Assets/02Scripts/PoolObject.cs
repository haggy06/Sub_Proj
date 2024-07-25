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

    [Space(10)]
    [SerializeField]
    protected DetectionBase connectedDetection;
    protected virtual void Awake()
    {
        if (connectedDetection != null)
        {
            connectedDetection.DetectionStartEvent += DetectionStart;
            connectedDetection.DetectionEndEvent += DetectionEnd;
            connectedDetection.HitGroundEvent += HitGround;
        }
        else
        {
            print(connectedDetection);
        }
    }
    protected virtual void DetectionStart()
    {

    }
    protected virtual void DetectionEnd()
    {

    }
    protected virtual void HitGround(string tag)
    {

    }

    public void RememberPool(Stack<PoolObject> connectedPool, Transform myPoolTrans) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.connectedPool = connectedPool;
        this.myPoolTrans = myPoolTrans;
    }

    public virtual void ReturnToPool()
    {
        if (connectedPool == null)
        {
            Debug.Log(name + "은 부모가 없음");
            Destroy(gameObject);
            return;
        }

        if (isReturned)
        {
            Debug.Log("얜 이미 반납된 오브젝트임");
            return;
        }

        StopCoroutine("AutoReturn"); // 자동 리턴 취소

        gameObject.SetActive(false);
        isReturned = true;

        transform.parent = myPoolTrans;
        connectedPool.Push(this);
    }
    
    public virtual void ExitFromPool(Transform newParent = null)
    {
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