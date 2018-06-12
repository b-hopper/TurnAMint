using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnAMint.Items
{
    public class Ammunition : ItemBase
    {
        public AmmoType ammoType;
    }

    public enum AmmoType
    {
        cal_556 = 0,
        cal_9mm = 1,
        cal_762 = 2,
        cal_45 = 3
    }
}