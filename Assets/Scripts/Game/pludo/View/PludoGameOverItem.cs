using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Framework;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     PludoGameOverItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-13
 *描述:        	大结算Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoGameOverItem : PludoFreshView 
    {
        #region UI Param
        [Tooltip("玩家头像相关")]
        public PludoGamePlayer Player;
        [Tooltip("分数Item")]
        public YxView ScoreItemPrefab;
        [Tooltip("Item 父级")]
        public Transform ItemParent;
        [Tooltip("总分")]
        public PludoScoreLabel TotalAccout;

        #endregion

        #region Data Param
        [Tooltip("显示Id相关标签的Item索引")]
        public int ShowIdIndex = 0;

        public bool BigWinner { set; get; }

        #endregion

        #region Local Data
        [Tooltip("飞行棋大结算数据")]
        private PludoGameOverItemData _curData;
        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData = Data as PludoGameOverItemData;
            if (_curData!=null)
            {
                Player.UpdateView(_curData.Info);
                var count = _curData.Scores.Count;
                _curData.TotalScore = 0;
                for (int i = 0; i < count; i++)
                {
                    var view=ItemParent.GetChildView(i,ScoreItemPrefab);
                    if (view)
                    {
                        var data = _curData.Scores[i];
                        data.IdNumber = i + 1;
                        data.ShowId = IdCode == ShowIdIndex;
                        _curData.TotalScore += data.ScoreNumber;
                        view.UpdateView(data);
                    }
                }
                if (TotalAccout)
                {
                    TotalAccout.UpdateView(YxUtiles.GetShowNumber(_curData.TotalScore));
                }
                BigWinner = _curData.IsBigWinner;
            }
        }

        #endregion
        
        #region Function

        #endregion
    }

    /// <summary>
    /// 大结算Item 数据
    /// </summary>
    public class PludoGameOverItemData
    {
        /// <summary>
        /// 服务器座位号
        /// </summary>
        public int ServerSeat;
        /// <summary>
        /// 玩家信息
        /// </summary>
        public PludoPlayerInfo Info;
        /// <summary>
        /// 玩家分数
        /// </summary>
        public List<PludoGameOverScoreItemData> Scores;
        /// <summary>
        /// 总分
        /// </summary>
        public int TotalScore;
        /// <summary>
        /// 是否为大赢家
        /// </summary>
        public bool IsBigWinner;

        public PludoGameOverItemData(ISFSObject data)
        {
            Scores = new List<PludoGameOverScoreItemData>();
            if (data==null)
            {
                ServerSeat = ConstantData.IntDefValue;
                return;
            }
            SfsHelper.Parse(data, RequestKey.KeySeat, ref ServerSeat);
            if (data.ContainsKey(ConstantData.KeyRoundInfo))
            {
                var roundInfo = data.GetSFSArray(ConstantData.KeyRoundInfo);
                var count = roundInfo.Count;
                for (int i = 0; i < count; i++)
                {
                    var score = new PludoGameOverScoreItemData();
                    SfsHelper.Parse(roundInfo.GetSFSObject(i), RequestKey.KeyScore, ref score.ScoreNumber);
                    Scores.Add(score);
                }
            }
        }

        /// <summary>
        /// 关联玩家信息，确定大赢家
        /// </summary>
        /// <param name="info"></param>
        /// <param name="bigWinnerId"></param>
        public void RelatePlayerInfo(PludoPlayerInfo info,string bigWinnerId)
        {
            if (ServerSeat==ConstantData.IntDefValue)
            {
                ServerSeat = info.Seat;
            }
            Info = info;
            IsBigWinner = bigWinnerId.Equals(Info.UserId);
        }
    }
}
