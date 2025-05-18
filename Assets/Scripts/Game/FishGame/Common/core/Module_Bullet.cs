using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class Module_Bullet : MonoBehaviour {
        [System.NonSerialized]
        public bool[] Fireable;//索引是玩家i 

        void Start () {
            Fireable = new bool[Defines.NumPlayer];
            for (int i = 0; i != Fireable.Length; ++i)
                Fireable[i] = true;
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtPlayerGunFired += Handle_GunFire;
            HitProcessor.Evt_Hit += Handle_BulletHit;
        }

        void Handle_BulletHit(bool isMiss, Player p, Bullet b, Fish f)
        { 
            //如果打中了目标,目标有效.子弹消失
            if (!isMiss)
            {
                b.SelfDestroy();
            } 
        }

        /// <summary>
        /// 开枪(创建子弹)
        /// </summary>
        /// <param name="p">开枪者</param>
        /// <param name="g">枪</param>
        /// <param name="score">分数</param>
        /// <param name="isLock"></param>
        /// <param name="bulletId"></param>
        void Handle_GunFire(Player p, Gun g, int score, bool isLock, int bulletId)
        {
            if (isLock) return;
            var prefabBullet = g.Prefab_BulletNormal;//Get_PrefabBullet_Used(g);//找到枪对应的子弹   啬
            if (prefabBullet == null) return; 
            var b = Pool_GameObj.GetObj(prefabBullet.gameObject).GetComponent<Bullet>();
            b.Id = bulletId;  
            b.transform.position = g.local_GunFire.position;
            b.Score = score;
            b.Fire(p, null, g.TsGun.rotation);//g.AniSpr_GunPot.transform.rotation);
        }  

        public bool GetFireable(int index)
        {
            return index < Fireable.Length && Fireable[index];
        }
    }
}
