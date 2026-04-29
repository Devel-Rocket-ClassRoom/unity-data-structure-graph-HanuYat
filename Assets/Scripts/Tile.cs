public enum Sides
{
    None = -1,
    Bottom,
    Right,
    Left,
    Top
}

public class Tile
{
    public int id;
    public int autoTileId;
    public int fowTileId;
    public bool isVisited = false;

    public Tile[] adjacents = new Tile[4];

    public bool CanMove => autoTileId != (int)TileTypes.Empty;

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null && adjacents[i].autoTileId != (int)TileTypes.Empty)
            {
                // 1000 0
                // 0100 1
                // 0010 2
                // 0001 3
                autoTileId |= 1 << adjacents.Length - 1 - i;
            }
        }
    }

    public void UpdateFowTileId()
    {
        fowTileId = 15;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || adjacents[i].isVisited)
            {
                // 1000 0
                // 0100 1
                // 0010 2
                // 0001 3
                fowTileId |= 1 << adjacents.Length - 1 - i;
            }
        }
    }

    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }

            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                break;
            }
        }
    }

    public void ClearAdjacents()
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }
            adjacents[i].RemoveAdjacents(this);
        }

        UpdateAutoTileId();
    }
}