using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { None, Wall, Trap };
public enum Element { None, Earth, Fire, Water, Dark, Light };
public enum Sigil { None, Up, Down, Left, Right };             //Temporary Sigils

public class GameManager : MonoBehaviour
{
    public GameObject card;

    private int initialCardDrawAmount = 5;
    private int cardPoolSize = 10;
    private GameObject[] cardPool;
    private int currentNumHand;

    private bool cardSelected = false;

    void Awake()
    {
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
}
