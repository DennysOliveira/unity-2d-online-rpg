// Grid Based proximity checker.

using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkIdentity))]
public class NetworkProximityGridChecker : NetworkVisibility
{
    // static variables common across all grid checkers
    // view range has to be static becuase we need the same for everyone
    public static int visRange = 100;

    // if we see 8 neighbors, then 1 entry is visRange/3
    public static int resolution => visRange / 3;

    // the grid
    static Grid2D<NetworkConnection> grid = new Grid2D<NetworkConnection>();

    [TooltipAttribute("How often (in seconds) that this object should update the set of players that can see it.")]
    public float visUpdateInterval = 1; // in seconds

    [TooltipAttribute("Enable to force this object to be hidden from players.")]
    public bool forceHidden;

    /// <summary>
    /// Enumeration of methods to use to check proximity.
    /// </summary>
    public enum CheckMethod
    {
        XZ_FOR_3D,
        XY_FOR_2D
    }


    [TooltipAttribute("Which method to use for checking proximity of players.")]
    public CheckMethod checkMethod = CheckMethod.XY_FOR_2D;

    // previous position in the grid
    Vector2Int previous = new Vector2Int(int.MaxValue, int.MaxValue);

    // from original checker
    float m_VisUpdateTime;


    // called when a new player enters
    public override bool OnCheckObserver(NetworkConnection newObserver)
    {
        if (forceHidden)
            return false;

        // calculate projected positions
        Vector2Int projected = ProjectToGrid(transform.position);
        Vector2Int observerProjected = ProjectToGrid(newObserver.identity.transform.position);

        // distance needs to be at max one of the 8 neighbors, which is
        //   1 for the direct neighbors
        //   1.41 for the diagonal neighbors (= sqrt(2))
        // => use sqrMagnitude and '2' to avoid computations. same result.
        return (projected - observerProjected).sqrMagnitude <= 2;

        
    }

    Vector2Int ProjectToGrid(Vector3 position)
    {
        if (checkMethod == CheckMethod.XZ_FOR_3D)
        {
            return Vector2Int.RoundToInt(new Vector2(position.x, position.z) / resolution);
        }
        else
        {
            return Vector2Int.RoundToInt(new Vector2(position.x, position.y) / resolution);
        }
    }

    void Update()
    {
        if(!NetworkServer.active) 
            return;

        if(connectionToClient != null)
        {
            // calculate current grid position
            Vector2Int current = ProjectToGrid(transform.position);

            // changed since last time?
            if (current != previous)
            {
                // update position in grid
                grid.Remove(previous, connectionToClient);
                grid.Add(current, connectionToClient);

                // save as previous
                previous = current;
            }

            // possibly rebuild AFTER updating position in grid, so it's always up
            // to date. otherwise player might have moved and not be in current grid
            // hence OnRebuild wouldn't even find itself there
            if (Time.time - m_VisUpdateTime > visUpdateInterval)
            {
                netIdentity.RebuildObservers(false);
                m_VisUpdateTime = Time.time;
            }
        }

    }

    void OnDestroy()
    {
        if(connectionToClient != null && connectionToClient.identity == netIdentity)
            grid.Remove(ProjectToGrid(transform.position), connectionToClient);
    }

    public override void OnRebuildObservers(HashSet<NetworkConnection> observers, bool initial)
    {
        // if force hidden then return without adding any observers.
        if (forceHidden)
            return;

        // add everyone in 9 neighbour grid
        // -> pass observers to GetWithNeighbours directly to avoid allocations
        //    and expensive .UnionWith computations.
        Vector2Int current = ProjectToGrid(transform.position);
        grid.GetWithNeighbours(current, observers);
    }

    // called hiding and showing objects on the host
    public override void OnSetHostVisibility(bool visible)
    {
        foreach (Renderer rend in GetComponentsInChildren<Renderer>())
        {
            rend.enabled = visible;
        }
    }
}
