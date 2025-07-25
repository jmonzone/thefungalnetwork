using UnityEngine;

public class ScrollUnlitBaseMap : MonoBehaviour
{
    public Material material;
    public float scrollSpeedY = 0.2f;

    private Vector2 baseOffset;

    void Start()
    {
        if (material == null)
        {
            Renderer renderer = GetComponentInChildren<Renderer>();
            material = renderer.material;
        }

        baseOffset = material.GetTextureOffset("_BaseMap");
    }

    void Update()
    {
        float offsetY = Mathf.Repeat(Time.time * scrollSpeedY, 1f);
        material.SetTextureOffset("_BaseMap", new Vector2(baseOffset.x, offsetY));
    }
}
