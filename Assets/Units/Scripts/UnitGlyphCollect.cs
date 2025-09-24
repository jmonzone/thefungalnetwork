using UnityEngine;
using UnityEngine.Events;

public class UnitGlyphCollect : MonoBehaviour, INoteTarget
{
    [Header("References")]
    public GlyphCollectController glyphCollectPrefab; // prefab with GlyphCollectReveal script

    [Header("Settings")]
    public int emissionStep;
    public float spiralRadius = 50f;
    public float spiralHeight = 100f;
    public float spiralDuration = 0.8f;
    public float spiralRotations = 2f;

    public float scaleUp = 1.5f;
    public float scaleDuration = 0.5f;
    public float jumpHeight = 0.75f; // max vertical offset above line to head


    int INoteTarget.EmissionStep => emissionStep;

    Transform ITarget.Transform => transform;

    public event UnityAction<DJTrack> OnNoteHit;

    void INoteTarget.OnHit(DJTrack track)
    {
        if (glyphCollectPrefab == null) return;

        // Spawn the prefab in world-space at player position
        var vfx = Instantiate(glyphCollectPrefab, transform.position, Quaternion.identity, transform);

        if (vfx != null)
        {
            // Pass parameters
            vfx.glyphSprite = track.Glyph.Sprite;
            vfx.spiralRadius = spiralRadius;
            vfx.spiralHeight = spiralHeight;
            vfx.spiralDuration = spiralDuration;
            vfx.spiralRotations = spiralRotations;
            vfx.scaleUp = scaleUp;
            vfx.scaleDuration = scaleDuration;
            vfx.jumpHeight = jumpHeight;

            // Start the reveal animation
            StartCoroutine(vfx.PlayReveal());
        }

        OnNoteHit?.Invoke(track);
    }
}