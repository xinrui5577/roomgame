using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongContainerManager : SceneManagerBase
    {
        /// <summary>
        /// 麻将编号记录
        /// </summary>
        private Dictionary<int, int> mMahjongIndexRecord = new Dictionary<int, int>();
        /// <summary>
        /// 麻将管理
        /// </summary>
        private Queue<MahjongContainer> mMahjongPool = new Queue<MahjongContainer>();

        public void InitalizationMahjong(IList<int> cards)
        {
            if (cards.Count == 0) { return; }
            var template = GameCenter.Assets.MahjongTemplate;
            for (int i = 0; i < cards.Count; i++)
            {
                OnCreateMahjong(template, GameUtils.GetMahjongCardAount(cards[i]));
            }
        }

        private void OnCreateMahjong(MahjongContainer obj, int aount)
        {
            MahjongContainer mahjongItem = null;
            for (int i = 0; i < aount; i++)
            {
                mahjongItem = Instantiate(obj);
                mahjongItem.transform.ExSetParent(transform);
                mahjongItem.OnReset();
                PushMahjongToPool(mahjongItem);
            }
        }

        /// <summary>
        /// 获取麻将
        /// </summary>
        /// <param name="card">牌值</param>
        /// <returns></returns>
        public MahjongContainer PopMahjong(int card)
        {
            MahjongContainer mahjong = null;
            if (mMahjongPool.Count > 10)
            {
                mahjong = mMahjongPool.Dequeue();                  
            }
            else
            {
                mahjong = Instantiate(GameCenter.Assets.MahjongTemplate);
            }
            if (mahjong != null)
            {
                if (!mMahjongIndexRecord.ContainsKey(card))
                {
                    mMahjongIndexRecord.Add(card, 0);
                }
                mahjong.Value = card;
                mahjong.MahjongIndex = mMahjongIndexRecord[card]++;
                mahjong.gameObject.SetActive(true);
            } 
            return mahjong;
        }

        /// <summary>
        /// 获取麻将组合，带有牌值的麻将牌
        /// </summary>
        /// <param name="cards">牌组</param>
        /// <returns></returns>
        public List<MahjongContainer> PopMahjong(IList<int> cards)
        {
            var ret = new List<MahjongContainer>();
            for (int i = 0; i < cards.Count; i++)
            {
                ret.Add(PopMahjong(cards[i]));
            }
            return ret;
        }

        /// <summary>
        /// 获取空白牌
        /// </summary>
        public MahjongContainer PopMahjong()
        {
            MahjongContainer mahjong = null;
            if (mMahjongPool.Count > 0)
            {
                mahjong = mMahjongPool.Dequeue();
            }
            else
            {
                var template = GameCenter.Assets.MahjongTemplate;
                mahjong = Instantiate(template);
            }
            mahjong.gameObject.SetActive(true);
            return mahjong;
        }

        /// <summary>
        /// 获取麻将列表，是空白的麻将牌
        /// </summary>
        /// <param name="num">获取数量</param>
        /// <returns></returns>
        public List<MahjongContainer> PopMahjongByNum(int num)
        {
            var ret = new List<MahjongContainer>();
            for (int i = 0; i < num; i++)
            {
                ret.Add(PopMahjong());
            }
            return ret;
        }

        /// <summary>
        /// 回收麻将
        /// </summary>
        public void PushMahjongToPool(MahjongContainer mahjong)
        {
            if (mahjong == null) return;
            //如果当前麻将 已经存在缓存池中，不再加入防止出现重复引用
            if (!mMahjongPool.Contains(mahjong))
            {
                mMahjongPool.Enqueue(mahjong);
            }
            mahjong.transform.ExSetParent(transform);
            mahjong.OnReset();
        }

        public override void OnReset()
        {
            mMahjongIndexRecord.Clear();
        }

        /// <summary>
        /// 更换麻将牌值
        /// </summary>
        /// <param name="mahjong">更换的麻将</param>
        /// <param name="newValue">新的牌值</param>
        public void SwitchMahjongValue(MahjongContainer mahjong, int newValue)
        {
            if (mahjong != null)
            {
                mahjong.Value = newValue;
            }
        }
    }
}