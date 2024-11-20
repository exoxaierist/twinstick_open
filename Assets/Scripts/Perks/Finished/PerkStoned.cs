using System.Collections;
using UnityEngine;

public class PerkStoned : Perk
{
    public PerkStoned()
    {
        ID = PERK_STONED;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }
}