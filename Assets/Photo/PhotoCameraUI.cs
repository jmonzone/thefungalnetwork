using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCameraUI : MonoBehaviour
{
    [SerializeField] private PhotoReference photoReference;
    [SerializeField] private ViewReference screenshotView;

    [SerializeField] private Button cameraButton;
    [SerializeField] private Button backButton;

    [SerializeField] private Canvas hudCanvas;

    [SerializeField] private RectTransform targetView;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        photoReference.Initialize();

        cameraButton.onClick.AddListener(() =>
        {
            StartCoroutine(CaptureScreenshot());
        });

        backButton.onClick.AddListener(() =>
        {
            photoReference.ExitPhotoView();
        });
    }

    private IEnumerator CaptureScreenshot()
    {
        // Hide UI
        hudCanvas.enabled = false;
        // wait until end of frame so UI has rendered
        yield return new WaitForEndOfFrame();

        Vector3[] worldCorners = new Vector3[4];
        targetView.GetWorldCorners(worldCorners);

        // bottom-left corner
        Vector3 bottomLeft = RectTransformUtility.WorldToScreenPoint(null, worldCorners[0]);
        Vector3 topRight = RectTransformUtility.WorldToScreenPoint(null, worldCorners[2]);

        int width = Mathf.RoundToInt(topRight.x - bottomLeft.x);
        int height = Mathf.RoundToInt(topRight.y - bottomLeft.y);

        // capture pixels in that rect
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(bottomLeft.x, bottomLeft.y, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();
        string path = System.IO.Path.Combine(Application.persistentDataPath, "inMemoryScreenshot.png");
        System.IO.File.WriteAllBytes(path, bytes);

        // assign to RawImage
        photoReference.TakePhoto(tex);

        yield return new WaitForSeconds(1f);
        hudCanvas.enabled = true;

        GetObjectsInScreenshot<UnitController>(targetView, mainCamera);
        //screenshotContainer.SetActive(false);
    }

    public List<T> GetObjectsInScreenshot<T>(RectTransform targetRect, Camera cam) where T : Component
    {
        List<T> foundObjects = new List<T>();
        T[] allObjects = FindObjectsOfType<T>();

        // Get screen-space rect of targetRect
        Rect screenRect = RectTransformToScreenSpace(targetRect);

        foreach (T obj in allObjects)
        {
            Vector3 screenPos = cam.WorldToScreenPoint(obj.transform.position + Vector3.up);

            // Object must be in front of the camera & inside rect
            if (screenPos.z > 0 && screenRect.Contains(screenPos))
            {
                foundObjects.Add(obj);
            }
        }

        Debug.Log($"Found {foundObjects.Count}/{allObjects.Length} objects of type {typeof(T)}");
        return foundObjects;
    }

    // Utility: convert rectTransform into screen-space rect
    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        Vector3[] corners = new Vector3[4];
        transform.GetWorldCorners(corners);
        float xMin = corners[0].x;
        float xMax = corners[2].x;
        float yMin = corners[0].y;
        float yMax = corners[2].y;
        return new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }

}
