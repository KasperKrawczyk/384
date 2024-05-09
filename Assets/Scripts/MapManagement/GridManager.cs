using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
public class GridManager : MonoBehaviour
{

    public class TileNode
    {
        public bool isWalkable;
        public TileNode parent;
        public Vector3 worldPosition;
        public int gridX;
        public int gridY;
        public int gCost;
        public int hCost;
        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }
        
        public TileNode(bool isWalkable, Vector3 worldPosition, int gridX, int gridY)
        {
            this.isWalkable = isWalkable;
            this.worldPosition = worldPosition;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
    
    public Tilemap walkableTilemap;
    public Tilemap obstaclesTilemap;
    public static GridManager instance;

    private TileNode[,] grid;
    private Vector3Int bottomLeft;
    private Vector3Int topRight;
    private int width, height;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        bottomLeft = walkableTilemap.cellBounds.min;
        topRight = walkableTilemap.cellBounds.max;
        CreateGrid();
    }

    private void CreateGrid()
    {
        width = Mathf.Abs(topRight.x - bottomLeft.x) + 1;
        height = Mathf.Abs(topRight.y - bottomLeft.y) + 1;
        grid = new TileNode[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(bottomLeft.x + x, bottomLeft.y + y, 0);
                bool isWalkable = walkableTilemap.HasTile(tilePosition) && !obstaclesTilemap.HasTile(tilePosition);
                Vector3 worldPos = walkableTilemap.CellToWorld(tilePosition) + new Vector3(walkableTilemap.cellSize.x / 2, walkableTilemap.cellSize.y / 2, 0); // Centre of the cell!
                grid[x, y] = new TileNode(isWalkable, worldPos, x, y);
            }
        }
    }


    public List<Vector2> FindPath(Vector2 currentPosition, Vector2 targetPosition)
    {
        // Convert world positions to grid positions
        TileNode startNode = WorldPointToGridNode(currentPosition);
        TileNode endNode = WorldPointToGridNode(targetPosition);

        List<TileNode> openSet = new List<TileNode>();
        HashSet<TileNode> closedSet = new HashSet<TileNode>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            TileNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (TileNode neighbour in GetNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return new List<Vector2>(); // Return an empty path if one could not be found
    }
    
    public TileNode WorldPointToGridNode(Vector2 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.y);
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        return grid[x, y];
    }
    
    public List<TileNode> GetNeighbours(TileNode node)
    {
        List<TileNode> neighbours = new List<TileNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Vector2> RetracePath(TileNode startNode, TileNode endNode)
    {
        List<TileNode> path = new List<TileNode>();
        TileNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        List<Vector2> waypoints = path.Select(node => new Vector2(node.worldPosition.x, node.worldPosition.y)).ToList();
        return waypoints;
    }

    public int GetDistance(TileNode nodeA, TileNode nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
