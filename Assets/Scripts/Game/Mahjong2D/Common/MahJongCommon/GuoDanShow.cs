/** 
 *文件名称:     GuoDanShow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-16 
 *描述:         过蛋显示处理
 *历史记录: 
*/

using System.Collections;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class GuoDanShow : MonoSingleton<GuoDanShow>
    {
        [SerializeField]
        private UILabel _showInfo;
        [SerializeField]
        private GameObject _showParent;
        [SerializeField]
        private MahjongItem _showMahjong;
        [SerializeField]
        private float _zhangMaoShow = 3;

        public override void Awake()
        {
            base.Awake();
            _showParent.SetActive(false);
        }

        public void ShowGuoDanInfo(string playerName, int card)
        {
            _showParent.SetActive(true);
            _showInfo.text = string.Format("玩家{0}过蛋", playerName);
            _showMahjong.Value = card;
            StartCoroutine(OnPlayFinish(_zhangMaoShow));
        }
        private IEnumerator OnPlayFinish(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            if (_showParent)
            {
                _showParent.SetActive(false);
            }
        }
    }
}
