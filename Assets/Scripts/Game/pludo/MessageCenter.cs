using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

/*===================================================
 *文件名称:     MessageCenter.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-04
 *描述:         消息中心，用于处理消息队列(与服务器交互信息)
 *              
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo
{
    public class MessageCenter
    {
        #region Data Param
        /// <summary>
        /// 消息最大长度
        /// </summary>
        private static int _messageMaxLenth=10;
        /// <summary>
        /// 消息队列
        /// </summary>
        private static Queue<MessageData> _messageQueue = new Queue<MessageData>();
        /// <summary>
        /// 当前执行消息
        /// </summary>
        private static MessageData _curMessage;
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle
        #endregion

        #region Function

        public static void Equeue(MessageData message)
        {
            _messageQueue.Enqueue(message);
            ExcuteMessageQueue();
        }

        private static void ExcuteMessageQueue()
        {
            if (_curMessage == null)
            {
                ExcuteNext();
            }
            else
            {
                ExcuteMessageWithType();
            }
        }


        /// <summary>
        /// 执行当前消息
        /// </summary>
        private static void ExcuteMessageWithType()
        {
            switch (_curMessage.MessageState)
            {
                case MessageState.Init:
                    _curMessage.ExcuteAction(_curMessage);
                    _curMessage.MessageState = MessageState.IsPlaying;
                    break;
                case MessageState.Finish:
                    _curMessage = null;
                    ExcuteNext();
                    break;
                case MessageState.IsPlaying:
                    break;
            }
        }

        /// <summary>
        /// 执行下条消息
        /// </summary>
        private static void ExcuteNext()
        {
            var messageCount = _messageQueue.Count;
            if (messageCount > _messageMaxLenth)
            {
                Debug.LogError("数据堆积过多，尝试重连");
                App.RServer.SendReJoinGame();
            }
            else if (messageCount > ConstantData.IntValue)
            {
                _curMessage = _messageQueue.Dequeue();
                ExcuteMessageWithType();
            }
        }

        #endregion
    }
    /// <summary>
    /// 消息数据
    /// </summary>
    public class MessageData
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public int MessageType;
        /// <summary>
        /// 消息数据主体
        /// </summary>
        public ISFSObject Data;
        /// <summary>
        /// 消息状态
        /// </summary>
        public MessageState MessageState;
        /// <summary>
        /// 执行动作
        /// </summary>
        public Action<MessageData> ExcuteAction;
    }
    /// <summary>
    /// 消息状态
    /// </summary>
    public enum MessageState
    {
        Init,
        IsPlaying,
        Finish,
    }
}
