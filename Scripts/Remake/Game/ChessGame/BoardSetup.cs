using UnityEngine;

public class BoardSetup : MonoBehaviour
{
    public static BoardSetup instance { get; set; }
    public float tileSize { get; private set; }
    [SerializeField] public float yOffset = 0.2f;
    [SerializeField] public Vector3 boardCenter = Vector3.zero;
    [SerializeField] public Material tileMaterial;

    public float piecesDeathSize { get; set; } // control the death piece size
    public float piecesDeathSpacing { get; set; } // control the each piece which is ate the spacing
    public Vector3 bounds { get; private set; }

    private void Awake()
    {
        instance = this;
        tileSize = 1.0f;
        
    }

    public void GenerateAllTiles(Board board, int tileCountX, int tileCountY, string layerName)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, -0.3125f, (tileCountY / 2) * tileSize) + boardCenter;
        board.tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                board.tiles[x, y] = GenerateSingleTile(x, y, layerName);
    }
    private GameObject GenerateSingleTile(int x, int y, string layerName)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y)); // tile's name
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial; // add the color into tile

        Vector3[] vertices = new Vector3[4];  // the four coordinates of a triangle
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 }; // render order

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

        ChangeToLayer(ref tileObject, layerName);
        AddBoxColliderComponent(ref tileObject);

        return tileObject;
    }

    private void ChangeToLayer(ref GameObject tileObject, string layerName)
    {
        tileObject.layer = LayerMask.NameToLayer(layerName); // Tile
    }
    private void AddBoxColliderComponent(ref GameObject tileObject)
    {
        tileObject.AddComponent<BoxCollider>();
    }
    public Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    public Vector3 ResetCoordinate(int x, int z, Vector3 r, int deathTeamCount)
    {
        return
            new Vector3(x * tileSize, yOffset, z * tileSize)
            - bounds
            + new Vector3(tileSize / 2, 0, tileSize / 2)
            + (r * piecesDeathSpacing) * deathTeamCount;
    }

}
