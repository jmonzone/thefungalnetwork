using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

[CreateAssetMenu]
public class StoryReference : ScriptableObject
{
    [Header("References")]
    [SerializeField] private LocalData localData;
    [SerializeField] private List<StoryData> allStories;

    [Header("Runtime")]
    [SerializeField] private List<StoryData> completedStories;

    public List<StoryData> CompletedStories => completedStories;

    private const string STORY_KEY = "story";

    public void Initialize()
    {
        try
        {
            completedStories = new List<StoryData>();

            if (localData.JsonFile.ContainsKey(STORY_KEY))
            {
                foreach (var json in localData.JsonFile[STORY_KEY] as JArray)
                {
                    if (json is JObject storyJson)
                    {
                        var storyData = allStories.Find(story => story.Id == storyJson["id"].ToString());
                        completedStories.Add(storyData);
                    };
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void SaveData()
    {
        var storyJson = new JArray();

        foreach (var story in completedStories)
        {
            storyJson.Add(new JObject
            {
                ["id"] = story.Id
            });
        }

        localData.SaveData(STORY_KEY, storyJson);
    }

    public void CompleteStory(StoryData story)
    {
        completedStories.Add(story);
        SaveData();
    }

    public bool HasCompleted(StoryData story)
    {
        return completedStories.Contains(story);
    }
}
