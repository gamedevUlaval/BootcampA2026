using UnityEngine;

public class WorldBounds : MonoBehaviour
{
    [Header("Global Scale")]
    [Tooltip("Multiplies all 4 arena distances at once.")]
    [Range(0.25f, 4f)] public float uniformScale = 1f;

    [Header("Arena Limits")]
    [Min(0f)] public float distanceToLeftWall = 5.33f;

    [Min(0f)] public float distanceToRightWall = 5.33f;

    [Min(0f)] public float distanceToBottomWall = 4.4815f;

    [Min(0f)] public float distanceToTopWall = 4.4815f;

    [Header("Center Divider")]
    public bool enableCenterDivider = true;

    [Tooltip("0 = exactly in the middle, positive = shifted toward the right of the screen.")]
    public float centerDividerOffset = 0f;

    [Min(0.05f)] public float centerDividerThickness = 0.5f;

    [Header("Walls")]
    [Min(0f)] public float wallHeight = 5f;

    [Min(0f)] public float wallThickness = 1f;

    float LeftDistance
    {
        get { return distanceToLeftWall * uniformScale; }
    }

    float RightDistance
    {
        get { return distanceToRightWall * uniformScale; }
    }

    float BottomDistance
    {
        get { return distanceToBottomWall * uniformScale; }
    }

    float TopDistance
    {
        get { return distanceToTopWall * uniformScale; }
    }

    float LeftEdge
    {
        get { return transform.position.x - LeftDistance; }
    }

    float RightEdge
    {
        get { return transform.position.x + RightDistance; }
    }

    float BottomEdge
    {
        get { return transform.position.z - BottomDistance; }
    }

    Vector3 ArenaCenter
    {
        get
        {
            float midWidth = (RightDistance - LeftDistance) / 2f;
            float midDepth = (TopDistance - BottomDistance) / 2f;
            return transform.position + new Vector3(midWidth, 0f, midDepth);
        }
    }

    float ArenaWidth
    {
        get { return LeftDistance + RightDistance; }
    }

    float ArenaDepth
    {
        get { return BottomDistance + TopDistance; }
    }

    float SplitX
    {
        get { return ArenaCenter.x + (enableCenterDivider ? centerDividerOffset : 0f); }
    }

    void Start()
    {
        BuildWalls();
    }

    void BuildWalls()
    {
        Vector3 origin = transform.position;
        Vector3 center = ArenaCenter;

        CreateWall("Wall", new Vector3(origin.x - LeftDistance, wallHeight / 2f, center.z),
                   new Vector3(wallThickness, wallHeight, ArenaDepth));

        CreateWall("Wall", new Vector3(origin.x + RightDistance, wallHeight / 2f, center.z),
                   new Vector3(wallThickness, wallHeight, ArenaDepth));

        CreateWall("Wall", new Vector3(center.x, wallHeight / 2f, origin.z - BottomDistance),
                   new Vector3(ArenaWidth, wallHeight, wallThickness));

        CreateWall("Wall", new Vector3(center.x, wallHeight / 2f, origin.z + TopDistance),
                   new Vector3(ArenaWidth, wallHeight, wallThickness));

        if (enableCenterDivider)
        {
            CreateWall("CenterDivider", new Vector3(SplitX, wallHeight / 2f, center.z),
                       new Vector3(centerDividerThickness, wallHeight, ArenaDepth));
        }
    }

    void CreateWall(string wallName, Vector3 position, Vector3 size)
    {
        GameObject wall = new GameObject(wallName);
        wall.transform.position = position;
        wall.transform.parent = transform;
        wall.AddComponent<BoxCollider>().size = size;
    }

    public Rect GetSpawnArea(int playerIndex)
    {
        return playerIndex == 0
            ? new Rect(LeftEdge, BottomEdge, SplitX - LeftEdge, ArenaDepth)
            : new Rect(SplitX, BottomEdge, RightEdge - SplitX, ArenaDepth);
    }
}