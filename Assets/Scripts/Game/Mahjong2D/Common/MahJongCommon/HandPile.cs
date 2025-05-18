/** 
 *文件名称:     HandPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         手牌的处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    public class HandPile : MahjongPile
    {
        [SerializeField]
        private GetInCard _getInCard;
        
        public GetInCard NewCard
        {
            set { _getInCard = value; }
            get
            {
                if (_getInCard==null)
                {
                    _getInCard = GetComponentInChildren<GetInCard>();
                }
                return _getInCard;
            }
        }
        public virtual Transform AddCard(int value,bool isSingle,bool changeNumber=true)
        {
            Transform newMahJong = GameTools.CreateMahjong(value,changeNumber);
            ParseItemToThis(newMahJong);
            if (isSingle)
            {
                MahjongItem item= newMahJong.GetComponent<MahjongItem>();
                SetMahjongAsNewGetIn(item);
            }
            else
            {
                AddItem(newMahJong);
            }
            newMahJong.GetComponent<MahjongItem>().JudgeHunTag(App.GetGameManager<Mahjong2DGameManager>().LaiZiNum);
            return newMahJong;
        }

        /// <summary>
        /// 设置一张麻将作为新抓手牌
        /// </summary>
        /// <param name="item"></param>
        public void SetMahjongAsNewGetIn(MahjongItem item)
        {
            if (item)
            {
                NewCard.GetIn = item;
                GameTools.AddChild(NewCard.transform, item.transform, ItemScaleX, ItemScaleY);
                item.SelfData.MahjongLayer = BaseLayer;
                Layout.ResetPositionNow = true;
            }
        }

        public void SetGetCardItem(int value)
        {
            Transform findItem = Layout.GetChildList().Find(item => item.GetComponent<MahjongItem>().Value.Equals(value));
            if (findItem!=null)
            {
                SetMahjongAsNewGetIn(findItem.GetComponent<MahjongItem>());
            }
            else
            {
                YxDebug.LogError("There is not exist such value in handcards,value is :"+value);
            }
            
        }

        public MahjongItem GetCardByValue(int value)
        {
            if (_getInCard.GetIn!=null&& _getInCard.GetIn.Value.Equals(value))
            {
                return _getInCard.GetIn;
            }
            Transform findTrans =Layout.GetChildList().Find(item => item.GetComponent<MahjongItem>().Value.Equals(value));
            if (findTrans)
            {
                return findTrans.GetComponent<MahjongItem>();
            }
            return null;
        }

        public override void ResetPile()
        {
            base.ResetPile();
            if (NewCard!=null)
            {
                if (NewCard.GetIn != null)
                {
                    NewCard.GetIn = null;
                }
                while (NewCard.transform.childCount > 0)
                {
                    DestroyImmediate(NewCard.transform.GetChild(0).gameObject);
                }

            }

        }

    }
}
