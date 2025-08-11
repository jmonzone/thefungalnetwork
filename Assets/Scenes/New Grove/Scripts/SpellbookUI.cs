using System.Collections.Generic;
using UnityEngine;

public class SpellbookUI : MonoBehaviour
{
    [SerializeField] private SpellbookReference spellbookReference;

    private List<SpellbookPageUI> itemViewList = new List<SpellbookPageUI>();

    private void Awake()
    {
        GetComponentsInChildren(true, itemViewList);
    }

    private void OnEnable()
    {
        UpdateView();
        spellbookReference.OnShow += UpdateView;
    }

    private void OnDisable()
    {
        spellbookReference.OnShow -= UpdateView;
    }

    private void UpdateView()
    {
        var i = 0;
        foreach (var itemView in itemViewList)
        {
            if (i < spellbookReference.Items.Count)
            {
                var item = spellbookReference.Items[i];
                itemView.SetItem(item);
            }
            else
            {
                itemView.SetItem(null);
            }

            itemView.gameObject.SetActive(i <= spellbookReference.Items.Count);

            i++;
        }
    }
}
