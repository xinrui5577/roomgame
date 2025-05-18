using Assets.Scripts.Game.lyzz2d.Utils;

namespace Assets.Scripts.Game.lyzz2d.Game.Data
{
    /// <summary>
    ///     吃，碰，杠的记录
    /// </summary>
    public class MahjongGroupData
    {
        private int index;
        public GroupType type;
        public int[] values;

        public MahjongGroupData(GroupType type)
        {
            this.type = type;
            switch (type)
            {
                case GroupType.Chi:
                case GroupType.Peng:
                case GroupType.JueGang:
                    values = new int[3];
                    break;
                default:
                    values = new int[4];
                    break;
            }
            index = 0;
        }

        public void AddValue(int val)
        {
            values[index++] = val;
        }
    }
}