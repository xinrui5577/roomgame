using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class Lyzz2DUserInfoDetail : YxNguiWindow
    {
        [SerializeField] private YxBaseTextureAdapter _userHead;

        [SerializeField] private UILabel UserID;

        [SerializeField] private UILabel UserIP;

        [SerializeField] private UILabel UserName;


        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data == null)
            {
                return;
            }
            GetUserData();
        }

        private void GetUserData()
        {
            if (Data is string)
            {
                var manager = App.GetGameManager<Lyzz2DGameManager>();
                if (manager)
                {
                    var USeat = int.Parse(Data.ToString());
                    var sit = (manager.CurrentSeat + USeat)%manager.PlayerNumber;
                    if (manager.Players != null && (manager.Players.Length > sit))
                    {
                        var info = manager.Players[sit].UserInfo;
                        UserName.text = name;

                        YxTools.TrySetComponentValue(UserName, info.name);
                        YxTools.TrySetComponentValue(UserID, info.id.ToString());
                        YxTools.TrySetComponentValue(UserIP, info.ip);
                        if (_userHead)
                        {
                            var texture = manager.Players[sit].CurrentInfoPanel.UserIcon;
                            if (texture)
                            {
                                _userHead.SetTexture(texture.mainTexture);
                            }
                        }
                    }
                }
            }
        }
    }
}