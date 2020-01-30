using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    [SerializeField] Vector2Int boardSize = new Vector2Int(10, 10);
    [SerializeField] Texture2D gridTexture = default;
    private float tileSize = 1f;
    public float TileSize { get { return tileSize; } }

    void Start()
    {
        Material m = this.GetComponent<MeshRenderer>().material;
        m.mainTexture = gridTexture;
        m.SetTextureScale("_MainTex", boardSize);        
    }

    public Vector3 GetNearestPointOnGrid(Vector3 position)
    {
        position -= transform.position;

        int xCount = Mathf.RoundToInt(position.x / tileSize);
        int yCount = Mathf.RoundToInt(position.y / tileSize);
        int zCount = Mathf.RoundToInt(position.z / tileSize);

        //float xCount = (position.x / tileSize);
        //float yCount = (position.y / tileSize);
        //float zCount = (position.z / tileSize);

        Vector3 result = new Vector3(
            (float)xCount * tileSize,
            (float)yCount * tileSize,
            (float)zCount * tileSize);
        
        result += transform.position;

        return result;
    }

    public bool IsOnGrid(Vector3 position)
    {
        Vector2 boardOffset = new Vector2((boardSize.x - 1) * 0.5f, (boardSize.y - 1) * 0.5f);

        if (position.x >= transform.position.x - boardOffset.x && position.x <= transform.position.x + boardOffset.x)
        {
            if (position.z >= transform.position.z - boardOffset.y && position.z <= transform.position.z + boardOffset.y)
                return true;
            else
                return false;
        }
        else
        {
            //Debug.Log("Position x: " + position.x);
            //Debug.Log("Transform x: " + transform.position.x);
            //Debug.Log("boardOffset x: " + boardOffset.x);
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 boardOffset = new Vector2((boardSize.x - 1) * 0.5f, (boardSize.y - 1) * 0.5f);

        for (float x = 0; x < boardSize.x; x += tileSize)
        {
            for (float z = 0; z < boardSize.y; z += tileSize)
            {
                var point = GetNearestPointOnGrid(new Vector3(x - boardOffset.x, 0f, z - boardOffset.y));
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
