using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public WorldBounds worldBounds;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int playerIndex = playerInput.playerIndex;
        Rect spawnZone = worldBounds.GetSpawnArea(playerIndex);

        float spawnX = spawnZone.x + (spawnZone.width / 2f);
        float spawnZ = spawnZone.y + (spawnZone.height / 2f);

        playerInput.transform.position = new Vector3(spawnX, 0f, spawnZ);
    }
}