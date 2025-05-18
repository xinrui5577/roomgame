using Assets.Scripts.Hall.View.RecordWindows;
using UnityEngine;

namespace Assets.Skins.SkinResource.skin_0019.Scripts.Tea.Page.TeaTotalRecordContainer
{
    public class JpmjRecordDetailItem : RecordDetailItem
    {
        public RecordDetailWindow RecordDetailWindow;
        public GameObject SssDetailBtn;

        public override void ShowItem(string roundNum, string selfId)
        {
            base.ShowItem(roundNum, selfId);
            SssDetailBtn.gameObject.SetActive(RecordDetailWindow.CurGameKey == "sssjp");
        }
    }
}
