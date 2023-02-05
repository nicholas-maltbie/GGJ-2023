
using System.Collections.Generic;
using System.Linq;
using nickmaltbie.IntoTheRoots.Plants;
using nickmaltbie.IntoTheRoots.Player;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots
{
    public enum GameState
    {
        Lobby,
        Planting,
        Score,
    }

    public class GameLoop : NetworkBehaviour
    {
        public static GameLoop Singleton { get; private set; }

        public GameState CurrentState => gameState.Value;
        public int Winner => winner.Value;

        public float scorePhaseTime = 15.0f;

        private float scoreElapsedTime = 0.0f;

        private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(
            value: GameState.Lobby,
            readPerm: NetworkVariableReadPermission.Everyone
        );

        private NetworkVariable<int> winner = new NetworkVariable<int>(
            value: -1,
            readPerm: NetworkVariableReadPermission.Everyone
        );

        public GameObject planterPlayerPrefab;
        public GameObject lobbyPlayerPrefab;

        public void Start()
        {
            NetworkManager.Singleton.AddNetworkPrefab(planterPlayerPrefab);
            NetworkManager.Singleton.AddNetworkPrefab(lobbyPlayerPrefab);

            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            NetworkManager.Singleton.OnServerStarted += () =>
            {
                StartLobbyState();
            };
            NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerJoin;
        }

        public void Update()
        {
            switch (gameState.Value)
            {
                case GameState.Score:
                    scoreElapsedTime += Time.deltaTime;
                    if (scoreElapsedTime > scorePhaseTime)
                    {
                        StartLobbyState();
                    }

                    break;
            }
        }

        public void OnPlayerJoin(ulong clientId)
        {
            if (!IsServer)
            {
                return;   
            }

            switch (gameState.Value)
            {
                case GameState.Lobby:
                    // Create a lobby player to track total players.
                    var lobbyPlayer = GameObject.Instantiate(lobbyPlayerPrefab);
                    lobbyPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                    break;
                case GameState.Planting:
                    var planterPlayer = GameObject.Instantiate(planterPlayerPrefab);
                    planterPlayer.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
                    SpawnManager.Singleton.CreateStartingPlantsForPlayer(clientId);
                    break;
                case GameState.Score:
                    // do nothing in this state.
                    break;
            }
        }

        public void DestroyAllPlayers()
        {
            // Destroy all currently created players
            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                NetworkObject player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id);
                if (player != null)
                {
                    player.Despawn();
                    Destroy(player);
                }
            }
        }

        public void SpawnPlayerPrefabForEachPlayer(GameObject prefab)
        {
            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                GameObject player = Instantiate(prefab);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(id);
            }
        }

        public void StartLobbyState()
        {
            if (!IsServer)
            {
                return;
            }

            winner.Value = -1;

            // Cleanup any plants and roots owned by players
            IEnumerable<ulong> playerIds = NetworkManager.Singleton.ConnectedClientsIds;
            IEnumerable<NetworkObject> ownedObjects = playerIds.SelectMany(clientId => NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(clientId))
                .Where(obj =>
                    obj.gameObject.GetComponent<Plant>() != null ||
                    obj.gameObject.GetComponent<Root>() != null);

            foreach (NetworkObject obj in ownedObjects)
            {
                obj.Despawn();
                Destroy(obj.gameObject);
            }

            DestroyAllPlayers();

            gameState.Value = GameState.Lobby;
            SpawnPlayerPrefabForEachPlayer(lobbyPlayerPrefab);
        }

        public void StartPlantingState()
        {
            if (!IsServer)
            {
                return;
            }

            DestroyAllPlayers();
            gameState.Value = GameState.Planting;

            SpawnPlayerPrefabForEachPlayer(planterPlayerPrefab);

            foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnManager.Singleton.CreateStartingPlantsForPlayer(id);
            }
        }

        public void StartScoreState()
        {
            if (!IsServer)
            {
                return;
            }

            scoreElapsedTime = 0.0f;
            gameState.Value = GameState.Score;
            winner.Value = 0;
        }
    }
}
