using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Mahjong2D.Game.Item
{
    /// <summary>
    /// 大结算Item
    /// </summary>
    public class GameOverItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject _windner;
        [SerializeField]
        private GameObject _loser;
        [SerializeField]
        private  GameObject _houseOwner;
        [SerializeField]
        private UITexture _head;
        [SerializeField]
        private UILabel _playerName;
        [SerializeField]
        private UILabel _ziMoNumber;
        [SerializeField]
        private UILabel _jiePaoNumber;
        [SerializeField]
        private UILabel _dianPaoNumber;
        [SerializeField]
        private UILabel _anGangUiLabelNumber;
        [SerializeField]
        private UILabel _mingGangNumber;
        [SerializeField]
        private UILabel _totalNumber;
        [SerializeField]
        private UILabel _idNum;
        [Tooltip("座位号0标记")]
        public GameObject ZeroSeatFlag;
        [Tooltip("Id 格式")]
        public string IdFormat = "ID:{0}";
        public void InitInfo(OverData userData)
        {
            _windner.TrySetComponentValue(userData.isYingJia);
            _loser.TrySetComponentValue(userData.isPaoShou);
            Texture tex = App.GetGameManager<Mahjong2DGameManager>().Players[userData.seat].CurrentInfoPanel.UserIcon.GetTexture();
            if (tex!=null)
            {
                _head.mainTexture = tex;
            }
            else
            {
                YxDebug.LogError(string.Format("设置玩家头像失败,座位号是：{0}，昵称是{1}", userData.seat, userData.nick));
            }
            _playerName.TrySetComponentValue(userData.nick);
            _ziMoNumber.TrySetComponentValue(userData.zimo.ToString());
            _jiePaoNumber.TrySetComponentValue(userData.hu.ToString());
            _dianPaoNumber.TrySetComponentValue(userData.pao.ToString());
            _anGangUiLabelNumber.TrySetComponentValue(userData.anGang.ToString());
            _mingGangNumber.TrySetComponentValue(userData.anGang.ToString());
            _idNum.TrySetComponentValue(string.Format(IdFormat, userData.id));
            _totalNumber.TrySetComponentValue(YxUtiles.GetShowNumber(userData.gold).ToString());
            _houseOwner.TrySetComponentValue(userData.HouseOwner);
            ZeroSeatFlag.TrySetComponentValue(userData.seat == 0);
        }
    }
}
