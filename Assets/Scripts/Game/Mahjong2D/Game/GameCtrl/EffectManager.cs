/** 
 *文件名称:     EffectManager.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-26 
 *描述:         特效管理类//todo 打算做一个特效管理类，目前是挂载在对应的玩家菜单下，感觉比较费，将来做优化处理
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class EffectManager :MonoSingleton<EffectManager>
    {
        [SerializeField]
        private List<ParticleSystem> _effects=new List<ParticleSystem>();
        public override void Awake()
        {
            base.Awake();
        }
    }
}
