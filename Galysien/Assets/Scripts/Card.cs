using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string Element { get; set; } = "none";
    public string Sigil { get; set; } = "none";

    public float PosOffset { get; set; } = 1.75f;
}
