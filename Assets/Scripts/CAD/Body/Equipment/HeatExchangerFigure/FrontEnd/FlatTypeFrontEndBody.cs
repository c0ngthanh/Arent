﻿using System;
using UnityEngine;

namespace Chiyoda.CAD.Body
{
  public class FlatTypeFrontEndBody : FrontEndBody
  {
    [SerializeField]
    GameObject packFlangeBody;

    [SerializeField]
    GameObject cylinderBody;

    public GameObject PackFlangeBody { get => packFlangeBody; set => packFlangeBody = value; }
    public GameObject CylinderBody { get => cylinderBody; set => cylinderBody = value; }
  }
}