using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementsUI : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup canvasGroup;
    [SerializeField] private Image player1Image;
    [SerializeField] private Image player2Image;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private PufferballReference pufferballReference;

    private void OnEnable()
    {
        pufferballReference.OnKill += PufferballReference_OnKill;
    }

    private void OnDisable()
    {
        pufferballReference.OnKill -= PufferballReference_OnKill;
    }

    private void PufferballReference_OnKill(int killIndex, int victimIndex)
    {
        var killPlayer = pufferballReference.Players[killIndex];
        var victimPlayer = pufferballReference.Players[victimIndex];
        player1Image.sprite = killPlayer.Fungal.Data.ActionImage;
        player2Image.sprite = victimPlayer.Fungal.Data.ActionImage;

        text.text = $"{killPlayer.DisplayName} bogged down {victimPlayer.DisplayName}";
        StopAllCoroutines();
        StartCoroutine(canvasGroup.FadeIn());
    }
}
