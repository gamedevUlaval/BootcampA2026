using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawner : MonoBehaviour
{
    public WorldBounds worldBounds;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int playerIndex = playerInput.playerIndex;
        Rect spawnArea = worldBounds.GetSpawnArea(playerIndex);

        float spawnX = spawnArea.x + (spawnArea.width / 2f);
        float spawnZ = spawnArea.y + (spawnArea.height / 2f);

        CharacterController controller = playerInput.GetComponent<CharacterController>();
        controller.enabled = false;
        playerInput.transform.position = new Vector3(spawnX, 0f, spawnZ);
        controller.enabled = true;
    }
}