using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Player;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.GameResult
{
    /// <summary>
    /// 小结算单个玩家信息
    /// </summary>
    public class PlayerResultInfo : MonoBehaviour
    { 
        /// <summary>
        /// 玩家头像
        /// </summary>
        public UITexture HeadTexture;
        /// <summary>
        /// 庄logo
        /// </summary>
        public GameObject ZhuangSprite;
        /// <summary>
        /// 玩家名称
        /// </summary>
        public UILabel LabelUserName;
        /// <summary>
        /// 番名称
        /// </summary>
        public UILabel LabelFanName;
        /// <summary>
        /// 胡牌分
        /// </summary>
        public UILabel LabelHuScore;
        /// <summary>
        /// 杠分数
        /// </summary>
        public UILabel LabelGangNum;
        /// <summary>
        /// 总分数
        /// </summary>
        public UILabel LabelTotalScore;
        /// <summary>
        /// 蛋分数
        /// </summary>
        public UILabel GuoDanScoreLabel;
        /// <summary>
        /// 结算itemBg
        /// </summary>
        [SerializeField]
        private UISprite _infoBg;
        /// <summary>
        /// 胡牌logo
        /// </summary>
        public GameObject HuLogo;
        /// <summary>
        /// 结算数据
        /// </summary>
        private ResultInfoData _resultData;
        /// <summary>
        /// 小结算的所有牌
        /// </summary>
        [SerializeField]
        private ResultCards _resultCards;
        /// <summary>
        /// 分数组
        /// </summary>
        [SerializeField]
        private UIGrid _scoreGrid;
        /// <summary>
        /// 清风分数
        /// </summary>
        [SerializeField]
        private GameObject _qingfengObj;
        /// <summary>
        /// 清风分数
        /// </summary>
        [SerializeField]
        private UILabel _labelQingfeng;
        /// <summary>
        /// 是不是赢家
        /// </summary>
        public bool IsWiner
        {
            get { return _resultData.IsWiner; }
        }

        private MahjongPlayer[] Players
        {
            get { return App.GetGameManager<Mahjong2DGameManager>().Players; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultData"></param>
        /// <param name="huList"></param>
        /// <param name="isBaoExist">是否需要冲宝</param>
        /// <param name="isQingfengExist"></param>
        /// <returns></returns>
        public int SetResultInfo(ResultInfoData resultData, List<int> huList,bool isBaoExist=false,bool isQingfengExist=false)
        {
            #region data
            _resultData=resultData;
            if(_resultData.HuType==2)//目前自摸情况下，会把胡的那张牌从手牌中带回来，这里删掉。目前只有一个胡牌，所以这么删，待扩展
            {
                resultData.HandList.Remove(huList[0]);
                if (isBaoExist)
                {
                    resultData.HandList.Add(huList[0]);
                }
            }
            #endregion
            #region UI
            MahjongPlayer player = Players[_resultData.UserSeat];
            LabelUserName.text = player.UserInfo.name;
            ZhuangSprite.SetActive(player.IsZhuang);
            LabelFanName.text = _resultData.FanName;
            LabelHuScore.TrySetComponentValue(YxUtiles.GetShowNumber(_resultData.HuNumber).ToString());
            LabelGangNum.TrySetComponentValue(YxUtiles.GetShowNumber(_resultData.GangNum).ToString());
            GuoDanScoreLabel.TrySetComponentValue(YxUtiles.GetShowNumber(_resultData.GuoDanSocre).ToString());
            _qingfengObj.TrySetComponentValue(isQingfengExist);
            _labelQingfeng.TrySetComponentValue(YxUtiles.GetShowNumber(_resultData.QingFengScore).ToString());
            LabelTotalScore.TrySetComponentValue(YxUtiles.GetShowNumber(_resultData.NowRoundScore).ToString());
            if (HeadTexture)
            {
                HeadTexture.mainTexture = player.CurrentInfoPanel.UserIcon.GetTexture();
            }
            player.UserInfo.Gold = _resultData.TotalGold;
            player.CurrentInfoPanel.SetGold(_resultData.TotalGold);
            HuLogo.TrySetComponentValue(IsWiner);
            _infoBg.TrySetComponentValue(IsWiner ? ConstantData.KeyWinerBg : ConstantData.KeyNormalBg);
            if (_scoreGrid)
            {
                _scoreGrid.repositionNow = true;
            }

            #endregion

            if (resultData.FenZhangCard>0)
            {
                huList.Clear();
                huList.Add(resultData.FenZhangCard);
            }
            _resultCards.Init(_resultData.MahjongGroups, resultData.HandList, huList, _resultData.IsWiner, resultData.FenZhangCard);
            return _resultData.HuType;
        }
        public void ResetInfo()
        {
            _resultData.Reset();
            _resultCards.Reset();
        }
    }
    /// <summary>
    /// 小结算数据
    /// </summary>
    public class ResultInfoData
    {
        /// <summary>
        /// 胡类型
        /// </summary>
        public int HuType;
        /// <summary>
        ///胡牌分数
        /// </summary>
        public int HuNumber;
        /// <summary>
        /// 番型
        /// </summary>
        public string FanName;
        /// <summary>
        /// 杠分
        /// </summary>
        public int GangNum;
        /// <summary>
        /// 本局分数
        /// </summary>
        public int NowRoundScore;
        /// <summary>
        /// 当前玩家的分数
        /// </summary>
        public long TotalGold;
        /// <summary>
        /// 是否是赢家
        /// </summary>
        public bool IsWiner;
        /// <summary>
        /// 是否为自摸
        /// </summary>
        public bool IsZimo;
        /// <summary>
        /// 玩家座位
        /// </summary>
        public int UserSeat;
        /// <summary>
        /// 过蛋分数
        /// </summary>
        public int GuoDanSocre;
        /// <summary>
        /// 清风分数
        /// </summary>
        public int QingFengScore;
        /// <summary>
        /// 分张牌
        /// </summary>
        public int FenZhangCard;
        /// <summary>
        /// 麻将组牌
        /// </summary>
        public List<MahjongGroupData> MahjongGroups;
        /// <summary>
        /// 胡牌
        /// </summary>
        public List<int> HuList;
        /// <summary>
        /// 手牌
        /// </summary>
        public List<int> HandList;
        /// <summary>
        /// 小结算数据
        /// </summary>
        private ISFSObject _data;
        public ResultInfoData(ISFSObject data,List<int> handCards)
        {
            _data = data;
            ISFSArray Groups;
            FenZhangCard = 0;
            GameTools.TryGetValueWitheKey(data, out HuType, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(data, out HuNumber, RequestKey.KeyHuNum);
            GameTools.TryGetValueWitheKey(data, out GangNum, RequestKey.KeyGangNum);
            GameTools.TryGetValueWitheKey(data, out FanName, RequestKey.KeyHuName);
            GameTools.TryGetValueWitheKey(data, out NowRoundScore, RequestKey.KeyGold);
            GameTools.TryGetValueWitheKey(data, out TotalGold, RequestKey.KeyTotalGold);
            GameTools.TryGetValueWitheKey(data, out Groups, RequestKey.KeyGroups);
            GameTools.TryGetValueWitheKey(data, out UserSeat, RequestKey.KeySeat);
            GameTools.TryGetValueWitheKey(data,out GuoDanSocre,RequestKey.KeyDanScore);
            GameTools.TryGetValueWitheKey(data, out QingFengScore, RequestKey.KeyQingfengScore);
            IsWiner = HuType > 0;
            IsZimo = HuType.Equals(2);
            HandList = handCards;
            MahjongGroups = GameTools.GetGroupData(Groups);
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            IsWiner = false;
            IsZimo = false;
            if(HuList!=null)
            {
                HuList.Clear();
            }
            else
            {
                HuList=new List<int>();
            }
            if(HandList!=null)
            {
                HandList.Clear();
            }
            else
            {
                HandList=new List<int>();
            }
            if (MahjongGroups!=null)
            {
                MahjongGroups.Clear();
            }
            else
            {
                MahjongGroups=new List<MahjongGroupData>();
            }
        }

        public bool ContainKey(string key)
        {
            return _data.ContainsKey(key);
        }
    }
}
