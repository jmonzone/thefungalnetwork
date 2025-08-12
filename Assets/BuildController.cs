using System.Collections;
using UnityEngine;

public class BuildController : MonoBehaviour
{
    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void StartBuild(LayerMask layerMask)
    {
        StopAllCoroutines();
        StartCoroutine(BuildRoutine(layerMask));
    }

    private IEnumerator BuildRoutine(LayerMask layerMask)
    {
        while (true)
        {
            // Take the center of the screen and move it up by `screenYOffset` pixels
            Vector3 screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f + 25f, 0f);
            Ray ray = mainCamera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                transform.position = hit.point;
            }

            yield return null;
        }
    }

    public void CompleteBuild()
    {
        StopAllCoroutines();
    }
}
