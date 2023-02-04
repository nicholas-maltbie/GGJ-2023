
using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class Planter : NetworkBehaviour
    {
        public PlantDatabase plantDatabase;
        public Plant toPlant;
        public float cooldown = 1.0f;
        public InputActionReference plantAction;

        private float elapsedSincePlanted = Mathf.Infinity;

        public void Update()
        {
            elapsedSincePlanted += Time.deltaTime;
        }

        public bool CanPlant()
        {
            return elapsedSincePlanted >= cooldown;
        }

        public void Start()
        {
            plantAction.action.Enable();
            plantAction.action.performed += (_) =>
            {
                if (CanPlant())
                {
                    elapsedSincePlanted = 0.0f;
                    SpawnPlantServerRpc(plantDatabase.GetPlantIndex(toPlant));
                }
            };
        }

        [ServerRpc(RequireOwnership = true)]
        public void SpawnPlantServerRpc(int plantIdx)
        {
            Plant plant = plantDatabase.GetPlant(plantIdx);
            GameObject go = Instantiate(plant.gameObject, transform.position, Quaternion.identity);
            go.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
        }
    }
}