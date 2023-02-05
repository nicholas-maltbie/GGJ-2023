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

using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    public static class PlantUtils
    {
        public static int PlantLayerMask()
        {
            int plantMask = LayerMask.NameToLayer("Plant");

            if (plantMask == -1)
            {
                return Physics2D.AllLayers;
            }

            return Physics2D.GetLayerCollisionMask(plantMask);
        }

        public static IEnumerable<Plant> GetPlantsOwnedByPlayer(ulong owner)
        {
            if (NetworkManager.Singleton == null || NetworkManager.Singleton.SpawnManager is null)
            {
                return Enumerable.Empty<Plant>();
            }

            return NetworkManager.Singleton.SpawnManager.GetClientOwnedObjects(owner)
                .Select(networkObj => networkObj.GetComponent<Plant>())
                .Where(plant => plant != null);
        }

        public static IEnumerable<Plant> GetAllPlayerPlantsOfType(ulong owner, PlantType plantType)
        {
            return GetPlantsOwnedByPlayer(owner).Where(plant => plant.plantType == plantType);
        }

        public static IEnumerable<Plant> GetPlantsInRadius(Vector2 position, float radius, ulong owner, PlantType type)
        {
            return GetPlantsInRadius(position, radius).Where(plant => plant.OwnerClientId == owner && plant.plantType == type);
        }

        public static IEnumerable<Plant> GetPlantsInRadius(Vector2 position, float radius)
        {
            return Physics2D.OverlapCircleAll(position, radius, PlantLayerMask())
                .Select(collider => collider.GetComponent<Plant>())
                .Where(plant => plant != null);
        }

        public static IEnumerable<GameObject> GetPlantsAndRootsInRadius(Vector2 position, float radius)
        {
            return Physics2D.OverlapCircleAll(position, radius, PlantLayerMask())
                .Where(collider =>
                    collider.GetComponent<Plant>() != null ||
                    collider.GetComponent<Root>() != null)
                .Select(collider => collider.gameObject);
        }

        public static void SpawnRootBetweenPlants(GameObject rootPrefab, Plant source, Plant dest, ulong owner)
        {
            // Spawn a root between the closest grow zone and the plant
            var rootGo = GameObject.Instantiate(rootPrefab);
            rootGo.GetComponent<NetworkObject>().SpawnWithOwnership(owner);
            Root root = rootGo.GetComponent<Root>();
            root.SetPath(source.GetComponent<NetworkObject>(), dest.GetComponent<NetworkObject>());
        }

        public static Plant ClosestGrowZone(Vector2 position, ulong owner)
        {
            IEnumerable<Plant> playerGrowZones = GetPlantsOwnedByPlayer(owner)
                .Where(plant => plant.growRange > 0);

            return playerGrowZones.Select(
                growZone =>
                {
                    float dist = Vector2.Distance(position, growZone.transform.position);
                    return (dist, growZone);
                })
                .OrderBy(tuple => tuple.Item1)
                .FirstOrDefault().growZone;
        }

        public static bool IsInGrowRadius(Vector2 position, Plant plant, ulong owner)
        {
            // Get all plants with a non-zero grow range owned
            // by this player
            IEnumerable<Plant> playerGrowZones = GetPlantsOwnedByPlayer(owner)
                .Where(plant => plant.growRange > 0);

            // Check if this plant's radius intersects
            // with the grow range of at least one of those
            // grow zones.
            float radius = plant.Radius();
            return playerGrowZones.Any(growZone =>
            {
                float dist = Vector2.Distance(position, growZone.transform.position);
                return dist <= growZone.growRange + radius;
            });
        }

        public static bool IsPlantPlacementAllowed(Vector2 position, Plant plant, ulong owner)
        {
            float radius = plant.Radius();
            float restrictedRadius = plant.restrictedDistance;
            PlantType restrictedType = plant.plantType;

            // Don't allow placement within restricted range of
            // plants of the same type.
            IEnumerable<Plant> restrictedOverlapping = GetPlantsInRadius(position, restrictedRadius, owner, restrictedType);
            if (restrictedOverlapping.Any())
            {
                return false;
            }

            // Check if the plant is overlapping with anything else
            IEnumerable<GameObject> overlapping = GetPlantsAndRootsInRadius(position, radius);
            if (overlapping.Any())
            {
                return false;
            }

            return true;
        }
    }
}
