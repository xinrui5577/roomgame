using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{ 
    //麻将播放特效
    public enum EnCpgEffect
    {
        chi,
        peng,
        gang,
        hu,
        zimo,

        //泉州
        youjin,
        shuangyou,       
        sanyou,
        sanjindao,
        liangpai,
        mobao,
        chongbao,
        piaohu,
        tingpai,
        none,
    }

    public class CpgEffect : MonoBehaviour
    {
        public ParticleSystem[] CpgParticle;

        public virtual void PlayEffect(EnCpgEffect effect)
        {
            if(effect==EnCpgEffect.none)
                return;

            var i = (int) effect;
            if (CpgParticle == null || CpgParticle.Length<=i || i < 0)return;
            CpgParticle[i].Stop();
            CpgParticle[i].Play();
        }

    }
}