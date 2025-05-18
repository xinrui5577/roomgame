using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_WebBubble : MonoBehaviour {
        public tk2dSprite Prefab_Bubble;
 
        public float ScaleTarget = 1F;//目标缩放  
        public float InttervalBubble = 0.075F;//泡泡发射间隔
        public float TimeOneBubble = 0.15F;//单个泡放到最大时间
        public float AlphaStart = 0.3F;
        public int NumGenerate = 3;
        // Use this for initialization
        void Start()
        {
            StartCoroutine(_Coro_GenerateBubble());
            //yield return new WaitForSeconds(Elapse); 
            //Destroy(gameObject);
        }

        IEnumerator _Coro_GenerateBubble()
        {
            int numGenerated = 0;
            Vector3 localPos = new Vector3(0F, 0F, 0.01F);
            while (numGenerated < NumGenerate)
            {
                //tk2dSprite sprBubble = Instantiate(Prefab_Bubble) as tk2dSprite;
                tk2dSprite sprBubble = Pool_GameObj.GetObj(Prefab_Bubble.gameObject).GetComponent<tk2dSprite>();
                sprBubble.gameObject.SetActive(true);

                sprBubble.transform.parent = transform;
                sprBubble.transform.localPosition = localPos;
                localPos.z += 0.01F;
                StartCoroutine(_Coro_BubbleScaleUp(sprBubble));
                //sprBubble.
                ++numGenerated;
                yield return new WaitForSeconds(InttervalBubble);
            }
        }
        IEnumerator _Coro_BubbleScaleUp(tk2dSprite spr)
        {
            //设置颜色
            Color c = spr.color;
            c.a = AlphaStart;
            spr.color = c;

            float elapse = 0F;
            //放大
            Transform tsSpr = spr.transform;
            float scaleUpTime = TimeOneBubble * 0.7F;
            while (elapse < scaleUpTime)
            {
                tsSpr.localScale = (ScaleTarget * (0.5F + elapse / scaleUpTime * 0.5F)) * Vector3.one;
                elapse += Time.deltaTime;
                yield return 0F;
            }
            tsSpr.localScale = Vector3.one * ScaleTarget;
            //渐隐
            elapse = 0F;
            float fadeoutTime = TimeOneBubble * 0.3F;

            while (elapse < fadeoutTime)
            {
                c.a = (1F - elapse / fadeoutTime) * AlphaStart; 
                spr.color = c;
                elapse += Time.deltaTime;
                yield return 0F;
            }


            //删除
            spr.gameObject.SetActive(false);
            Pool_GameObj.RecycleGO(Prefab_Bubble.gameObject, spr.gameObject);
            //Destroy(spr.gameObject);
        }

    
    }
}
