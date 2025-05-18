using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows
{
    /// <summary>
    /// 窗口
    /// </summary>
    public class YxQueueWindow : YxNguiWindow
    {
        /// <summary>
        /// 字段：数据中表示 队列的字段
        /// </summary>
        public string QueueField = "pictures";
        /// <summary>
        /// 发送消息的Action名称
        /// </summary>
        public string ActionName = "criclePicture";
        /// <summary>
        /// 检测时间
        /// </summary>
        public bool NeedCheckDate;
        /// <summary>
        /// 队列预制体
        /// </summary>
        public YxWindow QueueItemWindowPrefab;

        private readonly Queue<object> _queue = new Queue<object>();

        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            InitStateTotal = 1;
            CurTwManager.SendAction(ActionName, null,UpdateView);
        }
          
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<Dictionary<string, object>>();
            if (data == null) { return;}
            if (data.ContainsKey(QueueField))
            {
                var dict = data[QueueField] as Dictionary<string, object>;
                if (dict != null && dict.Count > 0)
                {
                    _queue.Clear();
                    foreach (var queueData in dict)
                    {
                        _queue.Enqueue(queueData);
                    } 
                    OpenItemWindow();
                    return;
                }
            }
            Close();
        }

        protected YxWindow OpenItemWindow()
        {
            if (_queue.Count < 1) { return null;}
            var first = _queue.Dequeue();
            var win = YxWindowUtils.CreateItem(QueueItemWindowPrefab, transform);
            win.UpdateView(first);
            win.Show();
            return win;
        }

        public override void Close()
        {
            if (!OpenItemWindow())
            {
                base.Close();
            }
        }
    }
}
