using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    public class PlantSetup : MonoBehaviour
    {
        [SerializeField]
        public PlantDatabase plantDatabase;

        public void Start()
        {
            foreach (Plant plant in plantDatabase.plants)
            {
                NetworkManager.Singleton.AddNetworkPrefab(plant.gameObject);
            }
        }
    }
}
