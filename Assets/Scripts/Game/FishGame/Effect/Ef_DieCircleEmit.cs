using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_DieCircleEmit : MonoBehaviour {
        public Transform Prefab_ToEmitObj;//∑¢…‰∂‘œÛ
        public int NumGenerate;
        private Transform[] mTsObjs;
        public float MoveSpeed = 1F;
        public float MoveTime = 3f;
        void Start()
        {
            Generate();
            StartCoroutine(_Coro_MoveAndDestroy());
        }
 
        void Generate () {
            if(mTsObjs != null)
            {
                for (int i = 0; i != NumGenerate; ++i)
                {
                    Destroy(mTsObjs[i].gameObject);
                }
                mTsObjs = null;
            }

            float angleGenerateSpace = 360F / NumGenerate;
            float curAngle = 0F;
            mTsObjs = new Transform[NumGenerate];
            Transform ts = transform;
            for (int i = 0; i != NumGenerate; ++i )
            {
                mTsObjs[i] = Instantiate(Prefab_ToEmitObj) as Transform;
                mTsObjs[i].eulerAngles = new Vector3(0F, 0F, curAngle);
                mTsObjs[i].parent = ts;
                mTsObjs[i].localPosition = Vector3.zero;
                curAngle += angleGenerateSpace;
            }
        

   
        }


        //void OnGUI()
        //{
        //    if (GUILayout.Button("explode"))
        //    {
        //        Generate();
        //        StartCoroutine(_Coro_MoveAndDestroy());
        //    }
        //}
        IEnumerator _Coro_MoveAndDestroy()
        {
            if (mTsObjs == null)
                yield break;  

            float moveElapse = 0;

            float tmpF = 0F;
            Vector3 tmpVec3 = Vector3.one;
            while (moveElapse < MoveTime)
            {
                tmpF = 0.2F + moveElapse / MoveTime;//1.2F - Mathf.Pow(0.2F,);
                tmpVec3.x = tmpF;
                tmpVec3.y = tmpF;
                foreach (Transform ts in mTsObjs)
                {

                    ts.localPosition += Time.deltaTime * MoveSpeed * ts.right;
                
                    ts.localScale = tmpVec3;
                }
                moveElapse += Time.deltaTime;
                yield return 0;
            }


            for (int i = 0; i != NumGenerate; ++i)
            {
                Destroy(mTsObjs[i].gameObject);
            }
            mTsObjs = null;
            Destroy(gameObject);
        }
	 
    }
}
