using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     PludoGameResultItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-19
 *描述:        	飞行棋结算Item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoGameResultItem : PludoColorItem
    {
        #region UI Param
        [Tooltip("头像相关显示")]
        public PludoGamePlayer Player;
        [Tooltip("排行名次")]
        public UISprite RankSprite;
        [Tooltip("分数")]
        public PludoScoreLabel ScoreLabel;
        [Tooltip("星星数量(完成数量)")]
        public YxBaseLabelAdapter StarNum;
        #endregion

        #region Data Param
        [Tooltip("排行文本格式")]
        public string RankSpriteFormat = "Rank_{0}";
        [Tooltip("星星分数文本格式")]
        public string StarScoreFormat = "{0}/{1}";
        /// <summary>
        /// 标记是否为当前玩家
        /// </summary>
        public bool IsSelf { get; private set; }
        /// <summary>
        /// 是否为赢家
        /// </summary>
        public bool IsWinner { get; private set; }


        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var localData = Data as PludoGameResultItemData;
            if (localData!=null)
            {
                IsSelf = localData.IsSelf;
                var score = localData.ScoreNum;
                RankSprite.TrySetComponentValue(string.Format(RankSpriteFormat,IdCode));
                if (RankSprite)
                {
                    RankSprite.MakePixelPerfect();
                }
                IsWinner = score >= ConstantData.IntValue;
                StarNum.TrySetComponentValue(string.Format(StarScoreFormat, localData.FinishNum,localData.TotalFinishNum));
                if (ScoreLabel)
                {
                    ScoreLabel.UpdateView(YxUtiles.GetShowNumber(score));
                }
                Player.UpdateView(localData.Info);
            }
        }

        #endregion

        #region Function

        #endregion
    }
    /// <summary>
    /// 小结算Item数据
    /// </summary>
    public class PludoGameResultItemData
    {
        /// <summary>
        /// 服务器座位号
        /// </summary>
        public int ServerSeat;
        /// <summary>
        /// 分数
        /// </summary>
        public int ScoreNum;
        /// <summary>
        /// 完成数量
        /// </summary>
        public int FinishNum;
        /// <summary>
        /// 总计飞机数量
        /// </summary>
        public int TotalFinishNum;
        /// <summary>
        /// 攻击飞机数量
        /// </summary>
        public int AttackNum;
        /// <summary>
        /// 被攻击次数
        /// </summary>
        public int BeAttackNum;
        /// <summary>
        /// 玩家总金币
        /// </summary>
        public long TotalGold;
        /// <summary>
        /// 是否为零起飞
        /// </summary>
        public bool IsZero;
        /// <summary>
        /// 是否为当前玩家
        /// </summary>
        public bool IsSelf;
        /// <summary>
        /// 玩家信息
        /// </summary>
        public PludoPlayerInfo Info;


        public PludoGameResultItemData(ISFSObject data)
        {
            SfsHelper.Parse(data, RequestKey.KeySeat, ref ServerSeat);
            SfsHelper.Parse(data, RequestKey.KeyScore, ref ScoreNum);
            SfsHelper.Parse(data, ConstantData.KeyAttackNum, ref AttackNum);
            SfsHelper.Parse(data, ConstantData.KeyBeAttackNum, ref BeAttackNum);
            SfsHelper.Parse(data, ConstantData.KeyFinishNum, ref FinishNum);
            SfsHelper.Parse(data, RequestKey.KeyTotalGold, ref TotalGold);
            IsZero = FinishNum == ConstantData.IntValue;
        }

        public void SetOtherData(PludoPlayerInfo info,int totalStar,bool isSelf)
        {
            Info = info;
            TotalFinishNum = totalStar;
            IsSelf = isSelf;
        }
    }
}
