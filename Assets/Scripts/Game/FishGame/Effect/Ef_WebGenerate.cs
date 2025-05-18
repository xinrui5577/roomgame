using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Lite;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Effect
{
    /// <summary>
    /// 代理绑定，子弹销毁触发EvtBulletDestroy
    /// </summary>
    public class Ef_WebGenerate : MonoBehaviour {
        public WebDatas Prefab_WebDataNormal;
        public WebDatas Prefab_WebDataLizi;

        public ColorSet Prefab_WebColorNormal;
        // Use this for initialization
        void Start () {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtBulletDestroy += Handle_BulletDestroy;
        }

        void Handle_BulletDestroy(Bullet b)
        {
            // 1.使用webData 来区分网
            // 2.使用FishOddsMulti来区分离子炮
            bool isLizi = b.FishOddsMulti == 2;
            WebDatas wdToIteration = isLizi ? Prefab_WebDataLizi : Prefab_WebDataNormal;
        
            WebScoreScaleRatio useWebData = null;

            var count = wdToIteration._WebDatas.Length;
            for (var i = 0; i < count; ++i)
            {
                if (b.Score > wdToIteration._WebDatas[i].StartScore) continue;
                useWebData = wdToIteration._WebDatas[i];
                break;
            }

            if (useWebData == null)
            {
                useWebData = count > 0 ? wdToIteration._WebDatas[count-1] : new WebScoreScaleRatio(); 
            } 
            CreateWeb(b, useWebData, isLizi);
        }


        void CreateWeb(Bullet b, WebScoreScaleRatio webData,bool isLizi)
        {
            var goWebBoom = Instantiate(webData.PrefabWebBoom) as GameObject;
            goWebBoom.transform.parent = transform; 
            var efBubble = goWebBoom.GetComponent<Ef_WebBubble>();
            if (efBubble != null)
            {
                efBubble.ScaleTarget = webData.BubbleScale;
            } 
            var efWebs = goWebBoom.GetComponentsInChildren<Ef_WebBoom>();
            var selfIdx = App.GameData.SelfSeat % 6;
            foreach (var efWeb in efWebs)
            {
                efWeb.Prefab_GoSpriteWeb = webData.PrefabWeb; 
                efWeb.NameSprite = webData.NameSprite; 
                efWeb.ScaleTarget = webData.Scale; 
                efWeb.transform.localPosition *= webData.PositionScale;
                var idx = b.Owner.Idx;
                if (!isLizi)
                    efWeb.ColorInitialize = selfIdx == idx ? Prefab_WebColorNormal.Colors[b.Owner.Idx % Prefab_WebColorNormal.Colors.Length] : Color.white; 
            }

            var tsWeb = goWebBoom.transform;
            var tsBullet = b.transform;
            tsWeb.position = new Vector3(tsBullet.position.x, tsBullet.position.y, Defines.GlobleDepth_Web);
            tsWeb.rotation = tsBullet.rotation;
        }


    }
}
