using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.SportsCarClub 
{
    public class CschGameData : YxGameData
    { 
        public  Dictionary<string, object> UserInfor;  
        public  ISFSObject GameInfo; 
        /// <summary>
        /// 服务器时间，秒
        /// </summary>
        public  long ServerTime = 0;   
        public  Dictionary<string, string> GetParameterDict(IList<string> args, int startIndex = 0, Dictionary<string, string> envArgs = null)
        {
            if (args == null) return new Dictionary<string, string>();
            if (envArgs == null) envArgs = new Dictionary<string, string>();
            var len = args.Count - startIndex;
            for (var i = startIndex; i < len; i++)
            {
                var key = args[i].Trim();
                i++;
                var value = args[i].Trim();
                if (envArgs.ContainsKey(key)) envArgs[key] = value;
                else envArgs.Add(key, value);
            }
            return envArgs;
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            GameInfo = gameInfo;
        }
    }
}
