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
using nickmaltbie.IntoTheRoots.Plants;
using nickmaltbie.IntoTheRoots.Player;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.UI
{
    public class ResourceCounterManager : MonoBehaviour
    {
        private Resource[] trackedResources = System.Enum.GetValues(typeof(Resource)) as Resource[];
        public ResourceCounter resourceCounterPrefab;
        public int bufferPixels = 5;

        private Dictionary<Resource, ResourceCounter> counters = new Dictionary<Resource, ResourceCounter>();

        public void Awake()
        {
            float height = resourceCounterPrefab.GetComponent<RectTransform>().sizeDelta.y;

            // Within this object, spawn a resource counter
            // and space them out by buffer pixels vertically
            // for each resource specified.
            for (int index = 0; index < trackedResources.Length; index++)
            {
                // Spawn each one at the height of the previous + 5 * index
                Resource resource = trackedResources[index];
                GameObject go = Instantiate(resourceCounterPrefab.gameObject, transform.position, Quaternion.identity);
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(0, -(height + bufferPixels) * index);
                counters[resource] = go.GetComponent<ResourceCounter>();
                counters[resource].resource = resource;
            }
        }

        public void Update()
        {
            var resources = PlayerResources.GetLocalPlayerResources();

            if (resources == null)
            {
                return;
            }

            foreach (Resource resource in trackedResources)
            {
                ResourceCounter counter = counters[resource];

                if (resource != Resource.VP)
                {
                    counter.UpdateMax(resources.GetResourceMax(resource));
                    counter.UpdateCurrent(resources.GetResourceCount(resource));
                }
                else
                {
                    counter.UpdateCurrent(resources.GetVictoryPoints());
                }
            }
        }
    }
}
