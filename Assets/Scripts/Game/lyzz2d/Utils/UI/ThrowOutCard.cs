/** 
 *文件名称:     ThrowOutCard.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         打出的那张牌，需要特殊处理
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class ThrowOutCard : MonoBehaviour
    {
        private static readonly object _safeLock = new object();

        [SerializeField] private MahjongItem _throwCard;

        public MahjongItem ThrowCard
        {
            set
            {
                lock (_safeLock)
                {
                    _throwCard = value;
                }
            }
            get { return _throwCard; }
        }

        private void Awake()
        {
            _throwCard = GetComponentInChildren<MahjongItem>();
            if (_throwCard != null)
            {
                Destroy(_throwCard.gameObject);
                _throwCard = null;
            }
        }
    }
}