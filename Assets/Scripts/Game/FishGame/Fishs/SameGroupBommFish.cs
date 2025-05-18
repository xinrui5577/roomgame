using Assets.Scripts.Game.FishGame.Common.Compements;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Fishs
{
    public class SameGroupBommFish : Fish
    {
        public Vector3[] Positions;
        protected YxShadow[] Shadows;
        private tk2dSpriteAnimator[] _aniSprites;
        public override tk2dSpriteAnimator AniSprite
        {
            get
            {
                if (_aniSprites == null)
                {

                    var count = Positions.Length;
                    if (count < 1) return null;
                    if (_aniSprites==null) _aniSprites = new tk2dSpriteAnimator[count];
                    if(Shadows==null)Shadows = new YxShadow[count];
                    for (var i = 0; i < count; i++)
                    {
                        
                        MGoAniSprite = Pool_GameObj.GetObj(Prefab_GoAniSwim);
                        if (MGoAniSprite == null) continue;
                        MGoAniSprite.SetActive(true);
                        MAnimationSprite = MGoAniSprite.GetComponent<tk2dSpriteAnimator>();
                        var renderers = MGoAniSprite.GetComponentsInChildren(typeof(Renderer));
                        foreach (var r in renderers)
                        {
                            ((Renderer) r).enabled = true;
                        }
                        var tsAni = MGoAniSprite.transform;
                        tsAni.parent = transform;
                        var lpos = Positions[i];
                        tsAni.localPosition = lpos;
                        tsAni.localRotation = Quaternion.identity;
                        var shadowM = Shadows[i] ?? YxShadow.AddShadow(MGoAniSprite); 
                        shadowM.SetShadowModel(Prefab_GoAniSwim, MAnimationSprite.transform);
                        _aniSprites[i] = MAnimationSprite;
                        
                    }
                }
                return MAnimationSprite;
            }
        }
 
        protected override void SetFishBack()
        {
            if (Back == null) return;
             var count = Positions.Length;
            if (count < 1) return ;
             _aniSprites = new tk2dSpriteAnimator[count];
            for (var i = 0; i < count; i++)
            {
                var lpos = Positions[i];
                var back = Instantiate(Back);
                back.transform.parent = transform;
                back.transform.localPosition = new Vector3(lpos.x, lpos.y, swimmer.SwimDepth + 0.01f);
                var shadow = YxShadow.AddShadow(back);
                shadow.SetShadowModel(Back, back.transform);
            }
        }

        protected override void DeathAnimation(float delayTotal)
        {
            if (Prefab_GoAniDead == null) return;
            foreach (var pos in Positions)
            {
                var goDieAnimation = Pool_GameObj.GetObj(Prefab_GoAniDead);
                goDieAnimation.GetComponent<tk2dSprite>().color = Colour;
                var goDieAniTs = goDieAnimation.transform;
                goDieAnimation.SetActive(true);

                goDieAniTs.parent = MTs;
                goDieAniTs.localPosition = pos;
                goDieAniTs.rotation = MTs.rotation;
                goDieAniTs.parent = GameMain.Singleton.FishGenerator.transform;

                var fishRecycleDelay = goDieAnimation.AddComponent<RecycleDelay>();
                fishRecycleDelay.delay = delayTotal;
                fishRecycleDelay.Prefab = Prefab_GoAniDead;
            }
        }

        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            foreach (var pos in Positions)
            {
                var p = transform.TransformPoint(pos);
                Gizmos.DrawWireSphere(p, 10);
            } 
        }
    }
}
