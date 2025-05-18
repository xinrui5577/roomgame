using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = System.Random;

namespace Assets.Scripts.Game.Ttzkf
{
    public class TtzPlayerSelf : TtzPlayer
    {
        public GameObject OpenCard;
        public GameObject BoxClider;

        private List<int> _pokersPos;
        protected bool IsOpenCard;
        private bool _isComplete;


        /// <summary>
        /// 设置结算的时候牌的显示（如果kind=0五张牌的搓牌,kind=1正常的看牌,kind=2 4张牌的搓牌）
        /// </summary>
        /// <param name="kind"></param>
        public override void OnsetResult(int kind)
        {
            var gmanager = App.GetGameManager<TtzGameManager>();
            gmanager.Tip.T("正在等待玩家翻牌");
            switch (kind)
            {
                case 0:
                    if (!IsLiang)
                    {
                        foreach (var c in Card)
                        {
                            c.MoveOnCuoPai();
                        }

                        StartCoroutine("OnCuoPai");
                    }
                    break;
                case 1:
                    OnFanPaiClick();
                    break;
                default:
                    NewFanPai();
                    if (gmanager.BlackBg.activeSelf)
                    {
                        gmanager.BlackBg.SetActive(false);
                    }
                    NiuNumShow();
                    break;
            }
            ShowLiangPai(!IsShowCards);
        }
        /// <summary>
        /// 提示和亮牌按钮的显示
        /// </summary>
        /// <param name="isShow"></param>
        private void ShowLiangPai(bool isShow)
        {
            if (isShow) IsLiang = true;

            if (OpenCard != null)
            {
                OpenCard.SetActive(isShow);
            }
        }
        

        protected void OnFanPaiClick()
        {
            if (!IsLiang)
            {
                var count = Card.Count;
                for (FpIndex = 0; FpIndex < count; FpIndex++)
                {
                    Card[FpIndex].CardValue = Cards[FpIndex];
                    Card[FpIndex].RotateOnFanPai();
                }
            }
        }

        protected IEnumerator OnCuoPai()
        {
            var gmanager = App.GetGameManager<TtzGameManager>();
            var temp = Card[0].WaitTime * 2;
            yield return new WaitForSeconds(temp);
            gmanager.TurnCard.Init();
            //进入搓牌界面
        }

        private void NiuNumShow()
        {
            var sfsObject = UserData.GetSFSObject(InteractParameter.NiuData);
            if (sfsObject == null)
            {
                StatuSprite.gameObject.SetActive(false);
            }
            else
            {
                SetNiuName(sfsObject);
                var str = GetAudio(sfsObject);
                var rand = new Random();
                int sex = rand.Next(0, 2);

                string source = "";

                if (sex == 0)
                {
                    source = "woman";
                }
                else if (sex == 1)
                {
                    source = "man";
                }
                Facade.Instance<MusicManager>().Play(str, source);
            }
        }
        /// <summary>
        /// 点击亮牌按钮
        /// </summary>
        public void OnOpenCardBtn()
        {
            NiuNumShow();
            ShowLiangPai(false);
            App.GetRServer<TtzGameServer>().SendLiang();
        }

        public override void ShowResult(ISFSObject data)
        {
            base.ShowResult(data);
            ShowLiangPai(false);
            IsOpenCard = false;
            _isComplete = false;
            Facade.Instance<MusicManager>().Play(data.GetInt("gold") >= 0 ? "win" : "lost");
        }

        /// <summary>
        /// 游戏重置
        /// </summary>
        public override void ReSetGame()
        {
            base.ReSetGame();
            UpCards.Clear();
        }

        protected List<GameObject> UpCards = new List<GameObject>();

    }
}
