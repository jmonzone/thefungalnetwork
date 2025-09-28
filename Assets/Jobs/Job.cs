using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Job : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private string actionName;
    [SerializeField] private Color actionColor;
    [SerializeField] private Sprite actionSprite;
    [SerializeField] private ActivityReference activity;


    public string Id => id;
    public string ActionName => actionName;
    public Color ActionColor => actionColor;
    public Sprite ActionSprite => actionSprite;

    public void StartJob(List<UnitController> units)
    {
        activity.StartActivity(units);
    }
}
