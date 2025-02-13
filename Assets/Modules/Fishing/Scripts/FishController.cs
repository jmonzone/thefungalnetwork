using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FishController : MonoBehaviour
{
    [SerializeField] private bool isIdle = true;
    [SerializeField] private float idleTimer;
    [SerializeField] private float minIdleDuration = 2f;
    [SerializeField] private float maxIdleDuration = 5f;
    [SerializeField] private float speed = 2f;

    private Vector3 targetPosition;
    private Collider bounds;

    public event UnityAction OnHit;
    public event UnityAction OnCaught;

    public void Initialize(Collider bounds)
    {
        this.bounds = bounds;

        transform.forward = Utility.RandomXZVector;
        transform.localScale = Vector3.zero;

        IEnumerator LerpScale()
        {
            while (transform.localScale.x < 1)
            {
                var scale = transform.localScale.x + Time.deltaTime;
                transform.localScale = Vector3.one * scale;
                yield return null;
            }

            transform.localScale = Vector3.one;
        }

        StartCoroutine(LerpScale());
    }

    private void Update()
    {
        if (isIdle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > maxIdleDuration)
            {
                isIdle = false;
                targetPosition = bounds.GetRandomXZPosition();
            }
            return;
        }

        var direction = targetPosition - transform.position;

        if (direction.magnitude > 0.05f)
        {
            transform.forward = direction;
            transform.position += speed * Time.deltaTime * direction.normalized;
        }
        else
        {
            idleTimer = Random.Range(0f, maxIdleDuration - minIdleDuration);
            isIdle = true;
        }
    }

    public void Hit()
    {
        OnHit?.Invoke();   
    }

    public void Catch()
    {
        gameObject.SetActive(false);
        OnCaught?.Invoke();
    }
}