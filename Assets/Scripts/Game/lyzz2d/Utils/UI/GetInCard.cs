/** 
 *文件名称:     GetInCard.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-18 
 *描述:         获得的新的一张牌的处理，需要与手牌分开显示处理
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class GetInCard : MonoBehaviour
    {
        [SerializeField] private MahjongItem _getInIn;

        public MahjongItem GetIn
        {
            set { _getInIn = value; }
            get { return _getInIn; }
        }

        private void Awake()
        {
            _getInIn = GetComponentInChildren<MahjongItem>();
            if (_getInIn != null)
            {
                Destroy(_getInIn.gameObject);
                _getInIn = null;
            }
        }
    }
}