using UnityEngine;
using UnityEditor;
using System.IO;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public class CreateMahjongPrefabs
    {
        /// <summary>
        /// 创建麻将预制体
        /// </summary>
        //[MenuItem("MahTools/Create MahPrafabs")]
        //public static void CreateMahPrafabs()
        //{
        //    string createtPath = "/_Assets/Prefabs/MahjongPrefabs/";
        //    //选择的麻将预制体
        //    Object[] objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.TopLevel);
        //    for (int i = 0; i < objs.Length; i++)
        //    {
        //        GameObject go = objs[i] as GameObject;
        //        Debug.LogError("选择麻将 --> " + go.name);
        //        string filePath = Application.dataPath + createtPath + go.name + ".prefab";
        //        //如果存在就删除
        //        if (File.Exists(@filePath))
        //        {
        //            File.Delete(filePath);
        //            Debug.LogError("删除存在的麻将 --> " + go.name);
        //        }
        //        //创建预制体              
        //        GameObject a = PrefabUtility.CreatePrefab("Assets" + createtPath + go.name + ".prefab", go);
        //        //添加标记
        //        GameObject flag = new GameObject();
        //        flag.transform.parent = (a.transform);
        //        flag.transform.localRotation = Quaternion.Euler(0, 0, 0);
        //        flag.transform.localPosition = new Vector3(0.4f, 0.4f, 1f);
        //        flag.transform.localScale = new Vector3(0.048f, 0.088f, -0.115f); ;
        //        flag.layer = a.layer;
        //        PrefabUtility.CreatePrefab("Assets" + createtPath + 1111 + ".prefab", a);

        //        //刷新资源  
        //        AssetDatabase.Refresh();
        //    }
        //}

        //[MenuItem("MahTools/Test")]
        //public static void TestSelection()
        //{
        //    string createtPath = Application.dataPath + "/_Assets/Prefabs/MahjongPrefabs/" + 56 + ".prefab";
        //    File.Delete(createtPath);
        //    AssetDatabase.Refresh();
        //    Dictionary<string, Vector3> dic = new Dictionary<string, Vector3>();
        //    //transforms是Selection类的静态字段，其返回的是选中的对象的Transform
        //    Transform[] transforms = Selection.transforms;
        //    //将选中的对象的postion保存在字典中
        //    for (int i = 0; i < transforms.Length; i++)
        //    {
        //        dic.Add(transforms[i].name, transforms[i].position);
        //    }
        //    //将字典中的信息打印出来
        //    foreach (Transform item in transforms)
        //    {
        //        Debug.Log(item.name + ":" + item.position);
        //    }
        //    string[] arrStrAudioPath = Directory.GetFiles(Application.dataPath + "/Test/", "*", SearchOption.AllDirectories);
        //    foreach (string strAudioPath in arrStrAudioPath)
        //    {
        //        //替换路径中的反斜杠为正斜杠       
        //        string strTempPath = strAudioPath.Replace(@"\", "/");
        //        //截取我们需要的路径
        //        strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
        //        //过滤.meta 文件
        //        if (!strTempPath.Contains("meta"))
        //        {
        //            //根据路径加载资源
        //            Object objAudio = AssetDatabase.LoadAssetAtPath(@strTempPath, typeof(Object));
        //            Debug.LogError(objAudio.name);
        //        }
        //    }
        //}

        /// <summary>
        /// 删除文件夹中所有文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="keyword">路径关键字，避免删错</param>
        //private static void DeleteAllFile(string path, string keyword)
        //{
        //    try
        //    {
        //        //判断路径是否存在和是否符合要求
        //        if (Directory.Exists(path) && path.Contains(keyword))
        //        {
        //            Debug.LogError("开始删除文件");
        //            DirectoryInfo dir = new DirectoryInfo(@path);
        //            foreach (FileInfo f in dir.GetFiles())
        //            {
        //                Debug.LogError("删除文件 --> " + f.Name);
        //                f.Delete();
        //            }
        //            AssetDatabase.Refresh();
        //        }
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.LogError(e.Message);
        //    }
        //}
    }
}