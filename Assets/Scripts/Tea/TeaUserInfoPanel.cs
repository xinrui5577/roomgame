using Assets.Scripts.Common.Windows;
using UnityEngine;

namespace Assets.Scripts.Tea
{
    public class TeaUserInfoPanel : YxNguiWindow
    {
        public UILabel GameName;
        public UILabel RoomId;
        public UILabel RoundAndUse;
        public UILabel RuleInfo;
        public UILabel[] UserNames;
        public UILabel[] Ids;
        public UILabel[] Scores;
        public UITexture[] Heads;      
    }
}
