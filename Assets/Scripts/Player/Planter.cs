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
