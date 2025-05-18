using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.mdx
{
    public class MdxUserInfo :YxBaseGameUserInfo
    {
        public bool Forbiden;
        public string Msg; 

        public void SimpleParse(ISFSObject userData)
        {
            if(userData.ContainsKey("todayWin"))WinTotalCoin = userData.GetInt("todayWin");
            if (userData.ContainsKey("forbiden")) Forbiden = userData.GetBool("forbiden");
            if (userData.ContainsKey("total"))
            {
                CoinA = userData.GetLong("total");
            } 
            Msg = userData.ContainsKey("msg") ? userData.GetUtfString("msg") : "--";
        }
    }
}