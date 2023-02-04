
using System.Collections.Generic;
using nickmaltbie.IntoTheRoots.Plants;
using Unity.Netcode;

namespace nickmaltbie.IntoTheRoots.Player
{
    public class PlayerResources : NetworkBehaviour
    {
        Dictionary<Resource, int> resources = new Dictionary<Resource, int>();
    }
}
