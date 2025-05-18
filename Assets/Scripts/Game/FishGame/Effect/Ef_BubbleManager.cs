using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_BubbleManager : MonoBehaviour { 
        public ParticleSystem Par_Bubble;
      
        private float mElapse = 0F;
        private float mEmitInterval;
        void Awake()
        {
            mEmitInterval = Par_Bubble.duration + 3F;
        }

        private void Start()
        {
            var tsPos = transform.position;
            tsPos.z = Defines.GlobleDepth_SceneBubblePar;
            transform.position = tsPos;
        }

        void Update()
        {
            if (mElapse < mEmitInterval)
            {
                mElapse += Time.deltaTime;
            }
            else
            {
                mElapse = 0F;

                Rect worldDim = GameMain.Singleton.WorldDimension;
                Transform tsPar = Par_Bubble.transform;
                tsPar.localPosition = new Vector3(Random.Range(worldDim.xMin,worldDim.xMax)
                                                  ,Random.Range(worldDim.yMin,worldDim.yMax)
                                                  ,0);
                Vector3 toward = Vector3.zero - tsPar.position+ Random.onUnitSphere * 80F;
                toward.z = 0F;
                tsPar.forward = toward;
                Par_Bubble.Play();
            }
        }

        //void OnGUI()
        //{
        //    if (GUILayout.Button("stop play"))
        //    {
        //        Par_Bubble.Stop();
        //    }
        //}
    }
}
