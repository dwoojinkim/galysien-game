using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubePlacer : MonoBehaviour
{
    private GameGrid grid;

    // Start is called before the first frame update
    void Awake()
    {
        grid = FindObjectOfType<GameGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo))
            {
                PlaceCubeNear(hitInfo.point);
            }
        }
    }

    private void PlaceCubeNear(Vector3 nearPoint)
    {
        var finalPosition = grid.GetNearestPointOnGrid(nearPoint);
        if (grid.IsOnGrid(finalPosition))
            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = finalPosition;
    }
}
