/** 
 *文件名称:     MahjongItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         池对象接口
 *历史记录:     
*/

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Pool
{
    public interface IPoolItem
    {
        void Reset();
        void Collect();
        void Show();
    }
}
