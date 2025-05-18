using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.View.YxListViews;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// 数字计时器
    /// </summary>
    public class NumberTimer : YxView
    {
        /// <summary>
        /// 当前数字
        /// </summary>
        public long Number;
        /// <summary>
        /// 是否每位都执行动画
        /// </summary>
        public bool ChangeAll;
        /// <summary>
        /// 
        /// </summary>
        public float Spacing = 10;
        /// <summary>
        /// 方向
        /// </summary>
        public YxListView.EMoveMent Direction;
        /// <summary>
        /// 从高到低
        /// </summary>
        public YxBaseLabelAdapter[] Labels;
        /// <summary>
        /// 增加量范围，不等于0 走动变化值
        /// </summary>
        public Vector2 Raise = Vector2.zero;
        /// <summary>
        /// 间隔
        /// </summary>
        public float WaitTime;

        public string CacheName;

        private YxBaseLabelAdapter[] _newLabels;
        private Vector3[] _labelsPos;
   
        protected override void OnAwake()
        {
            base.OnAwake();
            var len = Labels.Length;
            _newLabels = new YxBaseLabelAdapter[len];
            _labelsPos = new Vector3[len];
            var pos = Vector3.zero;
            if (Direction == YxListView.EMoveMent.Horizontal)
            {
                pos.x = Spacing;
            }
            else
            {
                pos.y = Spacing;
            }
            var num = GetNumberString();
            for (var i = 0;i< len ; i++)
            {
                var label = Labels[i];
                var newLabel = Instantiate(label);
                var ts = label.transform;
                var nts = newLabel.transform;
                _labelsPos[i] = ts.localPosition;
                GameObjectUtile.ResetTransformInfo(nts, ts);
                nts.localPosition = pos;
                _newLabels[i] = newLabel;
                label.Text(num[i].ToString());
            }
        }

        private void InitLabels()
        {
            var len = Labels.Length;
            for (var i = 0; i < len; i++)
            {
                var label = Labels[i];
                label.transform.localPosition = _labelsPos[i];
            }
        }
         
        protected override void OnEnable()
        {
            base.OnEnable();
            AddRandomNumber();
            InitLabels();
            StartChangeNubmer();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            if (_changeCoroutine != null)
            {
                StopCoroutine(_changeCoroutine);
                _changeCoroutine = null;
            }
        }

        private Coroutine _changeCoroutine;
        private readonly Queue<long> _numbers = new Queue<long>();
        /// <summary>
        /// 设置数字
        /// </summary>
        public void SetNumber(long number)
        {
            _numbers.Enqueue(number);
            StartChangeNubmer();
        }

        protected void StartChangeNubmer()
        {
            if (_changeCoroutine == null)
            {
                _changeCoroutine = StartCoroutine(ChangeNumber());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetNumberString()
        {
            return GetNumberString(Number);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string GetNumberString(long number)
        {
            var labelLen = Labels.Length;
            return number.ToString(string.Format("D{0}", labelLen));
        }

        /// <summary>
        /// 切换数值
        /// </summary>
        /// <returns></returns>
        private IEnumerator ChangeNumber()
        {
            var labelLen = _newLabels.Length;
            var startIndex = labelLen - 1;
            var wait = new WaitForFixedUpdate();
            var isHor = Direction == YxListView.EMoveMent.Horizontal;
            while (_numbers.Count > 0)
            {
                //赋值
                var oldNumber = Number;
                Number = _numbers.Dequeue();
                var number = GetNumberString();
                var numberStartIndex = number.Length -1;
                for (var i = startIndex; i >=0; i--)
                {
                    var num = number[numberStartIndex--];
                    _newLabels[i].Text(num.ToString());
                }
                var velocity = Vector3.zero;
                var count = GetChangeIndex(number, GetNumberString(oldNumber));
                //动画
                for (var i = startIndex; i >= 0; i--)
                {
                    var label = Labels[i];
                    var newLabels = _newLabels[i];
                    var labelTs = label.transform;
                    var pos = labelTs.localPosition;
                    var oldPos = pos;
                    var targetPos = pos;
                    if (isHor)
                    {
                        targetPos.x -= Spacing;
                    }
                    else
                    {
                        targetPos.y -= Spacing;
                    }
                    if (ChangeAll || i >= count)
                    {
                        while (Vector3.Distance(pos,targetPos)>0.1f) //newLabel到0位置
                        {
                            pos = Vector3.SmoothDamp(pos, targetPos, ref velocity, 0.05f);
                            labelTs.localPosition = pos;
                            yield return wait;
                        }
                    }
                    label.Text(newLabels.Value);
                    labelTs.localPosition = oldPos;
                } 
            }
            _changeCoroutine = null;
            yield return new WaitForSeconds(WaitTime);
            AddRandomNumber();
        }

        private void SaveCache()
        {
            if (string.IsNullOrEmpty(CacheName)) return;
            PlayerPrefs.SetString(CacheName, Number.ToString());
        }

        private void InitCache()
        {
            if (string.IsNullOrEmpty(CacheName)) return;
            if(!PlayerPrefs.HasKey(CacheName)) return;
            var longStr = PlayerPrefs.GetString(CacheName);
            long l;
            if (long.TryParse(longStr, out l))
            {
                Number = l;
            }
        }

        private int GetChangeIndex(string number, string oldNumber)
        {
            var len = Labels.Length;
            for (var i = 0; i < len; i++)
            {
                if (number[i] != oldNumber[i])
                {
                    return i;
                }
            }
            return int.MaxValue;
        }

        [ContextMenu("添加随机数")]
        protected void AddRandomNumber()
        {
            if (Raise == Vector2.zero) return;
            var raise = (int)Random.Range(Raise.x, Raise.y);
            SetNumber(Number + raise);
        }
    }
}
