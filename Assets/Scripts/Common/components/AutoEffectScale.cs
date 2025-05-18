using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Common.components
{

    public class AutoEffectScale : MonoBehaviour
    {

        private List<ScaleData> scaleDatas = null;


        public float designWidth = 1334;//开发时分辨率宽
        public float designHeight = 750;//开发时分辨率高

        void Start()
        {
            scaleDatas = new List<ScaleData>();
            foreach (ParticleSystem p in transform.GetComponentsInChildren<ParticleSystem>(true))
            {
                scaleDatas.Add(new ScaleData() { transform = p.transform, beginScale = p.transform.localScale });
            }

            AutoScale();
        }

        void AutoScale()
        {
            float designScale = designWidth / designHeight;
            float scaleRate = (float)Screen.width / (float)Screen.height;

            foreach (ScaleData scale in scaleDatas)
            {
                if (scale.transform != null)
                {
                    if (scaleRate < designScale)
                    {
                        float scaleFactor = scaleRate / designScale;
                        scale.transform.localScale = scale.beginScale * scaleFactor;
                    }
                    else
                    {
                        scale.transform.localScale = scale.beginScale;
                    }
                }
            }
        }

#if UNITY_EDITOR
        void Update()
        {
            AutoScale(); //Editor下修改屏幕的大小实时预览缩放效果
        }
#endif

        class ScaleData
        {
            public Transform transform;
            public Vector3 beginScale = Vector3.one;
        }
    }
}