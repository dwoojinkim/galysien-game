using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    private GameGrid grid;
    private Ray mouseOverRay;
    private RaycastHit mouseOverHit;
    private GameObject mouseOverHitObject;
    private GameObject hoverCube;
    private Vector3 hoverPoint;

    // Start is called before the first frame update
    void Awake()
    {
        grid = FindObjectOfType<GameGrid>();
        /*
        hoverCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        hoverCube.transform.localScale = new Vector3 (grid.TileSize, hoverCube.transform.localScale.y, grid.TileSize);
        hoverCube.layer = 2; //Ignore Raycast Layer

        //Entire chunk necessary to properly set the Rendering Mode of the material to Transparent.
        hoverCube.GetComponent<Renderer>().material.SetFloat("_Mode", 2);   //2 = Fade mode where object can be completely invisible
        hoverCube.GetComponent<Renderer>().material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        hoverCube.GetComponent<Renderer>().material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        hoverCube.GetComponent<Renderer>().material.SetInt("_ZWrite", 0);
        hoverCube.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
        hoverCube.GetComponent<Renderer>().material.EnableKeyword("_ALPHABLEND_ON");
        hoverCube.GetComponent<Renderer>().material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        hoverCube.GetComponent<Renderer>().material.renderQueue = 3000;
        hoverCube.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 1.0f, 0.0f);
        //End Chunk*/


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.GetComponent<GameGrid>() != null)
                PlaceCubeNear(hitInfo.point);
        }

/*
        mouseOverRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseOverRay, out mouseOverHit))
        {
            hoverPoint = grid.GetNearestPointOnGrid(mouseOverHit.point);
            mouseOverHitObject = mouseOverHit.collider.gameObject;

            if (grid.IsOnGrid(hoverPoint) && (mouseOverHitObject.GetComponent<GameGrid>() != null || mouseOverHitObject.GetComponent<Tile>() != null))
            {
                if(mouseOverHitObject.GetComponent<GameGrid>() != null) // Checking if collider is the Grid

                hoverCube.GetComponent<Renderer>().material.SetFloat("_Mode", 3);   //3 = transparent mode

                //TODO: More efficient way to color instead of constantly making a new color when hovered over grid.
                //Use of a Ternary Operator
                hoverCube.GetComponent<Renderer>().material.color = !grid.PlaceableTile(hoverPoint) ? new Color(1f, 0.4f, 0.4f, 0.5f) : new Color(0.4f, 0.4f, 1.0f, 0.5f);

                hoverCube.transform.position = hoverPoint;
            }
            else
            {
                hoverCube.GetComponent<Renderer>().material.SetFloat("_Mode", 2);   //2 = fade mode

                //TODO: More efficient way to color instead of constantly making a new color when hovered over grid.
                hoverCube.GetComponent<Renderer>().material.color = new Color(0.4f, 0.4f, 1.0f, 0.0f);

                hoverCube.transform.position = hoverPoint;
            }
        }*/
        
    }

    private void PlaceCubeNear(Vector3 nearPoint)
    {
        var finalPosition = grid.GetNearestPointOnGrid(nearPoint);

        if (grid.IsOnGrid(finalPosition) && grid.PlaceableTile(finalPosition))
        {
            Vector2Int test = grid.PosToCoord(finalPosition);
            grid.PlaceTile(test);
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = finalPosition;
            cube.transform.localScale = new Vector3(grid.TileSize * 0.9f, cube.transform.localScale.y * 0.9f, grid.TileSize * 0.9f);
            cube.AddComponent<Tile>();
        }
    }
}
