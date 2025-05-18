using UnityEngine;
using System.Collections;
using Assets.Scripts.Hall.View.RecordWindows;
using Assets.Scripts.Common.Utils;

public class SpecialRecordDetailItem : RecordDetailItem
{
    public UILabel GameName;

    public override void ShowItem(string roundNum, string selfId)
    {
        base.ShowItem(roundNum, selfId); 
    }
}
