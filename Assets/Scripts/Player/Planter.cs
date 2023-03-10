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

using System;
using System.Linq;
using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace nickmaltbie.IntoTheRoots.Player
{
    [RequireComponent(typeof(PlayerResources))]
    public class Planter : NetworkBehaviour
    {
        public PlantDatabase plantDatabase;
        public Plant toPlant;
        public InputActionReference plantAction;

        public void SpendResourcesForPlant(Plant plant)
        {
            ResourceValues cost = plant.cost;
            PlayerResources stored = GetComponent<PlayerResources>();

            foreach (Resource resource in Enum.GetValues(typeof(Resource)))
            {
                stored.RemoveResources(resource, cost.GetResourceValue(resource));
            }
        }

        public bool HasResourcesForPlant(Plant plant)
        {
            ResourceValues cost = plant.cost;
            PlayerResources stored = GetComponent<PlayerResources>();

            foreach (Resource resource in Enum.GetValues(typeof(Resource)))
            {
                int required = cost.GetResourceValue(resource);
                int current = stored.GetResourceCount(resource);

                if (required <= 0)
                {
                    continue;
                }

                if (required > current)
                {
                    return false;
                }
            }

            return true;
        }

        public void Update()
        {
            UpdatePlantDisplay();
        }

        public void UpdatePlantDisplay()
        {
            ResourceValues cost = toPlant.cost;
            PlayerResources stored = GetComponent<PlayerResources>();

            foreach (Resource resource in Enum.GetValues(typeof(Resource)))
            {
                int required = cost.GetResourceValue(resource);
                int current = stored.GetResourceCount(resource);

                if (required > current)
                {
                    // cannot plant
                    PlantDetailsDisplay.Singleton.SetResourceHighlight(resource, true);
                }
                else
                {
                    // can plant
                    PlantDetailsDisplay.Singleton.SetResourceHighlight(resource, false);
                }
            }
        }

        public bool CanPlant(Plant target)
        {
            bool hasResources = HasResourcesForPlant(target);
            bool allowedPlacement = PlantUtils.IsPlantPlacementAllowed(transform.position, target, OwnerClientId);
            bool inGrowZone = PlantUtils.IsInGrowRadius(transform.position, target, OwnerClientId);

            return hasResources && allowedPlacement && inGrowZone;
        }

        public void Start()
        {
            plantAction.action.Enable();
            plantAction.action.performed += OnPlant;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            plantAction.action.performed -= OnPlant;
        }

        public void OnPlant(CallbackContext context)
        {
            if (IsOwner && CanPlant(toPlant))
            {
                SpawnPlantServerRpc(plantDatabase.GetPlantIndex(toPlant));
            }
        }

        [ServerRpc(RequireOwnership = true)]
        public void SpawnPlantServerRpc(int plantIdx)
        {
            Plant plant = plantDatabase.GetPlant(plantIdx);

            if (!CanPlant(plant))
            {
                return;
            }

            // Closest tree
            Plant[] connectedTrees = PlantUtils.AllAvailableGrowZones(transform.position, plant, OwnerClientId).ToArray();

            GameObject plantGo = Instantiate(plant.gameObject, transform.position, Quaternion.identity);
            plantGo.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);

            // Decrease player resources by requested amount
            SpendResourcesForPlant(plant);

            // If vegetable, multiply points by # of connections.
            int points = plant.plantType == PlantType.Vegetable ? plant.victoryPoints * connectedTrees.Length : plant.victoryPoints;
            PlayerResources.AddVictoryPoints(OwnerClientId, points);

            foreach (Plant connected in connectedTrees)
            {
                PlantUtils.SpawnRootBetweenPlants(plantDatabase.rootPrefab.gameObject, connected, plantGo.GetComponent<Plant>(), OwnerClientId);
            }
        }
    }
}
