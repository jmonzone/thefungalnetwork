using System.Collections;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Controller playerReference;
    [SerializeField] private Attackable boss;
    [SerializeField] private ViewReference resultView;

    private void Awake()
    {
        boss.OnDeath += ShowResults;
    }

    private void OnEnable()
    {
        playerReference.OnDeath += ShowResults;
    }

    private void OnDisable()
    {
        playerReference.OnDeath -= ShowResults;
    }

    private void ShowResults()
    {
        StartCoroutine(WaitToShowResults());
    }

    private IEnumerator WaitToShowResults()
    {
        yield return new WaitForSeconds(2f);
        playerReference.Movement.Stop();
        resultView.RequestShow();
    }
}
