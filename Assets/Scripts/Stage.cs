using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    private GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;

    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float treePercent = 0.4f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float mountainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.4f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.2f;

    public Vector2 tileSize = new Vector2(16f, 16f);

    public Sprite[] islandSprites;

    private Map map;
    public Map Map => map;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }
    }

    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent, treePercent, hillPercent, mountainPercent, townPercent, monsterPercent);
        CreateGrid();
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tileObj in tileObjs)
            {
                Destroy(tileObj.gameObject);
            }
        }

        tileObjs = new GameObject[mapWidth * mapHeight];

        var position = Vector3.zero;
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                var tileId = i * mapWidth + j;
                var newGo = Instantiate(tilePrefab, transform);
                newGo.transform.position = position;
                position.x += tileSize.x;

                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            position.x = 0;
            position.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var rend = tileGo.GetComponent<SpriteRenderer>();

        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            rend.sprite = islandSprites[tile.autoTileId];
        }
        else
        {
            rend.sprite = null;
        }
    }
}