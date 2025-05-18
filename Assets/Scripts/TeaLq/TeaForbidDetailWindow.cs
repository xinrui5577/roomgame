using Assets.Scripts.Common.Windows;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;

public class TeaForbidDetailWindow : YxNguiWindow
{
    public NguiLabelAdapter Desc;
    public UIGrid DescGrid;
    protected override void OnFreshView()
    {
        base.OnFreshView();
        var datas = Data as List<string>;
        if (datas == null) return;
        foreach (var str in datas)
        {
            if (string.IsNullOrEmpty(str)) return;
            var item= YxWindowUtils.CreateItem(Desc, DescGrid.transform);
            item.TrySetComponentValue(str);
        }
        DescGrid.repositionNow = true;
    }
}
