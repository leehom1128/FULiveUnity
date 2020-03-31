using UnityEngine;
using System.Collections;
using System.IO;

public class Util
{

    //这里需要注意一下安卓以外平台上要加file协议。  
    public static string GetStreamingAssetsPath()
    {
        string path = Application.streamingAssetsPath;
        if (Application.platform != RuntimePlatform.Android)
        {
            path = "file://" + path;
        }
        return path;
    }

    public static WaitForEndOfFrame _endOfFrame = new WaitForEndOfFrame();
    public static WaitForFixedUpdate _fixedupdate = new WaitForFixedUpdate();

    /**
\brief 把一个bytes数组保存成一个文件
\param data 要保存的bytes数组
\param path 保存路径
\param nameWithExtension 文件名加后缀名，如：example.bytes
\return 保存的文件全称
    */
    public static string SaveBytesFile(byte[] data, string path, string nameWithExtension)
    {
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }
        string fullfilename = path + nameWithExtension;
        File.WriteAllBytes(fullfilename, data);
        Debug.Log("保存了一个文件:" + fullfilename);
        return fullfilename;
    }

    public static byte[] ReadBytesFile(string path, string nameWithExtension)
    {
        if (Directory.Exists(path) == false)
        {
            return null;
        }
        string fullfilename = path + nameWithExtension;
        byte[] data = File.ReadAllBytes(fullfilename);
        Debug.Log("读取了一个文件:" + fullfilename);
        return data;
    }
}
