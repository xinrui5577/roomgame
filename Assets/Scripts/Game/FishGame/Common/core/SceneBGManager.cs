using System;
using Assets.Scripts.Game.FishGame.Backgrounds;
using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// 场景管理
    /// </summary>
    public class SceneBGManager : MonoBehaviour
    {
        /// <summary>
        /// 场景配置
        /// </summary>
        public SceneBgConfig SbgConfig; 
        public Shader OldBGShader;
        /// <summary>
        /// 切换背景时的镜头
        /// </summary>
        public Camera Camera_NewScene; 
        /// <summary>
        /// 当前背景索引
        /// </summary>
        private int CurrentBGIdx;
        /// <summary>
        /// 当前背景
        /// </summary>
        private GameObject _currentBg;
        /// <summary>
        /// 当前水纹
        /// </summary>
        private GameObject _currentWave;
        /// <summary>
        /// 是否已加载
        /// </summary>
        private bool _isLoadBg;
        //private  
        public GameObject Background;

        private void Awake()
        {  
            //创建背景容器
            var worldDim = GameMain.Singleton.WorldDimension;
            var bgTs = Background.transform; 
        }
         
        /// <summary>
        /// 初始化水纹
        /// </summary>
        public void InitWave()
        {
            if (SbgConfig == null) return;
            var wave = SbgConfig.PrefabWave;
            if (_currentWave == null && wave != null)
            {
                _currentWave = Instantiate(wave);
                var waveTs = _currentWave.transform;
                waveTs.parent = transform;
                waveTs.localPosition = Vector3.zero;
            }
        }

        /*void Handle_StartGame()
        {
            if (SbgConfig.IsNewBgWhenStartGame) NewBG(); 
        }*/

        /// <summary>
        /// 初始化一个场景
        /// </summary>
        public void NewBG()
        {
            NewBG(-1);
        }

        /// <summary>
        /// 初始化背景
        /// </summary>
        /// <param name="finished"></param>
        public void InitBackground(Action finished = null)
        { 
            StartCoroutine(ChangeBackGround(finished));
        }

        /// <summary>
        /// 切换背景协程
        /// </summary>
        /// <param name="finished"></param>
        /// <returns></returns>
        private IEnumerator ChangeBackGround(Action finished)
        {
            InitWave();
            CurrentBGIdx = SbgConfig.RandomBackGroundIndex();
            var oldBg = _currentBg;
            LoadNewBackGround(CurrentBGIdx);
            while (_isLoadBg) yield return 1;
            if (_currentBg == null) yield break;
            SetBackground(_currentBg.transform);
            _currentBg.SetActive(true);
            if (oldBg != null) Destroy(oldBg);
            if (finished != null) finished();
        }

        /// <summary>
        /// 设置背景
        /// </summary>
        /// <param name="bgTs"></param>
        public void SetBackground(Transform bgTs)
        {
            bgTs.parent = Background.transform;
            bgTs.localPosition = Vector3.zero;
            bgTs.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 新背景
        /// </summary>
        /// <param name="bgIdx">如果为-1则随机初始化</param>
        public void NewBG(int bgIdx)
        {
            var bgCount = SbgConfig.BackGroundNames.Length;
            if (bgCount < 1) return;

            CurrentBGIdx = bgIdx < 0 ? SbgConfig.RandomBackGroundIndex() : bgIdx % bgCount;

            if (_currentBg != null)
            {
                Destroy(_currentBg.gameObject);
                _currentBg = null;
            } 
            InitWave();
        }
         
        /// <summary>
        /// 加载背景
        /// </summary>
        /// <param name="index">背景索引</param>
        private void LoadNewBackGround(int index)
        { 
            if (_isLoadBg) return;
            _isLoadBg = true;
            var map = SbgConfig.LoadMap(index);
            var info = new AssetBundleInfo {Asset = map };
            Finishedload(info);
        }

        /// <summary>
        /// 加载背景
        /// </summary>
        /// <param name="index">背景索引</param>
        private void LoadNewBackGroundAync(int index)
        { 
            if (_isLoadBg) return;
            _isLoadBg = true;
//            var map = SbgConfig.LoadMap(index);
            SbgConfig.LoadMapAsync(index, Finishedload);
        }

        private void Finishedload(AssetBundleInfo info)
        {
            var asset = info.Asset;
            if (asset != null)
            {
                var go = asset as GameObject;
                _currentBg = Instantiate(go);
                _currentBg.SetActive(false);
            }
            _isLoadBg = false;
        }

        private bool _isSweep;
        public bool IsSweep {
            get { return _isSweep; }
        }

        /// <summary>
        /// 海浪换图
        /// </summary>
        public void Sweep()
        {
            _isSweep = true; 
            StartCoroutine(ChangeBg());
        }

        private IEnumerator ChangeBg()
        {
            //if (_currentBg == null) yield break; 
            var gm = GameMain.Singleton;
            //音效
            if (SbgConfig.SndZhuanChangLangHua != null)
            {
                Facade.Instance<MusicManager>().Play(SbgConfig.SndZhuanChangLangHua);
            }

            //打开背景camera
            Camera_NewScene.enabled = true;

            //创建newScene
            var oldBg = _currentBg;
            CurrentBGIdx = (++CurrentBGIdx) % SbgConfig.BackGroundNames.Length;
            LoadNewBackGroundAync(CurrentBGIdx);
            while (_isLoadBg) yield return 1;
            
            var bgTs = _currentBg.transform; 
            bgTs.parent = Camera_NewScene.transform;
            _currentBg.SetActive(true);
            var pond = gm.PondTs;
            var pondy = pond.parent.localPosition.y;
            var pondR = pond.localRotation;
            bgTs.localPosition = new Vector3(0, pondy, 950);
            bgTs.localRotation = pondR;
            var sweeperPerfab = SbgConfig.PrefabTsSweeper;
            //创建新背景
            //初始化遮罩sweeper
            var tsSweeper = Instantiate(sweeperPerfab);
            tsSweeper.parent = transform;
            tsSweeper.position = new Vector3(gm.WorldDimension.xMax, 0F, SbgConfig.SweeperZ);
         
            var useTime = SbgConfig.UseTime;
            //扫过
            var elapse = 0F; 
            var spd = (gm.WorldDimension.width + 0.84F) / useTime;

            while (elapse < useTime)
            {
                tsSweeper.position += spd * Time.deltaTime * tsSweeper.right; 
                elapse += Time.deltaTime; 
                yield return 1;
            }
             
            //new清除所有鱼
            GameMain.Singleton.FishGenerator.KillAllImmediate();

            //减淡tsSweeper
            var sprAniSea = tsSweeper.GetComponentInChildren<tk2dSprite>();
             
            const float fadeOutUseTime = 1F;
            elapse = 0F;
            var aniCol = sprAniSea.color;
            aniCol.a = 1F;
            while (elapse < fadeOutUseTime)
            {
                aniCol.a = 1F - elapse / fadeOutUseTime;
                sprAniSea.color = aniCol;
                  
                elapse += Time.deltaTime;
                yield return 1;
            } 
            //将新场景移动主镜头处  
            SetBackground(_currentBg.transform);
            //关闭背景camera
            Camera_NewScene.enabled = false;

            //删除 sweeper
            Destroy(tsSweeper.gameObject); 
                 
            //删除旧场景
            Destroy(oldBg);

            _isSweep = false;
        }

        public void RotateBg()
        {
        }
    }
}
