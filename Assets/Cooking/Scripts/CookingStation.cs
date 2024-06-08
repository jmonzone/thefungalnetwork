using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookingStation : EntityController
{
    [SerializeField] private Sprite actionImage;
    [SerializeField] private Color actionColor;

    public override Sprite ActionImage => actionImage;

    public override Color ActionColor => actionColor;

    public override string ActionText => "Cook";
}
