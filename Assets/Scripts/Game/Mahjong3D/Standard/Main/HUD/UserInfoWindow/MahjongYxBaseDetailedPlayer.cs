using YxFramwork.Framework;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongYxBaseDetailedPlayer : YxBaseDetailedPlayer
    {
        /// <summary>
        /// 房卡
        /// </summary>
        public YxBaseLabelAdapter Item2qLabel;

        public void SetFk(string item2q) { Item2qLabel.Text(item2q); }

        public void SetId(string id) { UserIdLabel.Text(id); }
    }
}