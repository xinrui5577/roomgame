using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.lyzz2d.Utils;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.lyzz2d.Game.Item.GameOverItem
{
    /// <summary>
    /// 大结算Item
    /// </summary>
    public class GameOverItem : MonoBehaviour
    {
        [SerializeField] private UILabel _anGangUiLabelNumber;

        [SerializeField] private UILabel _dianPaoNumber;

        [SerializeField] private UITexture _head;

        [SerializeField] private GameObject _houseOwner;

        [SerializeField] private UILabel _idNum;

        [SerializeField] private UILabel _jiePaoNumber;

        [SerializeField] private GameObject _loser;

        [SerializeField] private UILabel _mingGangNumber;

        [SerializeField] private UILabel _playerName;

        [SerializeField] private UILabel _totalNumber;

        [SerializeField] private GameObject _windner;

        [SerializeField] private UILabel _ziMoNumber;

        public void InitInfo(OverData userData)
        {
            _windner.SetActive(userData.isYingJia);
            _loser.SetActive(userData.isPaoShou);
            // 房卡模式中seat为0的就是房主？好像是的，第一个进入的就是
            if (userData.seat == 0 && App.GetGameData<Lyzz2DGlobalData>().CurrentGame.GameRoomType == -1)
            {
                _houseOwner.SetActive(true);
            }
            else
            {
                _houseOwner.SetActive(false);
            }
            _head.mainTexture =
                App.GetGameManager<Lyzz2DGameManager>().Players[userData.seat].CurrentInfoPanel.UserIcon.mainTexture;
            _playerName.text = userData.nick;
            _ziMoNumber.text = userData.zimo.ToString();
            _jiePaoNumber.text = userData.hu.ToString();
            _dianPaoNumber.text = userData.pao.ToString();
            _anGangUiLabelNumber.text = userData.anGang.ToString();
            _mingGangNumber.text = userData.anGang.ToString();
            YxTools.TrySetComponentValue(_totalNumber, YxUtiles.GetShowNumber(userData.gold).ToString());
            _idNum.text = string.Format("ID:{0}", userData.id);
        }
    }
}