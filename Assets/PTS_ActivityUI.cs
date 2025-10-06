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
    [SerializeField] private float loopDuration = 1.2f;


    private List<PTS_Glyph> glyphs = new List<PTS_Glyph>();

    protected override void Awake()
    {
        base.Awake();

        passButton.onClick.AddListener(() =>
        {
            Controller.PassSpore();
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
        if (PlayerIsSelected)
        {
            foreach (var glyph in glyphs)
            {
                Destroy(glyph.gameObject);
            }

            glyphs = new List<PTS_Glyph>();

            Controller.PassSpore();
        }

        base.OnPlayerExit(player);
        virtualCamera.Priority = 0;
        StopAllCoroutines();
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

    private IEnumerator GlyphRoutine()
    {
        if (glyphCount <= 0) yield break;

        // Generate pattern positions (now time-aware for Psychic)
        Vector3[] positions = GeneratePatternPositions(glyphPattern, glyphCount);

        for (int i = 0; i < glyphCount; i++)
        {
            // Spawn glyph
            var glyphController = Instantiate(glyphPrefab, glyphContainer);
            glyphController.InitializeAtPosition(positions[i], glyphContainer);
            glyphController.OnCollected += _ =>
            {
                Player.CollectSpore();
                Controller.SporeController.LightSpore();
                glyphs.Remove(glyphController);
            };

            glyphs.Add(glyphController);

            // Sequential overlap
            yield return new WaitForSeconds(glyphDuration * 0.3f);
        }

        // Wait for all glyphs to finish animating
        yield return new WaitForSeconds(glyphDuration + 0.1f);

        Controller.PassSpore();
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
            case GlyphPattern.Fairy:
                // Spiral pattern
                for (int i = 0; i < count; i++)
                {
                    float t = i / (float)count;
                    float angle = t * Mathf.PI * 4f;
                    float radiusX = Mathf.Lerp(0f, halfWidth, t);
                    float radiusY = Mathf.Lerp(0f, halfHeight, t);
                    float yOffset = Mathf.Lerp(-halfHeight, halfHeight, t);
                    positions[i] = new Vector3(Mathf.Cos(angle) * radiusX, yOffset, Mathf.Sin(angle) * radiusY);
                }
                break;

            case GlyphPattern.Lightning:
                // Jagged zig-zag bolt
                for (int i = 0; i < count; i++)
                {
                    float t = i / (float)(count - 1);
                    float xOffset = Mathf.Sin(t * Mathf.PI * 6f) * halfWidth;
                    float yOffset = Mathf.Lerp(-halfHeight, halfHeight, t);
                    positions[i] = new Vector3(xOffset, yOffset, 0f);
                }
                break;

            case GlyphPattern.Grass:
                // Flowing wave
                for (int i = 0; i < count; i++)
                {
                    float t = i / (float)(count - 1);
                    float xOffset = Mathf.Lerp(-halfWidth, halfWidth, t);
                    float yOffset = Mathf.Sin(t * Mathf.PI * 2f) * (halfHeight * 0.5f);
                    positions[i] = new Vector3(xOffset, yOffset, 0f);
                }
                break;

            case GlyphPattern.Psychic:
                // Circular orbit — time-aware for loop speed
                float radius = Mathf.Min(halfWidth, halfHeight) * 0.8f;
                float totalAngle = psychicLoops * Mathf.PI * 2f;

                for (int i = 0; i < count; i++)
                {
                    float t = i / (float)count;

                    // Base angle based on total loops
                    float angle = t * totalAngle;

                    // Use loopDuration to control angular speed
                    // The faster the duration, the more spread-out the pattern appears over time
                    float angularSpeed = totalAngle / loopDuration;
                    angle = t * angularSpeed * loopDuration;

                    float x = Mathf.Cos(angle) * radius;
                    float y = Mathf.Sin(angle) * radius;
                    positions[i] = new Vector3(x, y, 0f);
                }
                break;
        }

        return positions;
    }



}
