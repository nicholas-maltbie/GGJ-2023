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

namespace nickmaltbie.IntoTheRoots.Player
{
    public class PlayerResources : NetworkBehaviour
    {
        public ResourceValues startingResources;
        public ResourceValues startingMax;

        private NetworkVariable<int> water;
        private NetworkVariable<int> sun;
        private NetworkVariable<int> seeds;
        private NetworkVariable<int> vp = new NetworkVariable<int>(value: 0, readPerm: NetworkVariableReadPermission.Everyone);
        private NetworkVariable<int> maxWater;
        private NetworkVariable<int> maxSun;
        private NetworkVariable<int> maxSeeds;

        public void Awake()
        {
            water = new NetworkVariable<int>(
                value: startingResources.GetResourceValue(Resource.Water),
                readPerm: NetworkVariableReadPermission.Everyone
            );
            sun = new NetworkVariable<int>(
                value: startingResources.GetResourceValue(Resource.Sun),
                readPerm: NetworkVariableReadPermission.Everyone
            );
            seeds = new NetworkVariable<int>(
                value: startingResources.GetResourceValue(Resource.Seeds),
                readPerm: NetworkVariableReadPermission.Everyone
            );
            maxWater = new NetworkVariable<int>(
                value: startingMax.GetResourceValue(Resource.Water),
                readPerm: NetworkVariableReadPermission.Everyone
            );
            maxSun = new NetworkVariable<int>(
                value: startingMax.GetResourceValue(Resource.Sun),
                readPerm: NetworkVariableReadPermission.Everyone
            );
            maxSeeds = new NetworkVariable<int>(
                value: startingMax.GetResourceValue(Resource.Seeds),
                readPerm: NetworkVariableReadPermission.Everyone
            );
        }

        protected NetworkVariable<int> GetNetworkResourceCount(Resource resource)
        {
            switch (resource)
            {
                case Resource.Water:
                    return water;
                case Resource.Sun:
                    return sun;
                case Resource.Seeds:
                    return seeds;
                case Resource.VP:
                    return vp;
                default:
                    return null;
            }
        }

        protected NetworkVariable<int> GetNetworkResourceMax(Resource resource)
        {
            switch (resource)
            {
                case Resource.Water:
                    return maxWater;
                case Resource.Sun:
                    return maxSun;
                case Resource.Seeds:
                    return maxSeeds;
                case Resource.VP:
                    return vp;
                default:
                    return null;
            }
        }

        public bool HasResources(Resource resource, int value)
        {
            return GetResourceCount(resource) >= value;
        }

        public void RemoveResources(Resource resource, int toRemove)
        {
            SetResourceCount(resource, GetResourceCount(resource) - toRemove);
        }

        public void AddResources(Resource resource, int toAdd)
        {
            SetResourceCount(resource, GetResourceCount(resource) + toAdd);
        }

        public void SetResourceMax(Resource resource, int max)
        {
            GetNetworkResourceMax(resource).Value = max;
        }

        public void SetResourceCount(Resource resource, int value)
        {
            NetworkVariable<int> stored = GetNetworkResourceCount(resource);
            int max = GetResourceMax(resource);

            if (value > max)
            {
                value = max;
            }

            stored.Value = value;
        }

        public int GetResourceCount(Resource resource)
        {
            return GetNetworkResourceCount(resource).Value;
        }

        public int GetResourceMax(Resource resource)
        {
            return GetNetworkResourceMax(resource).Value;
        }

        public static PlayerResources GetLocalPlayerResources()
        {
            if (NetworkManager.Singleton?.SpawnManager?.GetLocalPlayerObject() is NetworkObject obj)
            {
                return obj.GetComponent<PlayerResources>();
            }

            return null;
        }

        public static PlayerResources GetResources(ulong clientId)
        {
            if (NetworkManager.Singleton?.SpawnManager?.GetPlayerNetworkObject(clientId) is NetworkObject obj)
            {
                return obj.GetComponent<PlayerResources>();
            }

            return null;
        }

        public static void AddVictoryPoints(ulong clientId, int points)
        {
            GetResources(clientId).vp.Value += points;
        }

        public int GetVictoryPoints()
        {

            return GetResources(OwnerClientId).vp.Value;
        }
    }
}
