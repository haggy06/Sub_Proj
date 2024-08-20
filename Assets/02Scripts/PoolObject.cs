using System.Collections;
using UnityEngine;
public class PoolObject : MonoBehaviour
{
    protected ObjectPool parentPool;

    [SerializeField]
    protected Color initialColor = Color.white;

    [Space(10)]
    [SerializeField]
    protected bool useAutoReturn = true;
    [SerializeField]
    protected float lifeTime = 3f;

    [Space(10)]
    [SerializeField]
    protected bool isIndependent = true;

    [Space(10)]
    [SerializeField, Tooltip("Rigidbody2D가 있을 떄만 작동")]
    protected float initialVelocity;

    protected bool isReturned = false;
    public bool IsReturned => isReturned;

    [Space(10)]
    [SerializeField]
    protected DetectionBase connectedDetection;
    [SerializeField, Tooltip("dectionBase가 있을 떄만 작동")]
    protected float detectionTime = 0f;
    protected virtual void Awake()
    {
        if (connectedDetection != null)
        {
            connectedDetection.DetectionStartEvent += DetectionStart;
            connectedDetection.DetectionEndEvent += DetectionEnd;
            connectedDetection.HitGroundEvent += HitGround;
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

    public void RememberPool(ObjectPool parentPool) // *Dictionary의 value로 있는 Stack을 받기 때문에 오류가 날 가능성이 있음
    {
        this.parentPool = parentPool;
    }

    public virtual void ReturnToPool()
    {
        if (parentPool == null)
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

        parentPool.ReturnObject(this);
    }
    
    public virtual void Init(Transform owner, float rotation)
    {
        StopCoroutine("AutoReturn"); // 자동 리턴 취소
        if (TryGetComponent<SpriteRenderer>(out SpriteRenderer sRenderer))
            sRenderer.color = initialColor;

        gameObject.SetActive(true);
        isReturned = false;

        transform.parent = isIndependent ? null : owner;
        transform.position = owner ? owner.position : Vector3.zero;
        transform.eulerAngles = Vector3.forward * rotation;

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigid2D))
            rigid2D.velocity = MyCalculator.Deg2Vec(rotation) * initialVelocity;

        if (connectedDetection)
        {
            connectedDetection.enabled = true;
            Invoke("DetectionOFF", detectionTime);
        }

        if (useAutoReturn)
            StartCoroutine("AutoReturn"); // 자동 리턴 시작
    }
    private void DetectionOFF()
    {
        if (gameObject.activeInHierarchy)
            connectedDetection.enabled = false;
    }

    private IEnumerator AutoReturn()
    {
        yield return YieldReturn.WaitForSeconds(lifeTime);

        if (!isReturned)
            ReturnToPool();
    }
}