using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_KillBlueShark : MonoBehaviour {
        public GameObject GO_EfBackground;
        public GameObject GO_EfSameBackground;
        public float Duration = 2F;
        public float LocalHeight = 130F;  
        public int MinScroe = 30;

        private bool[] IsFishBombViewing; 

        private static float mShareOffsetZ = 0F;//z偏移,用于解决重叠问题
    
        //public float SameTypeBombCoolDown = 0.5F;//在0.5秒之内再次出现同类炸弹死亡不会显示分数框
        //private float mLastTimePreSameTypeBombViewPad;//上次显示同类炸弹奖励框的时间 
        // Use this for initialization
        void Start () {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFishKilled += Handle_FishKilled;
//            gdata.EvtFishBombKilled += Handle_FishBomb;
            gdata.EvtSameTypeBombKiled += Handle_SameTypeBomb;
            IsFishBombViewing = new bool[Defines.MaxNumPlayer];
        }
//
//        /// <summary>
//        /// 炸弹
//        /// </summary>
//        /// <param name="k"></param>
//        /// <param name="totalGet"></param>
//        void Handle_FishBomb(Player k, int totalGet)
//        {
//            StartCoroutine(_Coro_EffectProcessing(totalGet, k, 1.5F, true));
//        }

        /// <summary>
        /// 同类鱼
        /// </summary>
        /// <param name="k"></param>
        /// <param name="totalGet"></param>
        void Handle_SameTypeBomb(Player k, int totalGet)
        {
            StartCoroutine(_Coro_EffectProcessing(totalGet, k, 1.5F, false, GO_EfSameBackground));
        }

        /// <summary>
        /// 指定分数盘的鱼死亡
        /// </summary>
        /// <param name="killer">击杀者</param>
        /// <param name="bulletScore">子弹分数</param>
        /// <param name="fishOddBonus">倍数</param>
        /// <param name="fish">鱼</param>
        /// <param name="bulletOddsMulti"></param>
        /// <param name="reward">获得金币</param>
        void Handle_FishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish fish,int reward)
        {
            if (fish.Odds < MinScroe) return;
            var temp = bulletScore*fishOddBonus*bulletOddsMulti;
            var getReward = reward > 0 ? reward : temp;
            if (reward > 0 && reward != temp)
            {
                YxDebug.LogWrite("Ef_KillBlueShark    显示{0}【计算{1}】", null, reward, temp);
            }
            if (getReward > 100000)
            {
                Debug.LogError("getReward:" + getReward + "    bulletOddsMulti: " + bulletOddsMulti  + "    bulletScore:" + bulletScore  + "      fish:" + fish.name);
            }
            StartCoroutine(_Coro_EffectProcessing(getReward, killer, fish.TimeDieAnimation, false, GO_EfBackground));//bulletScore * fishOddBonus * bulletOddsMulti
        }
 
        IEnumerator _Coro_EffectProcessing(int num,Player killer,float delay,bool isAreaFishBomb,GameObject backG)
        { 
            if (isAreaFishBomb)
                IsFishBombViewing[killer.Idx] = true;
            else
                if (IsFishBombViewing[killer.Idx])
                    yield break;

            //蓝鲨死亡动作之后再播放
            yield return new WaitForSeconds(delay);
            //声音
            if (killer.Idx == App.GameData.SelfSeat % 6)
            {
                Facade.Instance<MusicManager>().Play("getprice");
            }

            //弹出奖励框
            var goEffect = Instantiate(backG);
            var aniSpr = goEffect.GetComponentInChildren<tk2dSprite>();
            var tsEffect = goEffect.transform;
            var text = goEffect.GetComponentInChildren<tk2dTextMesh>();
            text.text = YxUtiles.GetShowNumberToString(num);
            if(num> 100000) Debug.LogError(text.text);
            text.Commit();

            //初始方位设置
            var originLocalPos = new Vector3(0, -192F, 0F);
            var targetLocalPos = new Vector3(0, LocalHeight, 0F);
        
            tsEffect.parent = killer.KillBlueSharkPos;
            tsEffect.localPosition = originLocalPos;
            tsEffect.localRotation = Quaternion.identity;
            tsEffect.localScale = Vector3.one;
            //摇动数字
            iTween.RotateAdd(text.gameObject, iTween.Hash("z", 8F, "time", 0.27F, "looptype", iTween.LoopType.pingPong, "easetype", iTween.EaseType.easeInOutSine));


            //向上移动
            mShareOffsetZ -= 0.5F;
            if (mShareOffsetZ < -5F)
                mShareOffsetZ = 0F;

            float elapse = 0F;
            float useTime = 0.2F;
            Vector3 posTmp;
            while (elapse < useTime)
            {
                tsEffect.localPosition = originLocalPos + (targetLocalPos - originLocalPos) * (elapse / useTime);

                posTmp = tsEffect.position;
                posTmp.z = Defines.GlobleDepth_LiziKa + mShareOffsetZ;
                tsEffect.position = posTmp;

                elapse += Time.deltaTime;
                yield return 0;
            }
            tsEffect.localPosition = targetLocalPos;

            posTmp = tsEffect.position;
            posTmp.z = Defines.GlobleDepth_LiziKa + mShareOffsetZ;
            tsEffect.position = posTmp;

            yield return new WaitForSeconds(Duration);

            //渐隐
            elapse = 0F;
            useTime = 0.2F;
            while (elapse < useTime)
            {
                aniSpr.color = new Color(1F, 1F, 1F, 1F- elapse / useTime);
                text.color = new Color(1F, 1F, 1F,1F- elapse / useTime);
                text.Commit();
                elapse += Time.deltaTime;
                yield return 0;
            } 
            Destroy(goEffect.gameObject);
            if (isAreaFishBomb)
                IsFishBombViewing[killer.Idx] = false;
        } 
    }
}
