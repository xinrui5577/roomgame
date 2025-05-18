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
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.UI;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class ResultCards : MonoBehaviour
    {
        /// <summary>
        ///     组牌
        /// </summary>
        [SerializeField] private GroupPile _groupPile;

        /// <summary>
        ///     手牌
        /// </summary>
        [SerializeField] private MahjongPile _handPile;

        /// <summary>
        ///     胡牌，目前只有一张，但是有可能以后会做血流模式，所以定义了一个堆，用于支持扩展
        /// </summary>
        [SerializeField] private MahjongPile _huPile;

        private float nextPos;

        public void Init(List<MahjongGroupData> groups, List<int> handCards, List<int> huCards, bool isWiner)
        {
            InitGroups(groups);
            InitHandCards(handCards);
            if (isWiner)
            {
                InitHuCards(huCards);
            }
        }

        /// <summary>
        ///     组牌
        /// </summary>
        /// <param name="groups"></param>
        private void InitGroups(List<MahjongGroupData> groups)
        {
            var lenth = groups.Count;
            for (var i = 0; i < lenth; i++)
            {
                _groupPile.AddGroup(groups[i], null, false, false);
            }
            _groupPile.ResetPosition();
            nextPos = lenth*_groupPile.Layout.Width;
        }

        /// <summary>
        ///     手牌
        /// </summary>
        /// <param name="handCards"></param>
        private void InitHandCards(List<int> handCards)
        {
            var lenth = handCards.Count;
            handCards =
                GameTools.SortCardWithLaiZiOppset(handCards, App.GetGameManager<Lyzz2DGameManager>().LaiZiNum).ToList();
            _handPile.transform.localPosition = new Vector3(nextPos, 0);
            for (var i = 0; i < lenth; i++)
            {
                var item = GameTools.CreateMahjong(handCards[i], false);
                item.GetComponent<MahjongItem>().JudgeHunTag(App.GetGameManager<Lyzz2DGameManager>().LaiZiNum);
                _handPile.AddItem(item, false);
            }
            _handPile.ResetPosition();
            nextPos += lenth*_handPile.Layout.Width;
        }

        /// <summary>
        ///     胡牌
        /// </summary>
        /// <param name="huCards"></param>
        private void InitHuCards(List<int> huCards)
        {
            var lenth = huCards.Count;
            _huPile.transform.localPosition = new Vector3(nextPos, 0);
            for (var i = 0; i < lenth; i++)
            {
                var item = GameTools.CreateMahjong(huCards[i], false);
                item.GetComponent<MahjongItem>().SetHuTag();
                _huPile.AddItem(item, false);
            }
            _handPile.ResetPosition();
            nextPos += lenth*_handPile.Layout.Width;
        }

        public void Reset()
        {
            _groupPile.ResetPile();
            _handPile.ResetPile();
            _huPile.ResetPile();
            nextPos = 0;
        }
    }
}