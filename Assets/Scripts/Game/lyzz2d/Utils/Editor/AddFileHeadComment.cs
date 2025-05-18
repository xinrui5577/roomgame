/** 
 *文件名称:     MahJongData.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-02 
 *描述:         自定义Unity 中创建的C#代码模版
 *历史记录: 
*/

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.Editor
{
    public class AddFileHeadComment : UnityEditor.AssetModificationProcessor
    {
        /// <summary>  
        /// 此函数在asset被创建完，文件已经生成到磁盘上，但是没有生成.meta文件和import之前被调用  
        /// </summary>  
        /// <param name="newFileMeta">newfilemeta 是由创建文件的path加上.meta组成的</param>  
        public static void OnWillCreateAsset(string newFileMeta)
        {
            string newFilePath = newFileMeta.Replace(".meta", "");
            string fileExt = Path.GetExtension(newFilePath);
            if (fileExt != ".cs")
            {
                return;
            }
            //注意，Application.datapath会根据使用平台不同而不同  
            string realPath = Application.dataPath.Replace("Assets", "") + newFilePath;
            string scriptContent = File.ReadAllText(realPath);

            //这里实现自定义的一些规则  
            scriptContent = scriptContent.Replace("#SCRIPTFULLNAME#", Path.GetFileName(newFilePath));
            scriptContent = scriptContent.Replace("#COMPANY#", PlayerSettings.companyName);
            scriptContent = scriptContent.Replace("#AUTHOR#", "AndeLee");
            scriptContent = scriptContent.Replace("#VERSION#", "1.0");
            scriptContent = scriptContent.Replace("#UNITYVERSION#", Application.unityVersion);
            scriptContent = scriptContent.Replace("#DATE#", System.DateTime.Now.ToString("yyyy-MM-dd"));
            File.WriteAllText(realPath, scriptContent);
        }

    }
}
