using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { None, Wall, Trap };
public enum Element { None, Earth, Fire, Water, Dark, Light };
public enum Sigil { None, Up, Down, Left, Right };             //Temporary Sigils

public class GameManager : MonoBehaviour
{
    public GameObject card;

    private Camera mainCamera;
    private GameGrid grid;
    private int initialCardDrawAmount = 4;
    private int cardPoolSize = 10;
    private GameObject[] cardPool;
    private List<GameObject> currentHand;
    private int currentNumHand;
    private Ray mouseOverRay;
    private RaycastHit mouseOverHit;
    private GameObject hoverCube;
    private Vector3 hoverPoint;
    private GameObject mouseOverHitObject;
    private GameObject selectedCard;

    //Test variables for moving card around camera
    private float distance;
    private Vector3 toObjectVector;
    private Vector3 linearDistanceVector;
    private Vector3 mousePosition;
    private Vector3 initialCameraPosition;
    private Vector3 initialCameraForward;

    private bool cardSelected = false;

    void Awake()
    {
        mainCamera = Camera.main;

        grid = FindObjectOfType<GameGrid>();
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
        //End Chunk
        cardPool = new GameObject[cardPoolSize];

        for (int i = 0; i < cardPoolSize; i++)
        {
            GameObject obj = (GameObject)Instantiate(card);
            obj.SetActive(false);
            cardPool[i] = obj;
        }

        currentHand = new List<GameObject>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitialCardDraw();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (cardSelected && Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.GetComponent<GameGrid>() != null)
                PlaceCubeNear(hitInfo.point);

            CheckCardSelection();
        }

        if (cardSelected)
        {
            //Using flawed logic ATM because it would break when for example activated cards are non-sequential
            for (int i = 0; i < cardPoolSize; i++)
            {
                //if (cardPool[i].activeSelf && !cardPool[i].GetComponent<Card>().IsSelected)
                //    cardPool[i].transform.localPosition = new Vector3(cardPool[i].transform.localPosition.x, -6.0f, cardPool[i].transform.localPosition.z);

                if (cardPool[i].activeSelf && cardPool[i].GetComponent<Card>().IsSelected)
                    MoveSelectedCard(cardPool[i]);
                else
                    cardPool[i].transform.localPosition = new Vector3(cardPool[i].transform.localPosition.x, -6.0f, cardPool[i].transform.localPosition.z);
            }

            HoverGrid();
        }
        else
        {
            for (int i = 0; i < cardPoolSize; i++)
            {
                if (cardPool[i].activeSelf)
                    cardPool[i].transform.localPosition = new Vector3(cardPool[i].transform.localPosition.x, -3.5f, cardPool[i].transform.localPosition.z);
            }
        }
    }

    private void InitialCardDraw()
    {
        currentNumHand = initialCardDrawAmount;

        int halfCards;
        float cardPosX;
        GameObject card;

        halfCards = initialCardDrawAmount % 2 == 0 ? initialCardDrawAmount / 2 : (initialCardDrawAmount - 1) / 2;

        for (int i = -halfCards; i <= halfCards; i++)
        {
            card = cardPool[i + halfCards];

            cardPosX = i * (card.transform.localScale.x + card.GetComponent<Card>().PosOffset);

            if (initialCardDrawAmount % 2 == 0)
            {
                cardPosX += (card.transform.localScale.x + card.GetComponent<Card>().PosOffset) / 2;
                if (i == halfCards - 1)
                    i++;
                
                currentHand.Add(card);
            }

            card.transform.parent = mainCamera.transform;
            card.transform.localPosition = new Vector3 (cardPosX, -3.5f, 9f);
            card.SetActive(true);
        }
    }

    //Change method so there is only one raycast done, instead of the two separate ones for no reason
    private void CheckCardSelection()
    {
        if (!cardSelected)
        {
            RaycastHit hitInfo;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.GetComponent<Card>() != null)
            {
                selectedCard = hitInfo.collider.gameObject;

                selectedCard.GetComponent<Card>().IsSelected = true;
                selectedCard.GetComponent<Renderer>().material.color = new Color(selectedCard.GetComponent<Renderer>().material.color.r, 
                                                                                 selectedCard.GetComponent<Renderer>().material.color.g,
                                                                                 selectedCard.GetComponent<Renderer>().material.color.b,
                                                                                 0.5f);
                selectedCard.layer = 2;         //Ignore Raycast layer

                //Set so that if the camera moves around a bit, the card won't move with it
                initialCameraPosition = mainCamera.transform.position;
                initialCameraForward = mainCamera.transform.forward;

                cardSelected = true;
            }
        }
        else
        {
            RaycastHit hitInfo;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.GetComponent<Card>() != null)
            {
                selectedCard = hitInfo.collider.gameObject;

                if (selectedCard.GetComponent<Card>().IsSelected)
                {
                    selectedCard.GetComponent<Card>().IsSelected = false;
                    selectedCard.GetComponent<Card>().IsSelected = true;
                    selectedCard.GetComponent<Renderer>().material.color = new Color(selectedCard.GetComponent<Renderer>().material.color.r, 
                                                                                     selectedCard.GetComponent<Renderer>().material.color.g,
                                                                                     selectedCard.GetComponent<Renderer>().material.color.b,
                                                                                     1.0f);
                    selectedCard.layer = 0;     //Back to Default layer
                    cardSelected = false;
                }

            }
        }
    }

    private void HoverGrid()
    {
        mouseOverRay = mainCamera.ScreenPointToRay(Input.mousePosition);
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
        }
    }

    private void MoveSelectedCard(GameObject card)
    {
        toObjectVector = card.transform.position - initialCameraPosition;
        linearDistanceVector = Vector3.Project(toObjectVector, initialCameraForward);
        distance = linearDistanceVector.magnitude;

        mousePosition = Input.mousePosition;
        mousePosition.z = distance;

        card.transform.position = mainCamera.ScreenToWorldPoint(mousePosition);
    }

    //Test method for placing things on grid
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
            cardSelected = false;
        }
    }
}
