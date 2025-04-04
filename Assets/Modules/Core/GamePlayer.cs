using System;

[Serializable]
public class GamePlayer
{
    public ulong ClientId;
    public int Index;
    public string DisplayName;
    public NetworkFungal Fungal;
    public float Score => Fungal.Score;
    public int Lives => Fungal.Lives;
    public bool IsWinner = false;

    public GamePlayer(ulong clientId, int playerIndex, string displayName, NetworkFungal fungal)
    {
        ClientId = clientId;
        Index = playerIndex;
        DisplayName = displayName;
        Fungal = fungal;
    }
}
