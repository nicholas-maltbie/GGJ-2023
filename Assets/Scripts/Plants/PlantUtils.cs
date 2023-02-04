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
