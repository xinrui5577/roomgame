using Sfs2X.Entities.Data;
using YxFramwork.Controller;

/*===================================================
 *文件名称:     NbjlGameServer.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:        	百家乐2d游戏交互服务
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class NbjlGameServer : YxGameServer
    {
        /// <summary>
        /// 下注请求
        /// </summary>
        /// <param name="positionName">下注位置名称</param>
        /// <param name="value">下注值</param>
        public void UserBet(string positionName,int value)
        {
            var sfsObject=new SFSObject();
            sfsObject.PutUtfString(ConstantData.KeyBetPosition, positionName);
            sfsObject.PutInt(ConstantData.KeyGold,value);
            sfsObject.PutInt(ConstantData.KeyType,(int)ServerRequest.ReqBet);
            SendGameRequest(sfsObject);
        }


        /// <summary>
        /// 批量下注
        /// </summary>
        /// <param name="golds"></param>
        public void UserBets(int[] golds)
        {
            var sfsObject = new SFSObject();
            sfsObject.PutIntArray(ConstantData.KeyGolds, golds);
            sfsObject.PutInt(ConstantData.KeyType, (int)ServerRequest.ReqBet);
            SendGameRequest(sfsObject);
        }

        /// <summary>
        /// 申请上庄
        /// </summary>
        public void ApplyBanker()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt(ConstantData.KeyType, (int)ServerRequest.ReqApplyBanker);
            SendGameRequest(sfsObject);
        }

        /// <summary>
        /// 申请下庄
        /// </summary>
        public void ApplyQuitBanker ()
        {
            var sfsObject = new SFSObject();
            sfsObject.PutInt(ConstantData.KeyType, (int)ServerRequest.ReqApplyQuitBanker);
            SendGameRequest(sfsObject);
        }
    }
}
