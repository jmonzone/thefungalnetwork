using UnityEngine;
using UnityEngine.UI;

public class SpellbookUI : MonoBehaviour
{
    [SerializeField] private Button backButton;
    [SerializeField] private SpellbookReference spellbookReference;

    private void Awake()
    {
        backButton.onClick.AddListener(spellbookReference.Close);
    }
}
