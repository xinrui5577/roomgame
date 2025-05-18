using System.Collections;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    //集合类型参数
    public class VarCollection : Variable<ICollection>
    {
        public VarCollection() { }

        public VarCollection(ICollection collection) : base(collection) { }
    }
}