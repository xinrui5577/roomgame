using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.NnFriendWindow
{
    public class NnGroupMemberItem : MonoBehaviour
    {
        [HideInInspector] 
        public string UserId;
        public UILabel MemberName;
        public UILabel MemBerCoin;
        public UILabel MemberId;
        public YxBaseTextureAdapter MemberHead;
        public GameObject GroupOwner;
        public GameObject BtnDeleteMember;

        public void InitData(string userName,string userId,string userHead,bool isShow,string coin="0")
        {
            UserId = userId;
            gameObject.SetActive(true);
            MemberName.text = userName;
//            if (!coin.Equals("0"))
//            {
//                if (MemBerCoin==null)return;
                MemBerCoin.gameObject.SetActive(true);
                MemBerCoin.text ="金币："+ YxUtiles.ReduceNumber(long.Parse(coin));
//            }
            MemberId.text ="UID:"+userId;
            PortraitDb.SetPortrait(userHead, MemberHead, 1);
            if (GroupOwner != null)GroupOwner.SetActive(isShow);
        }

       
    }
}
