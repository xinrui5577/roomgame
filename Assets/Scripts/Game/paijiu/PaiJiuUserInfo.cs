using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.paijiu
{
    public class PaiJiuUserInfo : YxBaseGameUserInfo
    {
        public bool IsPut;
        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            IsPut = userData.ContainsKey("isput") && userData.GetInt("isput") > 0;
        }
    }
}
