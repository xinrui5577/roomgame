using System.Collections;
using Assets.Scripts.Game.lswc.Data;
using DG.Tweening;
using UnityEngine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Item
{
    public class LSAnimalItem : LSItemBase
    {
        private string[] _animations = { "1", "2", "3", "4" };

        Vector3 _defaultDeskVec = new Vector3(0,3, 0);

        public  LSAnimalType CurType;

        public LSAnimalAnimationType CurAnimation = LSAnimalAnimationType.WATCH;

        private string _curAni;

        private float _curSpeed;

        private string _watchAni;

        private string _rewardAni;

        private string[] _betAnimations;

        private Animation _animaition;

        private GameObject _animalObject;

        private float _delayTime;

        private void Start()
        {
           InitData();
           Find();
        }
        private void Find()
        {
            _animalObject = transform.Find("RotateCheck/AnimalObject").gameObject;
            _animaition = _animalObject.GetComponent<Animation>();
        }

        private void InitData()
        {
            _curAni = null;
            _betAnimations = new string[2];
            _delayTime = Random.Range(0, 2);
            switch (CurType)           //原来的是在编译器中添加参数，太麻烦了，就用代码固定了。？？？为什么兔子的两个等待下注动画一样？
            {
                case LSAnimalType.SZ:
                case LSAnimalType.GOLD_SZ:
                    _watchAni = _animations[0];
                    _rewardAni = _animations[2];
                    _betAnimations[0] = _animations[1];
                    _betAnimations[1] = _animations[3];
                    break;
                case LSAnimalType.GOLD_HZ:
                case LSAnimalType.HZ:
                    _watchAni = _animations[3];
                    _rewardAni = _animations[0];
                    _betAnimations[0] = _animations[1];
                    _betAnimations[1] = _animations[2];
                    break;
                case LSAnimalType.TZ:
                case LSAnimalType.GOLD_TZ:
                    _watchAni = _animations[0];
                    _rewardAni = _animations[1];
                    _betAnimations[0] = _animations[2];
                    _betAnimations[1] = _animations[2];
                    break;
                case LSAnimalType.XM:
                case LSAnimalType.GOLD_XM:
                    _watchAni = _animations[0];
                    _rewardAni = _animations[1];
                    _betAnimations[0] = _animations[2];
                    _betAnimations[1] = _animations[3];
                    break;
            }
        }

        public override int GetItemsNumber()
        {
            return LSConstant.Num_AnimalItemNumber;
        }

        public IEnumerator PlayAnimalAnimation(int time)
        {
 
            for (int i = 0; i < time; i++)
            {
                while (_animaition.isPlaying)
                {
                   yield return new WaitForEndOfFrame();
                }
                switch (CurAnimation)
                {
                    case LSAnimalAnimationType.BET:
                        PlayBetAnimation();
                        break;
                    case LSAnimalAnimationType.RAWARD:
                        PlayRewardAnimation();
                        break;
                    case LSAnimalAnimationType.WATCH:
                        PlayWatchAnimation();
                        break;
                    default:
                        YxDebug.LogError("Not exist such animationType " + CurAnimation.ToString());
                        break;
                }
            }
  
        }

        public bool IsRightAnimal(LSAnimalType type)
        {
            return type == CurType;
        }

        private void PlayBetAnimation()
        {
            _curAni = _animations[Random.Range(0, _betAnimations.Length - 1)];
            PlayeAni(_curAni, LSConstant.Num_BetAnimationSpeed);
        }

        private void PlayRewardAnimation()
        {
            _curAni = _rewardAni;
            PlayeAni(_curAni, LSConstant.Num_WatchOrRewardAnimationSpeed);
        }

        private void PlayWatchAnimation()
        {
            _curAni = _watchAni;
            PlayeAni(_curAni, LSConstant.Num_WatchOrRewardAnimationSpeed);
        }

        private void PlayeAni(string aniName, float speed)
        {
            _animaition[aniName].speed = speed;
            _animaition.Play(aniName);
        }

        public void MoveToCenter(float time )
        {
            SavePosition();
        
            Sequence mySequence = DOTween.Sequence();

            Tweener shake = this.transform.DOShakeScale(1f, 0.1f);

            mySequence.Append(shake);

            Vector3 p1 = this.transform.localPosition;

            Vector3 m = (p1 +_defaultDeskVec) * 0.5f + new Vector3(0, 10, 0);

            Vector3 m1 = (p1 + m) * 0.5f;
            Vector3 m2 = (_defaultDeskVec + m) * 0.5f;

            //设置贝塞尔曲线
            Bezier myBezier = new Bezier(p1, m1, m2, _defaultDeskVec);
            Vector3[] vs = new Vector3[100];
            for (int i = 0; i < 100; i++)
            {
                Vector3 vec = myBezier.GetPointAtTime((float)(i * 0.01));
                vs[i] = vec;
            }

            Tweener moveTween = this.transform.DOLocalPath(vs, time);

            moveTween.OnComplete(ResetPosition);

            mySequence.Append(moveTween);

            #region  这个地方对当前动物的索引进行了判断，处理了角度不同时的一些处理，暂时先不处理

            float angle = transform.localEulerAngles.y;
            //if((angle>90&&angle<180)||(angle<-90&&angle>180))
            //{
            Vector3 distiE = new Vector3(0, 180, 0) + this.transform.localEulerAngles;

            Tweener RotateTween = this.transform.DOLocalRotate(distiE, time).SetDelay(0.1f);

            mySequence.Join(RotateTween);
            #endregion

        }


        private Vector3 _savePosition;

        private Vector3 _saveEuler;

        private void SavePosition()
        {
            _savePosition = transform.localPosition;

            _saveEuler = transform.localEulerAngles;
        }

        private void OnMovefinished()
        {
           App.GetGameManager<LswcGamemanager>().CameraManager.transform.LookAt(transform);
        }

        private void ResetPosition()
        {
            transform.localPosition = _defaultDeskVec;
        }

        public void MoveBack()
        {
            if(_saveEuler.Equals(Vector3.zero))
            {
                return;
            }
            this.transform.localEulerAngles = _saveEuler;
            this.transform.localPosition = _savePosition;
        }
    }
}
