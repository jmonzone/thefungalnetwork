using UnityEngine;

[CreateAssetMenu]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private Color primaryColor;
    [SerializeField] private Color secondaryColor;
    [SerializeField] private Color accentColor;

    public string Id => id;
    public Color PrimaryColor => primaryColor;
    public Color SecondaryColor => secondaryColor;
    public Color AccentColor => accentColor;
}
