using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefab;
    private GameObject[] tileObjs;

    public PlayerMovement playerPrefab;
    private PlayerMovement player;

    public int mapWidth = 20;
    public int mapHeight = 20;
    public int fowRadius = 3;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    public int erodeIterations = 2;

    [Header("Percents")]
    [Range(0f, 0.9f)]
    public float lakePercent = 0.05f;
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

    private Vector3 FirstTilePos
    {
        get
        {
            var pos = transform.position;
            pos.x -= mapWidth * tileSize.x * 0.5f;
            pos.y += mapHeight * tileSize.y * 0.5f;

            pos.x += tileSize.x * 0.5f;
            pos.y -= tileSize.y * 0.5f;
            return pos;
        }
    }

    private int prevTileId = -1;

    public Sprite[] islandSprites;
    public Sprite[] fowSprites;

    private Map map;
    public Map Map => map;

    private Graph graph;
    public Graph Graph => graph;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }

        if (tileObjs != null)
        {
            int currentTileId = ScreenPosToTileId(Input.mousePosition);
            if (prevTileId != currentTileId)
            {
                tileObjs[currentTileId].GetComponent<SpriteRenderer>().color = Color.green;
                if (prevTileId >= 0 && prevTileId < tileObjs.Length)
                {
                    tileObjs[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
                }
                prevTileId = currentTileId;
            }
        }
    }

    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent,
            erodeIterations,
            lakePercent,
            treePercent,
            hillPercent,
            mountainPercent,
            townPercent,
            monsterPercent);

        CreateGrid();
        CreatePlayer();
        CreateGraph();
    }

    private int GetTileWeight(int autoTileId)
    {
        return (TileTypes)autoTileId switch
        {
            TileTypes.Empty => -1,
            TileTypes.Grass => 1,
            TileTypes.Tree => 2,
            TileTypes.Hills => 4,
            TileTypes.Mountains => -1,
            TileTypes.Towns => 1,
            TileTypes.Castle => 1,
            TileTypes.Monster => 1,
            _ => 1
        };
    }

    private void CreateGraph()
    {
        var grid = new int[mapHeight, mapWidth];
        for (int r = 0; r < mapHeight; r++)
        {
            for (int c = 0; c < mapWidth; c++)
            {
                var tile = map.tiles[r * mapWidth + c];
                grid[r, c] = GetTileWeight(tile.autoTileId);
            }
        }

        graph = new Graph();
        graph.Init(grid);
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }

        player = Instantiate(playerPrefab);
        player.Warp(map.startTile.id);
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

        var firstPos = FirstTilePos;
        var position = firstPos;
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
            position.x = firstPos.x;
            position.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var rend = tileGo.GetComponent<SpriteRenderer>();

        if (tile.isVisited)
        {
            if (tile.autoTileId != (int)TileTypes.Empty)
            {
                rend.sprite = islandSprites[tile.autoTileId];
            }
            else
            {
                rend.sprite = null;
            }
        }
        else
        {
            rend.sprite = fowSprites[tile.fowTileId];
        }

    }

    public void OnTileVisited(int tileId)
    {
        OnTileVisited(map.tiles[tileId]);
    }

    public void OnTileVisited(Tile tile)
    {
        int centerX = tile.id % mapWidth;
        int centerY = tile.id / mapWidth;

        for (int i = -fowRadius; i <= fowRadius; i++)
        {
            for (int j = -fowRadius; j <= fowRadius; j++)
            {
                int x = centerX + j;
                int y = centerY + i;
                if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                {
                    continue;
                }

                int id = y * mapWidth + x;
                map.tiles[id].isVisited = true;
                DecorateTile(id);
            }
        }

        var radius = fowRadius + 1;
        for (int i = -radius; i <= radius; i++)
        {
            for (int j = -radius; j <= radius; j++)
            {
                if (i == radius || i == -radius || j == radius || j == -radius)
                {
                    int x = centerX + j;
                    int y = centerY + i;
                    if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                    {
                        continue;
                    }

                    int id = y * mapWidth + x;
                    map.tiles[id].UpdateFowTileId();
                    DecorateTile(id);
                }
            }
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);
        return WorldPosToTileId(mainCamera.ScreenToWorldPoint(screenPos));
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var first = FirstTilePos;
        int x = Mathf.FloorToInt((worldPos.x - first.x) / tileSize.x + 0.5f);
        int y = Mathf.FloorToInt((first.y - worldPos.y) / tileSize.y + 0.5f);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        return FirstTilePos + new Vector3(x * tileSize.x, -y * tileSize.y);
    }

    public Vector3 GetTilePos(int tileId)
    {
        return GetTilePos(tileId / mapWidth, tileId % mapWidth);
    }
}