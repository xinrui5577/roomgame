using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     PludoControl.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-27
 *描述:        	飞行棋控制类。包括对应玩家飞机移动相关
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoControl : PludoFreshView 
    {
        #region UI Param
        [Tooltip("飞机准备区")]
        public List<PludoMapItem> ReadyArea= new List<PludoMapItem>();
        [Tooltip("飞机安全区")]
        public List<PludoMapItem> SafeArea= new List<PludoMapItem>();
        [Tooltip("飞机容器")]
        public Transform PlaneContainer;
        [Tooltip("飞机预设")]
        public PludoPlane PlanePrefab;

        #endregion

        #region Data Param
        [Tooltip("飞机完成竖直方向偏移")]
        public Vector3 PlaneFinishOffset = Vector3.up*100;
        /// <summary>
        /// 颜色
        /// </summary>
        public int CurColor { get;private set; }

        /// <summary>
        /// 座位号
        /// </summary>
        public int Seat
        {
            get
            {
                if(_curPlayerInfo!=null)
                return _curPlayerInfo.Seat;
                else
                {
                    return ConstantData.IntDefValue;
                }
            }
        }

        #endregion

        #region Local Data
        /// <summary>
        /// 当前玩家飞机 key id,value 飞机对象
        /// </summary>
        public Dictionary<int,PludoPlane> PlanesDic;
        /// <summary>
        /// 公用地图区   key 地图id,value 地图对象
        /// </summary>
        private Dictionary<int, PludoMapItem> _commomMapDic=new Dictionary<int, PludoMapItem>();
        /// <summary>
        /// 当前玩家信息
        /// </summary>
        private PludoPlayerInfo _curPlayerInfo;

        /// <summary>
        /// 飞机准备区位置偏移,根据进入顺序决定(目前版本未使用)
        /// </summary>
        public List<Vector3> ReadyPosOffset = new List<Vector3>();
        /// <summary>
        /// 当前控制者公用区起始点Id
        /// </summary>
        private int _commonStartId;
        #endregion

        #region Life Cycle

        /// <summary>
        /// 设置当前控制脚本颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(int color)
        {
            CurColor = color;
        }

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curPlayerInfo = Data as PludoPlayerInfo;
            if (_curPlayerInfo != null)
            {
                ShowPlanes();
                CallBack(this);
            }
        }



        #endregion

        #region Function
        /// <summary>
        /// 初始化控制数据
        /// </summary>
        void InitControlData()
        {
            Reset();
        }

        /// <summary>
        /// 显示飞机
        /// </summary>
        void ShowPlanes()
        {
            var dic = _curPlayerInfo.PlaneDic;
            PlanesDic = new Dictionary<int, PludoPlane>();
            var listBetFitPlanes=new List<KeyValuePair<PludoPlane,PludoPlaneData>>();
            foreach (var item in dic)
            {
                var planeInfo = item.Value;
                var id = planeInfo.DataId;
                GetMapPosByPlaneData(planeInfo,false);
                var view = PlaneContainer.GetChildView(id, PlanePrefab).GetComponent<PludoPlane>();
                PlanesDic.Add(id, view);
                view.IdCode = id;
                if (planeInfo.CheckPlaneState(EnumPlaneStatus.BeFit))
                {
                    listBetFitPlanes.Add(new KeyValuePair<PludoPlane, PludoPlaneData>(view, planeInfo));
                }
                view.UpdateView(planeInfo);
            }

            var beFitCount = listBetFitPlanes.Count;
            for (int i = 0; i < beFitCount; i++)
            {
                var view = listBetFitPlanes[i].Key;
                var data = listBetFitPlanes[i].Value;
                var fitList = data.StartFitList;
                var fitCount = fitList.Count;
                var addViews=new List<PludoPlane>();
                for (int j = 0; j < fitCount; j++)
                {
                    var findId = fitList[j];
                    if (PlanesDic.ContainsKey(findId)&&findId!= data.DataId)
                    {
                        addViews.Add(PlanesDic[findId]);
                    }
                }
                view.AddFitPlanes(addViews);
            }
        }

        /// <summary>
        /// 初始化公用地图
        /// </summary>
        /// <param name="maps"></param>
        /// <param name="startId">起始点Id</param>
        public void InitCommonMap(List<PludoMapItem> maps,int startId)
        {
            InitControlData();
            int count = maps.Count;
            _commomMapDic=new Dictionary<int, PludoMapItem>();
            for (int i = 0; i < count; i++)
            {
                var item = maps[i];
                _commomMapDic.Add(item.DataId,item);
            }
            _commonStartId = startId;
        }

        /// <summary>
        /// 飞机处于可选择状态
        /// </summary>
        public void OnPlaneCanChoose(bool select)
        {
            var planeIds = _curPlayerInfo.RollDiceData.PlaneIds;
            if(planeIds.Count<=ConstantData.IntValue)
            {
                return;
            }
            var count = planeIds.Count;
            for (int i = 0; i < count; i++)
            {
                GetPlaneById(planeIds[i]).OnPlaneInSelect(select);
            }
        }

        /// <summary>
        /// 根据飞机Id 获取飞机对象
        /// </summary>
        /// <param name="planeId"></param>
        /// <returns></returns>
        public PludoPlane GetPlaneById(int planeId)
        {
            if (PlanesDic.ContainsKey(planeId))
            {
                return PlanesDic[planeId];
            }
            else
            {
                Debug.LogError(string.Format("There is not exist such plane,what plane id is {0}",planeId));
                return new PludoPlane();
            }
        }
        /// <summary>
        /// 获得移动路径
        /// </summary>
        /// <param name="movePath">移动路径</param>
        /// <returns></returns>
        public List<Vector3> GetMovePath(List<int> movePath)
        {
            return movePath.Select(path => GetMapItemPos(path)).ToList();
        }

        /// <summary>
        /// 根据飞机数据获取地图位置
        /// </summary>
        /// <param name="planeData">飞机数据</param>
        /// <param name="checkOnly">是否只是检测</param>
        public void GetMapPosByPlaneData(PludoPlaneData planeData,bool checkOnly=true)
        {
            var planeId = planeData.DataId;
            int curPosId;
            int tarGetPosId;
            switch ((EnumPlaneStatus)planeData.Status)
            {
                case EnumPlaneStatus.Finish:
                case EnumPlaneStatus.Home:
                    curPosId = PludoGameData.GetReadyId(planeData.ItemColor, planeId);
                    if (planeId==ConstantData.ValueLastPlaneId)
                    {
                        tarGetPosId = PludoGameData.GetReadyId(planeData.ItemColor, ConstantData.IntValue);
                    }
                    else
                    {
                        tarGetPosId = PludoGameData.GetReadyId(planeData.ItemColor, ConstantData.ValueReadyPosIndex);
                    }
                    break;
                case EnumPlaneStatus.Ready:
                    curPosId =PludoGameData.GetReadyId(planeData.ItemColor, ConstantData.ValueReadyPosIndex);
                    tarGetPosId = _commonStartId;
                    break;
                default:
                    curPosId = planeData.LocalPosition;
                    if (planeData.LocalPosition < ConstantData.ValueReadyMapItemColorBase)
                    {
                        var findItem = _commomMapDic[planeData.LocalPosition];
                        if (findItem.EnumMapType == EnumMapItemType.End && findItem.Color == planeData.ItemColor)
                        {
                            tarGetPosId = SafeArea[ConstantData.IntValue].DataId;
                        }
                        else
                        {
                            tarGetPosId = (planeData.LocalPosition + 1)%PludoGameData.MapNormalLenth;
                        }
                    }
                    else
                    {
                        var index = SafeArea.FindIndex(item => item.DataId == curPosId);
                        if (index > ConstantData.IntDefValue)
                        {
                            tarGetPosId = SafeArea[index+1].DataId;
                        }
                        else
                        {
                            tarGetPosId = ConstantData.IntDefValue;
                            Debug.LogError("could not find such item in safeArea,local is:"+ curPosId);
                        }
                    }
                    break;
            }
            Vector3 curPos =GetMapItemPos(curPosId);
            Vector3 defTargetPos = GetMapItemPos(tarGetPosId);
            planeData.SetVector(curPos,defTargetPos);
        }

        /// <summary>
        /// 获取对应Id的地图位置，用于飞机移动
        /// </summary>
        /// <param name="mapId">地图位置Id</param>
        /// <returns></returns>
        private Vector3 GetMapItemPos(int mapId)
        {
            if (_commomMapDic.ContainsKey(mapId))
            {
                return _commomMapDic[mapId].MapPos;
            }
            var safeIndex = SafeArea.FindIndex(item => item.DataId == mapId);
            if (safeIndex > ConstantData.IntDefValue)
            {
                return SafeArea[safeIndex].MapPos;
            }
            var readyIndex = ReadyArea.FindIndex(item => item.DataId == mapId);
            if (readyIndex > ConstantData.IntDefValue)
            {
                if(readyIndex== ReadyArea.Count-1)
                {
                    return ReadyArea.Last().MapPos;
                }
                return ReadyArea[readyIndex].MapPos;
            }
            Debug.LogError(string.Format(" Cound not find map id {0},please check again!!", mapId));
            return Vector3.zero;
        }

        #region 飞机交互操作事件

        /// <summary>
        /// 飞机准备
        /// </summary>
        /// <param name="newPlaneData"></param>
        public void OnPlaneReady(PludoPlaneData newPlaneData)
        {
            var readyPlane = GetPlaneById(newPlaneData.DataId);
            var moveList=new List<Vector3>();
            newPlaneData.PlaneStateChange((int)EnumPlaneStatus.Ready);
            GetMapPosByPlaneData(newPlaneData,false);
            moveList.Add(newPlaneData.CurPos);
            ConstantData.PlaySoundBySex(_curPlayerInfo.SexI,ConstantData.KeyStarFly);
            MoveAndRotateData moveRoateData=new MoveAndRotateData()
            {
                MovePath = moveList.ToList(),
                FinishRotate = newPlaneData.DefTargetPos
            };
            readyPlane.PlaneMovePathWithRotate(moveRoateData,true, delegate
            {
                readyPlane.PlaneListReset();
                readyPlane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Ready);
            });
        }

        /// <summary>
        /// 飞机移动（常规）
        /// </summary>
        public int OnPlaneMove(PludoPlaneData newPlaneData, AsyncCallback call = null)
        {
            var movePlane = GetPlaneById(newPlaneData.DataId);
            return PlaneMove(movePlane, newPlaneData, delegate
            {
               movePlane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Load);
               movePlane.PlaneData.PlaneLocalPosChange(newPlaneData.MovePath.Last());
               GetMapPosByPlaneData(movePlane.PlaneData,false);
                if (call!=null)
                {
                    call(null);
                }
            });
        }



        private int PlaneMove(PludoPlane movePlane,PludoPlaneData newPlaneData, AsyncCallback call = null)
        {
            return movePlane.PlaneMovePath(GetMovePath(newPlaneData.MovePath),true,call);
        }

        /// <summary>
        /// 飞机合体
        /// </summary>
        public void OnPlaneFit(PludoPlaneData newPlaneData, int betFitId,bool fitWithMove=true)
        {
            var fitPlane = GetPlaneById(newPlaneData.DataId);
            YxDebug.LogError("当前操作的飞机存在子飞机数量是："+ fitPlane.PlaneList.Count);
            var beFitPlane = GetPlaneById(betFitId);
            beFitPlane.ShowPlaneFit(
                fitWithMove?PlaneMove(fitPlane, newPlaneData):ConstantData.IntValue,fitPlane, delegate
                {
                    beFitPlane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.BeFit); 
                });
        }
        /// <summary>
        /// 飞机攻击
        /// </summary>
        public float OnPlaneAttack(int attackPlanId,Vector3 beAttackPos)
        {
            var attackPlane = GetPlaneById(attackPlanId);
            return attackPlane.OnPlaneAttack(beAttackPos);
        }

        /// <summary>
        /// 飞机被攻击(包括撞机)
        /// </summary>
        /// <param name="planeId">飞机Id</param>
        /// <param name="widthLock"></param>
        /// <returns>被攻击飞机位置</returns>
        public Vector3 OnPlaneBeAttack(int planeId,bool widthLock=true)
        {
            var beAttackPlane = GetPlaneById(planeId);
            if (widthLock)
            {
                beAttackPlane.OnPlaneBeLock();
            }
            var planeCount = beAttackPlane.PlaneList.Count;
            var planeList = beAttackPlane.PlaneList.ToList();
            var planeMoveList = new List<MoveAndRotateData>();
            for (int i = 0; i < planeCount; i++)
            {
                var plane = planeList[i];
                plane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Home);
                GetMapPosByPlaneData(plane.PlaneData, false);
                MoveAndRotateData moveRotaData=new MoveAndRotateData();
                moveRotaData.MovePath=new List<Vector3>();
                moveRotaData.MovePath.Add(plane.PlaneData.CurPos);
                moveRotaData.FinishRotate = plane.PlaneData.DefTargetPos;
                planeMoveList.Add(moveRotaData);
            }
            if (widthLock)
            {
                YxDebug.LogError((ItemColor)CurColor+"被攻击飞机数量是：" + planeMoveList.Count);
            }
            else
            {
                YxDebug.LogError((ItemColor)CurColor + "被撞击飞机数量是：" + planeMoveList.Count);
            }
            beAttackPlane.OnPlaneBeAttack(planeMoveList,widthLock);
            return beAttackPlane.LocalPosition;
        }

        private Coroutine _boomCor;
        /// <summary>
        /// 等待爆炸时间并移动
        /// </summary>
        public void WaitBoomAndMove(PludoPlaneData newPlaneData,AsyncCallback moveFinishCall=null)
        {
            var movePlane = GetPlaneById(newPlaneData.DataId);
            if (_boomCor!=null)
            {
                StopCoroutine(_boomCor);
            }
            _boomCor=StartCoroutine(WaitBoomMove(movePlane.BoomAniTime, newPlaneData,moveFinishCall));
           
        }
        /// <summary>
        /// 等待爆炸并移动
        /// </summary>
        /// <param name="boomTime">爆炸时间</param>
        /// <param name="newPlaneData">新飞机数据</param>
        /// <returns></returns>
        private IEnumerator WaitBoomMove(float boomTime, PludoPlaneData newPlaneData, AsyncCallback moveFinishCall)
        {
            yield return new WaitForSeconds(boomTime);
            OnPlaneMove(newPlaneData,moveFinishCall);
        }



        /// <summary>
        /// 飞机完成
        /// </summary>
        /// <param name="newPlaneData"></param>
        public void OnPlaneFinish(PludoPlaneData newPlaneData)
        {
            var finishPlane = GetPlaneById(newPlaneData.DataId);
            var finishDic = new Dictionary<int, List<Vector3>>();
            var planeList = finishPlane.PlaneList;
            var count = planeList.Count;
            var moveList= GetMovePath(newPlaneData.MovePath);
            Vector3 finishShowPos;
            if (moveList.Count>ConstantData.IntValue)
            {
                finishShowPos= moveList.Last() + PlaneFinishOffset;
            }
            else
            {
                finishShowPos =PlaneFinishOffset;
            }
            for (int i = 0; i < count; i++)
            {
                var finishList=new List<Vector3>();
                finishList.Add(finishShowPos);
                var plane = planeList[i];
                plane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Finish);
                GetMapPosByPlaneData(plane.PlaneData);
                finishList.Add(plane.PlaneData.CurPos);
                finishDic.Add(plane.IdCode, finishList);
                YxDebug.LogError(string.Format("飞机{0}_{1}最终的位置是{2}", (ItemColor)plane.Color, plane.IdCode, plane.PlaneData.CurPos));
            }
            finishPlane.OnPlaneFinish(moveList, finishDic, delegate
            {
                for (int i = 0; i < count; i++)
                {
                    var plane = planeList[i];
                    plane.PlaneData.PlaneStateChange((int)EnumPlaneStatus.Finish);
                }
                Facade.Instance<MusicManager>().Play(ConstantData.KeyPlaneFinish);
                Facade.EventCenter.DispatchEvent(LoaclRequest.FreshStartNum, ConstantData.IntDefValue);
            });
        }

        /// <summary>
        /// 获取对应飞机与对应位置的距离
        /// </summary>
        /// <param name="planeId"></param>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public int GetDistance(int planeId,int mapId)
        {
            var planePos = GetPlaneById(planeId).PlaneData.LocalPosition;
            if (planePos>ConstantData.ValueReadyMapItemBase)
            {
                planePos = 0;
            }
            return (mapId - planePos+PludoGameData.MapNormalLenth)% PludoGameData.MapNormalLenth;
        }

        #endregion


        /// <summary>
        /// 控制类重置
        /// </summary>
        private void Reset()
        {
            _commonStartId = ConstantData.IntDefValue;
        }

        #endregion
    }

    /// <summary>
    /// 移动与旋转数据
    /// </summary>
    public struct MoveAndRotateData
    {
        /// <summary>
        /// 移动路径
        /// </summary>
        public List<Vector3> MovePath;
        /// <summary>
        /// 结束旋转角度
        /// </summary>
        public Vector3 FinishRotate;
    }
}
