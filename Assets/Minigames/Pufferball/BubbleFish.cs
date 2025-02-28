using Unity.Netcode;

public class BubbleFish : NetworkBehaviour
{
    private void Awake()
    {
        var throwFish = GetComponent<ThrowFish>();
        throwFish.OnThrowComplete += OnThrowComplete;
    }

    private void OnThrowComplete()
    {
        if (IsOwner)
        {

        }
    }
}
