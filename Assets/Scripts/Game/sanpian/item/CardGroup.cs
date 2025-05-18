using System.Collections.Generic;

namespace Assets.Scripts.Game.sanpian.item
{
    public class CardGroup {
        public int GroupNum = 0;
        public List<int> Member = new List<int>();
        /// <summary>
        /// 组上标注的牌值
        /// </summary>
        public int Gtype = 0;
        /// <summary>
        /// 该组的大小，仅在排序中使用此值
        /// </summary>
        public int GValue = 0;
        public void AddMember(int value)
        {
            Member.Add(value);
        }
        public void AddMemeberList(List<int> list)
        {
            Member.AddRange(list);
        }
    }
}
