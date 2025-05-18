using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;

/*===================================================
 *文件名称:     PludoPlane.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	飞飞飞飞机
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoPlane : PludoColorItem
    {
        #region UI Param
        [Tooltip("需要旋转的物体")]
        public GameObject RotateObj;
        [Tooltip("飞机选中效果")]
        public TweenScale InSelectTween;
        [Tooltip("飞行火焰")]
        public GameObject FlyFire;
        [Tooltip("子弹")]
        public PludoBullet BulletPrefab;
        [Tooltip("子弹容器")]
        public Transform BulletContainer;
        [Tooltip("锁定")]
        public GameObject LockObj;
        [Tooltip("爆炸效果")]
        public UISpriteAnimation BoomAnimation;
        [Tooltip("飞机数量")]
        public UISprite PlaneNumSprite;
        [Tooltip("点击区域")]
        public BoxCollider BoxCollider;
        #endregion

        #region Data Param
        [Tooltip("飞机图片索引（ColorSprites中的索引）")]
        public int PlaneUiIndex = 0;
        [Tooltip("飞机数量格式")]
        public string PlaneNumFormat = "Plane_{0}";
        /// <summary>
        /// 飞机火焰偏移量（索引对应颜色）
        /// </summary>
        public List<Vector3> ColorFireOffset = new List<Vector3>();
        /// <summary>
        /// 显示合体事件
        /// </summary>
        public List<EventDelegate> OnShowFitAction=new List<EventDelegate>();

        /// <summary>
        /// 完成事件
        /// </summary>
        public List<EventDelegate> OnFinishAction = new List<EventDelegate>();
        [Tooltip("飞机常规移动时间")]
        public float PlaneMoveTime = 1f;
        [Tooltip("飞机各种动作间的间隔时间")]
        public float PlaneMoveCellTime = 0.1f;
        [Tooltip("锁定飞机旋转时间")]
        public float LockTweenTime = 0.5f;
        [Tooltip("子弹飞行时间")]
        public float BulletMoveTime = 1f;
        [Tooltip("爆炸动画执行时间")]
        public float BoomAniTime = 1.5f;
        [Tooltip("合体动画时间")]
        public float FitAniTime = 3;
        [Tooltip("完成动画时间")]
        public float FinishShowTime = 1.5f;

        public float PlaneMoveWithCellTime
        {
            get { return PlaneMoveTime + PlaneMoveCellTime; }
        }

        /// <summary>
        /// 被击中飞机等待显示爆炸动画时长
        /// </summary>
        public float WaitShowBoomTime
        {
            get
            {
                return LockTweenTime + BulletMoveTime;
            }
        }

        public Vector3 LocalPosition
        {
            get { return transform.localPosition; }
        }

        public Transform RotateTrans
        {
            get { return RotateObj.transform; }
        }

        #endregion

        #region Local Data

        /// <summary>
        /// 飞机数据
        /// </summary>
        public PludoPlaneData PlaneData { get; private set; }
        /// <summary>
        /// 飞机集合(当前飞机为合体后的主要时，会新增成员，默认只有一个，用来控制飞机数量显示)
        /// </summary>
        private List<PludoPlane> _planeList=new List<PludoPlane>();

        /// <summary>
        /// 飞机列表
        /// </summary>
        public List<PludoPlane> PlaneList
        {
            get { return _planeList; }
        }

        #endregion

        #region Life Cycle

        new void Start()
        {
            //屏蔽掉YxView中无效的刷新，会影响本地数据的刷新问题
        }

        protected override void OnColorItemFresh()
        {
            PlaneData = Data as PludoPlaneData;
            if (PlaneData != null)
            {
                Reset();
                if (PlaneData.CheckPlaneState(EnumPlaneStatus.Finish))
                {
                    OnChangeFinishState(false);
                }
                else
                {
                    UiTypes[PlaneUiIndex] = ColorItemUiType.PlaneNormal;
                }
                transform.localPosition = PlaneData.CurPos;
                TweenRotate(PlaneData.DefTargetPos, ConstantData.IntValue);
                if (PlaneData.CheckPlaneState(EnumPlaneStatus.Fit))
                {
                    Hide();
                }
            }
            if (FlyFire)
            {
                FlyFire.transform.localPosition = ColorFireOffset[CurColorData.ItemColor];
            }
            base.OnColorItemFresh();
        }
        public override void SetColorItem(UISprite colorSprite, string spriteName)
        {
            base.SetColorItem(colorSprite, spriteName);
            colorSprite.depth += IdCode;
        }

        /// <summary>
        /// 飞机执行完毕
        /// </summary>
        public void OnChangeFinishState(bool fresh)
        {
            UiTypes[PlaneUiIndex]= ColorItemUiType.RoadStar;
            if (fresh)
            {
               SetColorItems(ColorSprites,UiTypes);
            }
            if (PlaneData!=null)
            {
                PlaneData.PlaneStateChange((int)EnumPlaneStatus.Finish);
            }
            FreshPlaneNum();
        }

        #endregion

        #region Function

        /// <summary>
        /// 飞机处于可选择列表
        /// </summary>
        public void OnPlaneInSelect(bool select)
        {
            if(PlaneData.CheckPlaneState(EnumPlaneStatus.Home))//处于基地时飞机不可选
            {
                select = false;
            }
            BoxCollider.enabled = select;
            if (select)
            {
                InSelectTween.ResetToBeginning();
                InSelectTween.PlayForward();
                YxDebug.LogError(string.Format("飞机{0}_{1}",(ItemColor)PlaneData.ItemColor,PlaneData.DataId));
            }
            else
            {
                transform.localScale = Vector3.one;
                InSelectTween.@from= Vector3.one;
                InSelectTween.to=Vector3.one*1.3f;
                InSelectTween.enabled = false;
            }
        }

        #region 飞机飞行
        /// <summary>
        /// 飞机进入飞行状态
        /// </summary>
        /// <param name="state"></param>
        private void PlaneInFlyState(bool state)
        {
            FlyFire.TrySetComponentValue(state);
        }

        /// <summary>
        /// 飞机移动协程
        /// </summary>
        private Coroutine _planeMovePathCor;

        /// <summary>
        /// 飞机移动+结束旋转
        /// </summary>
        public void PlaneMovePathWithRotate(MoveAndRotateData data,bool hideFireOnFinish = true,AsyncCallback rotateFinishCall=null)
        {
            PlaneMovePath(data.MovePath, hideFireOnFinish, delegate
            {
                TweenRotate(data.FinishRotate, BulletMoveTime/2, delegate
                {
                    PlaneInFlyState(false);
                    if (rotateFinishCall!=null)
                    {
                        rotateFinishCall(null);
                    }
                });
            });
        }

        /// <summary>
        /// 飞机移动多步
        /// </summary>
        /// <param name="path"></param>
        /// <param name="hideFire"></param>
        /// <param name="call"></param>
        public int PlaneMovePath(List<Vector3> path,bool hideFire = true, AsyncCallback call = null)
        {
            PlaneInFlyState(hideFire);
            if (_planeMovePathCor != null)
            {
                StopCoroutine(_planeMovePathCor);
            }
            _planeMovePathCor = StartCoroutine(OnPlaneMovePath(path, hideFire, call));
            return path.Count;
        }

        /// <summary>
        /// 飞机移动路径
        /// </summary>
        IEnumerator OnPlaneMovePath(List<Vector3> path, bool hideFireOnFinish=true,AsyncCallback call=null)
        {
            var count = path.Count;
            for (int i = 0; i < count; i++)
            {
                PlaneMoveOneStep(path[i]);
                yield return new WaitForSeconds(PlaneMoveTime);
            }
            if (hideFireOnFinish)
            {
                PlaneInFlyState(false);
            }
            if (call!=null)
            {
                call(null);
            }
        }
        /// <summary>
        /// 飞机移动协程
        /// </summary>
        private Coroutine _planeMoveStepCor;
        public void PlaneMoveOneStep(Vector3 target)
        {
            if (_planeMoveStepCor != null)
            {
                StopCoroutine(_planeMoveStepCor);
            }
            _planeMoveStepCor = StartCoroutine(OnPlaneMoveAndRotate(target));
        }

        /// <summary>
        /// 继续旋转
        /// </summary>
        IEnumerator OnPlaneMoveAndRotate(Vector3 targetPos)
        {
            TweenRotate(targetPos, PlaneMoveCellTime);
            TweenPosition.Begin(gameObject, PlaneMoveTime, targetPos);
            yield return new WaitForSeconds(PlaneMoveTime);
            if (PlaneData!=null)
            {
                PlaneData.SetVector(targetPos,targetPos);
            }
        }
        /// <summary>
        /// 旋转协程
        /// </summary>
        private Coroutine _tweenRoateCor;
        /// <summary>
        /// 旋转角度
        /// </summary>
        private void TweenRotate(Vector3 targetPos,float rotateTime, AsyncCallback finishCall=null)
        {
            if (_tweenRoateCor!=null)
            {
                StopCoroutine(_tweenRoateCor);
            }
            _tweenRoateCor=StartCoroutine(TweenRotateWithCall(targetPos,rotateTime,finishCall));
        }

        IEnumerator TweenRotateWithCall(Vector3 targetPos, float rotateTime, AsyncCallback finishCall = null)
        {
            var targetRotate = PludoGameData.GetQuaternion(transform.localPosition, targetPos);
            var tweenRotate = TweenRotation.Begin(RotateObj, rotateTime, targetRotate);
            tweenRotate.quaternionLerp = true;
            yield return new WaitForSeconds(rotateTime);
            if (finishCall!=null)
            {
                finishCall(null);
            }
        }



        #endregion

        #region 飞机攻击
        /// <summary>
        /// 攻击协程
        /// </summary>
        private Coroutine _attackCor;
        public float OnPlaneAttack(Vector3 targetPlaneVec)
        {
            TweenRotate(targetPlaneVec, LockTweenTime);
            if (_attackCor!=null)
            {
                StopCoroutine(_attackCor);
            }
            _attackCor = StartCoroutine(FireBullet(targetPlaneVec));
            return WaitShowBoomTime;
        }

        /// <summary>
        /// 发射子弹
        /// </summary>
        IEnumerator FireBullet(Vector3 targetPos)
        {
            yield return new WaitForSeconds(LockTweenTime);
            var view = BulletContainer.GetChildView(0, BulletPrefab);
            var bulletData = new PludoBulletData()
            {
                TargetPos = UICamera.mainCamera.transform.TransformPoint(targetPos),
                Time =BulletMoveTime
            };
            view.UpdateView(bulletData);
        }

        /// <summary>
        /// 飞机被锁定
        /// </summary>
        public void OnPlaneBeLock()
        {
            LockObj.TrySetComponentValue(true);
        }

        /// <summary>
        /// 被攻击协程
        /// </summary>
        private Coroutine _beAttackCor;

        /// <summary>
        /// 飞机被攻击
        /// </summary>
        public void OnPlaneBeAttack(List<MoveAndRotateData> moveData,bool widthLock)
        {
            if (gameObject.activeInHierarchy)
            {
                if (_beAttackCor != null)
                {
                    StopCoroutine(_beAttackCor);
                }
                _beAttackCor = StartCoroutine(PlaneBoom(moveData, widthLock));
            }
        }

        /// <summary>
        /// 飞机爆炸
        /// </summary>
        /// <returns></returns>
        IEnumerator PlaneBoom(List<MoveAndRotateData> moveData,bool widthLock)
        {
            yield return new WaitForSeconds(widthLock?WaitShowBoomTime:ConstantData.IntValue);
            LockObj.TrySetComponentValue(false);
            if (BoomAnimation)
            {
                BoomAnimation.enabled= true;
                BoomAnimation.Play();
            }
            yield return new WaitForSeconds(BoomAniTime);
            var planeList = PlaneList.ToList();
            for (int i = 0; i < moveData.Count; i++)
            {
                yield return new WaitForSeconds(PlaneMoveCellTime);
                var plane = planeList[i];
                plane.Show();
                plane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Home);
                plane.PlaneListReset();
                plane.FreshPlaneNum();
                plane.PlaneMovePathWithRotate(moveData[i],false);
            }

        }


        #endregion
        #region 飞机合体
        /// <summary>
        /// 合体协程
        /// </summary>
        private Coroutine _fitCoroutine;
        public void ShowPlaneFit(int moveDistance,PludoPlane fitPlane, AsyncCallback call = null)
        {
            if (_fitCoroutine!=null)
            {
                StopCoroutine(_fitCoroutine);
            }
            _fitCoroutine=StartCoroutine(ShowPlaneFit(GetWaitMoveTime(moveDistance),fitPlane,call));
        }

        IEnumerator ShowPlaneFit(float waitTime,PludoPlane fitPlane, AsyncCallback call = null)
        {
            yield return new WaitForSeconds(waitTime);
            var planeList = fitPlane.PlaneList.ToList();
            var count = planeList.Count;
            YxDebug.LogError(string.Format("移动飞机的子飞机有{0}个", count));
            for (int i = 0; i < count; i++)
            {
                var plane = planeList[i];
                plane.PlaneListReset();
                plane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Fit);
                plane.Hide();
                _planeList.Add(plane);
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnShowFitAction.WaitExcuteCalls());
            }
            FreshPlaneNum();
            yield return new WaitForSeconds(FitAniTime);
            if (call!=null)
            {
                call(null);
            }
        }
        /// <summary>
        /// 添加合体飞机
        /// </summary>
        /// <param name="fitPlanes"></param>
        public void AddFitPlanes(List<PludoPlane> fitPlanes)
        {
            _planeList.AddRange(fitPlanes);
            FreshPlaneNum();
        }

        #endregion

        #region 飞机完成
        /// <summary>
        /// 飞机完成协程
        /// </summary>
        private Coroutine _finishCor;
        public void OnPlaneFinish(List<Vector3> targets,Dictionary<int,List<Vector3>> finalMove, AsyncCallback call = null)
        {
            PlaneMovePath(targets);
            if (_finishCor!=null)
            {
                StopCoroutine(_finishCor);
            }
            _finishCor=StartCoroutine(ShowFinishAni(targets.Count, finalMove,call));
        }

        IEnumerator ShowFinishAni(int distance, Dictionary<int, List<Vector3>> finalMove, AsyncCallback call = null)
        {
            yield return new WaitForSeconds(GetWaitMoveTime(distance));
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnFinishAction.WaitExcuteCalls());
            }
            yield return new WaitForSeconds(FinishShowTime);
            Vector3 finishStartVec;
            if (PlaneData!=null)
            {
                finishStartVec = PlaneData.CurPos;
            }
            else
            {
                finishStartVec = transform.localPosition;
            }
            //主体飞机
            var beFitList = _planeList.ToList();
            //合体飞机列表处理
            var count = beFitList.Count;
            for (int i = 0; i < count; i++)
            {
                yield return new WaitForSeconds(PlaneMoveCellTime);
                var plane= beFitList[i];
                plane.gameObject.transform.localPosition = finishStartVec;
                plane.Show();
                plane.PlaneListReset();
                plane.OnChangeFinishState(true);
                plane.PlaneMovePath(finalMove[plane.IdCode],false);
            }
            var lastPlane = beFitList.Last();
            var lastWaitTime = GetWaitMoveTime(finalMove[lastPlane.IdCode].Count);
            yield return new WaitForSeconds(lastWaitTime);
            if (call!=null)
            {
                call(null);
            }
        }

        #endregion

        private float GetWaitMoveTime(int distance)
        {
            return distance * PlaneMoveTime + PlaneMoveCellTime;
        }

        /// <summary>
        /// 刷新飞机显示数量
        /// </summary>
        private void FreshPlaneNum()
        {
            PlaneNumSprite.TrySetComponentValue(string.Format(PlaneNumFormat, _planeList.Count));
        }

        private void Reset()
        {
            if (BoxCollider)
            {
                BoxCollider.enabled = false;
            }
            if (BoomAnimation)
            {
                BoomAnimation.enabled = false;
            }

            if (!IsShow())
            {
                Show();
            }
            LockObj.TrySetComponentValue(false);
            PlaneListReset();
            FreshPlaneNum();
            PlaneInFlyState(false);
        }

        public void PlaneListReset()
        {
            _planeList = new List<PludoPlane>();
            _planeList.Add(this);
        }

        #endregion
    }

    /// <summary>
    /// 飞机数据
    /// </summary>
    public sealed class PludoPlaneData:ColorItemData
    {
        /// <summary>
        /// Id
        /// </summary>
        private int _id;
        /// <summary>
        /// 状态
        /// </summary>
        private int _planeStatus;
        /// <summary>
        /// 当前位置
        /// </summary>
        private int _localPosition;

        /// <summary>
        /// 当前位置
        /// </summary>
        public Vector3 CurPos { private set; get; }

        /// <summary>
        /// 默认目标位置
        /// </summary>
        public Vector3 DefTargetPos { private set; get;}
        /// <summary>
        /// 移动路径
        /// </summary>
        public List<int> MovePath { private set; get;}

        public List<int> StartFitList { private set; get; }

        /// <summary>
        /// 飞机Id
        /// </summary>
        public int DataId
        {
            get { return _id;}
        }

        public int Status
        {
            get { return  _planeStatus; }
        }

        public int LocalPosition
        {
            get { return _localPosition;}
        }

        
        public PludoPlaneData(ISFSObject data,int color)
        {
            SetColor(color);
            ParseData(data);
        }

        protected override void ParseData(ISFSObject data)
        {
            base.ParseData(data);
            SfsHelper.Parse(data, RequestKey.KeyId, ref _id);
            SfsHelper.Parse(data, ConstantData.KeyPlaneStatus, ref _planeStatus);
            PlaneStateChange(_planeStatus);
            MovePath = new List<int>();
            StartFitList=new List<int>();
            if (data.ContainsKey(ConstantData.KeyMovePath))
            {
                var path = data.GetSFSArray(ConstantData.KeyMovePath);
                for (int i = 0; i < path.Count; i++)
                {
                    MovePath.Add(path.GetInt(i));
                }
            }

            if (data.ContainsKey(ConstantData.KeyFitList))
            {
                var fitList = data.GetSFSArray(ConstantData.KeyFitList);
                for (int i = 0; i < fitList.Count; i++)
                {
                    StartFitList.Add(fitList.GetInt(i));
                }
            }
            if (data.ContainsKey(ConstantData.KeyLocalPosition))
            {
                SfsHelper.Parse(data, ConstantData.KeyLocalPosition, ref _localPosition);
            }
        }

        /// <summary>
        /// 多参数构造，用于无服务器自定义初始化处理
        /// </summary>
        /// <param name="id">飞机Id</param>
        /// <param name="planeStatus">飞机状态</param>
        /// <param name="color"></param>
        /// <param name="local"></param>
        public PludoPlaneData(int id,int planeStatus,int color,int local=ConstantData.IntValue)
        {
            SetColor(color);
            _id = id;
            _localPosition = local;
            PlaneStateChange(planeStatus);
        }

        /// <summary>
        /// 飞机状态变化
        /// </summary>
        /// <param name="changeState"></param>
        public void PlaneStateChange(int changeState)
        {
            switch ((EnumPlaneStatus)changeState)
            {
                case EnumPlaneStatus.Home:
                case EnumPlaneStatus.Finish:
                    _localPosition = PludoGameData.GetReadyId(CurColor, _id);
                    break;
                case EnumPlaneStatus.Ready:
                    _localPosition = PludoGameData.GetReadyId(CurColor, ConstantData.ValueReadyPosIndex);
                    break;
            }
            _planeStatus= changeState;
        }
        /// <summary>
        /// 本地位置变化
        /// </summary>
        /// <param name="local"></param>
        public void PlaneLocalPosChange(int local)
        {
            _localPosition = local;
        }

        /// <summary>
        /// 设置数据位置信息
        /// </summary>
        /// <param name="curPos">当前位置</param>
        /// <param name="targetPos">目标位置</param>
        public void SetVector(Vector3 curPos,Vector3 targetPos)
        {
            CurPos = curPos;
            DefTargetPos = targetPos;
        }

        /// <summary>
        /// 重置飞机状态
        /// </summary>
        public void Reset()
        {
            _planeStatus = (int) EnumPlaneStatus.Home;
            _localPosition = PludoGameData.GetReadyId(CurColor, _id);
        }

        /// <summary>
        /// 检测飞机状态
        /// </summary>
        /// <param name="checkState"></param>
        public bool CheckPlaneState(EnumPlaneStatus checkState)
        {
            return (_planeStatus &(int)checkState) == (int)checkState;
        }

        public bool CheckPlaneInMulState(int state)
        {
            return (_planeStatus & state) == state;
        }
    }

    /// <summary>
    /// 飞机状态
    /// </summary>
    public enum EnumPlaneStatus
    {
        Load=1,                              //落地(飞机执行动作之后默认状态)
        Fly=2,                               //飞行(飞机可以移动)
        Attack=4,                            //攻击
        BeFit =8,                            //被合体
        BeAttack =16,                        //被攻击
        Collide=32,                          //撞机状态
        Ready =1024,                         //准备起飞
        Finish =2048,                        //结束
        Fit =8192,                           //合体
        Home = 16384,                        //在基地
    }

}
