using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public GameObject HoverCube;
    [SerializeField] Vector2Int boardSize = new Vector2Int(10, 10);
    [SerializeField] Texture2D gridTexture = default;
    [SerializeField] private float tileSize = 2f;
    public float TileSize { get { return tileSize; } }
    public Vector2Int BoardSize { get { return boardSize;} }

    private Camera mainCamera;
    private bool[,] gridTile;
    private Vector2 pointOffset = new Vector2(0f, 0f);  //For GetNearestPointOnGrid method
    private Vector2 boardOffset = new Vector2(0f, 0f);  //For IsOnGrid method
    private Vector2Int gridCoord = new Vector2Int(0, 0);
    private Ray mouseOverRay;    
    private RaycastHit mouseOverHit;
    private GameObject mouseOverHitObject;
    private Vector3 hoverPoint;


    void Start()
    {
        mainCamera = Camera.main;
        transform.localScale = new Vector3 (boardSize.x * tileSize, boardSize.y * tileSize, 1);

        Material m = this.GetComponent<MeshRenderer>().material;
        m.mainTexture = gridTexture;
        m.SetTextureScale("_MainTex", boardSize);

        gridTile = new bool[boardSize.x, boardSize.y];
        HoverCube.transform.localScale = new Vector3 (TileSize, HoverCube.transform.localScale.y, TileSize);
        HoverCube.layer = 2; //Ignore Raycast Layer
        //TODO: Make the hoberCube a prefab so I can delete this nonsense.
        //Entire chunk necessary to properly set the Rendering Mode of the material to Transparent.
        HoverCube.GetComponent<Renderer>().sharedMaterial.SetFloat("_Mode", 2);   //2 = Fade mode where object can be completely invisible
        HoverCube.GetComponent<Renderer>().sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        HoverCube.GetComponent<Renderer>().sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        HoverCube.GetComponent<Renderer>().sharedMaterial.SetInt("_ZWrite", 0);
        HoverCube.GetComponent<Renderer>().sharedMaterial.DisableKeyword("_ALPHATEST_ON");
        HoverCube.GetComponent<Renderer>().sharedMaterial.EnableKeyword("_ALPHABLEND_ON");
        HoverCube.GetComponent<Renderer>().sharedMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        HoverCube.GetComponent<Renderer>().sharedMaterial.renderQueue = 3000;
        HoverCube.GetComponent<Renderer>().sharedMaterial.color = new Color(0.4f, 0.4f, 1.0f, 0.0f);
        //End Chunk

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

    //Indicates to the player when they are hovering over a tile on the grid.
    public void HoverGrid()
    {
        mouseOverRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseOverRay, out mouseOverHit))
        {
            hoverPoint = GetNearestPointOnGrid(mouseOverHit.point);
            mouseOverHitObject = mouseOverHit.collider.gameObject;

            if (IsOnGrid(hoverPoint) && (mouseOverHitObject.GetComponent<GameGrid>() != null || mouseOverHitObject.GetComponent<Tile>() != null))
            {
                if(mouseOverHitObject.GetComponent<GameGrid>() != null) // Checking if collider is the Grid

                HoverCube.GetComponent<Renderer>().sharedMaterial.SetFloat("_Mode", 3);   //3 = transparent mode

                //TODO: More efficient way to color instead of constantly making a new color when hovered over grid.
                //Use of a Ternary Operator
                HoverCube.GetComponent<Renderer>().sharedMaterial.color = PlaceableTile(hoverPoint) ? new Color(1f, 0.4f, 0.4f, 0.5f) : new Color(0.4f, 0.4f, 1.0f, 0.5f);

                HoverCube.transform.position = hoverPoint;

                Debug.Log("WE IN HERE. SHOULD BE VISIBLE");
            }
            else
            {
                Debug.Log("SHOULD BE INVISIBLE");
                DeactivateHoverCube();
            }
        }
    }

    public void DeactivateHoverCube()
    {
            HoverCube.GetComponent<Renderer>().sharedMaterial.SetFloat("_Mode", 2);   //2 = fade mode

            //TODO: More efficient way to color instead of constantly making a new color when hovered over grid.
            HoverCube.GetComponent<Renderer>().sharedMaterial.color = new Color(0.4f, 0.4f, 1.0f, 0.0f);

            HoverCube.transform.position = hoverPoint;
    }

    void OnValidate()
    {
        if (tileSize < 1f)
            tileSize = 1f;
    }
}
