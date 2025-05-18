using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelGameExplain), UIPanelhierarchy.Popup)]
    public class PanelGameExplain : UIPanelBase
    {
        public override void OnContinueGameUpdate() { Close(); }
        public GameObjectCollections UICollections;

        public override void Open()
        {
            base.Open();
            GameObject obj = UICollections.Get(MahjongUtility.GameKey);
            if (null != obj)
            {
                obj.gameObject.SetActive(true);
            }
        }
    }
}