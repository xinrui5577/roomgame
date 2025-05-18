using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.FishGame.Common.Utils
{
    public class ShowNumber : NumberLabel
    {   
        public long Number;
        public tk2dSprite NumberPerfab;
        public string NumberPrefix;
        public UIGrid Grid;
        private readonly List<tk2dSprite> _numberList = new List<tk2dSprite>();
        private long _oldNumber;
        private bool _isPlaying;
        private readonly Queue<long> _numbers = new Queue<long>();
        private long _curNumber;
        public Color NumberColor = Color.white;
        public Vector2 NumberScale = Vector2.one;
        public bool IsReduce;
        public void Start()
        {
            SetNumber(Number);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="number">数值</param>
        public override void SetNumber(long number)
        {
            Number = number;
            _curNumber = number;
            if (_numbers.Count > 0) return;
            Show(number);
        }


        public void Show(long number)
        {
            var strNum = IsReduce ? YxUtiles.GetShowNumberToString(number) : number.ToString();
            var len = strNum.Length; 
            var oldCount = _numberList.Count;
            var count = Mathf.Min(oldCount,len);
            var index = 0;
            for (; index < count; index++)//重用
            {
                var sp = _numberList[index].GetComponent<tk2dSprite>();
                InitNumberSp(sp, strNum[index]);
            }
            if (oldCount < len)
            {
                CreateNews(index, strNum, len);
            }
            else
            {
                RemoveRemainingNumber(index); 
            }
            enabled = true;
            Grid.enabled = true;
        }

        private void LateUpdate()
        {
            Grid.Reposition();
            enabled = false;
        }

        private void CreateNews(int startIndex, string nums, int maxLen)
        { 
            for (var i = startIndex; i < maxLen; i++)
            {
                var sp = Instantiate(NumberPerfab);
                _numberList.Add(sp);
                sp.color = NumberColor;
                InitNumberSp(sp, nums[i]);
            }
        }

        private void RemoveRemainingNumber(int startIndex)
        { 
            var oldCount = _numberList.Count; 
            for (var i = oldCount-1; i >= startIndex; i--)
            {
                var go = _numberList[i];
                Destroy(go.gameObject);
                _numberList.RemoveAt(i);
            }
        }

        private void InitNumberSp(tk2dSprite sp, char num)
        { 
            var ts = sp.transform;
            ts.parent = Grid.transform; 
            ts.localPosition = Vector3.zero;
            sp.SetSprite(NumberPrefix + num);
            sp.MakePixelPerfect();
            ts.localScale = NumberScale;
        }

        public delegate void OnTimes(long curNum);
        /// <summary>
        /// 设置数字(动画)
        /// </summary>
        /// <param name="number">最终数</param>
        /// <param name="processTime">动画时间</param>
        /// <param name="count">次数</param>
        /// <param name="onFinish">完成回调</param>
        /// <param name="onTimes">每次变更回调</param>
        public override void SetNumber(long number, float processTime, int count = 11, Action onFinish = null, Action<long> onTimes = null)
        {
            if (processTime>0)
            {
                Number = number;
                _numbers.Enqueue(number);
                if (_isPlaying) return;
                _isPlaying = true;
                StartCoroutine(Changing(processTime, count, onFinish, onTimes));
                return;
            }
            SetNumber(number);
            if (onFinish != null) onFinish();
        }

        private IEnumerator Changing(float processTime, int count = 13, Action onFinish = null, Action<long> onTimes = null)
        { 
            while (_numbers.Count>0)
            { 
                var temp = _numbers.Dequeue();
                var time = processTime / count;
                var d = temp - _curNumber;
                var nd = Mathf.Abs(d);
                float a;
                if (nd < count)
                {
                    if (d >= 0)
                    {
                        count = (int)d;
                        a = 1;
                    }
                    else
                    {
                        count = (int) -d;
                        a = -1;
                    } 
                }
                else
                {
                    a = (float)(d) / count;
                }
//                count = d < count ? (int)d : count;
//                var a = (float)(d)/count;
                //Debug.Log("----------   " + count + "   " + a + " time:" + time);
                //YxDebug.Log("最终数字: " + temp + " |  差值: " + dvalue + " | 速度: " + f + " | 时间:" + time);
                for (var i = 0; i < count;i++ )
                {
                    var y = (long)(a * i + _curNumber);
                    //Debug.Log(y);
                    Show(y);
                    if (onTimes != null) onTimes(y);
                    yield return new WaitForSeconds(time);
                }
                //YxDebug.Log("最终数字: " + temp + " |  当前: " + d);
                _curNumber = temp;
            }
            Show(Number);
            _isPlaying = false;
            if (onFinish != null) onFinish();
        } 
    }
}
