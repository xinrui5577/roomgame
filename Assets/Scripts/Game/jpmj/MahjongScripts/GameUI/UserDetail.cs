using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class UserDetail : PopPnlBase
    {
        public Text Name;
        public Text Id;
        public Text Ip;
        public RawImage Head;

        public virtual void SetShow(UserData data)
        {
            Name.text = data.name;
            Ip.text = data.ip;
            Id.text = data.id + "";
            gameObject.SetActive(true);
        }

        public void SetHead(string url, Texture define)
        {
            AsyncImage.GetInstance().SetTextureWithAsyncImage(url, Head, define);
        }

        public void Close()
        {
            Hide();
        }

    }
}
