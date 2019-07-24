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

    private static bool? isnexus6 = null;
    public static bool isNexus6()   //Nexus6前置摄像头的数据规格和常规安卓手机的不一样
    {
        if (isnexus6 == null)
        {
            string tmp = SystemInfo.deviceModel.ToLower();
            if (tmp.Contains("nexus") && tmp.Contains("6"))
                isnexus6 = true;
            else
                isnexus6 = false;
        }
        return isnexus6 == true;
    }

    private static bool? isnexus5x = null;
    public static bool isNexus5X()   //Nexus6前置摄像头的数据规格和常规安卓手机的不一样
    {
        if (isnexus5x == null)
        {
            string tmp = SystemInfo.deviceModel.ToLower();
            if (tmp.Contains("nexus") && tmp.Contains("5x"))
                isnexus5x = true;
            else
                isnexus5x = false;
        }
        return isnexus5x == true;
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
}
