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
        public void Start()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            Collider2D collider = GetComponent<Collider2D>();
            sr.sortingOrder = -Mathf.RoundToInt(collider.bounds.min.y * 100);
        }

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
        /// Restricted radius for building around this plant.
        /// </summary>
        [SerializeField]
        public int restrictedDistance;

        /// <summary>
        /// Victory points for this plant type.
        /// </summary>
        [SerializeField]
        public int victoryPoints;

        /// <summary>
        /// Grow range for improving the range of this plant.
        /// </summary>
        [SerializeField]
        public int growRange;
    }
}