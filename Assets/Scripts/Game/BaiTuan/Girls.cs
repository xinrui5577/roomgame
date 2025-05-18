using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.BaiTuan
{
    public class Girls : MonoBehaviour
    {
        public Animator GirlAn;
 
        public void SetShowUser(GameObject sender, object data)
        {
        
            //todo 播放动画
            Debug.Log("应播放动画");
        }

        public void OnGrilClick()
        {
            SFSObject sfsObject = new SFSObject();
            sfsObject.PutInt("type", 9);
            //EventMng.GetInstance().SendEvent(EventId.EventSendReward,sfsObject);
        }


        public void Play()
        {
            App.GetGameData<BtwGameData>().GetPlayer().Coin -= 100;
            GirlAn.Play(0); 
        }

    }
}
