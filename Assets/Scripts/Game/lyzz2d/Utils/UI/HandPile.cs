/** 
 *文件名称:     HandPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         手牌的处理
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Game;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class HandPile : MahjongPile
    {
        [SerializeField] private GetInCard _getInCard;

        public GetInCard NewCard
        {
            set { _getInCard = value; }
            get
            {
                if (_getInCard == null)
                {
                    _getInCard = GetComponentInChildren<GetInCard>();
                }
                return _getInCard;
            }
        }

        public virtual Transform AddCard(int value, bool isSingle, bool changeNumber = true)
        {
            var newMahJong = GameTools.CreateMahjong(value, changeNumber);
            ParseItemToThis(newMahJong);
            if (isSingle)
            {
                var item = newMahJong.GetComponent<MahjongItem>();
                ParseItemToThis(item);
                NewCard.GetIn = item;
                GameTools.AddChild(NewCard.transform, item.transform, ItemScaleX, ItemScaleY);
            }
            else
            {
                AddItem(newMahJong);
            }
            newMahJong.GetComponent<MahjongItem>().JudgeHunTag(App.GetGameManager<Lyzz2DGameManager>().LaiZiNum);
            return newMahJong;
        }

        public void SetGetCardItem(int value)
        {
            var findItem = Layout.GetChildList().Find(item => item.GetComponent<MahjongItem>().Value.Equals(value));
            if (findItem != null)
            {
                GameTools.AddChild(NewCard.transform, findItem.transform, ItemScaleX, ItemScaleY);
                NewCard.GetIn = findItem.GetComponent<MahjongItem>();
                Layout.ResetPositionNow = true;
            }
            else
            {
                YxDebug.LogError("There is not exist such value in handcards,value is :" + value);
            }
        }

        public MahjongItem GetCardByValue(int value)
        {
            if (_getInCard.GetIn != null && _getInCard.GetIn.Value.Equals(value))
            {
                return _getInCard.GetIn;
            }
            var FindTrans =
                Layout.GetChildList().Find(item => item.GetComponent<MahjongItem>().Value.Equals(value));
            if (FindTrans)
            {
                return FindTrans.GetComponent<MahjongItem>();
            }
            return null;
        }

        public override void ResetPile()
        {
            base.ResetPile();
            if (NewCard.GetIn = null)
            {
                NewCard.GetIn = null;
            }
            var lenth = NewCard.transform.childCount;
            while (lenth > 0)
            {
                Destroy(NewCard.transform.GetChild(0).gameObject);
                lenth--;
            }
        }
    }
}