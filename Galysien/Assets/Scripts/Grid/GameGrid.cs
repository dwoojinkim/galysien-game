using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(10, 10);
    [SerializeField] Texture2D gridTexture = default;
    [SerializeField] private float tileSize = 2f;
    public float TileSize { get { return tileSize; } }
    public Vector2Int BoardSize { get { return boardSize;} }

    private bool[,] gridTile;
    Vector2 pointOffset = new Vector2(0f, 0f);  //For GetNearestPointOnGrid method
    Vector2 boardOffset = new Vector2(0f, 0f);  //For IsOnGrid method
    Vector2Int gridCoord = new Vector2Int(0, 0);

    void Start()
    {
        transform.localScale = new Vector3 (boardSize.x * tileSize, boardSize.y * tileSize, 1);

        Material m = this.GetComponent<MeshRenderer>().material;
        m.mainTexture = gridTexture;
        m.SetTextureScale("_MainTex", boardSize);

        gridTile = new bool[boardSize.x, boardSize.y];

        //If Even, set offset to be half of a tile, else no offset needed
        if (boardSize.x % 2 == 0)
            pointOffset.x = 0.5f;
        if (boardSize.y % 2 == 0)
            pointOffset.y = 0.5f;

        boardOffset.x = (boardSize.x - 1) * 0.5f;
        boardOffset.y = (boardSize.y - 1) * 0.5f;
    }

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        float xCount = Mathf.RoundToInt((position.x / tileSize) - pointOffset.x);
        float yCount = Mathf.RoundToInt((position.y / tileSize));
        float zCount = Mathf.RoundToInt((position.z / tileSize) - pointOffset.y);

        Vector3 result = new Vector3(
            (xCount + pointOffset.x) * tileSize,
            yCount * tileSize,
            (zCount + pointOffset.y) * tileSize);
        
        result += transform.position;

        //Debug.Log("Initial Position: (" + position.x + ", " + position.y + ", " + position.z + ")");
        //Debug.Log("Result Position: (" + result.x + ", " + result.y + ", " + result.z + ")");
        return result;
    }

    public bool IsOnGrid(Vector3 position)
    {
        //Check if position is within the bounds of the grid
        if (position.x >= transform.position.x - (boardOffset.x * tileSize) && position.x <= transform.position.x + (boardOffset.x * tileSize))
        {
            if (position.z >= transform.position.z - (boardOffset.y * tileSize) && position.z <= transform.position.z + (boardOffset.y * tileSize))
                return true;
            else
                return false;
        }
        else
            return false;
    }

    public bool PlaceableTile(Vector3 position)
    {
        //Check if something is on the space preventing valid position (i.e. placed object, unplaceable area due to map, etc.)
        //TODO: Only checks for previously placed object. Check for other obstacles later
        //Debug.Log("PlaceableTile Position: (" + (int)position.x + ", " + (int)position.z + ")");
        if (!gridTile[PosToCoord(position).x, PosToCoord(position).y])
            return true;
        else
            return false;
    }

    //Translates from cartesian world coordinates (-inf to +inf, -inf to +inf), to grid coordinates (0 to inf, 0 to inf)
    public Vector2Int PosToCoord(Vector3 position)
    {
        gridCoord.x = (int)((position.x / tileSize) + boardOffset.x);
        gridCoord.y = (int)(boardOffset.y - (position.z / tileSize));
        return gridCoord;
    }

    public void PlaceTile(Vector2Int gridPosition)
    {
        gridTile[gridPosition.x, gridPosition.y] = true;
    }
/*
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        int _boardSizeX = boardSize.x;
        int _boardSizeY = boardSize.y;

        if (boardSize.x % 2 != 0)
            _boardSizeX -= 1;
        if (boardSize.y % 2 != 0)
            _boardSizeY -= 1;

        Vector2 boardOffset = new Vector2(_boardSizeX * 0.5f * tileSize, _boardSizeY * 0.5f * tileSize);
        //Debug.Log("X offset: " + boardOffset.x);
        //ebug.Log("Y offset: " + boardOffset.y);

        for (float x = 0; x < boardSize.x * tileSize; x += tileSize)
        {
            for (float z = 0; z < boardSize.y * tileSize; z += tileSize)
            {
                var point = GetNearestPointOnGrid(new Vector3(x - boardOffset.x, 0f, z - boardOffset.y));
                if (boardSize.x % 2 == 0)
                    point.x += tileSize / 2.0f;
                if (boardSize.y % 2 == 0)
                    point.y += tileSize / 2.0f;

                Gizmos.DrawSphere(point, 0.1f);

                //Debug.Log("X: " + (x - boardOffset.x + (tileSize / 2.0f)));
                //Debug.Log("Y: " + (z - boardOffset.y + (tileSize / 2.0f)));
            }
        }
    }*/

    void OnValidate()
    {
        if (tileSize < 1f)
            tileSize = 1f;
    }
}
