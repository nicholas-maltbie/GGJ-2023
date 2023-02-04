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
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    [CreateAssetMenu(fileName = "PlantDatabase", menuName = "ScriptableObjects/PlantDatabaseScriptableObject", order = 1)]
    public class PlantDatabase : ScriptableObject
    {
        public Plant[] plants;

        public int GetPlantIndex(Plant plant)
        {
            return GetPlantIndex(plant.name);
        }

        public IEnumerable<Plant> EnumeratePlants()
        {
            return plants;
        }

        public int GetPlantIndex(string plantName)
        {
            for (int i = 0; i < plants.Length; i++)
            {
                if (plantName.Equals(plants[i].name))
                {
                    return i;
                }
            }

            return -1;
        }

        public Plant GetPlant(int index)
        {
            if (index < 0 || index >= plants.Length)
            {
                return null;
            }

            return plants[index];
        }
    }
}
