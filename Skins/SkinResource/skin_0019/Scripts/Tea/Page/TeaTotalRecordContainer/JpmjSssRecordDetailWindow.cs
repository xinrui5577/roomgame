using UnityEngine;
using System.Collections.Generic;

namespace Assets.Skins.SkinResource.skin_0019.Scripts.Tea.Page.TeaTotalRecordContainer
{
    public class JpmjSssRecordDetailWindow : MonoBehaviour
    {
        [SerializeField]
        private JpmjSssRecordDetailItem[] _RecordItems;

        public void ShowWindow(Dictionary<int, SssReplayFrameData> dic)
        {
            SssReplayFrameData data;
            for (int i = 0; i < _RecordItems.Length; i++)
            {
                if (dic.TryGetValue(i, out data))
                {
                    _RecordItems[i].SetRecordDetail(data);
                }
            }
            gameObject.SetActive(true);
        }

        public void HideWindow()
        {
            for (int i = 0; i < _RecordItems.Length; i++)
            {
                _RecordItems[i].OnReset();
            }
            gameObject.SetActive(false);
        }
    }
}