using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Abstracts;

namespace Assets.Scripts.Game.biji.Modle
{
    public class BjRuleInfo : YxCreateRoomInfo
    {
        public Dictionary<string,string> Cargs = new Dictionary<string, string>();

        public void SetCargs(ISFSObject cargs2)
        {
            string[] keys = cargs2.GetKeys();
            foreach (string s in keys)
            {
                Cargs.Add(s,cargs2.GetUtfString(s));
            }
        }
        
    }
}
