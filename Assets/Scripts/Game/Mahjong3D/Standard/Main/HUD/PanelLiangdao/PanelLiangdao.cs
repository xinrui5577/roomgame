using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [UIPanelData(typeof(PanelLiangdao), UIPanelhierarchy.Popup)]
    public class PanelLiangdao : UIPanelBase
    {
        public UIItemsManager ItemStore;

        public void Open(int[] cards)
        {
            base.Open();

            GameCenter.Hud.GetPanel<PanelChooseOperate>().ClosePanel();
            GameCenter.Hud.GetPanel<PanelOpreateMenu>().Close();

            for (int i = 0; i < cards.Length; i++)
            {
                var item = ItemStore.GetItem<LiangdaoGridItem>(i);
                if (item != null)
                {
                    item.SetCard(cards[i]);
                }
            }
        }

        public void OnSureClick()
        {
            var anpuList = new List<int>();

            for (int i = 0; i < ItemStore.Store.Count; i++)
            {
                var item = ItemStore.GetItem<LiangdaoGridItem>(i);
                if (!item.ChooseFlag)
                {
                    anpuList.Add(item.Value);
                }
            }

            var hand = GameCenter.Scene.MahjongGroups.PlayerHand.GetComponent<QdjtMahjongPlayerHand>();
            hand.SendLiangPai(anpuList.ToArray());
            OnReset();
        }

        private void OnReset()
        {
            base.Close();
            ItemStore.HideItems();
        }

        public void OnCancelClick()
        {
            base.Close();
            ItemStore.HideItems();
            GameCenter.Hud.GetPanel<PanelOpreateMenu>().Open();
            //GameCenter.EventHandle.Dispatch((int)UIEventProtocol.QueryHuCard, new QueryHuArgs() { PanelState = false });
        }
    }
}
