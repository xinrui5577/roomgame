using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongTableSign : MahjongTablePart, IGameInfoICycle
    {
        public SpriteRenderer Sign;

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnGameInfoICycle()
        {
            Sign.sprite = GameCenter.Assets.GetSprite("Tablesign" + MahjongUtility.GameKey);
        }
    }
}