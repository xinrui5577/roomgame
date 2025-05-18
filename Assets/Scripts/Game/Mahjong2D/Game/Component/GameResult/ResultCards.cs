/** 
 *文件名称:     ResultCards.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-29 
 *描述:         处理小结算中显示结果的牌，包括组牌，手牌与胡牌
 *历史记录: 
*/

using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.Component.GameResult
{
    public class ResultCards : MonoBehaviour
    {
        /// <summary>
        /// 组牌
        /// </summary>
        [SerializeField]
        private GroupPile _groupPile;
        /// <summary>
        /// 手牌
        /// </summary>
        [SerializeField]
        private MahjongPile _handPile;
        /// <summary>
        /// 胡牌，目前只有一张，但是有可能以后会做血流模式，所以定义了一个堆，用于支持扩展
        /// </summary>
        [SerializeField]
        private MahjongPile _huPile;
        [Tooltip("不同类型牌组间距")]
        public float GroupTypeCell = 10;
        [Tooltip("胡牌额外间距")]
        public float HuCardTypeCell = 30;
        private float nextPos;

        /// <summary>
        /// 初始化结算的牌
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="handCards"></param>
        /// <param name="newGetCards"></param>
        /// <param name="isWiner"></param>
        public void Init(List<MahjongGroupData> groups,List<int> handCards,List<int> newGetCards,bool isWiner,int fenZhangData)
        {
            InitGroups(groups);
            InitHandCards(handCards);
            if (isWiner)
            {
                InitHuCards(newGetCards);
            }
            else
            {
                if (fenZhangData>0)
                {
                    InitHuCards(newGetCards, false);
                }
            }
        }
        /// <summary>
        /// 组牌
        /// </summary>
        /// <param name="groups"></param>
        private void InitGroups(List<MahjongGroupData> groups)
        {
            int lenth = groups.Count;
            for (int i = 0; i < lenth; i++)
            {
                _groupPile.AddGroup(groups[i], null, false);
            }
            _groupPile.ResetPosition();
            nextPos = lenth* _groupPile.Layout.Width+ GroupTypeCell;
        }
        /// <summary>
        /// 手牌
        /// </summary>
        /// <param name="handCards"></param>
        private void InitHandCards(List<int>handCards)
        {
            int lenth = handCards.Count;
            handCards = GameTools.SortCardWithLaiZiOppset(handCards, App.GetGameManager<Mahjong2DGameManager>().LaiZiNum).ToList();
            _handPile.transform.localPosition=new Vector3(nextPos,0);
            for (int i = 0; i < lenth; i++)
            {
                Transform item = GameTools.CreateMahjong(handCards[i], false);
                item.GetComponent<MahjongItem>().JudgeHunTag(App.GetGameManager<Mahjong2DGameManager>().LaiZiNum);
                _handPile.AddItem(item,false);
            }
            _handPile.ResetPosition();
            nextPos+= lenth * _handPile.Layout.Width+ GroupTypeCell+HuCardTypeCell;
        }
        /// <summary>
        /// 胡牌
        /// </summary>
        /// <param name="huCards"></param>
        private void InitHuCards(List<int> huCards,bool showHuTag=true)
        {
            int lenth = huCards.Count;
            _huPile.transform.localPosition = new Vector3(nextPos, 0);
            for (int i = 0; i < lenth; i++)
            {
                Transform item = GameTools.CreateMahjong(huCards[i], false);
                if (showHuTag)
                {
                    item.GetComponent<MahjongItem>().SetHuTag();
                }
                _huPile.AddItem(item, false);
            }
            _handPile.ResetPosition();
            nextPos += lenth * _handPile.Layout.Width+ GroupTypeCell;
        }
        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            _groupPile.ResetPile();
            _handPile.ResetPile();
            _huPile.ResetPile();
            nextPos = 0;
        }
    }
}
