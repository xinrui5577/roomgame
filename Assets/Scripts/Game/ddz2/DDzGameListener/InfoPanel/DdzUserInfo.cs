using Assets.Scripts.Game.ddz2.DDz2Common;
using YxFramwork.Common.Model;


namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class DdzUserInfo : YxBaseGameUserInfo
    {

        /// <summary>
        /// 当前局的倍数
        /// </summary>
        public int Rate = 1;

        /// <summary>
        /// 是否加倍
        /// </summary>
        public bool IsRate;

        public bool NetWork;

        /// <summary>
        /// 准备状态
        /// </summary>
        public bool AutoState;

        public override void Parse(Sfs2X.Entities.Data.ISFSObject userData)
        {
            base.Parse(userData);
            if (userData.ContainsKey(NewRequestKey.KeyRate))
            {
                Rate = userData.GetInt(NewRequestKey.KeyRate);
            }
            
            if (userData.ContainsKey(NewRequestKey.KeyIsRate))
            {
                IsRate = userData.GetInt(NewRequestKey.KeyIsRate) > 1;
            }

            if (userData.ContainsKey(NewRequestKey.KeyNetWork))
            {
                NetWork = userData.GetBool(NewRequestKey.KeyNetWork);
            }

            if (userData.ContainsKey(NewRequestKey.KeyTrusteeship))
            {
                AutoState = userData.GetBool(NewRequestKey.KeyTrusteeship);
            }
        }

    }
}