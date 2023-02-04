
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    public static class PlantUtils
    {
        public static int PlantLayerMask()
        {
            int mask = LayerMask.NameToLayer("Plant");
            
            if (mask != -1)
            {
                return Physics2D.AllLayers;
            }

            return Physics2D.GetLayerCollisionMask(mask);
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
            IEnumerable<Plant> overlapping = GetPlantsInRadius(position, radius);
            if (overlapping.Any())
            {
                return false;
            }

            return true;
        }
    }
}
