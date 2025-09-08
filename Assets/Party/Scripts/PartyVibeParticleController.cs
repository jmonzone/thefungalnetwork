using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PartyVibeParticleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PartyReference partyReference;
    [SerializeField] private RectTransform targetUI; // assign slider fill or handle
    [SerializeField] private Canvas canvas; // your main UI canvas
    [SerializeField] private GameObject particlePrefab; // whimsical sparkle prefab

    [Header("Settings")]
    [SerializeField] private int burstCount = 6;
    [SerializeField] private float travelTime = 1f;
    [SerializeField] private AnimationCurve arcCurve; // e.g. curve for whimsical arc

    private PartyVibeController partyVibeController;

    public event UnityAction OnParticlesReached;

    private void Awake()
    {
        partyVibeController = GetComponent<PartyVibeController>();
    }

    private void OnEnable()
    {
        partyReference.OnVibeIncreased += BurstFromWorld;
    }

    private void OnDisable()
    {
        partyReference.OnVibeIncreased -= BurstFromWorld;
    }

    //private IEnumerator Start()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(1);
    //        BurstFromWorld(Vector3.zero);
    //    }
    //}

    private int activeParticles; // track how many are active

    public void BurstFromWorld(Vector3 worldPos)
    {
        activeParticles = burstCount; // reset counter

        for (int i = 0; i < burstCount; i++)
        {
            GameObject p = Instantiate(particlePrefab, canvas.transform);
            StartCoroutine(AnimateParticle(p, worldPos));
        }
    }

    private IEnumerator AnimateParticle(GameObject particle, Vector3 worldPos)
    {
        RectTransform canvasRect = canvas.transform as RectTransform;

        // START: convert world position -> canvas local space
        Vector3 screenStart = Camera.main.WorldToScreenPoint(worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenStart,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main,
            out Vector2 localStart
        );

        // Particle rect
        RectTransform rect = particle.GetComponent<RectTransform>();
        rect.SetParent(canvasRect, false); // ensure it's under the canvas root
        rect.anchoredPosition = localStart;

        var image = particle.GetComponentInChildren<Image>();

        float t = 0;
        float randomOffset = Random.Range(-100f, 100f);

        while (t < 1f)
        {
            // END: convert targetUI world position -> canvas local space
            Vector3 screenEnd = RectTransformUtility.WorldToScreenPoint(
                null,
                targetUI.position
            );

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
               canvasRect,
               screenEnd,
               null,
               out Vector2 localEnd
            );

            t += Time.deltaTime / travelTime;

            // Whimsical curve
            Vector2 pos = Vector2.Lerp(localStart, localEnd, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * randomOffset * (1 - t);
            pos.x += Mathf.Sin(t * Mathf.PI) * randomOffset * (1 - t);

            var targetColor = partyVibeController.AnimatedColor;
            targetColor.a = 1 - t;
            image.color = targetColor;

            rect.anchoredPosition = pos;
            yield return null;
        }

        Destroy(particle);

        // decrement active particles
        activeParticles--;

        // if this was the last one -> fire event once
        if (activeParticles <= 0)
        {
            OnParticlesReached?.Invoke();
        }
    }


}
