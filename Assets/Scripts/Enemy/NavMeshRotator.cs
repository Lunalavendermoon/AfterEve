using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;


/* This script builds a NavMesh on the XY plane by temporarily rotating the relevant objects.
   It does so by rotating the plane and the NavMeshManager to align with the XZ plane during the baking process,
   then restoring their original rotations afterward.
*/

// TODO - Attach object to each Room Prefab , add in an Enemy Spawner manager and reference it here
// So enemies are only spawned after NavMesh is baked
public class NavMeshRotator : MonoBehaviour
{
    public GameObject surfaceplane;
    public NavMeshSurface navMeshSurface;
    public GameObject Enemy;

    void Start()
    {
        BuildNav();
      
        //Temp - Enemy Spawn to test
        Enemy= Instantiate(Enemy, new Vector3(0, 0, 19), Quaternion.identity);

    }

    public void BuildNav()
    {
        if (surfaceplane == null)
        {
            Debug.LogError("Grids object not assigned!");
            return;
        }

        var surface = navMeshSurface;

        // Save OG rotations
        Quaternion originalGridRot = surfaceplane.transform.rotation;
        Quaternion originalRot = transform.rotation;

        // Rotate to XZ plane
        
        surfaceplane.transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.rotation = Quaternion.Euler(0, 0, 0);

        surface.BuildNavMesh();

        // Restore rotation
        surfaceplane.transform.rotation = originalGridRot;
        transform.rotation = originalRot;

       
    }
}
