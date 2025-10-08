using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public enum GlyphPattern
{
    Fairy,
    Lightning,
    Grass,
    Psychic
}


public class PTS_ActivityUI : ActivityUI<PTS_Unit, PTS_ActivityController>
{
    [Header("Pass The Spore References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Button passButton;
    [SerializeField] private PTS_Glyph glyphPrefab;
    [SerializeField] private float glyphDuration;
    [SerializeField] private int glyphCount;

    [SerializeField] private RectTransform glyphContainer;
    [SerializeField] private GlyphPattern glyphPattern = GlyphPattern.Fairy;
    [SerializeField, Min(1)] private int psychicLoops = 2; // number of full rotations for psychic pattern


    private List<PTS_Glyph> glyphs = new List<PTS_Glyph>();

    protected override void Awake()
    {
        base.Awake();

        passButton.onClick.AddListener(() =>
        {
            Controller.OnSporeComplete();
        });
    }

    protected override void OnPlayerEnter(ActivityUnit player)
    {
        base.OnPlayerEnter(player);
        virtualCamera.Priority = 11;
        //PlayerReference.TogglePOVCamera(true);
    }

    protected override void OnPlayerExit(ActivityUnit player)
    {
        StopAllCoroutines();

        if (PlayerIsSelected)
        {
            foreach (var glyph in glyphs)
            {
                if (glyph) Destroy(glyph.gameObject);
            }

            glyphs = new List<PTS_Glyph>();

            Controller.OnSporeComplete();
        }

        base.OnPlayerExit(player);
        virtualCamera.Priority = 0;
    }

    protected override void OnUnitSelected(PTS_Unit unit)
    {
        base.OnUnitSelected(unit);
        passButton.interactable = unit.IsPlayer;

        if (unit.IsPlayer)
        {
            StartCoroutine(GlyphRoutine());
        }
    }

    public IEnumerator GlyphRoutine()
    {
        if (glyphCount <= 0) yield break;

        // Compute pattern positions for all glyphs
        Vector3[] positions = GeneratePatternPositions(glyphPattern, glyphCount);

        int collected = 0;
        float spawnDelay = glyphDuration / glyphCount * 0.3f; // proportionate to total duration

        for (int i = 0; i < glyphCount; i++)
        {
            var glyphController = Instantiate(glyphPrefab, glyphContainer);
            glyphController.InitializeAtPosition(positions[i], glyphContainer);

            glyphController.OnCollected += _ =>
            {
                collected++;

                // Trigger mild vibration once per loop
                if (collected >= Mathf.Floor(glyphCount / psychicLoops))
                {
                    collected = 0;
                    Handheld.Vibrate();
                }

                Player.CollectSpore(Controller.SporeController.transform.position + Vector3.up * 0.5f);
                Controller.SporeController.LightSpore();
                glyphs.Remove(glyphController);
            };

            glyphs.Add(glyphController);
            yield return new WaitForSeconds(spawnDelay);
        }

        yield return new WaitForSeconds(glyphPrefab.Duration * 2f);
        Controller.OnSporeComplete();
    }

    private Vector3[] GeneratePatternPositions(GlyphPattern pattern, int count)
    {
        Vector3[] positions = new Vector3[count];
        float width = glyphContainer.rect.width;
        float height = glyphContainer.rect.height;
        float halfWidth = width / 2f;
        float halfHeight = height / 2f;

        switch (pattern)
        {
            case GlyphPattern.Psychic:
                float radius = Mathf.Min(halfWidth, halfHeight) * 0.8f;
                float totalAngle = psychicLoops * Mathf.PI * 2f;

                for (int i = 0; i < count; i++)
                {
                    float t = i / (float)count;
                    float angle = t * totalAngle;

                    float x = Mathf.Cos(angle) * radius;
                    float y = Mathf.Sin(angle) * radius;
                    positions[i] = new Vector3(x, y, 0f);
                }
                break;
        }

        return positions;
    }

}
