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
using UnityEngine.InputSystem;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class PlayerMovement : NetworkBehaviour
    {
        private ClientRpcParams sendToOwner;

        public InputActionReference moveAction;
        public float moveSpeed = 8.0f;

        private NetworkVariable<bool> isFlipped = new NetworkVariable<bool>(
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Owner,
            value: false
        );

        public void Start()
        {
            sendToOwner = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { OwnerClientId }
                }
            };
        }

        // Update is called once per frame
        public void Update()
        {
            if (IsLocalPlayer)
            {
                Vector2 move = moveAction.action.ReadValue<Vector2>();

                if (move.magnitude > 1.0f)
                {
                    move = move.normalized;
                }

                Vector2 delta = move * moveSpeed * Time.deltaTime;

                Vector3 previousPosition = transform.position;
                transform.position += new Vector3(delta.x, delta.y);

                if (Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Wall")) != null)
                {
                    transform.position = previousPosition;
                }

                if (GetComponent<SpriteRenderer>() is SpriteRenderer sr && Mathf.Abs(delta.x) > 0.001f)
                {
                    sr.flipX = delta.x > 0;
                    isFlipped.Value = sr.flipX;
                }
            }
            else // not local player
            {
                GetComponent<SpriteRenderer>().flipX = GetNetworkPlayerFlippedValue().Value;
            }
        }

        public NetworkVariable<bool> GetNetworkPlayerFlippedValue()
        {
            return isFlipped;
        }

        public void TeleportPlayerViaServer(Vector3 position)
        {
            TeleportPlayerClientRpc(position, sendToOwner);
        }

        [ClientRpc]
        public void TeleportPlayerClientRpc(Vector3 position, ClientRpcParams clientRpcParams = default)
        {
            transform.position = position;
        }
    }
}
