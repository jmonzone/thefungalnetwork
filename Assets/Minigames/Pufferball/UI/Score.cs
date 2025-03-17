using System.Collections;
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
        PufferballMinigame_OnScoreUpdated();
    }

    private void OnDisable()
    {
        pufferball.OnScoreUpdated -= PufferballMinigame_OnScoreUpdated;
    }


    public void PufferballMinigame_OnScoreUpdated()
    {
        Debug.Log("PufferballMinigame_OnScoreUpdated");

        var playerScores = pufferball.Scores;
        var totalScore = Mathf.Max(1, playerScores.Sum());

        float barWidth = barContainer.rect.width;

        for (int index = 0; index < playerScores.Count; index++)
        {
            var score = playerScores[index];
            float normalizedScore = (float)score / totalScore;

            // Instantiate if necessary
            if (index >= segmentControllers.Count)
            {
                GameObject newSegment = Instantiate(segmentPrefab, barContainer);
                var segmentController = newSegment.AddComponent<ScoreSegment>();
                if (pufferball.Players.Count > index)
                {
                    segmentController.SetFungal(pufferball.Players[index]);
                }
                segmentControllers.Add(segmentController);
            }

            var segment = segmentControllers[index];
            segment.gameObject.SetActive(true);

            // Set color
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null && index < colors.Count)
            {
                segmentImage.color = colors[index];
            }

            // Set target width for smooth transition
            float segmentWidth = barWidth * normalizedScore;
            segment.SetTargetWidth(segmentWidth);
        }

        // Deactivate unused segments
        for (int i = playerScores.Count; i < segmentControllers.Count; i++)
        {
            segmentControllers[i].gameObject.SetActive(false);
        }
    }
}
