using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject card;

    private int initialCardDrawAmount = 5;
    private int cardPoolSize = 10;
    private GameObject[] cardPool;
    private int currentNumHand;

    private bool moveCards = false;

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
        if (Input.GetKeyDown("space"))
        {
            moveCards = !moveCards;
        }

        if (moveCards)
        {
            //Using flawed logic ATM because it would break when for example activated cards are non-sequential
            for (int i = 0; i < cardPoolSize; i++)
            {
                if (cardPool[i].activeSelf)
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
}
