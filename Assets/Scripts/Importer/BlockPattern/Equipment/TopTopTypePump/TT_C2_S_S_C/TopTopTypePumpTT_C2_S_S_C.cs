﻿using System ;
using System.Collections.Generic ;
using System.Linq ;
using Chiyoda.CAD.Core ;
using Chiyoda.CAD.Topology ;
using Chiyoda.CAD.Model ;
using Chiyoda.CAD.Model ;
using UnityEngine ;

namespace Importer.BlockPattern.Equipment.TopTopTypePump.TT_C2_S_S_C
{
  public class TopTopTypePumpTT_C2_S_S_C : TopTopTypePumpBase<BlockPatternArray>
  {
    TopTopTypePumpSystemLengthUpdater _systemLength;
    TopTopTypePumpPipeIndicesKeeper _keeper;

    public TopTopTypePumpTT_C2_S_S_C( Document doc ) : base( doc, "TT-C2-S-S-C")
    {
      Info = new SingleBlockPatternIndexInfoWithMinimumFlow
      {
        DischargeIndex = 0,
        MinimumFlowPreIndex = 1,
        MinimumFlowIndex = 2,
        SuctionIndex = 3,
        BasePumpIndex = 0,
        //DischargeAngleGroupIndexList = Enumerable.Range( 3, 9 ).ToList(),
        DischargeIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfo.DischargeIndexType, int>
        {
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeBOP, -1 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeEnd, 52 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeNozzle, 1 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeMinLength1, 30 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeMinLength2, 34 },

          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeSystemFlexStart, 50 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeSystemFlexStop,  26 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeSystemFlexOrigin, 25 },

        },
        SuctionIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfo.SuctionIndexType, int>
        {
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionNozzleFlange, 51 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionNozzle, 50 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionNozzleReducer, 49 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionEnd, 0 },

          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionSystemFlexStart,2 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionSystemFlexStop,  42 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionSystemFlexOrigin, 43 },
        },
        SuctionAdditionalIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfoWithMinimumFlow.SuctionAdditionalIndexType,int>{
          { SingleBlockPatternIndexInfoWithMinimumFlow.SuctionAdditionalIndexType.SuctionEndEdge, -1 },
        },
        DischargeAdditionalIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType, int>
        {
          { SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType.DischargeMinimumFlowSpacer, 19  },
          { SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType.MinimumFlowBranchTee,       18  },
          { SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType.MinimumFlowSpacingOrigin,    3  },  //  start elbow
          { SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType.MinimumFlowAdjustingStart,  17  },  //  used to keep it in maintainance group
          { SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType.MinimumFlowAdjustingStop,    6  },  //  used to keep it in maintainance group
          { SingleBlockPatternIndexInfoWithMinimumFlow.DischargeAdditionalIndexType.MinimumFlowAdjustingOrigin,  5  },   //  end elbow
                },
        NextOfIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfo.NextOfIndexType, int>
        {
          { SingleBlockPatternIndexInfo.NextOfIndexType.NextOfDischargeEnd, 51 },
          { SingleBlockPatternIndexInfo.NextOfIndexType.NextOfSuctionEnd, 1 }
        },
        MinimumFlowIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfoWithMinimumFlow.MinimumFlowIndexType, int>
        {
          { SingleBlockPatternIndexInfoWithMinimumFlow.MinimumFlowIndexType.NextOfMinimumFlowEnd, 10 },
          { SingleBlockPatternIndexInfoWithMinimumFlow.MinimumFlowIndexType.MinimumFlowEnd, 11 },
          { SingleBlockPatternIndexInfoWithMinimumFlow.MinimumFlowIndexType.MinimumFlowHeaderBOPSpacer, 7 },
          { SingleBlockPatternIndexInfoWithMinimumFlow.MinimumFlowIndexType.NextOfMinimumFlowHeaderBOPSpacer, 6 },
        },
        MinimumFlowPreIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfoWithMinimumFlow.MinimumFlowPreIndexType, int>
        {
        },
        SuctionFlexHelper = new int[,] { { 2, 42 }, },
        DischargeFlexHelper = new int[,] { { 26, 26 }, { 38, 50 }, { 1, 1 } },   //  ommit pre-post ori flow
        SuctionPipeIndexRange = new int[,] { { 0, 50 }, },
        DischargePipeIndexRange = new int[,] { { 1, 26 }, { 38, 52 } },         //  select pipes not to set pre-post oriflow pipes

        MinimumFlowInDischargeGroupPipeIndexRange = new int[,] { { 19, 21 } },  //  Note: They are in discharge group!

        MinimumFlowPrePipeIndexRange = new int[,] { { 2, 19 } },
        MinimumFlowPipeIndexRange = new int[,] { { 1, 11 } },

        MinimumFlowTeeAdjusterIndices = new int[] { 17, 6 },

        SuctionDiameterNPSInch = 14,
        DischargeDiameterNPSInch = 14,
        MinimumFlowDiameterNPSInch = 6
      } ;
    }
    protected override bool SelectIdf( string idf )
    {
       
      if ( ! idf.Contains( base.PumpShapeName ) ) {
        return false ;
      }
      if ( idf.Contains( "-DIS-A" ) || idf.Contains( "-SUC-A" ) || idf.Contains("-MIN-A" )|| idf.Contains("-MIN_-0" )) {
        return true ;
      }
      return false ;
    }

    internal override void PostProcess()
    {
      base.PostProcess();
      TopTopTypeMinimumFlowExtention.PostProcessMinimumFlow(this);
    }

    internal override void SetEdgeNames( SingleBlockPatternIndexInfo info )
    {
      base.SetEdgeNames(info);
      TopTopTypeMinimumFlowExtention.AppendEdgeNames(this,info);
    }

    protected override void SetBlockPatternInfo(SingleBlockPatternIndexInfo info)
    {
      TopTopTypeMinimumFlowExtention.SetBlockPatternInfoMinimumFlow(this,info);
    }
    

    public Chiyoda.CAD.Topology.BlockPattern Create( Action<Edge> onFinish )
    {
      ImportIdfAndPump() ;
      foreach ( var edge in BaseBp.NonEquipmentEdges ) {
        edge.LocalCod = LocalCodSys3d.Identity ;
      }
      if (!TopTopTypePumpBase.INDEXING_NOW) {
        TopTopTypePumpWithMinimumFlowIndexingHelper.BuildIndexList(BpOwner, BaseBp, Info);
        var group = BaseBp.NonEquipmentEdges.ElementAtOrDefault(Info.DischargeIndex) as IGroup;
        if (group != null && Info is SingleBlockPatternIndexInfoWithMinimumFlow infoDerived) {
          int[] systempipes = TopTopTypePumpIndexingHelper.ExtractGroupIndices(group,
            infoDerived.MinimumFlowTeeAdjusterIndices[0],
            infoDerived.MinimumFlowTeeAdjusterIndices[1]);
          var basePump = GetEquipmentEdge(BaseBp, Info.BasePumpIndex);
          /*
          if (systempipes.Length > 0){ 
            group.EdgeList.ElementAtOrDefault(systempipes[0]).ConnectionMaintenanceOrigin = basePump;
            group.EdgeList.ElementAtOrDefault(systempipes[systempipes.Length-1]).ConnectionMaintenanceOrigin = basePump;
          }
          */
          _systemLength = new TopTopTypePumpSystemLengthUpdater(Info.DischargeIndex, systempipes);
          _keeper = new TopTopTypePumpPipeIndicesKeeper(group,
            systempipes,
            infoDerived.MinimumFlowInDischargeGroupNormalPipes,
            infoDerived.MinimumFlowInDischargeGroupOletPrePostPipes
          );
        }
      }
      PostProcess() ;

      var cbp = BpOwner ;
      if ( ! TopTopTypePumpBase.INDEXING_NOW ) {
        cbp.GetProperty("HeaderInterval").Value = 0.5;
        cbp.GetProperty("MiniFlowHeaderInterval").Value = 3.0;
      }

      // vertexにflowを設定
      // 最終的には配管全体に向きを設定する事になるが、とりあえず暫定的に設定
      cbp.SetVertexName( "MinimumFlowEnd", "N-3", HalfVertex.FlowType.FromThisToAnother ) ;
      cbp.SetVertexName( "DischargeEnd", "N-2", HalfVertex.FlowType.FromThisToAnother ) ;
      cbp.SetVertexName( "SuctionEnd", "N-1" , HalfVertex.FlowType.FromAnotherToThis ) ;

      onFinish?.Invoke( (BlockEdge) BpOwner ?? BaseBp ) ;

      return BaseBp ;
    }

    /// <summary>
    /// グループ変更後、RuleBind 前に呼ばれる
    /// </summary>
    internal override void OnGroupChanged()
    {
      if (!TopTopTypePumpBase.INDEXING_NOW) {
        _systemLength.Activate(true);

        int[][] newindices = _keeper.ReassignIndices();
        if (newindices.Length == 3 && Info is SingleBlockPatternIndexInfoWithMinimumFlow infoDerived) {
          _systemLength.ReplaceIndices(newindices[0]);
          infoDerived.MinimumFlowInDischargeGroupNormalPipes = newindices[1];
          infoDerived.MinimumFlowInDischargeGroupOletPrePostPipes = newindices[2];
        }
        TopTopTypeMinimumFlowPipeLengthUpdater.AssortMinimumLengths(BaseBp, this.Info);
      }
    }
    protected override void RemoveExtraEdges(Group group, string file)
    {
      using ( Group.ContinuityIgnorer( group ) ) {
        List<Edge> removeEdgeList = null ;
        if ( file.Contains( "-DIS-A" ) ) {
          removeEdgeList = group.EdgeList.Reverse().Take( 3 ).ToList() ;
        }
        else if ( file.Contains( "-MIN_-0" ) ) {
          removeEdgeList = group.EdgeList.Reverse().Take( 17 ).ToList() ;
        }
        else if ( file.Contains( "-SUC-A" ) ) {
          removeEdgeList = group.EdgeList.Take( 3 ).ToList() ;
        }

        removeEdgeList?.ForEach( e => e.Unlink() ) ;
      }
    }


    /// <summary>
    /// IDFにノズル側にフランジ形状が潰れてしまっているので特別に追加する
    /// </summary>
    /// <param name="group"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    protected override LeafEdge GetNozzleSideFlange( Group group, string file )
    {
      return null ;
    }


    internal override void SetPropertyAndRule( SingleBlockPatternIndexInfo info )
    {
      {
        var bpa = BpOwner;
        bpa.RegisterUserDefinedProperty("BlockCount", PropertyType.GeneralInteger, 1);
        bpa.RuleList.AddRule(".ArrayCount", ".BlockCount");
      }
      BaseBp.RegisterUserDefinedProperty("AccessSpace", PropertyType.Length, 3.9);

      BaseBp.RuleList.AddRule(":Parent.ArrayOffsetX", "#BasePump.MaxX - #BasePump.MinX + .AccessSpace ");

      BaseBp.RegisterUserDefinedProperty("SuctionJoinType", CompositeBlockPattern.JoinType.Independent)
        .AddUserDefinedRule(CompositeBlockPattern.UserDefinedRules.GetChangeJointTypeRule("SuctionEnd"));
      BaseBp.RegisterUserDefinedProperty("DischargeJoinType", CompositeBlockPattern.JoinType.Independent)
        .AddUserDefinedRule(CompositeBlockPattern.UserDefinedRules.GetChangeJointTypeRule("DischargeEnd"));
      BaseBp.RegisterUserDefinedProperty("MinimumFlowJoinType", CompositeBlockPattern.JoinType.Independent)
        .AddUserDefinedRule(CompositeBlockPattern.UserDefinedRules.GetChangeJointTypeRule("MinimumFlowEnd"));

      TopTopTypePumpHeaderBOPFromStageWithMinimumFlow.SetPropertiesAndRules(BpOwner, BaseBp, Info);

      TopTopTypePumpMiniFlowHorizonalEndHeaderIntervalPropertiesAndRules.Set(BpOwner, BaseBp, Info, true);


      BaseBp.RegisterUserDefinedProperty("ConOriInMinLength", PropertyType.Length, 1.0);
      BaseBp.RegisterUserDefinedProperty("COIML", PropertyType.TemporaryValue, 1.0);
      BaseBp.RuleList.AddRule(".COIML", "Max(.ConOriInMinLength,0.001*DiameterToPipeMinLength(#DischargeEndPipe.Diameter))");
      BaseBp.RuleList.AddRule("#DischargeMinLengthPipe1.MinLength", ".COIML");
      BaseBp.RuleList.AddRule("#DischargeMinLengthPipe1.Length", ".COIML");

      BaseBp.RegisterUserDefinedProperty("ConOriOutMinLength", PropertyType.Length, 1.0);
      BaseBp.RegisterUserDefinedProperty("COOML", PropertyType.TemporaryValue, 1.0);
      BaseBp.RuleList.AddRule(".COOML", "Max(.ConOriOutMinLength,0.001*DiameterToPipeMinLength(#DischargeEndPipe.Diameter))");
      BaseBp.RuleList.AddRule("#DischargeMinLengthPipe2.MinLength", ".COOML");
      BaseBp.RuleList.AddRule("#DischargeMinLengthPipe2.Length", ".COOML");

      BaseBp.RegisterUserDefinedProperty("StageHeight", PropertyType.Length, 3.55);

      TopTopTypePumpUnifiedStageBOPPropertiesAndRules.SetPropertiesAndRules(BpOwner, BaseBp, Info);

      //  clearance between DischargePipes and MiniFlow pipes
      TopTopTypeMiniFlowStraitDischargeDistancePropertiesAndRules.Set(BpOwner, BaseBp, Info);

      BaseBp.RegisterUserDefinedProperty("MiniFlowDischargeDistanceD", PropertyType.Length, 0.2);
      BaseBp.RegisterUserDefinedProperty("MF_BOP_DPTH", PropertyType.TemporaryValue, 0.00);
      var prop = BaseBp.RegisterUserDefinedProperty("M_D_LENGTH", PropertyType.TemporaryValue, 0.00);

      BaseBp.RuleList.AddRule(".MF_BOP_DPTH", "#MinimumFlowBranchTee.MinY-#NextOfMinimumFlowHeaderBOPSpacer.MinY");
      BaseBp.RuleList.AddRule(".M_D_LENGTH", "#MinimumFlowSpacingOrigin.MaxY+.MiniFlowDischargeDistanceD+.MF_BOP_DPTH-#MinimumFlowAdjustingOrigin.MaxY");
      prop.AddUserDefinedRule(new GenericHookedRule(_systemLength.UpdateSystemLength)); //  Push M_D_LENGTH to system pipe.

      TopTopTypePumpMinimumFlowDiameterPropertiesAndRules.SetPropertiesAndRules(BpOwner, BaseBp, Info, null); 

    }
  }
}
