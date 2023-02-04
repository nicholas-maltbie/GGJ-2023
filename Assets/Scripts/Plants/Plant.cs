using System.Resources;
using System.Runtime.Versioning;
using System.Reflection;
using System.Diagnostics;
using System.Net.Http.Headers;
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

using nickmaltbie.IntoTheRoots.Player;
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    public enum PlantType
    {
        Producer,
        Tree,
        Vegetable,
    }

    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]
    public class Plant : NetworkBehaviour
    {
        /// <summary>
        /// Name of this type of plant.
        /// </summary>
        [SerializeField]
        public PlantType plantType;

        /// <summary>
        /// Cost to place the plant.
        /// </summary>
        [SerializeField]
        public ResourceValues cost;

        /// <summary>
        /// Production of the plant.
        /// </summary>
        [SerializeField]
        public ResourceValues production;

        /// <summary>
        /// Interval between producing resources.
        /// </summary>
        [SerializeField]
        public float produceInterval = 1.0f;

        /// <summary>
        /// Restricted radius for building around this plant.
        /// </summary>
        [SerializeField]
        public int restrictedDistance = 2;

        /// <summary>
        /// Victory points for this plant type.
        /// </summary>
        [SerializeField]
        public int victoryPoints = 1;

        /// <summary>
        /// Grow range for improving the range of this plant.
        /// </summary>
        [SerializeField]
        public int growRange = 0;

        /// <summary>
        /// Elapsed time since this plant has produced anything.
        /// </summary>
        private float elapsedSinceProduced;

        /// <summary>
        ///  Seed Particle System
        /// </summary>
        public ParticleSystem seedParticle;

        /// <summary>
        ///  Sun Particle System
        /// </summary>
        public ParticleSystem sunParticle;

        /// <summary>
        ///  Water Particle System
        /// </summary>
        public ParticleSystem waterParticle;

        public void Start()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Collider2D collider = GetComponent<Collider2D>();
            sr.sortingOrder = -Mathf.RoundToInt(collider.bounds.min.y * 100);
        }

        public void Update()
        {
            if (!IsServer)
            {
                return;
            }

            elapsedSinceProduced += Time.deltaTime;

            while (elapsedSinceProduced > produceInterval)
            {
                Produce();
                elapsedSinceProduced -= produceInterval;
            }
        }

        public void Produce()
        {
            var resources = PlayerResources.GetResources(OwnerClientId);

            if (resources == null)
            {
                return;
            }

            foreach ((Resource, int) produced in production.EnumerateResources())
            {
                resources.AddResources(produced.Item1, produced.Item2);
                //Produce corresponding resource particle effect
                var resourceParticle = new ParticleSystem();
                switch(produced.Item1) {
                    case Resource.Seeds:
                        resourceParticle = seedParticle;
                        break;
                    case Resource.Sun:
                        resourceParticle = sunParticle;
                        break;
                    case Resource.Water:
                        resourceParticle = waterParticle;
                        break;
                }
                resourceParticle.Play();
            }
        }
    }
}
