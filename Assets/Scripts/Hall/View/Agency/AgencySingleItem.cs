/** 
 *文件名称:     AgencySingleItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-11-18 
 *描述:         代理信息中的基本结构
 *              1.key 联系方式
 *              2.value 具体联系内容
 *              3.复制按钮，用来将对应联系内容复制到当前系统的剪切板
 *历史记录: 
*/
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.Agency
{
    public class AgencySingleItem : MonoBehaviour
    {
        [Tooltip("联系方式")]
        public UILabel ContactType;
        [Tooltip("具体联系内容")]
        public UILabel ContactContent;

        public void Refresh(string key,string value)
        {
            if (ContactType&&!string.IsNullOrEmpty(key))
            {
                ContactType.text = key;
            }
            if (ContactContent && !string.IsNullOrEmpty(value))
            {
                ContactContent.text = value;
            }
        }

        public void SaveValue(string value)
        {

            Facade.Instance<YxGameTools>().PasteBoard = value;
            YxMessageBox.Show(new YxMessageBoxData
            {
                Msg = "成功复制到剪切板",
            });
        }
    }
}
