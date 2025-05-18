using com.yxixia.utile.Utiles;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 炮架，用来显示自己炮架和别人炮架不同时加载不同资源
    /// </summary>
    public class Battery : YxBaseGamePlayer
    {
        /// <summary>
        /// 类型
        /// </summary>
        public int StyleType;

        public FishingPlayer Player;

        protected override void FreshUserInfo()
        {
            var info = Info;
            if (info == null) { return; }
            if (Player == null)
            {
                var isSelf = info.Seat == App.GameData.SelfSeat;
                var selfSgin = isSelf ? 0 : 1; 
                var pname = string.Format("Player_{0}_{1}", StyleType, selfSgin);
                var bname = string.Format("Players/{0}", pname);
                var go = ResourceManager.LoadAsset(pname, bname);
                if (go == null) return;
                var newPlayer = GameObjectUtile.Instantiate(go,transform);
                Player = newPlayer.GetComponent<FishingPlayer>();
            }
            Player.UpdateView(info);
        }
    }
}
