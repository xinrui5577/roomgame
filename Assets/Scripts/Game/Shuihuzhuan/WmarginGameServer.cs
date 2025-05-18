using Sfs2X.Entities.Data;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class WmarginGameServer : YxGameServer
    { 
        /// <summary>
        /// 开始服务器发送数据
        /// </summary>
        /// <param name="betting">压注的钱</param>
        public void SendMyGameStart(int betting)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", 1);
            sfsObject.PutInt("gold", betting);
            SendGameRequest(sfsObject);
        }

        /// <summary>
        /// 大小和服务器发送数据
        /// </summary>
        /// <param name="betting">压注的钱</param>
        /// <param name="dxh"></param>
        public void SendMyDaXiaoHe(int betting,int dxh)
        {
            YxDebug.LogError(betting);
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", 2);
            sfsObject.PutInt("gold", betting);
            sfsObject.PutInt("dx", dxh);
            SendGameRequest(sfsObject);
        }
        /// <summary>
        /// 玛丽向服务器发送数据
        /// </summary>
        public void SendMaLi()
        {   
            var sfsObject = new SFSObject();
            sfsObject.PutInt("type", 3);
            SendGameRequest(sfsObject);
            //            YxDebug.LogError("客户端=》-------玛丽----------");
        }
    }
      
}