using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Fishing.windows
{
    /// <summary>
    /// 切换枪炮窗口
    /// </summary>
    public class ChangeGunWindow : YxUguiWindow
    {
        public ChangeGunItemView[] Items;

        protected override void OnStart()
        {
            base.OnStart();
            var len = Items.Length;
            for (var i = 0; i < len; i++)
            {
                var item = Items[i];
                item.GunLeve = i;
                item.MainYxView = this;
            }
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var gdata = App.GetGameData<FishingGameData>();
            var userInfo = gdata.GetPlayerInfo<FishingUserInfo>();
            var gunLeve = userInfo.GunLeve;
            var gunType = gdata.GunType;
            var len = Items.Length;
            for (var i = 0; i < len; i++)
            {
                var item = Items[i];
                var data = new ChangeGunData
                {
                    IsLock = i != gunType && i != gunType + 1,
                    IsSelected = gunLeve == i,
                    Bet = 1,
                    GunLeve = i,
                    HasLockTip = gunLeve >= i
                };
                item.UpdateView(data);
            }
        }
         
    }
}
