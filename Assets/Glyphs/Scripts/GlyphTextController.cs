using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlyphTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void Initialize(string text)
    {
        this.text.text = text;
    }
}
