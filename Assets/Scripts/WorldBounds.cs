using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    public float boundsPadding = 0f;
    public float wallHeight = 5f;
    public float wallThickness = 1f;
    public Camera gameCamera;
    void Start()
    {
        Rect visibleArea = CalculateVisibleGroundArea();
        
        float worldWidth = visibleArea.width + (boundsPadding * 2f);
        float worldDepth = visibleArea.height + (boundsPadding * 2f);

        float centerX = visibleArea.x + (visibleArea.width / 2f);
        float centerZ = visibleArea.y + (visibleArea.height / 2f);

        float halfWidth = worldWidth / 2f;
        float halfDepth = worldDepth / 2f;

        CreateWall(new Vector3(centerX, wallHeight / 2f, centerZ + halfDepth), 
        new Vector3(worldWidth, wallHeight, wallThickness));

        CreateWall(new Vector3(centerX, wallHeight / 2f, centerZ - halfDepth), 
        new Vector3(worldWidth, wallHeight, wallThickness));

        CreateWall(new Vector3(centerX + halfWidth, wallHeight / 2f, centerZ), 
        new Vector3(wallThickness, wallHeight, worldDepth));

        CreateWall(new Vector3(centerX - halfWidth, wallHeight / 2f, centerZ), 
        new Vector3(wallThickness, wallHeight, worldDepth));

        Debug.Log("Camera position: " + gameCamera.transform.position);
        Debug.Log("Camera rotation: " + gameCamera.transform.rotation.eulerAngles);
        Debug.Log("Visible area: " + visibleArea);
        Debug.Log("World position of this WorldBounds object: " + transform.position);

    }
    Rect CalculateVisibleGroundArea()
    {
        Vector3 bottomLeft = RaycastToGround(0f, 0f);
        Vector3 bottomRight = RaycastToGround(1f, 0f);
        Vector3 topLeft = RaycastToGround(0f, 1f);
        Vector3 topRight = RaycastToGround(1f, 1f);
        
        float minX = Mathf.Min(bottomLeft.x, bottomRight.x, topLeft.x, topRight.x);
        float maxX = Mathf.Max(bottomLeft.x, bottomRight.x, topLeft.x, topRight.x);
        float minZ = Mathf.Min(bottomLeft.z, bottomRight.z, topLeft.z, topRight.z);
        float maxZ = Mathf.Max(bottomLeft.z, bottomRight.z, topLeft.z, topRight.z);
        
        return new Rect(minX, minZ, maxX - minX, maxZ - minZ);
    }

    Vector3 RaycastToGround(float viewportX, float viewportY)
    {
        Ray ray = gameCamera.ViewportPointToRay(new Vector3(viewportX, viewportY, 0));
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        groundPlane.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }

    void CreateWall(Vector3 position, Vector3 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.position = position;
        wall.transform.parent = transform;
        
        BoxCollider collider = wall.AddComponent<BoxCollider>();
        collider.size = size;
    }

    public Rect GetSpawnArea(int playerIndex)
    {
        Rect visibleArea = CalculateVisibleGroundArea();
        float halfWidth = visibleArea.width / 2f;

        if (playerIndex == 0)
        {
            return new Rect(visibleArea.x, visibleArea.y, halfWidth, visibleArea.height);
        }
        else
        {
            return new Rect(visibleArea.x + halfWidth, visibleArea.y, halfWidth, visibleArea.height);
        }
    }

}
