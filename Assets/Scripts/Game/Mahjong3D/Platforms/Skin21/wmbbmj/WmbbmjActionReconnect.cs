using Sfs2X.Entities.Data;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class WmbbmjActionReconnect : ActionReconnect
    {
        public override void ReconnectAction(ISFSObject data)
        {
            base.ReconnectAction(data);
            var arr = data.TryGetIntArray("caipiao");
            if (arr != null)
            {
                var panel = GameCenter.Hud.GetPanel<PanelPlayersInfo>();
                var go = GameUtils.GetAssets<GameObject>("Caipiao");
                var sprite = go.GetComponent<Image>().sprite;

                var tempArr = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    var chair = MahjongUtility.GetChair(i);
                    panel[chair].SetHeadOtherImage(arr[i] > 0, sprite);
                    tempArr[chair] = arr[i];
                }
            }
        }
    }
}