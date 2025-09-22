using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJTracklistUI : MonoBehaviour
{
    [SerializeField] private DJTableReference djReference;
    [SerializeField] private DJTracklistTrackUI trackViewPrefab;

    private List<DJTracklistTrackUI> allTrackViews = new List<DJTracklistTrackUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, allTrackViews);
    }

    private void Start()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        int itemCount = djReference.Tracks.Count;

        var sortedItems = djReference.Tracks;

        // Ensure we have enough views
        while (allTrackViews.Count < itemCount)
        {
            // Instantiate new ItemView if needed
            var newView = Instantiate(trackViewPrefab, transform);
            allTrackViews.Add(newView);
        }

        // Update active views
        for (int i = 0; i < allTrackViews.Count; i++)
        {
            if (i < itemCount)
            {
                allTrackViews[i].SetTrack(sortedItems[i]);
                allTrackViews[i].gameObject.SetActive(true);
            }
            else
            {
                allTrackViews[i].SetTrack(null);
                allTrackViews[i].gameObject.SetActive(false);
            }
        }
    }
}
