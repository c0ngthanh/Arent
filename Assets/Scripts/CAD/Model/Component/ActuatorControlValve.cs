using System;
using System.Collections.Generic;
using Chiyoda.CAD.Core;
using Chiyoda.CAD.Topology;
using MaterialUI;
using UnityEngine;


namespace Chiyoda.CAD.Model
{
  [Entity( EntityType.Type.ActuatorControlValve )]
  public class ActuatorControlValve : Component, ILinearComponent
  {
    public enum ConnectPointType
    {
      Term1,
      Term2,
    }

    public ConnectPoint Term1ConnectPoint => GetConnectPoint( (int)ConnectPointType.Term1 ) ;
    public ConnectPoint Term2ConnectPoint => GetConnectPoint( (int)ConnectPointType.Term2 ) ;
    private readonly Memento<double> mainValveLength;
    private readonly Memento<double> a_Length;
    private readonly Memento<double> d_Size;

    public ActuatorControlValve( Document document ) : base( document )
    {
      mainValveLength = CreateMementoAndSetupValueEvents( 0.0 ) ;
      a_Length = CreateMementoAndSetupValueEvents( 0.0 ) ;
      d_Size = CreateMementoAndSetupValueEvents( 0.0 ) ;

      ComponentName = "DiaphramControlValve" ;
    }
    protected internal override void InitializeDefaultObjects()
    {
      base.InitializeDefaultObjects();
      AddNewConnectPoint( (int) ConnectPointType.Term1 );
      AddNewConnectPoint( (int) ConnectPointType.Term2 );
    }

    public override void CopyFrom( ICopyable another, CopyObjectStorage storage )
    {
      base.CopyFrom( another, storage );

      var entity = another as ActuatorControlValve;
      mainValveLength.CopyFrom( entity.mainValveLength.Value );
      a_Length.CopyFrom( entity.a_Length.Value);
      d_Size.CopyFrom( entity.d_Size.Value);
    }

    public override void ChangeSizeNpsMm(int connectPointNumber, int newDiameterNpsMm)
    {
      var cp = GetConnectPoint(connectPointNumber);
      var beforeDiameter = cp.Diameter.OutsideMeter;
      var afterDiameter = DiameterFactory.FromNpsMm(newDiameterNpsMm).OutsideMeter;
      var rate = afterDiameter / beforeDiameter;
      Length *= rate;
      A_Length *= rate;
      D_Size *= rate;
      base.ChangeSizeNpsMm(connectPointNumber, newDiameterNpsMm);
    }

    public double Diameter
    {
      get { return Term1ConnectPoint.Diameter.OutsideMeter ; }

      set
      {
        var term1 = Term1ConnectPoint ;
        var term2 = Term2ConnectPoint ;

        term1.Diameter = DiameterFactory.FromOutsideMeter( value ); ;
        term2.Diameter = term1.Diameter ;
      }
    }

    public double Length
    {
      get { return mainValveLength.Value ; }
      set
      {
        var term1 = Term1ConnectPoint ;
        var term2 = Term2ConnectPoint ;

        term1.SetPointVector( 0.5 * value * Axis) ;
        term2.SetPointVector( -0.5 * value * Axis) ;

        mainValveLength.Value = value ;
      }
    }

    public double A_Length
    {
      get { return a_Length.Value; }
      set { a_Length.Value = value ; }
    }
    public double D_Size
    {
      get { return d_Size.Value; }
      set { d_Size.Value = value ; }
    }
    private float cylinder_Scale;
    public override Bounds GetBounds()
    {
      var bounds = new Bounds((Vector3)Origin, Vector3.zero);
      bounds.Encapsulate( (Vector3)Term1ConnectPoint.Point ) ;
      bounds.Encapsulate( (Vector3)Term2ConnectPoint.Point ) ;
      
      var flowRadius = Length / 4 ;
      bounds.Encapsulate( (Vector3) (SecondAxis * flowRadius) );
      bounds.Encapsulate( (Vector3) (-SecondAxis * flowRadius) );
      bounds.Encapsulate( (Vector3) (SecondAxis * (A_Length*Length*2+Math.Max(D_Size/10,0.22))));

      bounds.Encapsulate( (Vector3) (-ThirdAxis * D_Size/10) );
      bounds.Encapsulate( (Vector3) (ThirdAxis * D_Size/10) );
      bounds.Encapsulate( (Vector3) (ThirdAxis * flowRadius) );
      bounds.Encapsulate( (Vector3) (-ThirdAxis * flowRadius) );
            
      bounds.Encapsulate( (Vector3) (Axis * 0.54) );
      return bounds ;
    }
  }
}
