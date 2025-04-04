using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnnouncementsUI : MonoBehaviour
{
    [SerializeField] private FadeCanvasGroup canvasGroup;
    [SerializeField] private Image player1Image;
    [SerializeField] private Image player2Image;
    [SerializeField] private GameObject player2ImageContainer;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private RectTransform announcmentContainer;
    [SerializeField] private GameReference pufferballReference;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

    private void OnEnable()
    {
        visiblePosition = announcmentContainer.anchoredPosition;
        hiddenPosition = visiblePosition + new Vector3(0, 250f, 0);
        announcmentContainer.anchoredPosition = hiddenPosition;
        announcmentContainer.gameObject.SetActive(true);

        pufferballReference.OnKill += PufferballReference_OnKill;
        pufferballReference.OnSelfDestruct += PufferballReference_OnSelfDestruct;
    }

    private void OnDisable()
    {
        pufferballReference.OnKill -= PufferballReference_OnKill;
        pufferballReference.OnSelfDestruct -= PufferballReference_OnSelfDestruct;
    }

    private class Announcement
    {
        public string Message;
        public GamePlayer Player1;
        public GamePlayer Player2; // Optional for single-player announcements

        public Announcement(string message, GamePlayer player1, GamePlayer player2 = null)
        {
            Message = message;
            Player1 = player1;
            Player2 = player2;
        }

        public bool IsSolo => Player2 == null;
    }


    private Queue<Announcement> announcementQueue = new Queue<Announcement>();
    private bool isAnnouncing = false;


    private void PufferballReference_OnSelfDestruct(GamePlayer player)
    {
        string announcementMessage = $"{player.DisplayName} slipped up";

        announcementQueue.Enqueue(new Announcement(announcementMessage, player));

        if (!isAnnouncing)
            StartCoroutine(ProcessAnnouncements());
    }


    private void PufferballReference_OnKill(int killIndex, int victimIndex)
    {
        var killPlayer = pufferballReference.Players[killIndex];
        var victimPlayer = pufferballReference.Players[victimIndex];

        string announcementMessage = $"{killPlayer.DisplayName} bogged down {victimPlayer.DisplayName}!";

        announcementQueue.Enqueue(new Announcement(announcementMessage, killPlayer, victimPlayer));

        if (!isAnnouncing)
            StartCoroutine(ProcessAnnouncements());
    }

    private IEnumerator ProcessAnnouncements()
    {
        isAnnouncing = true;

        while (announcementQueue.Count > 0)
        {
            if (pufferballReference.isComplete) break;

            var announcement = announcementQueue.Dequeue();

            text.text = announcement.Message;

            // Update player images based on the Player's fungal action image
            player1Image.sprite = announcement.Player1.Fungal.Data.ActionImage;

            if (announcement.Player2 != null)
            {
                player2ImageContainer.SetActive(true);
                player2Image.sprite = announcement.Player2.Fungal.Data.ActionImage;
            }
            else
            {
                player2ImageContainer.SetActive(false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)horizontalLayoutGroup.transform);
            yield return new WaitForEndOfFrame();

            yield return SlideDownAndBoing(announcmentContainer);

            yield return new WaitForSeconds(0.2f);
        }

        isAnnouncing = false;
    }

    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    private IEnumerator SlideDownAndBoing(RectTransform rectTransform)
    {
        Vector3 startPos = hiddenPosition;
        Vector3 endPos = visiblePosition;
        float slideDuration = 0.5f;
        float elapsed = 0f;

        // Start with a little wobble (optional flair)
        rectTransform.localScale = Vector3.one * 1.2f;

        // Slide down into view
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;

            // Ease out: starts fast, ends slow
            float smoothT = Mathf.Sin(t * Mathf.PI * 0.5f);

            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, smoothT);

            yield return null;
        }

        // Small bounce when it lands
        float boingDuration = 0.2f;
        float boingElapsed = 0f;
        Vector3 overshootScale = Vector3.one * 1.3f;

        while (boingElapsed < boingDuration)
        {
            boingElapsed += Time.deltaTime;
            float t = boingElapsed / boingDuration;

            // Bounce squish (ease in-out)
            float bounce = Mathf.Sin(t * Mathf.PI);

            rectTransform.localScale = Vector3.Lerp(Vector3.one, overshootScale, bounce);

            yield return null;
        }

        // Reset scale
        rectTransform.localScale = Vector3.one;

        // Hang out here for 2 seconds
        yield return new WaitForSeconds(2f);

        // Slide back up to hidden position
        elapsed = 0f;
        startPos = rectTransform.anchoredPosition;
        endPos = hiddenPosition;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;

            // Ease in: starts slow, ends fast
            float smoothT = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);

            rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, smoothT);

            yield return null;
        }

        // Optionally reset scale again if you want to be tidy
        rectTransform.localScale = Vector3.one;
    }


}
