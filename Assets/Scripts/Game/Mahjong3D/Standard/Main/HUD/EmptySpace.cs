using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class EmptySpace : MonoBehaviour
    {
        public void OnClickEmptySpace()
        {
            if (GameCenter.GameProcess != null && GameCenter.GameProcess.IsCurrState<StateGamePlaying>())
            {
                var panel = GameCenter.Hud.GetPanel<PanelQueryHuCard>();
                if (null != panel) panel.Close();
                var PlayerHand = GameCenter.Scene.MahjongGroups.PlayerHand;
                if (PlayerHand.CurrState == HandcardStateTyps.Normal || PlayerHand.CurrState == HandcardStateTyps.DingqueOver)
                {
                    PlayerHand.HandCardsResetPos();
                }
            }
        }
    }
}


