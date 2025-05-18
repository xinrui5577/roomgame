using System.Collections.Generic;
using YxFramwork.Controller;
using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahServerManager : YxGameServer
    {
        //投票解散
        public event Action<ISFSObject> OnHandsUpEvent;
        //游戏结束
        public event Action<ISFSObject> OnGameOverEvent;   
        //麻将游戏状态
        public event Action<ISFSObject> OnRollDiceEvent;

        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {                 
            callBackDic[GameKey + AnalysisKeys.RollDICE] = OnRollDice;          
            callBackDic[GameKey + "over"] = OnGameOver;
            callBackDic["hup"] = OnHandsUp;         
        }

        protected void OnGameOver(ISFSObject data)
        {
            OnGameOverEvent(data);        
        }

        protected void OnRollDice(ISFSObject data)
        {
            OnRollDiceEvent(data);         
        }
        
        protected void OnHandsUp(ISFSObject data)
        {
            OnHandsUpEvent(data);
        }

        /// <summary>
        /// 向服务器发出请求
        /// </summary>
        public void OnRequestC2S(ISFSObject data)
        {
            SendGameRequest(data);          
        }
        /// <summary>
        /// 向服务器发出请求
        /// </summary>
        public void OnSendFrameRequest(string cmd, ISFSObject data)
        {
            SendFrameRequest(cmd, data);
        }
        /// <summary>
        /// 换房间
        /// </summary>   
        public void OnChangeRoom()
        {
            ChangeRoom();
        }      
    }
}
