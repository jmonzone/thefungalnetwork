using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTree : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private float baseSpeedX = 2f; // Base speed for X-axis
    [SerializeField] private float baseSpeedY = 2f; // Base speed for Y-axis
    [SerializeField] private float waveFrequencyX = 1f; // Frequency for X-axis movement
    [SerializeField] private float waveFrequencyY = 1f; // Frequency for Y-axis movement
    [SerializeField] private float waveAmplitudeX = 1f; // Amplitude for X-axis movement
    [SerializeField] private float waveAmplitudeY = 1f; // Amplitude for Y-axis movement

    void Update()
    {
        // Calculate the cosine wave value for the X-axis
        float waveX = Mathf.Cos(Time.time * waveFrequencyX) * waveAmplitudeX;

        // Calculate the sine wave value for the Y-axis
        float waveY = Mathf.Sin(Time.time * waveFrequencyY) * waveAmplitudeY;

        // Compute dynamic speeds for X and Y
        float dynamicSpeedX = baseSpeedX + waveX;
        float dynamicSpeedY = baseSpeedY + waveY;

        // Update the texture offset independently for X and Y
        var targetOffset = material.mainTextureOffset;
        targetOffset.x += dynamicSpeedX * Time.deltaTime;
        targetOffset.y += dynamicSpeedY * Time.deltaTime;
        material.mainTextureOffset = targetOffset;

        //transform.Rotate(Vector3.down, 50f * Time.deltaTime);

    }


}
