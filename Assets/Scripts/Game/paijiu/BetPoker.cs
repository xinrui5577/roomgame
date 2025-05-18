using com.yxixia.utile.YxDebug;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 649

namespace Assets.Scripts.Game.paijiu
{
    /// <summary>
    /// 下注区域的扑克管理
    /// </summary>
    public class BetPoker : MonoBehaviour
    {

        /// <summary>
        /// 玩家手中的牌
        /// </summary>
        [HideInInspector]
        public List<PaiJiuCard> PlayerPokers = new List<PaiJiuCard>();

        public int PokerCount
        {
            get
            {
                return PlayerPokers == null ? 0 : PlayerPokers.Count;
            }
        }

        private PaiJiuCard _selectCard1;
        private PaiJiuCard _selectCard2;

        internal void OnClickCard(int cardIndex)
        {
            ShowType(CouldShowType(cardIndex));
        }

        /// <summary>
        /// 牌型数组(含有影子效果)
        /// </summary>
        [SerializeField]
        private ShadelLabel[] _shadelLabels;


        /// <summary>
        /// 显示手牌牌型
        /// </summary>
        /// <param name="couldShow"></param>
        private void ShowType(bool couldShow)
        {
            if (_shadelLabels[0] == null || _shadelLabels[1] == null)
                return;

            _shadelLabels[0].gameObject.SetActive(couldShow);
            _shadelLabels[1].gameObject.SetActive(couldShow);
            if (!couldShow)
            {
                return;
            }
            ResortPlayerPokers();


            int[] valArr = GetPokersArr();


            int[] array1 = new int[] { valArr[0], valArr[1] };
            int[] array2 = new int[] { valArr[2], valArr[3] };

            //设置选择的牌组
            List<Group> gList = new List<Group>();
            Group g1 = new Group(array1);
            Group g2 = new Group(array2);
            gList.Add(g1);
            gList.Add(g2);
            gList.Sort((gr1, gr2) => gr2.CompareGroup(gr1));        //按牌组的大小排序

            //显示牌组的名字
            _shadelLabels[0].ShowLabel(GetTypeName(gList[0]));
            _shadelLabels[1].ShowLabel(GetTypeName(gList[1]));

            gList.Clear();
        }

        /// <summary>
        /// 按选择的牌的索引排序两个选择的牌.
        /// 避免的错误:避免先选大索引的牌,后选小索引的牌导致换牌导致的错误
        /// </summary>
        private void SortSelet()
        {
            if (_selectCard1.CardIndex < _selectCard2.CardIndex)
                return;
            var temp = _selectCard1;
            _selectCard1 = _selectCard2;
            _selectCard2 = temp;
        }

        public string GetTypeName(Group group)
        {

            return Tool.Tools.GetName(group);

        }

        public void CleanSelected()
        {
            if (PlayerPokers == null || PlayerPokers.Count == 0)
                return;
            foreach (PaiJiuCard card in PlayerPokers)
            {
                card.MoveDown();
            }
            _selectCard1 = null;
            _selectCard2 = null;
        }

        /// <summary>
        /// 将手牌数值转为int数组
        /// </summary>
        /// <returns></returns>
        public int[] GetPokersArr()
        {
            if (_selectCard1 == null || _selectCard2 == null)
                return null;
            int count = PlayerPokers.Count;
            int[] idArr = new int[count];
            for (int i = 0; i < count; i++)
            {
                idArr[i] = PlayerPokers[i].Id;
            }

            SortSelet();
            SwitchCard(idArr, _selectCard1.CardIndex, 0);
            SwitchCard(idArr, _selectCard2.CardIndex, 1);
            return idArr;
        }

        /// <summary>
        /// 交换数组中两个数的位置
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        private void SwitchCard(int[] array, int index1, int index2)
        {
            if (index1 == index2)
            {
                return;
            }

            var temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }


        private bool CouldShowType(int cardIndex)
        {
            var card = PlayerPokers[cardIndex];
            //如果牌被选择了,清理掉该牌
            if (card.IsSelected)
            {
                if (card.Equals(_selectCard1))
                {
                    _selectCard1 = null;
                }
                if (card.Equals(_selectCard2))
                {
                    _selectCard2 = null;
                }
                card.MoveDown();
                return _selectCard1 != null && _selectCard2 != null && !_selectCard1.Equals(_selectCard2);   //排除点击的牌不是选择牌队列中
            }
            else         //没被选中,被选中牌不为2的时候加入选中牌行列
            {
                if (_selectCard1 == null)
                {
                    _selectCard1 = card;
                    card.MoveUp();
                    return _selectCard2 != null;
                }
                if (_selectCard2 == null)
                {
                    _selectCard2 = card;
                    card.MoveUp();
                    return _selectCard1 != null;
                }
                return true;
            }
        }



        /// <summary>
        /// 直接显示牌的面值
        /// </summary>
        /// <param name="cardVal"></param>
        /// <param name="index"></param>
        public void SetPokerValue(int cardVal, int index)
        {
            if (index >= PlayerPokers.Count)
                return;

            PaiJiuCard card = PlayerPokers[index];
            if (card == null)
                return;
            card.SetCardId(cardVal);
            card.SetCardFront();
        }


        private readonly List<GroupInfo> _groupList = new List<GroupInfo>();
        public void AddGoupInfo(int[] cards, int type)
        {
            GroupInfo groupInfo = new GroupInfo(cards, type);
            _groupList.Add(groupInfo);
        }

        /// <summary>
        /// 一次显示4张牌
        /// </summary>
        public void SetBetPokerInfo()
        {
            if (_groupList == null || _groupList.Count < 2)
            {
                YxDebug.LogError(" ==== 手牌信息错误 ==== ");
                return;
            }

            for (int i = 0; i < PlayerPokers.Count / 2; i++)
            {
                int index = 0;
                for (int j = i * 2; j < 2 * (i + 1); j++)
                {
                    PlayerPokers[j].SetCardId(_groupList[i].Cards[index++]);
                    PlayerPokers[j].SetCardFront();
                }
            }

            ShowGroupType(0);
            ShowGroupType(1);
        }

        /// <summary>
        /// 分组显示牌
        /// </summary>
        /// <param name="groupIndex">牌组索引</param>
        public void SetBetPokerInfo(int groupIndex)
        {
            PlayerPokers[groupIndex * 2].Id = _groupList[groupIndex].Cards[0];
            PlayerPokers[groupIndex * 2 + 1].Id = _groupList[groupIndex].Cards[1];
        }

        /// <summary>
        /// 显示牌组的牌型
        /// </summary>
        /// <param name="groupIndex"></param>
        public void ShowGroupType(int groupIndex)
        {
            _shadelLabels[groupIndex].ShowLabel(_groupList[groupIndex].Name);
        }


        public void SetBetPokerInfo(int[] cards)
        {
            for (int i = 0; i < PlayerPokers.Count; i++)
            {
                PlayerPokers[i].SetCardId(cards[i]);
                PlayerPokers[i].SetCardFront();
            }
        }

        internal void AddPoker(PaiJiuCard pokerCard)
        {
            PlayerPokers.Add(pokerCard);
        }

        /// <summary>
        /// 按照牌的索引,重新排列数组
        /// </summary>
        private void ResortPlayerPokers()
        {
            PlayerPokers.Sort((card1, card2) => card1.CardIndex - card2.CardIndex);
        }


        internal void CleanCardsBoxCollider()
        {
            if (PlayerPokers == null || PlayerPokers.Count == 0)
                return;
            foreach (var item in PlayerPokers)
            {
                item.GetComponent<BoxCollider>().enabled = false;
            }
        }

        public void HideAllTypeLabel()
        {
            _shadelLabels[0].HideLabel();
            _shadelLabels[1].HideLabel();
        }

        public void Reset()
        {
            PlayerPokers.Clear();
            HideAllTypeLabel();
            _selectCard1 = null;
            _selectCard2 = null;
            _groupList.Clear();
        }
    }

    public class GroupInfo
    {
        public int[] Cards;
        public int Type;
        public string Name
        {
            get { return Tool.Tools.GetName(Cards, Type); }
        }

        public GroupInfo(int[] groupCards, int groupType)
        {
            Cards = groupCards;
            Type = groupType;
        }
    }
}