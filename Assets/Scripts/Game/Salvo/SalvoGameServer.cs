using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.Salvo
{
    public class SalvoGameServer : YxGameServer
    { 
        /// <summary>
        /// 更改底注
        /// </summary>
        /// <param name="bet"></param>
        public void SendChangeBet(int bet)
        {
            
        }

        /// <summary>
        /// 请求开始
        /// </summary>
        public void SendStart()
        {

        }


        public void SendGetPokersDate(int ante)
        { 
            YxDebug.Log("请求发牌");
            var data = new SFSObject();
            data.PutInt(RequestKey.KeyType, 1);
            data.PutInt(SalvoRequestKey.KeyRate, ante);
            SendGameRequest(data);
        }

        public void SendreplacePokersDate(int ante, int[] changePokers = null)
        {
            YxDebug.Log("请求换牌");
            YxDebug.LogArray(changePokers);
            var data = new SFSObject();
            data.PutInt(RequestKey.KeyType, 2);
            if (changePokers != null) data.PutIntArray(RequestKey.KeyCards,changePokers);
            SendGameRequest(data);
        }
    }
}
