using System;
using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.mx97
{
    public class MoveMgr : MonoBehaviour
    {
        readonly private int mTotalSp = 4;

        public bool IsBreakUpdate = true;

        private float _mMoveDis;
        private bool _mWillStop;                 // 是否进入停止流程

        private List<GameObject> mChilds = new List<GameObject>();
        private List<Vector3> mSpritePos = new List<Vector3>();

        private float _sumTime;
        private short _mSportStep;                   // 曲线运动时期 0上升期 1匀速期 2下降期        
        private float _mReduceRate = 1.0f;

        // ------ 外部设置数据
        [HideInInspector]
        public float ArgA;
        public float ArgB;
        public float ArgC;
        public float ArgD;
        public float HoverTime;                         // 匀速运动持续时间
        public int SwitchCountToStop;                   // 减速运动需要几次交互元素至停止
        public string StopSpriteName;                   // 停止时显示的图片
        protected void Start()
        {
            Vector3 startV0 = transform.FindChild("ChildSp0").transform.localPosition;
            Vector3 startV1 = transform.FindChild("ChildSp1").transform.localPosition;
            float judgeY = (Math.Abs(startV0.y) - Math.Abs(startV1.y)) * 2;

            for (int i = 0; i < mTotalSp; i++)
            {
                string childName = "ChildSp" + i;
                GameObject childObj = transform.FindChild(childName).gameObject;
                if (childObj != null)
                {
                    childObj.AddComponent<AniMoveFruit>();
                    childObj.GetComponent<AniMoveFruit>().JudgeY = judgeY;
                    //childObj.GetComponent<AniMoveFruit>().SetMoveToJudgeFun(MoveToJudge);

                    mChilds.Add(childObj);

                    mSpritePos.Add(childObj.GetComponent<UISprite>().transform.localPosition);
                }
                else
                {
                    Debug.Log("------> Get Child error in MoveMgr when i = " + i + "!\n");
                    return;
                }
            }

            //设置图片大小(缩放)
            foreach (var item in mChilds)
            {
                item.GetComponent<UISprite>().MakePixelPerfect();
                //float percent = item.GetComponent<UISprite>().height / 65f;
                item.GetComponent<UISprite>().height = 65;
                //item.GetComponent<UISprite>().width = (int)(item.GetComponent<UISprite>().width / percent);
            }
        }

        private void InitDatas()
        {
            _sumTime = 0f;
            _mMoveDis = 0f;
            _mWillStop = false;
            _mSportStep = 0;
        }

        protected void FixedUpdate()
        {
            if (IsBreakUpdate)
                return;

            switch (_mSportStep)
            {
                case 0:
                    {
                        _sumTime += Time.deltaTime;
                        _mMoveDis = Mathf.Abs(Mathf.Sin(_sumTime * ArgA)) * ArgB;
                        if (Mathf.PI / 2 <= _sumTime * ArgA && 0 < HoverTime)
                        {
                            _mSportStep = 1;
                        }
                    }
                    break;
                case 1:
                    {
                        HoverTime -= Time.deltaTime;
                        if (HoverTime <= 0)
                        {
                            _mSportStep = 2;
                            _mWillStop = true;
                            HoverTime = 0;

                            _mReduceRate = Mathf.PI / 2 * _mMoveDis;                          // 计算减速率
                        }
                    }
                    break;
                case 2:
                    {
                        _sumTime += Time.deltaTime;
                        _mMoveDis = Mathf.Abs(_mReduceRate / _sumTime * ArgC) * ArgD;
                    }
                    break;
            }

            for (int i = 0; i < mChilds.Count; i++)
            {
                GameObject childObj = mChilds[i];
                if (childObj == null)
                    continue;

                if (!childObj.GetComponent<AniMoveFruit>().Move(-_mMoveDis))
                    continue;

                MoveToJudge();
                break;
            }
        }

        void MoveToJudge()
        {
            string newName = "";

            if (_mWillStop)
            {
                ArgB = 40;
                _mSportStep = 2;
                SwitchCountToStop--;

                if (SwitchCountToStop < 0)
                {
                    // 为了限制停止位置 根据位置置换次数确定停止时机
                    IsBreakUpdate = true;
                }
                else if (SwitchCountToStop == 1)
                {
                    if (StopSpriteName == null)
                        newName = "";
                    else
                        newName = StopSpriteName;
                }
            }

            // 首元素移动到指定位置后 准备变换位置 先将其移至末尾
            GameObject temp = mChilds[mChilds.Count - 1];
            for (int i = mChilds.Count - 1; 0 < i; i--)
            {
                GameObject childObj = mChilds[i];
                if (childObj == null)
                    continue;
                mChilds[i] = mChilds[i - 1];
            }
            mChilds[0] = temp;

            // 校正位置
            for (int i = 0; i < mChilds.Count && i < mSpritePos.Count; i++)
            {
                mChilds[i].GetComponent<UISprite>().transform.localPosition = mSpritePos[i];
            }

            // 更换显示内容
            newName = newName.Equals("") ? App.GetGameData<Mx97GlobalData>().GetRanName() : newName;
            mChilds[0].GetComponent<UISprite>().spriteName = newName;

            //缩放
            mChilds[0].GetComponent<UISprite>().MakePixelPerfect();
            //float percent = mChilds[0].GetComponent<UISprite>().height / 65f;
            mChilds[0].GetComponent<UISprite>().height = 65;
            //mChilds[0].GetComponent<UISprite>().width = (int)(mChilds[0].GetComponent<UISprite>().width / percent);
        }

        public void StartScroll()
        {
            InitDatas();
            IsBreakUpdate = false;
        }

        public void StopScroll()
        {
            _mMoveDis = Mathf.Abs(Mathf.Sin(Mathf.PI / 2 * ArgA)) * ArgB;

            _mWillStop = true;
            _mReduceRate = Mathf.PI / 2 * _mMoveDis;                          // 计算减速率
        }


    }
}
