using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Block {

    public enum BlockType : ushort {
        Air,
        Ore_Coal,
        Stone,
        Dirt_Snow,
        Dirt_Grass,
        Grass,
        Plank_Oak,
        Leaves,
        Ore_Tin,
        Ice,
        Dirt,
        Gravel,
        Wood_Oak,
        Reeds,
        Ore_Iron,
        Iron_Block,
        Stone_Decorative,
        Ore_Emerald,
        Emerald_Block,

        Stone_Brick,
        Stone_Brick_Mossy,
        Stone_Brick_Decorative,
        Stone_Brick_Decorative_Alt,
        Stone_Brick_Mossy_Decorative,
        Clay_Brick,
        Ore_Gold,
        Gold_Block,
        Sandstone_Block,
        Sandstone_Brick,
        Sandstone_Block_Decorative,

        Crate_Oak,

        Plank_Ebony,
        Wood_Ebony,
        Ore_Sapphire,
        Sapphire_Block,
        Sand,
        Sandstone_Brick_Alt,
        Sandstone_Block_Decorative_Alt,
        Crate_Oak_Alt,
        Wood_Birch,
        Ore_Ruby,
        Coal_Block

    }

    public enum Face {
        Top,
        North,
        East,
        South,
        West,
        Bot
    }
}