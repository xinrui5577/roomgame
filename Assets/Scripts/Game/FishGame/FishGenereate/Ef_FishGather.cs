using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.FishGame.FishGenereate
{
    public class Ef_FishGather : MonoBehaviour {
        public tk2dSprite PrefabSpr;
        public string[] SpriteName;
        public Vector3[] LocalPoss;
        public GameObject PrefabGO_BGGatherView;//收集显示的背景
        public Vector3 LocalPlayerPos_BGGatherView = new Vector3(0.3864782F,0.07634497F,-0.09000015F);//收集鱼背景在Player中的本地坐标
        public float Duration_FinishPad = 5F;//收集完成奖励牌显示时间
        public GameObject PrefabGO_EfBackground;//收集完成奖励牌prefab
    

        public GameObject PrefabGO_EfBigViewer;//得奖显示横幅
    
        public AudioClip Snd_GetPrize;


        private Transform[] TsPlayerGatherBGs;//收集鱼背景,索引是玩家id
        private FishGather mFg;
        void Awake()
        {
            mFg = GetComponent<FishGather>();
            if (mFg == null)
            {
                YxDebug.LogError("Ef_FishGather 需要和 FishGather放在同一个脚本.");
                return;
            }
            mFg.EvtPlayerGatherFishInited += Evt_PlayerGatherFishInit;
            mFg.EvtPlayerGatherFishActive += Evt_PlayerGatherFishActive;
            mFg.EvtPlayerGatheredAllFish += Evt_PlayerGatheredAllFish;
            mFg.EvtPayBonus += Evt_PayBonus;
            TsPlayerGatherBGs = new Transform[Defines.MaxNumPlayer];
        
        
            //for (int i = 0; i != GameMain.Singleton.Players.Length; ++i)
            var players = GameMain.Singleton.PlayersBatterys;
            foreach (var p in players)
            {
                var bgGOGather = Instantiate(PrefabGO_BGGatherView) as GameObject;

                var tsCreditBoard = p.transform.FindChild("UI_CreditBoard");
                if (tsCreditBoard == null)
                {
                    YxDebug.LogError("GameObject.Player下必须有名为UI_CreditBoard的Go");
                    continue;
                }
                if (bgGOGather == null) continue;
                var bgTSGather = bgGOGather.transform;
                bgTSGather.parent = tsCreditBoard.transform; 
                bgTSGather.localPosition = LocalPlayerPos_BGGatherView;
                bgTSGather.localRotation = Quaternion.identity;
                TsPlayerGatherBGs[p.Idx] = bgGOGather.transform;
            }
        }

        void Evt_PlayerGatherFishInit(Player player, Fish fish, int gatherIdx)
        {
            tk2dSprite spr = Instantiate(PrefabSpr) as tk2dSprite;
            spr.spriteId = spr.GetSpriteIdByName(SpriteName[gatherIdx]);
            Transform tsSpr = spr.transform;
            tsSpr.parent = TsPlayerGatherBGs[player.Idx];
            tsSpr.localPosition = LocalPoss[gatherIdx];
            tsSpr.localRotation = Quaternion.identity;
            //StartCoroutine(_Coro_FlashFishSpr(spr));
        }

        void Evt_PlayerGatherFishActive(Player player, Fish fish, int gatherIdx)
        {
            tk2dSprite spr = Instantiate(PrefabSpr) as tk2dSprite;
            spr.spriteId = spr.GetSpriteIdByName(SpriteName[gatherIdx]);
            Transform tsSpr = spr.transform;
            tsSpr.parent = TsPlayerGatherBGs[player.Idx];
            tsSpr.localPosition = LocalPoss[gatherIdx];
            tsSpr.localRotation = Quaternion.identity;

            //收集齐所有鱼
            if (TsPlayerGatherBGs[player.Idx].childCount >= mFg.CountFishNeedGather)
            {
                StartCoroutine(_Coro_EffectFlashAllSpr(TsPlayerGatherBGs[player.Idx],mFg.TimeDelayBonus));
            }
            else
                StartCoroutine(_Coro_FlashFishSpr(spr,5F));

        }

        void Evt_PlayerGatheredAllFish(Player player, Fish fish, int gatherIdx)
        {
        
        
        }

        void Evt_PayBonus(Player player, int bouns)
        {

            StartCoroutine(_Coro_EffectProcessing(bouns, player));
            //显示大奖横幅
            for (int i = 0; i != GameMain.Singleton.ScreenNumUsing; ++i)
                StartCoroutine(_Coro_EffectViewBigPad(player.Idx, bouns, new Vector2(GameMain.Singleton.WorldDimension.x + (0.5F + i) * Defines.WorldDimensionUnit.width, 0F)));
        }

        /// <summary>
        /// 闪烁所有精灵
        /// </summary>
        /// <param name="parentTs"></param>
        /// <returns></returns>
        IEnumerator _Coro_EffectFlashAllSpr(Transform parentTs,float elapse)
        {
            //先把所有精灵移动出来(防止在闪烁期间加入新的,导致以后显示不正常)
            GameObject goFlashFishGahterSpr = new GameObject("tempFlashfishGatherSpr");
            List<Transform> sprAll = new List<Transform>();//由于直接设置parent会导致遍历错误

            foreach (Transform ts in parentTs)
            {
                sprAll.Add(ts);
            }
        
            foreach (Transform ts in sprAll)
            {
                ts.parent = goFlashFishGahterSpr.transform;
                StartCoroutine(_Coro_FlashFishSpr(ts.GetComponent<tk2dSprite>(), elapse));
            }


            yield return new WaitForSeconds(elapse);
            Destroy(goFlashFishGahterSpr);
        }

        /// <summary>
        /// 闪烁一个精灵
        /// </summary>
        /// <param name="spr"></param>
        /// <param name="elapse"></param>
        /// <returns></returns>
        IEnumerator _Coro_FlashFishSpr(tk2dSprite spr,float elapse)
        { 

            //spr.color 
            float flashElapse = elapse;
            float flashTotalElapse = flashElapse;
            float flashSpd = 6F;
            Color colorFix = spr.color;
            while (flashElapse > 0F)
            {
                if (spr == null)
                    yield break;

                colorFix.a = Mathf.Abs(Mathf.Cos(flashElapse / flashTotalElapse * Mathf.PI * flashSpd));
                spr.color = colorFix;
                flashElapse -= Time.deltaTime;
                yield return 0;
            }
            colorFix.a = 1F;
            spr.color = colorFix;
        }

        //玩家头上奖励牌
        IEnumerator _Coro_EffectProcessing(int num, Player killer )
        {
            if (PrefabGO_EfBackground == null)
                yield break;
            //声音
            if (killer.Idx == App.GameData.SelfSeat % 6)
            {
                Facade.Instance<MusicManager>().Play(Snd_GetPrize);
            }

            //弹出奖励框
            GameObject goEffect = Instantiate(PrefabGO_EfBackground) as GameObject;
            tk2dSprite aniSpr = goEffect.GetComponentInChildren<tk2dSprite>();
            Transform tsEffect = goEffect.transform;
            tk2dTextMesh text = goEffect.GetComponentInChildren<tk2dTextMesh>();

            text.text = num.ToString();
            text.Commit();

            //初始方位设置
            Vector3 originLocalPos = new Vector3(0.385F, -0.5F, -0.19F);
            Vector3 targetLocalPos = new Vector3(0.385F, 0.5F, -0.19F);

            tsEffect.parent = killer.transform;
            tsEffect.localPosition = originLocalPos;
            tsEffect.localRotation = Quaternion.identity;


            //摇动数字
            iTween.RotateAdd(text.gameObject, iTween.Hash("z", 8F, "time", 0.27F, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutSine));


            //向上移动
            float elapse = 0F;
            float useTime = 0.2F;
            while (elapse < useTime)
            {
                tsEffect.localPosition = originLocalPos + (targetLocalPos - originLocalPos) * (elapse / useTime);
                elapse += Time.deltaTime;
                yield return 0;
            }
            tsEffect.localPosition = targetLocalPos;
            yield return new WaitForSeconds(Duration_FinishPad);

            //渐隐
            elapse = 0F;
            useTime = 0.2F;
            while (elapse < useTime)
            {
                aniSpr.color = new Color(1F, 1F, 1F, 1F - elapse / useTime);
                text.color = new Color(1F, 1F, 1F, 1F - elapse / useTime);
                text.Commit();
                elapse += Time.deltaTime;
                yield return 0;
            }


            Destroy(goEffect.gameObject); 
        }

        /// <summary>
        /// 大横幅
        /// </summary>
        /// <param name="playeridx"></param>
        /// <param name="score"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        IEnumerator _Coro_EffectViewBigPad(int playeridx,int score,Vector2 center)
        {
            GameObject goEffect = Instantiate(PrefabGO_EfBigViewer) as GameObject;
            goEffect.transform.position = new Vector3(center.x, center.y, Defines.GlobleDepth_PrepareInBG);
            goEffect.transform.localScale = new Vector3(0F, 1F, 1F);

            tk2dTextMesh textPlayerID = goEffect.transform.FindChild("TextPlayerId").GetComponent<tk2dTextMesh>();
            tk2dTextMesh textScore = goEffect.transform.FindChild("TextScore").GetComponent<tk2dTextMesh>();
            textPlayerID.text = (playeridx + 1).ToString();
            textPlayerID.Commit();
            textScore.text = score.ToString();
            textScore.Commit();

            iTween.ScaleTo(goEffect, iTween.Hash("scale", Vector3.one, "easeType", iTween.EaseType.spring, "time", 0.2F));
            yield return new WaitForSeconds(5F);
            iTween.ScaleTo(goEffect, iTween.Hash("scale",new Vector3(0F,1F,1F), "easeType", iTween.EaseType.easeInQuad, "loopType", iTween.LoopType.none, "time", 0.2F));
            yield return new WaitForSeconds(1.1f);
            Destroy(goEffect);
        }

    
    }
}
