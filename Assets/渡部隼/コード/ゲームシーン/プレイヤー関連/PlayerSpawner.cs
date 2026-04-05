using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    private GameObject currentPlayer;

    public void SpawnPlayer(Transform spawnPoint)
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
    }
    public void DespawnPlayer()
{
    GameObject player = GameObject.FindWithTag("Player");

    if (player != null)
    {
        Destroy(player);
    }
}
}