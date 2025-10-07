using UnityEngine;

[CreateAssetMenu]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Element element;
    [SerializeField] private Color primaryColor;
    [SerializeField] private Color secondaryColor;
    [SerializeField] private Color accentColor;

    public Element Element => element;
    public Color PrimaryColor => primaryColor;
    public Color SecondaryColor => secondaryColor;
    public Color AccentColor => accentColor;
}
