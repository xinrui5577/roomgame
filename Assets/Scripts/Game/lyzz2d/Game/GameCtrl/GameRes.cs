using Assets.Scripts.Game.lyzz2d.Game.Item;
using Assets.Scripts.Game.lyzz2d.Utils.Single;

namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    /// <summary>
    ///     麻将资源管理类
    /// </summary>
    internal class GameRes : MonoSingleton<GameRes>
    {
        /// <summary>
        ///     麻将预设
        /// </summary>
        public MahjongItem MahjongStand;

        /// <summary>
        ///     创建一个新的麻将,啥也不干
        /// </summary>
        /// <returns></returns>
        public MahjongItem GetNewMahjong()
        {
            var mjItem = Instantiate(MahjongStand);
            return mjItem;
        }
    }
}