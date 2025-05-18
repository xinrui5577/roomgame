using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.brnn3d
{
    public class Brnn3DUserInfo : YxBaseGameUserInfo
    {
        public override void Parse(ISFSObject userData)
        {
            base.Parse(userData);
            if (userData.ContainsKey("username")) NickM = userData.GetUtfString("username");
        }
    }
}
