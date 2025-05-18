using Assets.Scripts.Game.paijiu.ImgPress.Main;
using UnityEngine;
using Sfs2X.Entities.Data;
using YxFramwork.Common;

#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{
    public class TalbeData : MonoBehaviour
    {
        /// <summary>
        /// 牌信息显示
        /// </summary>
        [SerializeField]
        private UILabel _cardLabel;

        /// <summary>
        /// 桌面各门的显示
        /// </summary>
        [SerializeField]
        private UISprite[] _menArr;

        /// <summary>
        /// 各门贴图名字显示
        /// </summary>
        [SerializeField]
        private string[] _menSpriteNames;

        /// <summary>
        /// 设置桌面信息
        /// </summary>
        /// <param name="data"></param>
        public void SetTalebData(ISFSObject data)
        {
            if (data.ContainsKey("cardcnt"))
            {
                SetCardCountLabel(data.GetInt("cardcnt"));
            }

            if (data.ContainsKey("banker"))
            {
                SetMen(data.GetInt("banker"));
            }

            //if(data.ContainsKey("leaveCnt"))
            //{
            //    SetCardCountLabel(data.GetInt("cardcnt"));
            //}
        }

        private int _cardCount = 32;

        /// <summary>
        /// 减去发过的牌的张数
        /// </summary>
        /// <param name="count">已经发牌的张数</param>
        public void SubCardCount(int count)
        {
            _cardCount -= count;
            SetCardCountLabel(_cardCount);
        }

        /// <summary>
        /// 显示牌的剩余个数
        /// </summary>
        /// <param name="count">剩余牌的张数</param>
        private void SetCardCountLabel(int count)
        {
            _cardLabel.text = string.Format("本局剩余 : {0}张", count);
        }

        /// <summary>
        /// 设置桌面上门贴图
        /// </summary>
        /// <param name="first"></param>
        private void SetMen(int first)
        {
            for (int i = 0; i < _menArr.Length; i++)
            {
                var spr = _menArr[(i + App.GetGameData<PaiJiuGameData>().GetLocalSeat(first)) % _menArr.Length];
                spr.gameObject.SetActive(true);
                spr.spriteName = _menSpriteNames[i];
                spr.MakePixelPerfect();
            }
        }

        public void Reset()
        {
            _cardCount = 32;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _menArr.Length; i++)
            {
                UISprite spr = _menArr[i];
                spr.gameObject.SetActive(false);
            }
        }
    }
}