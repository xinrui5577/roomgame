using System.Collections; 
using Assets.Scripts.Common.Components;
using UnityEngine;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 房间视图
    /// </summary>
    public class RoomListItemState : NguiListItem
    { 
        /// <summary>
        /// 切换时间
        /// </summary>
        public float ChangeTime = 0.2f;
        /// <summary>
        /// 正常状态
        /// </summary>
        public GameObject NormalState;
        /// <summary>
        /// 变换房间状态
        /// </summary>
        public GameObject ChangeState; 
        public GameObject ActionState;
        protected override void OnAwake()
        {
            OnChangeState(false);
            UpAnchor();
        } 

        /// <summary>
        /// 切换房间
        /// </summary>
        public void StartChangeRoom()
        {
            StartCoroutine(OnChangeRoom());
        }

        private IEnumerator OnChangeRoom()
        {
            OnChangeState(true); //todo 后期实现动画播放
            yield return new WaitForSeconds(ChangeTime);
            OnChangeState(false); 
        }

        public override void AwakAction(bool isAction)
        {
            if (ActionState == null) return;
            ActionState.SetActive(isAction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isChange"></param>
        private void OnChangeState(bool isChange)
        {
            if (ChangeState != null) ChangeState.SetActive(isChange);
            if (NormalState != null) NormalState.SetActive(!isChange);
//            RoomItem.InfoPanel.SetActive(!isChange);
        } 
    }
}
