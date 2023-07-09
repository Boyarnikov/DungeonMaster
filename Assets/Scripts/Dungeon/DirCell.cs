using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirCell: Cell
{
    public Tuple<int, int> rot;
    new public void Rotate()
    {
        rot = new Tuple<int, int>(rot.Item2, -rot.Item1);
        return;
    }
}

