using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { None, Wall, Trap };
public enum Element { None, Earth, Fire, Water, Dark, Light };
public enum Sigil { None, Up, Down, Left, Right };             //Temporary Sigils

public class GameManager : MonoBehaviour
{
    public GameObject card;

    private GameGrid grid;
    private int initialCardDrawAmount = 4;
    private int cardPoolSize = 10;
    private GameObject[] cardPool;
    private int currentNumHand;
    private Ray mouseOverRay;
    private RaycastHit mouseOverHit;
    private GameObject hoverCube;
    private Vector3 hoverPoint;
    private GameObject mouseOverHitObject;

    private bool cardSelected = false;

    void Awake()
    {
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
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InitialCardDraw();
    }

    // Update is called once per frame
    void Update()
    {
        HoverGrid();

        if (Input.GetMouseButtonDown(0))
        {
            CheckCardSelection();
        }

        if (cardSelected)
        {
            //Using flawed logic ATM because it would break when for example activated cards are non-sequential
            for (int i = 0; i < cardPoolSize; i++)
            {
                if (cardPool[i].activeSelf && !cardPool[i].GetComponent<Card>().IsSelected)
                    cardPool[i].transform.localPosition = new Vector3(cardPool[i].transform.localPosition.x, -6.0f, cardPool[i].transform.localPosition.z);
            }
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
            }

            card.transform.parent = Camera.main.transform;
            card.transform.localPosition = new Vector3 (cardPosX, -3.5f, 9f);
            card.SetActive(true);
        }
    }

    private void CheckCardSelection()
    {
        if (!cardSelected)
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.GetComponent<Card>() != null)
            {
                hitInfo.collider.gameObject.GetComponent<Card>().IsSelected = true;
                cardSelected = true;
            }
        }
        else
        {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.GetComponent<Card>() != null)
            {
                if (hitInfo.collider.gameObject.GetComponent<Card>().IsSelected)
                hitInfo.collider.gameObject.GetComponent<Card>().IsSelected = false;
                cardSelected = false;
            }
        }
    }

    private void HoverGrid()
    {
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
        }
    }
}
