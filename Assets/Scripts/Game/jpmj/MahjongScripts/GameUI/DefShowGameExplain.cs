using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using System;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
   public class DefShowGameExplain : ShowGameExplain
    {
        void Awake()
        {
           
            EventDispatch.Instance.RegisteEvent((int)UIEventId.RoomInfo, ParseGameExplainInfo);

        }

        protected void ParseGameExplainInfo(int id, EventData data)
        {
            try
            {
                TryParseGameExplainInfo(id,data);
            }
            catch (NullReferenceException e)
            {               
               YxDebug.LogError("服务发来的消息可能为空:" + e);
            }            
        }

        protected  void TryParseGameExplainInfo(int id, EventData data)
        {
            var roomInfo = (RoomInfo)data.data1;
            Dictionary<string,string> gameExplain = roomInfo.GetRoomRule();                    
            ShowExplains(gameExplain);
        }

    }
}
