using System;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.fillpit.Mgr
{
    public class AnimationMgr : MonoBehaviour {

        /// <summary>
        /// 开始下注动画,设置Active即开始播放
        /// </summary>
        public GameObject BeginBetAnim = null;

        /// <summary>
        /// 双倍场游戏动画,设置Active即开始播放
        /// </summary>
        public GameObject DoubleGameAnim = null;

        /// <summary>
        /// 烂底局游戏动画,设置Active即开始播放
        /// </summary>
        public GameObject LanDiGameAnim = null;

        /// <summary>
        /// 结算时烂底动画
        /// </summary>
        public GameObject ResultLanDiAnim = null;


        GameBeginAnim GetBeginAnim(GameObject gameOPbj)
        {
            if(gameOPbj == null) return null;

            var anim = gameOPbj.GetComponent<GameBeginAnim>() ?? gameOPbj.AddComponent<GameBeginAnim>();
            return anim;
        }

        /// <summary>
        /// 展示双倍场动画
        /// </summary>
        public void ShowDoubleGameAnim(Action finish)
        {
            GameBeginAnim doubleGameBegin = GetBeginAnim(DoubleGameAnim);
            doubleGameBegin.Finish = finish;

            DoubleGameAnim.SetActive(true);
        }

        /// <summary>
        /// 显示烂底动画
        /// </summary>
        public void ShowLanDiGameAnim()
        {
            GameBeginAnim lanDiGameBegin = GetBeginAnim(LanDiGameAnim);
            lanDiGameBegin.Finish = ShowBeginBetAnim;

            LanDiGameAnim.SetActive(true);
            Facade.Instance<MusicManager>().Play("landi");
        }

        private Action _beginAnim;

        private void Awake()
        {
            InitBeginAnim();
        }

        void InitBeginAnim()
        {
            if (BeginBetAnim == null)
            {
                _beginAnim = App.GetGameManager<FillpitGameManager>().PlayersGuoBet;
            }
            else
            {
                GameBeginAnim doubleGameBegin = GetBeginAnim(BeginBetAnim);
                if (!App.GetGameData<FillpitGameData>().IsLanDi)
                {
                    doubleGameBegin.Finish = App.GetGameManager<FillpitGameManager>().PlayersGuoBet;
                }
                _beginAnim = ShowGameStart;
            }
        }


        void ShowGameStart()
        {
            BeginBetAnim.SetActive(true);
        }

        /// <summary>
        /// 开始动画
        /// </summary>
        public void ShowBeginBetAnim()
        {
            _beginAnim();
        }


        public void PlayBeginAnim(ISFSObject data)
        {
            var gdata = App.GetGameData<FillpitGameData>();
            //如果是开房模式,记录房间开始过游戏
            if (gdata.IsRoomGame)
            {
                //开放模式有双倍场时,出现双倍场动画效果
                if (data.ContainsKey("dr") && data.GetBool("dr"))
                {
                    gdata.IsDoubleGame = true;
                    if (gdata.IsLanDi)
                        ShowDoubleGameAnim(ShowLanDiGameAnim);
                    else
                        ShowDoubleGameAnim(ShowBeginBetAnim);
                }
                else
                {
                    gdata.IsDoubleGame = false;
                    if (gdata.IsLanDi)
                        ShowLanDiGameAnim();
                    else
                        ShowBeginBetAnim();
                }

                gdata.IsPlayed = true;
                
            }
            else
            {
                ShowBeginBetAnim();
            }
        }
        /// <summary>
        /// 四同通杀动画特效
        /// </summary>
        public GameObject SfakEffect;

        /// <summary>
        /// 双王通杀动画特效
        /// </summary>
        public GameObject DkakEffect;

        /// <summary>
        /// 四通通杀
        /// </summary>
        public void ShowSfak()
        {
            SetObjActive(SfakEffect, false);
        }

        /// <summary>
        /// 双王通杀
        /// </summary>
        public void ShowDkak()
        {
            SetObjActive(DkakEffect, false);
        }

        public void SetResultLanDiAnim(bool active)
        {
            SetObjActive(ResultLanDiAnim, active);
        }

        void SetObjActive(GameObject obj,bool active)
        {
            if (obj != null)
                obj.SetActive(active);
        }


        public void Reset()
        {
            SetObjActive(SfakEffect,false);
            SetObjActive(DkakEffect, false);
            SetResultLanDiAnim(false);
        }
    }
}
