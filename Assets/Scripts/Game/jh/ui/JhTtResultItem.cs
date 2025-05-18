using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhTtResultItem : MonoBehaviour {

        public YxBaseTextureAdapter Head;
        public UILabel Name;
        public UISprite Icon;

        public UILabel Gold;

        public UILabel WinCnt;

        public UILabel LostCnt;


        public void SetInfo(string uname,string head,int sex,bool bigwinner,int gold,int wincnt,int lostcnt)
        {
            PortraitDb.SetPortrait(head, Head, sex);
            Name.text = uname;
            Icon.gameObject.SetActive(bigwinner);
            Gold.text = ""+gold;
            WinCnt.text = "" + wincnt;
            LostCnt.text = "" + lostcnt;
        }

    }
}
