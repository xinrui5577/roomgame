using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoFileIO;
using Assets.Scripts.Game.FishGame.FishGenereate;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.DragonSlayers
{
    /// <summary>
    /// 功能:屠龙刀
    /// </summary>
    /// <remarks>
    /// 注意:
    /// 1.可被屠龙刀杀死鱼列表内的所有鱼出现几率必须一致,(因为算的是平均几率)->被屠龙刀杀的鱼出现的几率会被强制设成一致
    /// 2.生成屠龙刀的鱼和所有可被屠龙刀杀死的鱼不能同时出现(否则玩家可以通过观察出现最大鱼时再杀死生成屠龙刀的鱼,从而获得比期望高的收益)
    /// 3.被屠鱼不能有双倍 (因为不考虑这些因素)
    /// </remarks>
    public class DragonSlayer : MonoBehaviour {
        public Fish Prefab_FishToGenerateDragonSlayer;//生成DragonSlayer的鱼

        public Fish[] Prefab_SlayFish;//可被杀死的鱼,所有被杀的鱼出现几率必须一致
        public int GenerateRate_SlayFish = 1000;//可被杀死鱼的出现几率
        public Fish Prefab_DragonSlayer;//屠龙刀prefab
        public PersistentData<int, int>[] DragonSlayerScores;//屠龙刀的分数,为0的话则没有刀
        // Use this for initialization
        public GameObject Ef_DragonKilled;//屠龙刀杀龙时生成效果
        public GameObject PrefabGO_EfBigViewer;//屠龙刀出现提示

        private int mSlayFishAveragyOdd;//被杀鱼的平均odd,                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
        private Dictionary<int, Fish> mSlayFishCache;//可被杀死鱼cache,与Prefab_SlayFish关联
        void Awake () {
            //HitProcessor.FuncGetFishAddtiveForDieRatio += Func_GetFishOddAddtiveForDieRatio;
            HitProcessor.AddFunc_Odd(Func_GetFishOddAddtiveForDieRatio,null);
            /*GameMain.EvtFishKilled += Event_FishKilled;
        GameMain.EvtMainProcessFinishPrelude += Event_MainProcess_FinishPrelude;
        GameMain.EvtMainProcessFirstEnterScene += Event_MainProcess_FirstEnterScene;
        GameMain.EvtBgClearAllDataBefore += Event_BackGroundClearAllData_Before;*///adq

            DragonSlayerScores = new PersistentData<int, int>[Defines.MaxNumPlayer];
            for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                DragonSlayerScores[i] = new PersistentData<int, int>("DragonSlayerScores" + i.ToString());

            foreach (Fish f in Prefab_SlayFish)
            {
                RandomOddsNum rndOdd = f.GetComponent<RandomOddsNum>();
                if (rndOdd != null)
                {
                    mSlayFishAveragyOdd += (rndOdd.MinOdds + rndOdd.MaxOdds) / 2;
                }
                else
                    mSlayFishAveragyOdd += f.Odds;
            }

            mSlayFishCache = new Dictionary<int, Fish>();
            foreach (Fish f in Prefab_SlayFish)
            {
                mSlayFishCache.Add(f.TypeIndex, f);
            }

            mSlayFishAveragyOdd /= Prefab_SlayFish.Length;
            //GenerateDragonSlayer(0);

         
        }

        HitProcessor.OperatorOddFix Func_GetFishOddAddtiveForDieRatio(Player killer, Bullet b, Fish f, Fish fCauser)
        {
            //不是生成屠龙刀的鱼
            if (Prefab_FishToGenerateDragonSlayer.TypeIndex != f.TypeIndex)
                return null;

            //已拥有屠龙刀
            if (DragonSlayerScores[killer.Idx].Val != 0)
                return null;

            return new HitProcessor.OperatorOddFix(HitProcessor.Operator.Add,mSlayFishAveragyOdd);

        }

        /// <summary>
        /// 响应事件:鱼死亡()
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        void Event_FishKilled(Player killer, Bullet b, Fish f)
        {
            if (Prefab_FishToGenerateDragonSlayer.TypeIndex == f.TypeIndex)
            {
                //出现刀
                //是否已存在屠龙刀
                if (DragonSlayerScores[killer.Idx].Val == 0)//只能同时存在一把屠龙刀
                {
                    DragonSlayerScores[killer.Idx].Val = b.Score;//记录分
                    GenerateDragonSlayer(killer.Idx, b.Score);

                    //效果显示
                    //显示大奖横幅
                    for (int i = 0; i != GameMain.Singleton.ScreenNumUsing; ++i)
                        StartCoroutine(_Coro_EffectViewBigPad(killer.Idx, new Vector2(GameMain.Singleton.WorldDimension.x + (0.5F + i) * Defines.WorldDimensionUnit.width, 0F)));

                }

            }
        }

        /// <summary>
        /// 响应事件:鱼阵结束,(需要在鱼阵结束时重新出屠龙刀)
        /// </summary>
        void Event_MainProcess_FinishPrelude()
        {
            for (int i = 0; i != Defines.MaxNumPlayer; ++i)
            {
                if (DragonSlayerScores[i].Val != 0)
                    GenerateDragonSlayer(i, DragonSlayerScores[i].Val);
            }
        }

        /// <summary>
        /// 响应事件:第一次进入场景
        /// </summary>
        void Event_MainProcess_FirstEnterScene()
        {
//        for (int i = 0; i != Defines.MaxNumPlayer; ++i)
//        {
//            if (DragonSlayerScores[i].Val != 0)
//                GenerateDragonSlayer(i, DragonSlayerScores[i].Val);
//        }
//
//        FishGenerator fg = GameMain.Singleton.FishGenerator;
//
//        //设置出鱼的几率
//        FishGenerator.FishGenerateData[] fgds = fg.FishGenerateUniqueDatas;
//        //foreach (FishGenerator.FishGenerateData fgd in fgds)
//        foreach (Fish slayFish in Prefab_SlayFish)
//        {
//            for (int i = 0; i != fgds.Length; ++i)
//            {
//                if (fgds[i].Prefab_Fish.TypeIndex == slayFish.TypeIndex)
//                {
//                    fgds[i].Weight = GenerateRate_SlayFish;
//                    
//                    break;
//                }
//            }
//        }
//
//        fg.RefreshAllGenerateWeight();
        }

        /// <summary>
        /// 后台双打码清0
        /// </summary>
        void Event_BackGroundClearAllData_Before()
        {
            for (int i = 0; i != Defines.MaxNumPlayer; ++i)
                DragonSlayerScores[i].Val = 0;
        }

        void GenerateDragonSlayer(int playerIdx,int slayerScore)
        {
            if (slayerScore == 0)
            {
                Debug.LogWarning("slayerScore分数不能为0");
                return;
            } 

            FishGenerator fishGenrator = GameMain.Singleton.FishGenerator;
            Player p = GameMain.Singleton.PlayersBatterys[playerIdx];
            Transform tsPlayer = p.transform;
            Fish dragonSlayer = Instantiate(Prefab_DragonSlayer) as Fish;
            Transform tsDragonSlayer = dragonSlayer.transform;
        
            tsDragonSlayer.parent = fishGenrator.transform;//以fishGenerator作为父节点,过场时可以删除
            tsDragonSlayer.position = tsPlayer.position;
            tsDragonSlayer.rotation = tsPlayer.rotation*Quaternion.Euler(0F,0F,90f);

            Vector3 tmpPos = tsDragonSlayer.localPosition;
            tmpPos.z = -3.1F;
            tsDragonSlayer.localPosition = tmpPos;

            dragonSlayer.swimmer.Go();

            FishEx_DragonSlayer fishExDragonSlayer = dragonSlayer.GetComponentInChildren<FishEx_DragonSlayer>();
            if (fishExDragonSlayer != null)
            {
                fishExDragonSlayer.Owner = p;
                fishExDragonSlayer.Creator = this;

                //效果:设置显示玩家
                Transform tsTextPlayerId = fishExDragonSlayer.transform.FindChild("TextPlayerId");
                if (tsTextPlayerId != null)
                {

                    tk2dTextMesh textPlayerID = tsTextPlayerId.GetComponent<tk2dTextMesh>();
                    textPlayerID.text = (p.Idx + 1).ToString();
                    textPlayerID.Commit();
                }
                else
                {
                    YxDebug.LogError("需要在fishDragonSlayer对象下放入tk2dtextmesh:TextPlayerId");
                }


            }
            else
            {
                YxDebug.LogError("屠龙刀在子gameobject必须包含 fishExDragonSlayer.");
            } 
        }

        /// <summary>
        /// 由FishEx_DragonSlayer回调(减少事件订阅消耗)
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="owner"></param>
        public void On_FishExDragonSlayerTouchFish(FishEx_DragonSlayer ds, Fish fishTouched,Player owner)
        {
            //接触到的fish已经死亡
            if (fishTouched == null || !fishTouched.Attackable)
                return;

            if (!mSlayFishCache.ContainsKey(fishTouched.TypeIndex))
                return;
            //*为了调用函数创建一个临时的bullet,非常呕心,但是没办法,因为很多地方使用这个变量.以后需要注意事件,公用函数的写法
            GameObject tmpBulletGo = new GameObject();
            Bullet b = tmpBulletGo.AddComponent<Bullet>();
            b.FishOddsMulti = 1;
            b.Score = DragonSlayerScores[owner.Idx].Val;

            fishTouched.Kill(owner, b.Score, fishTouched.Odds * b.FishOddsMulti, b.FishOddsMulti, 0F,0);

            owner.GainScore(fishTouched.Odds * DragonSlayerScores[owner.Idx].Val
                            , fishTouched.Odds
                            , DragonSlayerScores[owner.Idx].Val
                );
       
            //删除之前创建的
            Destroy(tmpBulletGo);

            //dragonSlayerScore归零
            DragonSlayerScores[owner.Idx].Val = 0;

            //删除dragonSlayer
            ds.Clear();

            //出现效果
            GameObject efGo = Instantiate(Ef_DragonKilled) as GameObject;
            Vector3 worldPos = fishTouched.transform.position;
            worldPos.z = Defines.GlobleDepth_BombParticle;
            efGo.transform.position = worldPos;
        }



        IEnumerator _Coro_EffectViewBigPad(int playeridx, Vector2 center)
        {
            GameObject goEffect = Instantiate(PrefabGO_EfBigViewer) as GameObject;
            goEffect.transform.position = new Vector3(center.x, center.y, Defines.GlobleDepth_PrepareInBG);
            goEffect.transform.localScale = new Vector3(0F, 1F, 1F);

            tk2dTextMesh textPlayerID = goEffect.transform.FindChild("TextPlayerId").GetComponent<tk2dTextMesh>();
            textPlayerID.text = (playeridx + 1).ToString();
            textPlayerID.Commit();

            iTween.ScaleTo(goEffect, iTween.Hash("scale", Vector3.one, "easeType", iTween.EaseType.spring, "time", 0.2F));
            yield return new WaitForSeconds(5F);
            iTween.ScaleTo(goEffect, iTween.Hash("scale", new Vector3(0F, 1F, 1F), "easeType", iTween.EaseType.easeInQuad, "loopType", iTween.LoopType.none, "time", 0.2F));
            yield return new WaitForSeconds(1.1f);
            Destroy(goEffect);
        }
    }
}
