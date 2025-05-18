using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.sssjp.Tool;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;

#pragma warning disable 649

namespace Assets.Scripts.Game.sssjp
{
    public class SumUserInfo : MonoBehaviour
    {

        [SerializeField]
        private UILabel _nameLabel;

        [SerializeField]
        private UILabel _idLabel;

        [SerializeField]
        private NguiTextureAdapter _userIcon;

        [SerializeField]
        private NguiLabelAdapter _totalScoreLabel;

        [SerializeField]
        private GameObject _line;

        [SerializeField]
        private GameObject _bigWnnerMark;

        /// <summary>
        /// 玩家得分数据存储
        /// </summary>
        [HideInInspector]
        public int[] ScoreArray;

        [HideInInspector]
        public int Id;

        public void Init(SummaryUserInfo sumInfo)
        {
            SetLabel(_totalScoreLabel, sumInfo.Gold);

            _nameLabel.text = sumInfo.Nick;
            Id = sumInfo.Id;
            _idLabel.text = "ID:" + Id;
            ScoreArray = sumInfo.Record;
            gameObject.SetActive(Id > 0);
            var seat = sumInfo.Seat;
            var userInfo = App.GetGameData<SssGameData>().GetPlayerInfo(seat, true);
            if (userInfo!=null)
            {
                PortraitDb.SetPortrait(userInfo.AvatarX, _userIcon, userInfo.SexI);
            }
        }

        public void SetWidget(int width)
        {
            var widget = GetComponent<UIWidget>();
            if (widget == null) return;
            widget.width = width;
        }

        void SetLabel(NguiLabelAdapter adapter, int score)
        {
            adapter.Text(score);
            var label = adapter.Label;
            if (score < 0)
            {
                label.gradientTop = Tools.ChangeToColor(0x6FFBF1);
                label.gradientBottom = Tools.ChangeToColor(0x0090FF);
                label.effectColor = Tools.ChangeToColor(0x002EA3);
            }
            else
            {
                label.gradientTop = Tools.ChangeToColor(0xFFFF00);
                label.gradientBottom = Tools.ChangeToColor(0xFF9600);
                label.effectColor = Tools.ChangeToColor(0x831717);
            }

            label.gameObject.SetActive(true);
        }

        public void HideLine()
        {
            _line.SetActive(false);
        }

        public void ShowBigWinnerMark()
        {
            _bigWnnerMark.SetActive(true);
        }
    }
}