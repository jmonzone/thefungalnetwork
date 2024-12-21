using System.Collections;
using TMPro;
using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private Controller playerReference;
    [SerializeField] private ShruneItem defaultShrune;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private MultiplayerArena multiplayerArena;

    [Header("Results")]
    [SerializeField] private Navigation navigation;
    [SerializeField] private ViewReference resultView;
    [SerializeField] private TextMeshProUGUI resultHeader;
    [SerializeField] private Color winResultColor;
    [SerializeField] private Color loseResultColor;

    private void Start()
    {
        if (itemInventory.GetItemCount(defaultShrune) == 0)
        {
            itemInventory.AddToInventory(defaultShrune, 1);
        }
    }

    private void OnEnable()
    {
        multiplayerArena.OnAllMushroomsCollected += MultiplayerArena_OnAllMushroomsCollected;
        playerReference.OnDeath += OnPlayerDeath;
    }

    private void MultiplayerArena_OnAllMushroomsCollected()
    {
        OnBossDeath();
    }

    private void OnDisable()
    {
        multiplayerArena.OnAllMushroomsCollected -= MultiplayerArena_OnAllMushroomsCollected;
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
        navigation.Navigate(resultView);
    }
}
