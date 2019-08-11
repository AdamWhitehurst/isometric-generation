using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Block
{

    public enum BlockType : ushort
    {
        Air,
        Grass,
        Grass_Solid,
        Stone,
        Dirt,
        Brick,
        Log_Birch,
        Plank_Birch,
        Plank_Oak,
        Wool_Black,
        Wool_Blue,
        Brick_Dark,
        Brick_Brown,
        Stone_Slab,
        CobbleStone
    }

    public enum Face
    {
        Top,
        North,
        East,
        South,
        West,
        Bot
    }
}