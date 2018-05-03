using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammunition : ItemBase {
    public AmmoType ammoType;
}

public enum AmmoType
{
    cal_556 = 0,
    cal_762 = 1,
    cal_45 = 2,
    cal_9mm = 3
}
