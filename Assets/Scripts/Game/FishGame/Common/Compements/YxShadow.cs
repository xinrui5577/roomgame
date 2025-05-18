using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.Compements
{
    public class YxShadow : MonoBehaviour
    {
        private Transform _body;
        /// <summary>
        /// 影子
        /// </summary>
        private Transform _shadowObj;

        public Vector3 SunPos = new Vector3(0, 0, 0.005f);
        public Color SColor = new Color(0, 0, 0, 0.6f);
        public float Rate = 0.1f;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (_shadowObj == null) return;
            var pts = _body.position;
            pts.x += SunPos.x + pts.x * Rate;
            pts.y += SunPos.y + pts.y * Rate;
            pts.z += SunPos.z;
            _shadowObj.position = pts;
        }

        public GameObject SetShadowModel(GameObject modelPerf, Transform parentTs,tk2dSpriteAnimator referAnimator=null)
        {
            if (_shadowObj != null)
            {
                Clear();
            }
            tk2dSpriteAnimator animator;
            var go = CreateModel(modelPerf, out animator, SunPos, SColor, parentTs);
            if (referAnimator != null) animator.PlayFrom(referAnimator.DefaultClip, Time.time);
            _body = parentTs;
            _shadowObj = go.transform;
            return go;
        }

        public void SetRecycleShadowModel(GameObject modelPerf, Transform parentTs, float delayTotal, tk2dSpriteAnimator referAnimator = null)
        {
            var model = SetShadowModel(modelPerf, parentTs, referAnimator);
            var fishRecycleDelay = model.AddComponent<RecycleDelay>();
            fishRecycleDelay.delay = delayTotal;
            fishRecycleDelay.Prefab = modelPerf;
        }

        private static GameObject CreateModel(GameObject modelPerf, out tk2dSpriteAnimator animator, Vector3 posOff, Color color, Transform parentTs)
        {
            var model = Pool_GameObj.GetObj(modelPerf);
            model.SetActive(true);
            animator = model.GetComponent<tk2dSpriteAnimator>() ?? model.GetComponentInChildren<tk2dSpriteAnimator>();
            var crenderers = model.GetComponentsInChildren<Renderer>();
            foreach (var r in crenderers)
            {
                r.enabled = true;
            }
            var aniTs = model.transform; 
            aniTs.parent = parentTs.parent;
            aniTs.position = parentTs.position + posOff;
            aniTs.rotation = parentTs.rotation;
            var tk2Ds = model.GetComponentsInChildren<tk2dSprite>();
            foreach (var tks in tk2Ds)
            {
                tks.color = color;
            }
            return model;
        }

        public static YxShadow AddShadow(GameObject obj)
        {
            return obj.GetComponent<YxShadow>() ?? obj.AddComponent<YxShadow>();
        }

        public void Clear()
        {
            if (_shadowObj != null)
            {
                var obj = _shadowObj.gameObject;
                Pool_GameObj.RecycleGO(obj, obj);
                obj.SetActive(false);
            }
            Destroy(this);
        }
    }
}
