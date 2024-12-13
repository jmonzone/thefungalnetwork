using System.Collections;
using TMPro;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Controller playerReference;
    [SerializeField] private Attackable boss;
    [SerializeField] private ViewReference resultView;
    [SerializeField] private TextMeshProUGUI resultHeader;
    [SerializeField] private Color winResultColor;
    [SerializeField] private Color loseResultColor;

    private void Awake()
    {
        boss.OnDeath += OnBossDeath;
    }

    private void OnEnable()
    {
        playerReference.OnDeath += OnPlayerDeath;
    }

    private void OnDisable()
    {
        playerReference.OnDeath -= OnPlayerDeath;
    }

    private void OnBossDeath()
    {
        resultHeader.text = "Bog Unclogged!";
        resultHeader.color = winResultColor;
        ShowResults();
    }

    private void OnPlayerDeath()
    {
        resultHeader.text = "Bogged Down?";
        resultHeader.color = loseResultColor;
        ShowResults();
    }
    private void ShowResults()
    {
        playerReference.Movement.Stop();
        playerReference.Movement.enabled = false;
        StartCoroutine(WaitToShowResults());
    }

    private IEnumerator WaitToShowResults()
    {
        yield return new WaitForSeconds(2f);
        resultView.RequestShow();
    }
}
