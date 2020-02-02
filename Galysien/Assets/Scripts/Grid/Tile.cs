using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Element CurrentElement { get; set; } = Element.None;
    public Sigil CurrentSigil { get; set; } = Sigil.None;
    public TileType CurrentTileType { get; set; } = TileType.None;
}
