using System;
using System.Collections;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Common.components
{
    /// <summary>
    /// 女郎
    /// </summary>
    public class YxBunny : MonoBehaviour
    {
        public YxBaseAnimationAdapter AnimationAdapter;

        public string[] Hellos;
        public string[] DefaultAction;
        public Vector2 Interval = new Vector2(10,30);
        protected IEnumerator Start()
        {
            if (Hellos.Length > 0)
            {
                Random.InitState((int)DateTime.Now.Ticks);
                var index = Random.Range(0, Hellos.Length);
                var soundName = Hellos.GetElement(index);
                Facade.Instance<MusicManager>().Play(soundName);
            }
            var actionCount = DefaultAction.Length;
            var flag = actionCount > 0;
            // ReSharper disable once LoopVariableIsNeverChangedInsideLoop
            while (flag)
            { 
                Random.InitState((int)DateTime.Now.Ticks);
                var random = Random.Range(Interval.x, Interval.y);
                yield return new WaitForSeconds(random);
                Random.InitState((int)DateTime.Now.Ticks);
                var index = Random.Range(1, actionCount);
                var defaultAction = DefaultAction.GetElement(index);
                if (!string.IsNullOrEmpty(defaultAction))
                {
                    AnimationAdapter.PlayByName(defaultAction, null, null, true);
                }
            }
        }

        /// <summary>
        /// 播放动作
        /// </summary>
        public void PlayAction(string actionName,string sound)
        {
            if (!string.IsNullOrEmpty(actionName))
            {
                AnimationAdapter.PlayByName(actionName,null, null, true);
            }
            if (!string.IsNullOrEmpty(sound))
            {
                var arr = sound.Split('|');
                var soundCount = arr.Length;
                if (soundCount > 0)
                {
                    Random.InitState((int)DateTime.Now.Ticks);
                    var index = Random.Range(0,soundCount);
                    var soundName = arr.GetElement(index);
                    Facade.Instance<MusicManager>().Play(soundName);
                }
            }
        }
    }
}
