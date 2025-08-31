using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBogRooms : MonoBehaviour
{
    [SerializeField] private List<GameObject> bogrooms;
    [SerializeField] private SceneNavigation sceneNavigation;

    private FadeCanvasGroup transition;

    private void Awake()
    {
        foreach(var room in bogrooms)
        {
            room.SetActive(false);
        }
    }

    public void ShowBogRooms()
    {
        StartCoroutine(BogRoomSequence());
    }

    public IEnumerator BogRoomSequence()
    {
        GameObject previousRoom = null;
        transition = GameManager.Instance.GetComponentInChildren<FadeCanvasGroup>(includeInactive: true);

        foreach(var room in bogrooms)
        {
            yield return transition.FadeIn();
            if (previousRoom) previousRoom.SetActive(false);
            room.SetActive(true);
            previousRoom = room;
            yield return transition.FadeOut();
            yield return new WaitForSeconds(2f);
        }
        sceneNavigation.NavigateToScene(5);
    }
}
