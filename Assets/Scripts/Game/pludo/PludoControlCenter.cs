using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.pludo.View;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     PludoControlCenter.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	飞行棋地图管理类
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class PludoControlCenter : PludoFreshView 
    {
        #region UI Param
        [Tooltip("常规地图，外围环绕一周的地图")]
        public List<PludoMapItem> RoundMap=new List<PludoMapItem>();
        [Tooltip("地图初始化事件")]
        public List<EventDelegate> MapInitAction = new List<EventDelegate>();
        [Tooltip("控制列表")]
        public List<PludoControl> ControlList=new List<PludoControl>();
        #endregion

        #region Data Param
        /// <summary>
        /// 安全区 Key 颜色 ;Value 安全区数据
        /// </summary>
        public Dictionary<int, List<PludoMapItem>> SafeArea=new Dictionary<int, List<PludoMapItem>>();
        /// <summary>
        /// 准备区 Key 颜色 ;Value 准备区数据
        /// </summary>
        public Dictionary<int, List<PludoMapItem>> ReadyArea = new Dictionary<int, List<PludoMapItem>>();
        #endregion

        #region Local Data
        /// <summary>
        /// 地图信息
        /// </summary>
        private PludoMapInfo _mapInfo;

        protected override void OnStart()
        {
            base.OnStart();
            Facade.EventCenter.AddEventListeners<LoaclRequest, PludoMapInfo>(LoaclRequest.MapInit,OnMapInit);
            Facade.EventCenter.AddEventListeners<LoaclRequest, ChoosePlaneResult>(LoaclRequest.ChoosePlaneResult,OnPlaneStateChange);
        }

        public override void OnDestroy()
        {
            Facade.EventCenter.RemoveEventListener<LoaclRequest, PludoMapInfo>(LoaclRequest.MapInit, OnMapInit);
            Facade.EventCenter.RemoveEventListener<LoaclRequest, ChoosePlaneResult>(LoaclRequest.ChoosePlaneResult, OnPlaneStateChange);
            base.OnDestroy();
        }

        #endregion

        #region Life Cycle

        #endregion

        #region Function

        #region MapArea
        /// <summary>
        /// 根据当前颜色，对准备区，公用区与安全区进行初始化
        /// </summary>
        private void RelateControls()
        {
            var count = Enum.GetValues(typeof (ItemColor)).Length;
            SafeArea = new Dictionary<int, List<PludoMapItem>>();
            ReadyArea = new Dictionary<int, List<PludoMapItem>>();
            for (int i = 0; i < count; i++)
            {
                var index = GetIndexWidthCurColor(i);
                var control = ControlList[i];
                control.SetColor(index);
                SafeArea.Add(index, control.SafeArea);
                ReadyArea.Add(index, control.ReadyArea);
            }
        }

        /// <summary>
        /// 玩家关联control
        /// </summary>
        /// <param name="color"></param>
        public PludoControl RelatePlayer(int color)
        {
            return ControlList.Find(ctrl => ctrl.CurColor == color);
        }

        /// <summary>
        /// 地图初始化
        /// </summary>
        /// <param name="mapInfo">地图信息</param>
        private void OnMapInit(PludoMapInfo mapInfo)
        {
            _mapInfo = mapInfo;
            InitRoundMap();
            SetCommonPos();
        }

        /// <summary>
        /// 初始化地图信息
        /// </summary>
        private void InitRoundMap()
        {
            RelateControls();
            //准备区
            FreshMapViews(_mapInfo.ReadyArea, ReadyArea);
            //环绕区（常规）
            FreshMapViews(_mapInfo.CommomArea, RoundMap);
            //安全区
            FreshMapViews(_mapInfo.SafeArea, SafeArea);
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(MapInitAction.WaitExcuteCalls());
            }
        }

        /// <summary>
        /// 获取地图公用位置
        /// </summary>
        private void SetCommonPos()
        {
            for (int i = 0; i < ControlList.Count; i++)
            {
                var index = GetIndexWidthCurColor(i);
                ControlList[i].InitCommonMap(RoundMap, _mapInfo.StartDataId[index]);
            }
        }

        /// <summary>
        /// 刷新地图数据
        /// </summary>
        /// <param name="mapDataDic">数据字典</param>
        /// <param name="mapViewDic">视图字典</param>
        private void FreshMapViews(Dictionary<int, List<PludoMapItemData>> mapDataDic, Dictionary<int, List<PludoMapItem>> mapViewDic)
        {
            foreach (var dataItem in mapDataDic)
            {
                var key = dataItem.Key;
                FreshMapViews(dataItem.Value, mapViewDic[key]);
            }
        }

        /// <summary>
        /// 刷新地图数据
        /// </summary>
        /// <param name="dataList">数据列表</param>
        /// <param name="viewList">视图列表</param>
        private void FreshMapViews(List<PludoMapItemData> dataList, List<PludoMapItem> viewList)
        {
            var count = Mathf.Min(dataList.Count, viewList.Count);
            for (int i = 0; i < count; i++)
            {
                var view = viewList[i];
                if (view)
                {
                    view.UpdateView(dataList[i]);
                }
            }
        }
        #endregion
        #region StateChange
        /// <summary>
        /// 飞机状态变化
        /// </summary>
        /// <param name="choosePlaneData"></param>
        private void OnPlaneStateChange(ChoosePlaneResult choosePlaneData)
        {
            var curOpSeat = choosePlaneData.OperationSeat;
            var curOpPlaneId = choosePlaneData.OperationPlaneId;
            var curControl = ControlList.Find(item => item.Seat == curOpSeat);
            if (curControl)
            {
                var curPlayerPlanes = choosePlaneData.PlaneDataDic[curOpSeat];
                var newPlaneData = curPlayerPlanes[curOpPlaneId];
                if (newPlaneData.CheckPlaneInMulState((int)EnumPlaneStatus.Collide|(int)EnumPlaneStatus.Attack))
                {
                    OnAttackState(choosePlaneData.PlaneDataDic, curControl, newPlaneData, delegate
                    {
                        CollidePlanes(choosePlaneData.PlaneDataDic);
                    });
                }
                else if (newPlaneData.CheckPlaneInMulState((int)EnumPlaneStatus.Attack | (int)EnumPlaneStatus.Fit))
                {
                    OnAttackState(choosePlaneData.PlaneDataDic, curControl, newPlaneData, delegate
                    {
                        OnFitState(curPlayerPlanes, curControl, newPlaneData,false);
                    });
                }
                else
                {
                    if (newPlaneData.CheckPlaneState(EnumPlaneStatus.Finish))
                    {
                        curControl.OnPlaneFinish(newPlaneData);
                        return;
                    }
                    if (newPlaneData.CheckPlaneState(EnumPlaneStatus.Collide))
                    {
                        OnCollideState(choosePlaneData.PlaneDataDic, curControl, newPlaneData);
                        return;
                    }
                    if (newPlaneData.CheckPlaneState(EnumPlaneStatus.Attack))
                    {
                        OnAttackState(choosePlaneData.PlaneDataDic, curControl, newPlaneData);
                        return;
                    }
                    if (newPlaneData.CheckPlaneState(EnumPlaneStatus.Fit))
                    {
                        OnFitState(curPlayerPlanes, curControl, newPlaneData);
                        return;
                    }
                    if (newPlaneData.CheckPlaneState(EnumPlaneStatus.Fly))
                    {
                        curControl.OnPlaneMove(newPlaneData);
                        return;
                    }
                    if (newPlaneData.CheckPlaneState(EnumPlaneStatus.Ready))
                    {
                        curControl.OnPlaneReady(newPlaneData);
                    }
                }
            }
        }

        /// <summary>
        /// 合体阶段
        /// </summary>
        /// <param name="curPlayerPlanes"/>合体玩家变化的飞机数据/param>
        /// <param name="curControl">合体玩家控制器</param>
        /// <param name="newPlaneData">合体玩家新飞机数据</param>
        /// <param name="fitWithMove">执行合体动作时是否需要显示移动</param>
        private void OnFitState(Dictionary<int,PludoPlaneData> curPlayerPlanes,PludoControl curControl,PludoPlaneData newPlaneData, bool fitWithMove = true)
        {
            var beFitPlaneId = ConstantData.IntDefValue;
            foreach (var planePair in curPlayerPlanes)
            {
                var checkPlaneData = planePair.Value;
                if (checkPlaneData.CheckPlaneState(EnumPlaneStatus.BeFit) && checkPlaneData.DataId != newPlaneData.DataId)
                {
                    beFitPlaneId = planePair.Key;
                }
            }
            if (beFitPlaneId == ConstantData.IntDefValue)
            {
                Debug.LogError("未找到被合体飞机数据，请检查");
                return;
            }
            curControl.OnPlaneFit(newPlaneData, beFitPlaneId, fitWithMove);
        }
        /// <summary>
        /// 攻击协程
        /// </summary>
        private Coroutine _attckCor;

        /// <summary>
        /// 攻击阶段
        /// </summary>
        /// <param name="planeDataDic">变化飞机数据</param>
        /// <param name="attackControl">攻击控制类</param>
        /// <param name="newPlaneData">攻击飞机变化数据</param>
        /// <param name="moveFinishCall">攻击完成回调</param>
        private void OnAttackState(Dictionary<int, Dictionary<int, PludoPlaneData>> planeDataDic,PludoControl attackControl, PludoPlaneData newPlaneData, AsyncCallback moveFinishCall=null
        )
        {
            if (_attckCor!=null)
            {
                StopCoroutine(_attckCor);
            }
            _attckCor = StartCoroutine(OnAttack(planeDataDic, attackControl, newPlaneData, moveFinishCall));
        }

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="planeDataDic"></param>
        /// <param name="attackControl"></param>
        /// <param name="newPlaneData"></param>
        /// <param name="moveFinishCall"></param>
        /// <returns></returns>
        IEnumerator OnAttack(Dictionary<int, Dictionary<int, PludoPlaneData>> planeDataDic, PludoControl attackControl, PludoPlaneData newPlaneData,AsyncCallback moveFinishCall)
        {
            var attackPlanId = newPlaneData.DataId;
            var beAttackInfos=new List<BeAttackInfo>();
            foreach (var planesItem in planeDataDic)
            {
                var beAttackList = new List<int>();
                var key = planesItem.Key;
                var planesDic = planesItem.Value;
                var control = ControlList.Find(item => item.Seat == key);
                beAttackList.AddRange(from planeDicItem in planesDic select planeDicItem.Value into planeData where planeData.CheckPlaneState(EnumPlaneStatus.BeAttack) select planeData.DataId);
                foreach (var beAttackId in beAttackList)
                {
                    var mapPos = control.PlanesDic[beAttackId].PlaneData.LocalPosition;
                    var distance= attackControl.GetDistance(attackPlanId, mapPos);
                    var info = new BeAttackInfo(control,beAttackId,distance);
                    beAttackInfos.Add(info);
                }
            }

            Comparison<BeAttackInfo> compare = (x, y) => { return x.Distance.CompareTo(y.Distance); };
            beAttackInfos.Sort(compare);
            foreach (var beAttackInfo in beAttackInfos)
            {
                YxDebug.LogError(string.Format("ID:{0}；Distance:{1}", beAttackInfo.PlaneId, beAttackInfo.Distance));
                yield return new WaitForSeconds(attackControl.OnPlaneAttack(attackPlanId, beAttackInfo.Control.OnPlaneBeAttack(beAttackInfo.PlaneId)));
                Facade.Instance<MusicManager>().Play(ConstantData.KeyPlaneBoom);
            }
            attackControl.WaitBoomAndMove(newPlaneData,moveFinishCall);
        }

        /// <summary>
        /// 撞机协程
        /// </summary>
        private Coroutine _collideCor;
        /// <summary>
        /// 攻击阶段
        /// </summary>
        /// <param name="planeDataDic">变化飞机数据</param>
        /// <param name="attackControl">撞机飞机控制类</param>
        /// <param name="newPlaneData">主动撞机飞机变化数据</param>
        private void OnCollideState(Dictionary<int, Dictionary<int, PludoPlaneData>> planeDataDic, PludoControl attackControl, PludoPlaneData newPlaneData)
        {
            attackControl.OnPlaneMove(newPlaneData, delegate
            {
                CollidePlanes(planeDataDic);
            });
        }

        /// <summary>
        /// 飞机撞机
        /// </summary>
        /// <param name="planeDataDic"></param>
        private void CollidePlanes(Dictionary<int, Dictionary<int, PludoPlaneData>> planeDataDic)
        {
            if (_collideCor != null)
            {
                StopCoroutine(_collideCor);
            }
            _collideCor = StartCoroutine(OnCollidePlane(planeDataDic));
        }

        /// <summary>
        /// 撞机
        /// </summary>
        /// <param name="planeDataDic"></param>
        /// <returns></returns>
        IEnumerator OnCollidePlane(Dictionary<int, Dictionary<int, PludoPlaneData>> planeDataDic)
        {
            Facade.Instance<MusicManager>().Play(ConstantData.KeyPlaneBoom);
            foreach (var planesItem in planeDataDic)
            {
                var beAttackList = new List<int>();
                var key = planesItem.Key;
                var planesDic = planesItem.Value;
                var control = ControlList.Find(item => item.Seat == key);
                beAttackList.AddRange(from planeDicItem in planesDic select planeDicItem.Value into planeData where planeData.CheckPlaneState(EnumPlaneStatus.Collide) select planeData.DataId);
                foreach (var beAttackId in beAttackList)
                {
                    control.OnPlaneBeAttack(beAttackId,false);
                }
            }
            yield return new WaitForSeconds(ConstantData.IntValue);
        }

        private int GetIndexWidthCurColor(int index)
        {
            return (_mapInfo.CurColor + index)% Enum.GetValues(typeof(ItemColor)).Length;
        }

        #endregion

        #endregion
    }

    /// <summary>
    /// 地图信息
    /// </summary>
    public class PludoMapInfo
    {
        /// <summary>
        /// 准备区
        /// </summary>
        public Dictionary<int,List<PludoMapItemData>> ReadyArea=new Dictionary<int, List<PludoMapItemData>>();
        /// <summary>
        /// 安全区
        /// </summary>
        public Dictionary<int, List<PludoMapItemData>> SafeArea = new Dictionary<int, List<PludoMapItemData>>();
        /// <summary>
        /// 起始点Id
        /// </summary>
        public Dictionary<int,int> StartDataId=new Dictionary<int, int>(); 
        /// <summary>
        /// 公用区
        /// </summary>
        public List<PludoMapItemData> CommomArea = new List<PludoMapItemData>();
        /// <summary>
        /// 地图起始颜色
        /// </summary>
        public int CurColor;
    }

    /// <summary>
    /// 选择飞机结果数据
    /// </summary>
    public class ChoosePlaneResult
    {
        /// <summary>
        /// 飞机信息
        /// </summary>
        public Dictionary<int,Dictionary<int,PludoPlaneData>> PlaneDataDic=new Dictionary<int, Dictionary<int, PludoPlaneData>>();
        /// <summary>
        /// 操作玩家座位号
        /// </summary>
        public int OperationSeat;
        /// <summary>
        /// 操作飞机Id
        /// </summary>
        public int OperationPlaneId;
    }
    /// <summary>
    /// 被攻击飞机信息
    /// </summary>
    public class BeAttackInfo
    {
        /// <summary>
        /// 飞机ID
        /// </summary>
        public int PlaneId;
        /// <summary>
        /// 飞机控制器
        /// </summary>
        public PludoControl Control;
        /// <summary>
        /// 距离当前玩家距离
        /// </summary>
        public int Distance;

        public BeAttackInfo(PludoControl control,int id,int distance)
        {
            Control = control;
            PlaneId = id;
            Distance = distance;
        }
    }
}
