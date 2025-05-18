using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Game.Item
{
    public class HelpItem : MonoBehaviour
    {
        [HideInInspector] public EnumMahjongValue MahjongValueName;

        private void Awake()
        {
            UIEventListener.Get(gameObject).onClick = delegate
            {
                ISFSObject data = new SFSObject();
                data.PutInt(RequestKey.KeyType, (int) EnumRequest.Card);
                data.PutInt(RequestKey.KeyCard, (int) MahjongValueName);
                App.GetRServer<Lyzz2DGameServer>().SendGameRequest(data);
                Debug.Log(string.Format("申请一张想要的牌{0},值是{1}", MahjongValueName, (int) MahjongValueName));
            };
        }
    }
}