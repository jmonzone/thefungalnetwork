using UnityEngine;

[CreateAssetMenu]
public class NewFishUpgrade : Upgrade
{
    [SerializeField] private FishData fish;

    public FishData Fish => fish;
    public override Sprite Sprite => fish.Sprite;
}
