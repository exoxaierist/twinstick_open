using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkEyeForEye : Perk
{
    public PerkEyeForEye()
    {
        ID = PERK_EYEFOREYE;
        level = 1;
        maxLevel = 1;
        OnInstantiate();
    }
}
