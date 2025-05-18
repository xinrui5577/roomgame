using Assets.Scripts.Game.FishGame.Common.Brains.FishAI;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class Ef_FreezeBomb : MonoBehaviour {

        public Vector3 Position = new Vector3(0.56F, 0.03F, -0.03F);
        public tk2dTextMesh PrefabTextCountDown;

        public float WaitTime = 10;
     
        private float _curCountDown = 0;
        private tk2dTextMesh _textCountDown;
        private float _lastTime;
        public void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFreezeBombActive += Handle_FreezeAllFishBegin;
            gdata.EvtFreezeBombDeactive += Handle_FreezeBombDeactive;
        }
      
        void Handle_FreezeAllFishBegin()
        {
            GameMain.IsMainProcessPause = true;
            _curCountDown += WaitTime;
            enabled = true;
            Handle_FreezeBombActive();
            if (_textCountDown == null)
            {
                _textCountDown = Instantiate(PrefabTextCountDown);
                _textCountDown.transform.position = Position;
            }
            _textCountDown.gameObject.SetActive(true); 
        }

        void Update()
        {
            if (_textCountDown == null) return;
            if (GameMain.State_ != GameMain.State.Normal)
            {
                Stop();
                return;
            }
            _lastTime += Time.deltaTime;
            if (_lastTime > 1)
            {
                _lastTime -= 1;
                if (_curCountDown < 0)
                {
                    Stop();
                    return;
                }
                _textCountDown.text = _curCountDown.ToString();
                _curCountDown--; 
            }
            if (_curCountDown<4)
            {
                _textCountDown.gameObject.SetActive(_lastTime < 0.5);
            }
        }

        public void Stop()
        {
            enabled = false;
            _curCountDown = 0;
            Destroy(_textCountDown.gameObject);
            _textCountDown = null;
            Recover(); 
        }

        //恢复出鱼,和鱼的移动
        void Recover()
        {
            GameMain.IsMainProcessPause = false;
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtFreezeBombDeactive != null)
                gdata.EvtFreezeBombDeactive();
        }

        void Handle_FreezeBombActive()
        {
            var mGm = GameMain.Singleton;
            if (mGm == null) return;
            //停止出鱼
            if (GameMain.State_ == GameMain.State.Normal)
            {
                mGm.FishGenerator.StopFishGenerate();
            }
            //停止所有鱼动作
            var generator = mGm.FishGenerator;
            var generatorTs = generator.transform;
            var count = generatorTs.childCount;
            for (var i = 0; i < count; i++)
            {
                var fish = generatorTs.GetChild(i);
                var aiFish = fish.GetComponent<IFishAI>(); 
                if (aiFish != null) aiFish.Pause();
                var swm = fish.GetComponent<Swimmer>();
                if (swm != null) swm.StopImm(); 
            } 
        }
        void Handle_FreezeBombDeactive()
        {
            var mGm = GameMain.Singleton;
            if (mGm == null) return;
            if (GameMain.State_ == GameMain.State.Normal)
            { 
                mGm.FishGenerator.StartFishGenerate();
            }
            //恢复出鱼
            var generator = mGm.FishGenerator;
            var generatorTs = generator.transform;
            var count = generatorTs.childCount;
            for (var i = 0; i < count; i++)
            {
                var fish = generatorTs.GetChild(i);
                var aiFish = fish.GetComponent<IFishAI>();
                if (aiFish != null) aiFish.Resume();
                var swm = fish.GetComponent<Swimmer>();
                if (swm != null) swm.Go();
            } 
        }
    }
}
