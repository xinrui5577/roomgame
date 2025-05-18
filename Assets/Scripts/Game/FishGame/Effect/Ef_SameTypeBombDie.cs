using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_SameTypeBombDie : MonoBehaviour {
        public GameObject Prefab_EffectLine;

        public float EffectWidthScale = 0.1F;
        public Vector3 ScaleToUnit;//缩放至单位大小
        public Vector3 PositionOffset;//
        //public Color EffectColor = Color.white;
        public int MaxInstanceEffectLine = 20;//最大效果数目
        private Fish mFish;
        private Transform mTs;
        // Use this for initialization
        void Awake () {
            mFish = GetComponent<Fish>();
            mFish.EvtFishKilled += Handle_FishKilled;
            mTs = transform;
        
        }

        public void Handle_FishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish sameTypeBomb, int reward)
        { 
            //找出死亡同类鱼的
            var sameKindFish = (sameTypeBomb as SameKindBomFish);
            if (sameKindFish == null) return;
            var affects = sameKindFish.Prefab_SameTypeBombAffect;
            var numTypeToBomb = affects.Length;
            var fishNormalDie = new List<Fish>();
            var fishTypeIndexMap = GameMain.Singleton.FishGenerator.FishTypeIndexMap;
            for (var i = 0; i < numTypeToBomb; ++i)
            {
                var type = affects[i].TypeIndex;
                if(!fishTypeIndexMap.ContainsKey(type))continue;
                var allFishDict = fishTypeIndexMap[type];
                if (allFishDict == null) continue;
                fishNormalDie.AddRange(allFishDict.Values);
            }
            var goSameTypeBombDie = new GameObject("goSameTypeBombDie");
            var goSameTypeBombDieTs = goSameTypeBombDie.transform;
            var efDestroyDelay = goSameTypeBombDie.AddComponent<Ef_DestroyDelay>();
            efDestroyDelay.delay = 2F;

            var currInstanceLineNum = 0;
            foreach (var f in fishNormalDie)
            {

                var tmpGO = Instantiate(Prefab_EffectLine);
                var tmpTS = tmpGO.transform;
                tmpTS.parent = goSameTypeBombDieTs;

                var dir = f.transform.position - mTs.position;
                var dirLen = dir.magnitude;

                Vector3 tmpScale;
                tmpScale.x = tmpTS.localScale.x * ScaleToUnit.x * EffectWidthScale;
                tmpScale.y = tmpTS.localScale.y * ScaleToUnit.y * dirLen;
                tmpScale.z = tmpTS.localScale.z * ScaleToUnit.z;

            
                tmpTS.rotation = Quaternion.LookRotation(Vector3.forward, dir);
                tmpTS.localScale = tmpScale;

                Vector3 tmpPos = mTs.position + (tmpTS.rotation * Vector3.Scale(PositionOffset, tmpScale));
                tmpPos.z = Defines.GlobleDepth_BombParticle;
                tmpTS.position = tmpPos;

                ++currInstanceLineNum;
                if(currInstanceLineNum>MaxInstanceEffectLine)
                    break;
            }
        
        
        }
	

    }
}
