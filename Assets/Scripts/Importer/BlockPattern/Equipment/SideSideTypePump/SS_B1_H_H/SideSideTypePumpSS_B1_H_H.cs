﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chiyoda;
using Chiyoda.CAD.Core;
using Chiyoda.CAD.Model;
using Chiyoda.CAD.Topology;
using Chiyoda.DB;
using IDF;
using UnityEngine;

namespace Importer.BlockPattern.Equipment.SideSideTypePump.SS_B1_H_H
{
  public class SideSideTypePumpSS_B1_H_H : SideSideTypePumpBase<BlockPatternArray>
  {
    public SideSideTypePumpSS_B1_H_H(Document doc) : base(doc, "SS-B1-H-H")
    {
      Info = new SingleBlockPatternIndexInfo
      {
        DischargeIndex = 0,
        SuctionIndex = 1,
        BasePumpIndex = 0,
        DischargeAngleGroupIndexList = new List<int>(),
        DischargeIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfo.DischargeIndexType, int>
        {
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeBOP, -1 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeEnd, 42 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeNozzle, 1 },
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeSystemFlexStart,  40 },  //  Flex 連動の開始 Edge（これよりPump側が影響を受ける）
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeSystemFlexStop,   10 },  //  Flex 連動の停止 Edge（これよりPump側は影響を受けない）
          { SingleBlockPatternIndexInfo.DischargeIndexType.DischargeSystemFlexOrigin,  9 },  //  Flex を連動させるDischarge 系のStopの一つPump側のLeafEdge
        },
        SuctionIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfo.SuctionIndexType, int>
        {
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionNozzle, 34 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionEnd, 0 },
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionSystemFlexStart,   2 },   //  Flex 連動の開始 Edge（これよりPump側が影響を受ける）
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionSystemFlexStop,   25 },   //  Flex 連動の停止 Edge（これよりPump側は影響を受けない）
          { SingleBlockPatternIndexInfo.SuctionIndexType.SuctionSystemFlexOrigin, 26 },   //  Flex を連動させるSuction 系のStopの一つPump側のLeafEdge
        },
        NextOfIndexTypeValue = new Dictionary<SingleBlockPatternIndexInfo.NextOfIndexType, int>
        {
          { SingleBlockPatternIndexInfo.NextOfIndexType.NextOfDischargeEnd, 41 },
          { SingleBlockPatternIndexInfo.NextOfIndexType.NextOfSuctionEnd, 1 },
        },
        
        SuctionFlexHelper = new int[,]{ { 2,25  }, },
        DischargeFlexHelper = new int[,]{ {40 ,10  }, },
        SuctionPipeIndexRange = new int[,]{ { 0, 34 }, },
        DischargePipeIndexRange = new int[,]{ {1 ,42  }, },
        
        DischargeDiameterNPSInch = 3,
        SuctionDiameterNPSInch = 6,

      };
    }

    public Chiyoda.CAD.Topology.BlockPattern Create(Action<Edge> onFinish)
    {
      ImportIdfAndPump();
      foreach (var edge in BaseBp.NonEquipmentEdges)
      {
        edge.LocalCod = LocalCodSys3d.Identity;
      }
      PostProcess() ;
      
      var cbp = BpOwner ;

      // vertexにflowを設定
      // 最終的には配管全体に向きを設定する事になるが、とりあえず暫定的に設定
      cbp.SetVertexName( "DischargeEnd", "N-2", HalfVertex.FlowType.FromThisToAnother ) ;
      cbp.SetVertexName( "SuctionEnd", "N-1" , HalfVertex.FlowType.FromAnotherToThis ) ;

      onFinish?.Invoke( (BlockEdge) BpOwner ?? BaseBp ) ;

      return BaseBp;
    }

    protected override void RemoveExtraEdges(Group group, string file)
    {
      using (Group.ContinuityIgnorer(group))
      {
        List<Edge> removeEdgeList = null;
        if (file.Contains("-DIS-A"))
        {
          removeEdgeList = group.EdgeList.Reverse().Take( 3 ).ToList();
        }
        else if (file.Contains("-SUC-A"))
        {
          removeEdgeList = group.EdgeList.Take( 3 ).ToList();
        }

        removeEdgeList?.ForEach(e => e.Unlink());
      }
    }


    /// <summary>
    /// IDFにノズル側にフランジ形状が潰れてしまっているので特別に追加する
    /// </summary>
    /// <param name="group"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    protected override LeafEdge GetNozzleSideFlange(Group group, string file)
    {
      return null;
    }

    protected override void SetPropertyAndRule(SingleBlockPatternIndexInfo info)
    {
      var range = DiameterRange.GetBlockPatternNpsMmRange();
      SideSideTypePumpGeneralPropertiesAndRules.SetPropertiesAndRules(BpOwner, BaseBp, info, range.min, range.max, range.min, range.max);
    }
  }
}
