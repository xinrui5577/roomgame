using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Salvo.Entity;
using UnityEngine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Salvo.View
{
    public class PokerAreaView : YxView
    {

        private bool _isPlaying;
        public List<Poker> Pokers;
        public float TurnIntervalTime = 0.1f;
        private readonly List<Poker> _rePlacePokers = new List<Poker>(); 

        public void InitPokers()
        {
            for (var i = 0; i < 5; i++)
            {
                var poker = Pokers[i];
                poker.Init(poker.Value);
            }
        }

        /// <summary>
        /// 初始化扑克数据
        /// </summary>
        /// <param name="datas">扑克数据</param>
        public void InitPokers(int[] datas)
        {
            YxDebug.LogError("第一次扑克");
            YxDebug.LogArray(datas);
            _rePlacePokers.Clear();
            var count = Mathf.Min(Pokers.Count, datas.Length);
            for (var i = 0; i < count;i++ )
            {
                var poker = Pokers[i];
                poker.Init(datas[i]);
            }
        }

        public void SetReplacePokers(int[] datas)
        {
            YxDebug.LogError("第二次扑克");
            YxDebug.LogArray(datas);
            var count = Mathf.Min(Pokers.Count, datas.Length);
            for (var i = 0; i < count; i++)
            {
                var poker = Pokers[i];
                var value = poker.Value;
                poker.SetValue(datas[i], value); //value);
            }
        }

        /// <summary>
        /// 翻牌
        /// </summary>
        /// <param name="onFinish"></param>
        public void TurnPoker(Action onFinish = null)
        {
            TurnPoker(Pokers, onFinish);
        }


        /// <summary>
        /// 替换扑克
        /// </summary>
        public void ReplacePokers(Action onFinish = null)
        {
            TurnPoker(_rePlacePokers, () => //翻拍->背面
                                      TurnPoker(_rePlacePokers, onFinish, 0.7f)); //翻牌->值面
        }

        /// <summary>
        /// 翻拍
        /// </summary>
        public void TurnPoker(List<Poker> pokers,Action onFinish=null,float delayed=0f)
        {
            if (!gameObject.activeSelf || _isPlaying) return;
            _isPlaying = true; 
            StartCoroutine(Turning(pokers, onFinish, delayed));
        }

        private IEnumerator Turning(List<Poker> pokers, Action onFinish, float delayed = 0f)
        { 
            yield return new WaitForSeconds(delayed);
            if(pokers == null)yield break;
            var count = pokers.Count;
            for (var i = 0; i < count;i++ )
            {
                var poker = pokers[i];
                poker.Turn();
                yield return new WaitForSeconds(TurnIntervalTime);
            }
            yield return new WaitForSeconds(delayed);
            _isPlaying = false;
            if (onFinish != null) onFinish();
        }
           
        /// <summary>
        /// 获得需要换牌的数组
        /// </summary>
        /// <returns></returns>
        public int[] GetChangePokers()
        {
            var cpokers = new List<int>();
            var count = Pokers.Count;
            for (var i = 0; i < count;i++ )
            {
                var poker = Pokers[i];
                if(poker.IsHeld())continue;
                cpokers.Add(i);
                _rePlacePokers.Add(poker);
            } 
            return cpokers.ToArray();
        }
         
        public int[] GetValues()
        {
            var list = new List<int>();
            var count = Pokers.Count;
            for (var i = 0; i < count;i++ )
            {
                list.Add(Pokers[i].Value);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 设置heldpuke
        /// </summary>
        /// <param name="helds">左→右|低→高</param>
        public void SetHeldPokers(int helds)
        {
            YxDebug.Log(Convert.ToString(helds, 2).PadLeft(5, '0'));
            var count = Pokers.Count;
            for (var i = 0; i < count; i++)
            {
                var poker = Pokers[i];
                poker.SetHeld((helds&(1<<i))>0);
            }
        }

        public void HighlightPoker(int helds)
        {
            YxDebug.Log(Convert.ToString(helds, 2).PadLeft(5, '0'));
            var count = Pokers.Count;
            for (var i = 0; i < count; i++)
            {
                var poker = Pokers[i];
                poker.SetHight((helds & (1 << i)) > 0);
                poker.ClickTarget.isEnabled = false;
            }
        }
    } 
}
