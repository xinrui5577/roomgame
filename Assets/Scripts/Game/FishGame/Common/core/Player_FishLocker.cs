using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Effect;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class Player_FishLocker : MonoBehaviour
    {
        public Transform LockLineRes;//锁鱼时产生的线
        public int LockLineDistance = 100;
        [System.Serializable]
        public class FishLockPosOffset
        {
            public Fish PrefabFish;
            public Vector3 PosOffsetLock;
        }

        public Transform TsTargeter; //瞄准图标
        public tk2dSprite Spr_Target;
        public tk2dSprite Spr_TargetMoving;

        public string NameSprTarget; //停留在目标上的瞄准器精灵名字,不带序号
        public string NameSprTargetMoving; //瞄准器移动精灵名字,不带序号
    
        public static readonly float MoveSpd = 2F;

        public delegate void Event_ReLock(Fish f, Player p);

        public Event_ReLock EvtRelock; //重锁定

        public EfFishLocker FishLocker; 

        public delegate void Event_Unlock(Player p);

        public Event_Unlock EvtUnlock;

        [System.NonSerialized] public bool IsLockable = true; //是否可进行锁定

        private int mCurLockIdx; //当前锁定索引,是Prefab_FishLockabe的数组索引
        private float mDepth; //瞄准框所在深度(避免与其他框重叠)
        private Rect mStartLockArea; //开始瞄准时的区域
        private Rect mChangeTargetArea; //改变目标区域
        private bool _isLock;

        private Transform mTs;
        private Player mPlayer;

        private void Awake()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtMainProcessFinishChangeScene += Handle_FinishChangeScene;
            gdata.EvtMainProcessPrepareChangeScene += Handle_PrepareChangeScene;
            mPlayer = GetComponent<Player>();
            FishLocker.Hide();
            enabled = false;
        }


        private void Start()
        {
            mTs = transform;

            mDepth = Defines.GlobleDepth_PlayerTargeter + 0.001F*mPlayer.Idx; //玩家idx可能未初始化完
            
            mChangeTargetArea =GameMain.Singleton.WorldDimension;
            mStartLockArea = GameMain.Singleton.WorldDimension;

            Spr_TargetMoving.spriteId = Spr_TargetMoving.GetSpriteIdByName(NameSprTargetMoving + mPlayer.Idx%10);
            Spr_Target.spriteId = Spr_Target.GetSpriteIdByName(NameSprTarget + mPlayer.Idx%10);

            Spr_TargetMoving.gameObject.SetActive(false);
            Spr_Target.gameObject.SetActive(false);
        }

        /// <summary>
        /// 锁定鱼
        /// </summary>
        /// <returns></returns>
        public Fish Lock()
        {
            if (!IsLockable) return null;
            enabled = true; 
            _state = State.Find;
            _isLock = true;
            return null;
        }

        public void UnLock()
        { 
            if (!IsLockable)
                return;
            if (EvtUnlock != null)
                EvtUnlock(mPlayer);
            _isLock = false;
            Restart();
        }
         
        private enum State
        {
            Find,
            Homing,
            Lock,
            StartLeave,
            Leave,
            End
        }

        private State _state;
        private Fish _curTarget;

        public void SetLockTarget(Fish target)
        {
            _curTarget = target;
            if (IsInvalidTarget())
            {
                _isLeaving = false;
                Restart();
            }
            else StartHoming();
            TurnGun();
        }

        private void LateUpdate()
        {
            switch (_state)
            {
                case State.Find:
                    if (NeedLeave()) break;
                    _curTarget = FindTarget();
                    if (IsInvalidTarget())
                    {
                        _isLeaving = false;
                        Restart();
                    }
                    else StartHoming();
                    TurnGun();
                    break;
                case State.Homing:
                    if (NeedLeave()) break;
                    Homing();
                    TurnGun();
                    break;
                case State.Lock:
                    if (NeedLeave()) break;
                    Locking();
                    TurnGun();
                    break;
                case State.StartLeave:
                    StartLeave();
                    break;
                case State.Leave:
                    Leave();
                    break;
                case State.End:
                    End();
                    break;
            } 
            //泡泡直线
            DrawLockLine();
        }

        private void TurnGun()
        {
            var gunTs = mPlayer.GunInst.TsGun;
            var lockLin = LockLineRes.parent; //line容器
            var upToward = (TsTargeter.position) - lockLin.position;//loclLocal != null ? loclLocal.LockBulletPos :
            upToward.z = 0F;
            var quaternion = Quaternion.FromToRotation(Vector3.up, upToward);
            quaternion = Quaternion.Slerp(gunTs.rotation, quaternion, 10F * Time.deltaTime);
            mPlayer.GunInst.Turn(quaternion);
        }

        private bool NeedLeave()
        {
            if (_isLock)
            { 
                return false;
            } 
            _state = State.StartLeave;
            return true;
        }

        private bool _isLeaving;
        private void Restart()
        {
            if (_isLeaving) return;
            _isLeaving = true;
            _state = State.StartLeave; 
            if (EvtUnlock != null) EvtUnlock(mPlayer);
        }

        private void StartHoming()
        {
            if (EvtRelock != null) EvtRelock(_curTarget, mPlayer);
            _state = State.Homing;
        }

        /// <summary>
        /// 是否无效目标
        /// </summary>
        /// <returns></returns>
        public bool IsInvalidTarget()
        {
            return _curTarget == null
                   || !_curTarget.Attackable
                   || !mChangeTargetArea.Contains(_curTarget.transform.position);
        }

        private Vector3 _targetVec;
        private float _movePercent = 1F;
        private float _processPercent;

        private Fish FindTarget()
        { 
            Spr_TargetMoving.gameObject.SetActive(true);
            Spr_Target.gameObject.SetActive(false);
            var lockFishs = GameMain.Singleton.FishGenerator.FishLockable;
            if (lockFishs == null) return null;
            var count = lockFishs.Count;
            if (count < 1) return null;
            var nextIndex = mCurLockIdx;
            if (++nextIndex >= count) nextIndex = 0;
            var lockFish = lockFishs[nextIndex];
            mCurLockIdx = nextIndex;
            _movePercent = 0;
            return lockFish;
        }

        public bool HasLock()
        {
            return _isLock;
        }

        /// <summary>
        /// 瞄准
        /// </summary>
        private void Homing()
        {
            if (IsInvalidTarget())
            {
                _state = State.Find;
                return;
            }
            var startPos = TsTargeter.position;
            var targetPos = _curTarget.transform.position;
            startPos.z = mDepth;
            targetPos.z = mDepth;
//        var tarVec = _curTarget.transform.position - startPos;
//        tarVec.z = 0F;
//        var vecPercent = 1f - _movePercent * _movePercent;
//        _movePercent -= MoveSpd * Time.deltaTime;
//        TsTargeter.position = startPos + tarVec * vecPercent;

            _movePercent += Time.deltaTime;
            TsTargeter.position = Vector2.Lerp(startPos, targetPos, _movePercent);
//        if (_movePercent >= 0F) return;
            if (_movePercent < 0.5) return;
            Spr_TargetMoving.gameObject.SetActive(false);
            Spr_Target.gameObject.SetActive(true);
            FishLocker.Show(_curTarget.LockSpriteName);
            _state = State.Lock;
        }


        /// <summary>
        /// 锁定
        /// </summary>
        private void Locking()
        {
            if (IsInvalidTarget())
            {
                _state = State.Find;
                return;
            }
            var tmpVec3 = _curTarget.transform.position;
            var tsSprTarget = Spr_Target.transform;
            tmpVec3.z = mDepth;
            TsTargeter.position = tmpVec3;
            tsSprTarget.rotation = Quaternion.AngleAxis(Mathf.Cos(Mathf.PI*2F*_processPercent)*25F, Vector3.forward);
            _processPercent += Time.deltaTime*0.5F;
        }

        /// <summary>
        /// 准备脱离目标
        /// </summary>
        private void StartLeave()
        {
            FishLocker.Hide();
            Spr_TargetMoving.gameObject.SetActive(true);   
            Spr_Target.gameObject.SetActive(false);
            _movePercent = 1f;
            _targetVec = TsTargeter.position - mTs.position;
            _targetVec.z = 0f;
            _state = State.Leave;
        }

        /// <summary>
        /// 脱离目标
        /// </summary>
        private void Leave()
        {
            var vecPercent = _movePercent*_movePercent;
            _movePercent -= MoveSpd*Time.deltaTime;
            var tmpPos = mTs.position + _targetVec*vecPercent;
            tmpPos.z = Defines.GlobleDepth_PlayerTargeter;
            TsTargeter.position = tmpPos;

            if (_movePercent >= 0F) return; 
            _state = State.End;
        }

        /// <summary>
        /// 结束
        /// </summary>
        private void End()
        {
            if (TsTargeter != null && mTs != null)
            {
                TsTargeter.position = mTs.position;
            }
            if (Spr_TargetMoving!= null) Spr_TargetMoving.gameObject.SetActive(false);
            if (Spr_Target!=null) Spr_Target.gameObject.SetActive(false);
            HidLockLineRes();
            _curTarget = null;
            _isLeaving = false;
            if (_isLock)
            {
                _state = State.Find;
            }
            else
            {
                enabled = false;
            } 
        }

        private void Handle_FinishChangeScene()
        {
            IsLockable = true;
        }

        private void Handle_PrepareChangeScene()
        { 
            UnLock();
            IsLockable = false;
            _isLock = false;
        }

        private void OnDisable()
        {
            End();
        }

        private readonly List<Transform> _lockLine = new List<Transform>();
        /// <summary>
        /// 绘制瞄准直线
        /// </summary>
        public void DrawLockLine()
        { 
            var tsGun = mPlayer.GunInst.TsGun;
            if (tsGun == null) return;
            var lockLin = LockLineRes.parent; //line容器
            //lockLin.rotation = tsGun.rotation;
            var resCount = _lockLine.Count;//资源个数
            var target = TsTargeter.position;

            var lookDirect = target - lockLin.position;
            lockLin.rotation = Quaternion.LookRotation(Vector3.forward, lookDirect);//gun's firepot is upward 
             
            LockLineRes.position = target;
            LockLineRes.gameObject.SetActive(false);
            var distance = LockLineRes.localPosition.y;
            LockLineDistance = 100;
            var count = (int)(distance/LockLineDistance);//个数
            var minCount = Mathf.Min(resCount, count);
            const float offset = 0.08f;
            var startScale = 0.8f;
            var i = 0;
            for (; i < minCount; i++)
            {
                var res = _lockLine[i];
                startScale += offset;
                res.localScale = new Vector3(startScale, startScale, startScale);
                res.gameObject.SetActive(true);
            }
            for (; i < count;i++ )
            {
                var res = Instantiate(LockLineRes);
                var pos = new Vector3(0, (i + 1) * LockLineDistance, 0);
                res.parent = lockLin;
                res.localPosition = pos;
                startScale += offset;
                res.localScale = new Vector3(startScale, startScale, startScale);
                res.GetComponent<tk2dSprite>().color = mPlayer.ColorType;
                _lockLine.Add(res);
            }
            for (; i < resCount;i++ )
            {
                var res = _lockLine[i];
                res.gameObject.SetActive(false);
            } 
        }

        public void OnDrawGizmos()
        {
            if (mPlayer == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(mPlayer.GunPlatform.position, TsTargeter.position); 
        }

        private void HidLockLineRes()
        {
            foreach (var res in _lockLine)
            {
                res.gameObject.SetActive(false);
            }
        }
    }
}
