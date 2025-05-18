using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jh.Modle
{
    public class JhTtResultInfo : MonoBehaviour
    {

        public EventObject EventObj;

        public class TtResultItem
        {
            public string Name;
            public string Avatar;
            public int Sex;
            public int WinCnt;
            public int LostCnt;
            public int BipaiCnt;
            public int Gold;
            public bool IsBigWinner;

            public ISFSObject GetSfsObj()
            {
                ISFSObject obj = SFSObject.NewInstance();
                obj.PutUtfString("Name",Name);
                obj.PutUtfString("Avatar", Avatar);
                obj.PutInt("Sex",Sex);
                obj.PutInt("WinCnt", WinCnt);
                obj.PutInt("LostCnt", LostCnt);
                obj.PutInt("BipaiCnt", BipaiCnt);
                obj.PutInt("Gold", Gold);
                obj.PutBool("IsBigWinner",IsBigWinner);
                return obj;
            }
        }

        protected List<TtResultItem> ItemList = new List<TtResultItem>();

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "TtResult":
                    OnTtResult(data.Data);
                    break;
            }
        }

        private void OnTtResult(object data)
        {
            ItemList.Clear();
            
            ISFSObject obj = (ISFSObject) data;

            ISFSArray usersArr = obj.GetSFSArray("users");

            for (int i = 0; i < usersArr.Count; i++)
            {
                ISFSObject objItem = usersArr.GetSFSObject(i);
                if (objItem.GetKeys().Length > 0)
                {
                    var rItem = new TtResultItem
                    {
                        Name = objItem.GetUtfString("nick"),
                        Avatar = objItem.GetUtfString("avatar"),
                        Sex = objItem.GetShort("sex"),
                        Gold = objItem.GetInt("gold"),
                        WinCnt = objItem.GetInt("win"),
                        LostCnt = objItem.GetInt("lose"),
                        BipaiCnt = objItem.GetInt("abandon")
                    };
                    ItemList.Add(rItem);
                }
            }
            int bigWiner = 0;
            int cnt = 0;
            for(int i = 0;i<ItemList.Count;i++)
            {
                TtResultItem item = ItemList[i];
                if (item.WinCnt > cnt)
                {
                    bigWiner = i;
                    cnt = item.WinCnt;
                }
            }

            ItemList[bigWiner].IsBigWinner = true;

            ISFSArray sendArr = SFSArray.NewInstance();
            foreach (TtResultItem item in ItemList)
            {
                sendArr.AddSFSObject(item.GetSfsObj());
            }
            EventObj.SendEvent("TtResultViewEvent", "TtResultInfo", sendArr);
        }
    }
}
