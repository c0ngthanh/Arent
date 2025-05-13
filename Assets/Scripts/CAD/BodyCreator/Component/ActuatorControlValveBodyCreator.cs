// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using Chiyoda.CAD.Model;

namespace Chiyoda.CAD.Body
{
  public class ActuatorControlValveBodyCreator : BodyCreator<ActuatorControlValve, Body>
  {
    public ActuatorControlValveBodyCreator( Entity _entity ) : base( _entity )
    {}

    protected override void SetupMaterials( Body body, ActuatorControlValve actuatorControlValve )
    {
      // 自動ルーティング要素であれば、色を設定する
      if ( Topology.Route.HasColor(entity, out Color newColor) ) {
        ChangeMaterialColor( body, newColor ) ;
      }
      else {
        var impl = body.MainObject.GetComponent<ActuatorControlValveBodyImpl>() ;
        var renderers = impl.MainValve.GetComponentsInChildren<MeshRenderer>();
        var material = GetMaterial( body, actuatorControlValve ) ;
        foreach (var render in renderers) {
          render.material = material;
        }
        // impl.A.GetComponent<MeshRenderer>().material = GetMaterial( body, actuatorControlValve ) ;
      }
    }
    
    protected override void SetupGeometry( Body body, ActuatorControlValve actuatorControlValve )
    {
      var go = body.gameObject;

      go.transform.localPosition = (Vector3)actuatorControlValve.Origin;
      go.transform.localRotation = Quaternion.identity;

      body.MainObject.transform.localPosition = Vector3.zero;
      body.MainObject.transform.localRotation = Quaternion.identity;

      var impl = body.MainObject.GetComponent<ActuatorControlValveBodyImpl>();
      impl.MainValve.transform.localScale = (float) actuatorControlValve.Length/2 * Vector3.one * ModelScale;
      var a_Scale = impl.A.transform.localScale;
      var D_Scale = impl.D.transform.localScale;
      impl.A.transform.localScale = new Vector3(a_Scale.x,(float) actuatorControlValve.A_Length*a_Scale.y,a_Scale.z);
      impl.D.transform.localScale = new Vector3((float)actuatorControlValve.D_Size,D_Scale.y,(float)actuatorControlValve.D_Size);
      impl.D_Hold.transform.localPosition = new Vector3(impl.D_Hold.transform.localPosition.x, impl.A.transform.localScale.y*2*impl.A_Cylinder.transform.localScale.y, impl.D_Hold.transform.localPosition.z);
    }
  }
}
