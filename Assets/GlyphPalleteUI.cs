using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GlyphPalleteUI : MonoBehaviour
{
    [SerializeField] private Transform glyphAnchor;

    [SerializeField] private Image glyphImage;
    [SerializeField] private TextMeshProUGUI fungalText;
    [SerializeField] private VertexGradient normalColor;
    [SerializeField] private VertexGradient blurColor;

    private List<Button> glyphButtons = new List<Button>();

    public event UnityAction OnGlyphReleased;

    private void Awake()
    {
        glyphAnchor.GetComponentsInChildren(includeInactive: true, glyphButtons);

        Debug.Log(glyphButtons.Count);
        foreach(var button in glyphButtons)
        {
            button.onClick.AddListener(() => OnGlyphSelected());
        }
    }

    private void OnGlyphSelected()
    {
        Debug.Log("clicked");
        glyphImage.enabled = false;
        fungalText.colorGradient = normalColor;
        glyphAnchor.gameObject.SetActive(false);
        OnGlyphReleased?.Invoke();
    }

    public void ShowGlyph()
    {
        glyphImage.enabled = true;
        fungalText.colorGradient = blurColor;
        glyphAnchor.gameObject.SetActive(true);
    }
}
