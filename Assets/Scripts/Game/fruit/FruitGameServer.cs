using System.Collections.Generic;
using Sfs2X.Entities.Data;
using YxFramwork.Controller;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.fruit
{
    public class FruitGameServer : YxGameServer
    {
        const int TypeYazhu = 1;
        const int TypeCasino = 2;
        const int TypeRestart = 3;
        
        public void SendYazhuPlay(Dictionary<FruitType, int> dict)
        {
            var sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, TypeYazhu);
            var arr = FruitGameData.GetYazhuFruitArray(dict);
            sfsObject.PutIntArray(RequestKey.KeyAnte, arr);
            SendGameRequest(sfsObject); 
        } 

        public void SendBiDxPlay(int bet,int isDx)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, TypeCasino);
            sfsObject.PutInt(FruitRequestKey.KeyTypeDx, isDx);
            sfsObject.PutInt(RequestKey.KeyGold, bet);
            SendGameRequest(sfsObject); 
        }

        public void SendRestart()
        {
            var sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, TypeRestart);
            SendGameRequest(sfsObject);
        } 
    }
}
