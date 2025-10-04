using System.Collections;
using UnityEngine;

public class PassTheSpore : ActivityController<PassTheSporeUnit>
{
    [Header("References")]
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private Transform sporeBall;

    private int currentUnitIndex;
    private PassTheSporeUnit currentUnit;

    protected override void OnActivityStart()
    {
        base.OnActivityStart();

        currentUnitIndex = 0;
        currentUnit = Units[currentUnitIndex];

        sporeBall.position = currentUnit.transform.position + Vector3.up;
        sporeBall.gameObject.SetActive(true);

        StartCoroutine(ActivityRoutine());
    }

    private IEnumerator ActivityRoutine()
    {
        while (true)
        {
            yield return currentUnit.PassRoutine();

            currentUnitIndex = (currentUnitIndex + 1) % Units.Count;
            currentUnit = Units[currentUnitIndex];

            yield return TossBall(sporeBall.position, currentUnit, djReference.BeatDuration * 2);

            currentUnit.GiveSpore(sporeBall);
        }
    }

    private IEnumerator TossBall(Vector3 start, PassTheSporeUnit target, float duration)
    {
        float elapsed = 0f;

        // Optional: add an arc height for a nicer toss
        float arcHeight = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Lerp position
            Vector3 horizontal = Vector3.Lerp(start, target.SporePosition, t);

            // Add simple vertical arc (parabola)
            float height = Mathf.Sin(t * Mathf.PI) * arcHeight;
            sporeBall.position = horizontal + Vector3.up * height;

            yield return null;
        }
    }

    protected override void OnActivityEnded()
    {
        base.OnActivityEnded();
        StopAllCoroutines();
        sporeBall.gameObject.SetActive(false);
    }

   
}
