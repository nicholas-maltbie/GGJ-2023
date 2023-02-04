
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
