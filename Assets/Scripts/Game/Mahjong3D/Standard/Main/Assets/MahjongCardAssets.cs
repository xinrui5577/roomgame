using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [CreateAssetMenu(fileName = "MahjongCardAssets", menuName = "Mahjong Scriptable/MahjongCardAssets", order = 102)]
    public class MahjongCardAssets : ScriptableObjBast
    {
        //提示框背景
        public Sprite TileBackground;
        //麻将牌图集
        public Sprite[] MahjongSprites;
        public Sprite[] MahjongSmallSprites;
        //麻将2D背景
        public Mah2DObject[] UIMahjongBg;
        public Mah2DObject[] UIMahjong;
    }
}