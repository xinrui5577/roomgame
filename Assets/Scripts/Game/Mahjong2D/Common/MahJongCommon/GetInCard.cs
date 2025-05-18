/** 
 *文件名称:     GetInCard.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-18 
 *描述:         获得的新的一张牌的处理，需要与手牌分开显示处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class GetInCard : MonoBehaviour
    {
        private MahjongItem _getInCard;

        public MahjongItem GetIn
        {
            set
            {
                _getInCard = value;
            }
            get
            {
                return _getInCard;
            }
        }

        private void Awake()
        {
            _getInCard = GetComponentInChildren<MahjongItem>();
            if (_getInCard!=null)
            {
                Destroy(_getInCard.gameObject);
                _getInCard = null;
            }
        }
    }
}
