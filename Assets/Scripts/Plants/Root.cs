
using Unity.Netcode;
using UnityEngine;

namespace nickmaltbie.IntoTheRoots.Plants
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Root : NetworkBehaviour
    {
        public bool successfulCreation;
        public float rootWidth = 0.1f;

        NetworkVariable<NetworkObjectReference> source = new NetworkVariable<NetworkObjectReference>(
            readPerm: NetworkVariableReadPermission.Everyone);
        NetworkVariable<NetworkObjectReference> dest = new NetworkVariable<NetworkObjectReference>(
            readPerm: NetworkVariableReadPermission.Everyone);

        public void SetPath(NetworkObject source, NetworkObject dest)
        {
            this.source.Value = source;
            this.dest.Value = dest;

            RenderPath();
        }

        public void Start()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.enabled = false;
            RenderPath();
        }

        public void Update()
        {
            if (!successfulCreation)
            {
                RenderPath();
            }
        }

        public void RenderPath()
        {
            this.source.Value.TryGet(out NetworkObject source);
            this.dest.Value.TryGet(out NetworkObject dest);

            if (source == null || dest == null)
            {
                // Sometimes on client source or dest will have not
                // spawned yet, so in that case, we will just 
                // need to keep attempting again until the objects
                // have spawned on this client.
                successfulCreation = false;
                sr.enabled = false;
                return;
            }

            // Set the position of this object
            // to be between the first and second object
            float dist = Vector2.Distance(source.transform.position, dest.transform.position);
            transform.position = (source.transform.position + dest.transform.position) / 2;
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.size = new Vector2(1, dist);

            Vector2 delta = source.transform.position - dest.transform.position;
            float angle = -Mathf.Rad2Deg * Mathf.Atan2(delta.x, delta.y);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            BoxCollider2D box = GetComponent<BoxCollider2D>();
            box.size = new Vector2(rootWidth, dist);

            successfulCreation = true;
            sr.enabled = true;
        }
    }
}
