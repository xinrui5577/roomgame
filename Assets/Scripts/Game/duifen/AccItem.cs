using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.duifen
{
    public class AccItem : MonoBehaviour {

        /// <summary>
        /// 玩家名字
        /// </summary>
        [SerializeField]
        private UILabel _playerName = null;

        [SerializeField]
        private NguiTextureAdapter _headImage = null;


        /// <summary>
        /// 玩家胜利次数
        /// </summary>
        [SerializeField]
        private UILabel _winTimesValue = null;

        /// <summary>
        /// 玩家总战绩
        /// </summary>
        [SerializeField]
        private UILabel _allScore = null;

        /// <summary>
        /// 玩家失败次数
        /// </summary>
        [SerializeField]
        private UILabel _lostTimesValue = null;

        /// <summary>
        /// 房主标记
        /// </summary>
        [SerializeField]
        private GameObject _owerMark = null;

        /// <summary>
        /// 大赢家标记
        /// </summary>
        [SerializeField]
        private GameObject _bigWinnerMark = null;

        [HideInInspector]
        public int PlayerScore;


        /// <summary>
        /// 初始化总结算成员
        /// </summary>
        /// <param name="data">总结算成员信息</param>
        public void InitAccItem(Sfs2X.Entities.Data.ISFSObject data)
        {
            _winTimesValue.text = data.GetInt("win").ToString();   //获取胜利次数
            _lostTimesValue.text = data.GetInt("lost").ToString();      //获取失败次数
            PlayerScore = data.GetInt("gold");      //获取玩家的得分
            _allScore.text = PlayerScore.ToString();        //获取总分数


            if (data.ContainsKey("seat"))
            { 
                int seat = data.GetInt("seat");
                var gdata = App.GameData;
                DuifenPlayerPanel panel = gdata.GetPlayer<DuifenPlayerPanel>(seat, true);
                _playerName.text = panel.Info.NickM;
                _headImage.SetTexture(panel.HeadPortrait.GetTexture());

                if (seat == gdata.SelfSeat)
                {
                    SetNameLabelColor(0xffff00);
                }
                _owerMark.SetActive(seat == 0);
            }
        }


        /// <summary>
        /// 设置用户信息的颜色
        /// </summary>
        /// <param name="hex">颜色值,可用16进制0xffffff</param>
        private void SetNameLabelColor(int hex)
        {
            _playerName.color = Tool.Tools.ChangeToColor(hex);
        }

        /// <summary>
        /// 设置是否显示大赢家标记
        /// </summary>
        /// <param name="active"></param>
        public void SetBigWinnerMark(bool active)
        {
            _bigWinnerMark.SetActive(active);
        }

   
    }
}
