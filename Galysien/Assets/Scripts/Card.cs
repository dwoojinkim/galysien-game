using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Element CurrentElement { get; set; } = Element.None;
    public Sigil CurrentSigil { get; set; } = Sigil.None;
    public bool IsSelected { get; set; } = false;

    public float PosOffset { get; set; } = 1.5f; //When in the player's hand
}
