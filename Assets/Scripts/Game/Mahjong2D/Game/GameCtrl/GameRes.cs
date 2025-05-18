using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Item;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl 
{
    /// <summary>
    /// 麻将资源管理类
    /// </summary>
    class GameRes:MonoSingleton<GameRes>
    {
        /// <summary>
        /// 麻将预设 
        /// </summary>
        public MahjongItem MahjongStand;

        /// <summary>
        /// 创建一个新的麻将,啥也不干
        /// </summary>
        /// <returns></returns>
        public MahjongItem GetNewMahjong()
        {
            MahjongItem mjItem = Instantiate(MahjongStand);
            return mjItem;
        }
    }
}
