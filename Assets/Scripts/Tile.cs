public enum Sides
{
    Bottom,
    Right,
    Left,
    Top
}

public class Tile
{
    public int id;
    public int autoTileId;
    public bool isVisited = false;

    public Tile[] adjacents = new Tile[4];

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                // 1000 0
                // 0100 1
                // 0010 2
                // 0001 3
                autoTileId |= 1 << adjacents.Length - 1 - i;
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
            adjacents[i] = null;
        }

        UpdateAutoTileId();
    }
}