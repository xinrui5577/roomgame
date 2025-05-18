using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI.InviteFriends
{
    public class InviteFriendsPanel : MonoBehaviour
    {
        public InviteFriendItem Item;
        public Transform Parent;
        public Text Meg;
      
        private List<InviteFriendItem> mItemCache = new List<InviteFriendItem>();

        private bool IsInit = false;       

        public void Init(List<FriendItemData> list, string roomId)
        {
            if (list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    //有多余的预制体
                    if (mItemCache.Count - 1 >= i)
                    {
                        mItemCache[i].gameObject.SetActive(true);
                        mItemCache[i].Init(list[i], roomId);
                    }
                    else
                    {
                        InviteFriendItem temp = YxWindowUtils.CreateItem(Item, Parent);
                        if (temp != null)
                        {
                            temp.Init(list[i], roomId);
                            mItemCache.Add(temp);
                        }
                    }
                }
            }

            Meg.gameObject.SetActive(list.Count == 0);
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);

            if (mItemCache.Count == 0) return;

            for (int i = 0; i < mItemCache.Count; i++)
            {
                mItemCache[i].gameObject.SetActive(false);
            }
        }

        public void ClearCache()
        {
            gameObject.SetActive(false);

            if (mItemCache.Count == 0) return;
          
            for (int i = 0; i < mItemCache.Count; i++)
            {
                DestroyImmediate(mItemCache[i].gameObject);
            }

            mItemCache.Clear();
        }

    }
     

    public class FriendItemData
    {
        public string UserID;
        public string Nick;
        public string Avater;      

        public FriendItemData(Dictionary<string, object> data)
        {
            if (data.ContainsKey("user_id"))
            {
                UserID = data["user_id"].ToString();
            }
            if (data.ContainsKey("nick_m"))
            {
                Nick = data["nick_m"].ToString();
            }
            if (data.ContainsKey("avatar_x"))
            {
                Avater = data["avatar_x"].ToString();
            }
        }
    }
}