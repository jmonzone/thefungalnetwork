using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private PufferballReference pufferball;
    [SerializeField] private List<Color> colors;

    [SerializeField] private RectTransform barContainer;
    [SerializeField] private GameObject segmentPrefab;

    private List<ScoreSegment> segmentControllers = new List<ScoreSegment>();

    private void Awake()
    {
        GetComponentsInChildren(includeInactive: true, segmentControllers);
    }

    private void Start()
    {
        PufferballMinigame_OnScoreUpdated();
    }

    private void OnEnable()
    {
        pufferball.OnScoreUpdated += PufferballMinigame_OnScoreUpdated;
    }

    private void OnDisable()
    {
        pufferball.OnScoreUpdated -= PufferballMinigame_OnScoreUpdated;
    }


    public void PufferballMinigame_OnScoreUpdated()
    {
        Debug.Log("PufferballMinigame_OnScoreUpdated");

        var players = pufferball.Players;
        var totalScore = 100f;

        float barWidth = barContainer.rect.width;

        for (int i = 0; i < players.Count; i++)
        {
            var score = players[i].score;
            float normalizedScore = (float)score / totalScore;

            // Instantiate if necessary
            if (i >= segmentControllers.Count)
            {
                GameObject newSegment = Instantiate(segmentPrefab, barContainer);
                var segmentController = newSegment.AddComponent<ScoreSegment>();
                if (pufferball.Players.Count > i)
                {
                    segmentController.SetFungal(pufferball.Players[i].fungal);
                }
                segmentControllers.Add(segmentController);
            }

            var segment = segmentControllers[i];
            segment.gameObject.SetActive(true);

            // Set color
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null && i < colors.Count)
            {
                segmentImage.color = colors[i];
            }

            // Set target width for smooth transition
            float segmentWidth = barWidth * normalizedScore;
            segment.SetTargetWidth(segmentWidth);
        }

        // Deactivate unused segments
        for (int i = players.Count; i < segmentControllers.Count; i++)
        {
            segmentControllers[i].gameObject.SetActive(false);
        }
    }
}
