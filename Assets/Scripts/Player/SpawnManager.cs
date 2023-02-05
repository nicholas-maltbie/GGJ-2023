// Copyright (C) 2023 Nicholas Maltbie
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and
// associated documentation files (the "Software"), to deal in the Software without restriction,
// including without limitation the rights to use, copy, modify, merge, publish, distribute,
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
// BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class SpawnManager : NetworkBehaviour
    {
        public static SpawnManager Singleton { get; private set; }

        public PlantDatabase plantDatabase;

        public Plant treePrefab;
        public Plant sunflowerPrefab;
        public Plant watermelonPrefab;

        public void Start()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
        }

        public void CreateStartingPlantsForPlayer(ulong clientId)
        {
            if (!IsServer)
            {
                return;
            }

            // When the player spawns in, teleport them to a unique
            // position and spawn some prefabs for them.
            PlayerMovement movement = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(clientId).GetComponent<PlayerMovement>();

            var spawnPos = new Vector2(
                Random.value * 100 - 50,
                Random.value * 100 - 50);

            movement.TeleportPlayerViaServer(spawnPos);

            GameObject tree = Instantiate(treePrefab.gameObject, spawnPos, Quaternion.identity);
            GameObject sunflower = Instantiate(sunflowerPrefab.gameObject, spawnPos + Vector2.right * 2, Quaternion.identity);
            GameObject watermelon = Instantiate(watermelonPrefab.gameObject, spawnPos + Vector2.left * 2, Quaternion.identity);

            tree.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            sunflower.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);
            watermelon.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);

            PlantUtils.SpawnRootBetweenPlants(plantDatabase.rootPrefab.gameObject, tree.GetComponent<Plant>(), sunflower.GetComponent<Plant>(), OwnerClientId);
            PlantUtils.SpawnRootBetweenPlants(plantDatabase.rootPrefab.gameObject, tree.GetComponent<Plant>(), watermelon.GetComponent<Plant>(), OwnerClientId);
        }
    }
}
