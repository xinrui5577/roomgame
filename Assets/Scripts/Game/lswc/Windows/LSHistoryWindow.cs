using System.Collections.Generic;
using Assets.Scripts.Game.lswc.Core;
using Assets.Scripts.Game.lswc.Data;
using Assets.Scripts.Game.lswc.UI.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lswc.Windows
{
    public class LSHistoryWindow : InstanceControl
    {
        public List<LSHistoryItem> historys;

        private Transform _grid;

        private void Awake()
        {
            historys = new List<LSHistoryItem>();
        }

        private void Start()
        {
            Find();
        }

        private void Find()
        {
            _grid = transform.FindChild("Panel/Grid");
            for (int i = _grid.childCount-1; i>=0; i--)
            {
                LSHistoryItem item = _grid.GetChild(i).GetComponent<LSHistoryItem>();
                historys.Add(item);
            }
        }

        public void InitHistorys()
        {
            int lenth=historys.Count;
            for (int  i= 0;  i<=lenth-1; i++)
            {
                historys[i].InitItem(App.GetGameData<LswcGameData>().GetHistoryResult(i));
            }
        }

        public override void OnExit()
        {
            historys.Clear();
        }

    }
}
