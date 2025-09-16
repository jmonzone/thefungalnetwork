using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphPalleteUI : MonoBehaviour
{
    [SerializeField] private Transform glyphAnchor;

    private List<Button> glyphButtons = new List<Button>();

    public event UnityAction OnGlyphSelected;

    private void Awake()
    {
        glyphAnchor.GetComponentsInChildren(includeInactive: true, glyphButtons);

        foreach(var button in glyphButtons)
        {
            button.onClick.AddListener(() => OnGlyphSelected?.Invoke());
        }
    }

   
}
