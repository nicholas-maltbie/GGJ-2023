


using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class SpawnManager : NetworkBehaviour
    {
        public Plant treePrefab;
        public Plant sunflowerPrefab;
        public Plant watermelonPrefab;

        public void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
            NetworkManager.Singleton.OnServerStarted += () =>
            {
                if (IsHost)
                {
                    SpawnPlayer(OwnerClientId);
                }
            };
        }

        public void SpawnPlayer(ulong clientId)
        {
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }

            // When the player spawns in, teleport them to a unique
            // position and spawn some prefabs for them.
            PlayerMovement movement = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<PlayerMovement>();

            Vector2 spawnPos = new Vector2(
                Random.value * 100 - 50,
                Random.value * 100 - 50);

            movement.TeleportPlayerViaServer(spawnPos);

            GameObject tree = Instantiate(treePrefab.gameObject, spawnPos, Quaternion.identity);
            GameObject sunflower = Instantiate(sunflowerPrefab.gameObject, spawnPos + Vector2.right * 2, Quaternion.identity);
            GameObject watermelon = Instantiate(watermelonPrefab.gameObject, spawnPos + Vector2.left * 2, Quaternion.identity);

            tree.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            sunflower.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            watermelon.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            
        }
    }
}