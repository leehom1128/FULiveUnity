using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using NatCamU.Core;
using NatCamU.Dispatch;
using System.Text;

public class RenderToTexture : MonoBehaviour
{
    //Camera参数
    public Facing facing = Facing.Rear;
    public ResolutionPreset previewResolution = ResolutionPreset.HD;
    public ResolutionPreset photoResolution = ResolutionPreset.HighestResolution;
    public FrameratePreset framerate = FrameratePreset.Default;

    //Debugging
    public Switch verbose;

    //Camera_TO_SDK
    int[] itemid_tosdk;
    GCHandle itemid_handle;
    IntPtr p_itemsid;

#if UNITY_EDITOR||UNITY_STANDALONE
    //byte[] img_bytes;
    Color32[] webtexdata;
    GCHandle img_handle;
    IntPtr p_img_ptr;

    //SDK返回(OUTPUT)
    private int m_fu_texid = 0;
    private Texture2D m_rendered_tex;

    //标记参数
    private bool m_tex_created;
#endif
    private bool LoadingItem = false;

    //渲染显示UI
    public RawImage RawImg_BackGroud;
    private Quaternion baseRotation;

    private string currentItem = "";
    private int currentItemID = -1;
    private string beautyitem = "";
    private int beautyitemID = -1;


    public void OnStart()
    {
#if !(UNITY_EDITOR || UNITY_STANDALONE)
        // Set the preview RawImage texture once the preview starts
        if (RawImg_BackGroud != null)
        {
            RawImg_BackGroud.texture = NatCam.Preview;
            RawImg_BackGroud.gameObject.SetActive(true);
        }
        else Debug.Log("Preview RawImage has not been set");
        Debug.Log("Preview started with dimensions: " + NatCam.Camera.PreviewResolution.x+","+ NatCam.Camera.PreviewResolution.y);
#else
        m_tex_created = false;
#endif
        SelfAdjusSize();
        //SetItemMirror();
    }

    public void SwitchCamera(int newCamera = -1)
    {
        FaceunityWorker.fu_OnCameraChange();

        // Select the new camera ID // If no argument is given, switch to the next camera
        newCamera = newCamera < 0 ? (NatCam.Camera + 1) % DeviceCamera.Cameras.Count : newCamera;
        // Set the new active camera
        NatCam.Camera = (DeviceCamera)newCamera;

        StartCoroutine(delaySetItemMirror());
    }

    IEnumerator delaySetItemMirror()
    {
        yield return Util._endOfFrame;
        SetItemMirror();
    }

    public void SelfAdjusSize()
    {
        Vector2 targetResolution = RawImg_BackGroud.canvas.GetComponent<CanvasScaler>().referenceResolution;
        Vector2 currentResolution = NatCam.Camera.PreviewResolution;

#if !UNITY_IOS || UNITY_EDITOR || UNITY_STANDALONE
        if (DispatchUtility.Orientation == Orientation.Rotation_0 || DispatchUtility.Orientation == Orientation.Rotation_180)
            RawImg_BackGroud.rectTransform.sizeDelta = new Vector2(targetResolution.y * currentResolution.x / currentResolution.y, targetResolution.y);
        else
            RawImg_BackGroud.rectTransform.sizeDelta = new Vector2(targetResolution.y, targetResolution.y * currentResolution.y / currentResolution.x);

        if (NatCam.Camera.Facing == Facing.Front)
        {
#if UNITY_ANDROID
            NatCam.SetFlipx(false);
            NatCam.SetFlipy(false);
#endif
            if (Util.isNexus6())
                RawImg_BackGroud.rectTransform.rotation = baseRotation * Quaternion.AngleAxis((int)DispatchUtility.Orientation * 90, Vector3.back);
            else
                RawImg_BackGroud.rectTransform.rotation = baseRotation * Quaternion.AngleAxis((int)DispatchUtility.Orientation * 90, Vector3.forward);
#if UNITY_EDITOR || UNITY_STANDALONE
            RawImg_BackGroud.uvRect = new Rect(1, 0, -1, 1);    //镜像处理
#else
		    RawImg_BackGroud.uvRect = new Rect(0, 0, 1, 1);
#endif
        }
        else
        {
#if UNITY_ANDROID
            NatCam.SetFlipx(true);
            NatCam.SetFlipy(false);
#endif
            if (Util.isNexus5X())
                RawImg_BackGroud.rectTransform.rotation = baseRotation * Quaternion.AngleAxis((int)DispatchUtility.Orientation * 90, Vector3.forward);
            else
                RawImg_BackGroud.rectTransform.rotation = baseRotation * Quaternion.AngleAxis((int)DispatchUtility.Orientation * 90, Vector3.back);
#if UNITY_EDITOR || UNITY_STANDALONE
            RawImg_BackGroud.uvRect = new Rect(0, 0, 1, 1);
#else
		    RawImg_BackGroud.uvRect = new Rect(0, 1, 1, -1);    //镜像处理
#endif
        }
#else
        NatCam.SetFlipx(false);
        NatCam.SetFlipy(false);
        RawImg_BackGroud.rectTransform.sizeDelta = new Vector2(targetResolution.y * currentResolution.y / currentResolution.x, targetResolution.y);
#endif
    }

    // 初始化摄像头 
    public IEnumerator InitCamera()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            // Set verbose mode
            NatCam.Verbose = verbose;
            // Set the active camera
            NatCam.Camera = facing == Facing.Front ? DeviceCamera.FrontCamera : DeviceCamera.RearCamera;
            if (!NatCam.Camera)
            {
                NatCam.Camera = DeviceCamera.RearCamera;
            }
            //Null checking
            if (!NatCam.Camera)
            {
                Debug.LogError("No camera detected!");
                StopCoroutine("InitCamera");
                yield return null;
            }
            // Set the camera's preview resolution
            NatCam.Camera.SetPreviewResolution(previewResolution);
            // Set the camera's photo resolution
            NatCam.Camera.SetPhotoResolution(photoResolution);
            // Set the camera's framerate
            NatCam.Camera.SetFramerate(framerate);
            // Play
            NatCam.Play();
            // Register callback for when the preview starts //Note that this is a MUST when assigning the preview texture to anything
            NatCam.OnStart += OnStart;

#if UNITY_EDITOR || UNITY_STANDALONE
            if (img_handle.IsAllocated)
                img_handle.Free();
            webtexdata = new Color32[(int)NatCam.Camera.PreviewResolution.x * (int)NatCam.Camera.PreviewResolution.y];
            img_handle = GCHandle.Alloc(webtexdata, GCHandleType.Pinned);
            p_img_ptr = img_handle.AddrOfPinnedObject();
#endif
        }
    }

    //nama插件使用gles2.0，不支持glGetTexImage，因此只能用ReadPixels来读取数据
    public Texture2D CaptureCamera(Camera[] cameras, Rect rect)
    {
        // 创建一个RenderTexture对象  
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);
        // 临时设置相关相机的targetTexture为rt, 并手动渲染相关相机  
        foreach (Camera cam in cameras)
        {
            cam.targetTexture = rt;
            cam.Render();
        }
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);// 注：这个时候，它是从RenderTexture.active中读取像素  
        screenShot.Apply();

        // 重置相关参数，以使用camera继续在屏幕上显示  
        foreach (Camera cam in cameras)
        {
            cam.targetTexture = null;
        }
        RenderTexture.active = null; // JC: added to avoid errors  
        Destroy(rt);
        Debug.Log("截屏了一张照片");

        return screenShot;
    }

    //仅仅保存图片，并不通知图库刷新，因此请用文件浏览器在对应路径打开图片
    public void SaveTex2D(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        if (Directory.Exists(Application.persistentDataPath + "/Photoes/") == false)
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Photoes/");
        }
        string name = Application.persistentDataPath + "/Photoes/" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".png";
        File.WriteAllBytes(name, bytes);
        Debug.Log("保存了一张照片:" + name);
    }

    void Start()
    {
        FaceunityWorker.instance.OnInitOK += InitApplication;
        if (itemid_tosdk == null)
        {
            itemid_tosdk = new int[5];
            itemid_handle = GCHandle.Alloc(itemid_tosdk, GCHandleType.Pinned);
            p_itemsid = itemid_handle.AddrOfPinnedObject();
        }
    }

    void InitApplication(object source, EventArgs e)
    {
        //Debug.Log("版本："+ Marshal.PtrToStringAnsi(FaceunityWorker.fu_GetVersion()));
        baseRotation = RawImg_BackGroud.rectTransform.rotation;
        StartCoroutine("InitCamera");
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (NatCam.Camera == null)
            return;

        WebCamTexture tex = (WebCamTexture)NatCam.Preview;

        if (tex != null && tex.isPlaying)
        {

            // pass data by byte buf, 

            if (tex.didUpdateThisFrame)
            {
                if (webtexdata.Length != tex.width * tex.height)
                {
                    if (img_handle.IsAllocated)
                        img_handle.Free();
                    webtexdata = new Color32[tex.width * tex.height];
                    img_handle = GCHandle.Alloc(webtexdata, GCHandleType.Pinned);
                    p_img_ptr = img_handle.AddrOfPinnedObject();
                }
                tex.GetPixels32(webtexdata);
                //Debug.LogFormat("data pixels:{0},img_btyes:{1}",data.Length,img_bytes.Length/4);
                //for (int i = 0; i < webtexdata.Length; i++)
                //{
                //    img_bytes[4 * i] = webtexdata[i].b;
                //    img_bytes[4 * i + 1] = webtexdata[i].g;
                //    img_bytes[4 * i + 2] = webtexdata[i].r;
                //    img_bytes[4 * i + 3] = 1;
                //}
                FaceunityWorker.SetImage(p_img_ptr,32, false, (int)NatCam.Camera.PreviewResolution.x, (int)NatCam.Camera.PreviewResolution.y);   //传输数据方法之一
            }
        }


        if (m_tex_created == false)
        {
            m_fu_texid = FaceunityWorker.fu_GetNamaTextureId();
            if (m_fu_texid > 0)
            {
                m_tex_created = true;
                m_rendered_tex = Texture2D.CreateExternalTexture((int)NatCam.Camera.PreviewResolution.x, (int)NatCam.Camera.PreviewResolution.y, TextureFormat.RGBA32, false, true, (IntPtr)m_fu_texid);
                Debug.LogFormat("Texture2D.CreateExternalTexture:{0}\n", m_fu_texid);
                if (RawImg_BackGroud != null)
                {
                    RawImg_BackGroud.texture = m_rendered_tex;
                    RawImg_BackGroud.gameObject.SetActive(true);
                    Debug.Log("m_rendered_tex: " + m_rendered_tex.GetNativeTexturePtr());
                }
            }
        }
#endif
    }

    void OnApplicationPause(bool isPause)
    {

        if (isPause)
        {
            Debug.Log("Pause");
#if UNITY_EDITOR || UNITY_STANDALONE
            m_tex_created = false;
#endif
            //FaceunityWorker.fu_OnDeviceLost();

        }
        else
        {
            Debug.Log("Start");
        }
    }

    public delegate void LoadItemCallback(string message);
    public IEnumerator LoadItem(Item item, LoadItemCallback cb=null)
    {
        if (LoadingItem == false && item.fullname != null && item.fullname.Length != 0)
        {
            LoadingItem = true;
            if (!string.Equals(currentItem, item.name)&&!string.Equals(beautyitem, item.name))
            {
                Debug.Log("载入Item：" + item.name + "    当前Item：" + currentItem);
                var bundledata = Resources.LoadAsync<TextAsset>(item.fullname);
                yield return bundledata;
                var data = bundledata.asset as TextAsset;
                byte[] bundle_bytes = data != null ? data.bytes : null;
                Debug.LogFormat("bundledata name:{0}, size:{1}", item.name, bundle_bytes.Length);
                GCHandle hObject = GCHandle.Alloc(bundle_bytes, GCHandleType.Pinned);
                IntPtr pObject = hObject.AddrOfPinnedObject();
                yield return FaceunityWorker.fu_CreateItemFromPackage(pObject, bundle_bytes.Length);
                hObject.Free();

                int itemid = FaceunityWorker.fu_getItemIdxFromPackage();

                int itemnum = 0;
                if (beautyitemID >= 0 && itemid >= 0)
                {
                    itemid_tosdk[0] = beautyitemID;
                    itemid_tosdk[1] = itemid;
                    itemnum = 2;
                }
                else if (beautyitemID >= 0)
                {
                    itemid_tosdk[0] = beautyitemID;
                    itemnum = 1;
                }
                else if (itemid >= 0)
                {
                    itemid_tosdk[0] = itemid;
                    itemnum = 1;
                }
                FaceunityWorker.fu_setItemIds(p_itemsid, itemnum, IntPtr.Zero);
                UnLoadItem(currentItem);

                if (string.Equals(item.name, ItemConfig.beautySkin[0].name))
                {
                    beautyitemID = itemid;
                    beautyitem = item.name;
                    Debug.LogFormat("fu_CreateItemFromPackage beautyitem id:{0}", beautyitemID);
                }
                else
                {
                    currentItemID = itemid;
                    currentItem = item.name;
                    Debug.LogFormat("fu_CreateItemFromPackage currentItem id:{0}", currentItemID);
                }
            }
            if (item.type==1)
                flipmark = false;
            else
                flipmark = true;
            if (cb != null)
                cb(item.name);//触发载入道具完成事件
            SetItemMirror();
            LoadingItem = false;
        }
    }

    public string GetCurrentItemName()
    {
        return currentItem;
    }

    public void UnLoadItem(string itemname)
    {
        Debug.Log("UnLoadItem name=" + itemname);
        if (itemname.Length > 0)
        {
            if (string.Equals(currentItem, itemname))
            {
                FaceunityWorker.fu_DestroyItem(currentItemID);
                currentItem = "";
                currentItemID = -1;
            }
            else if (string.Equals(beautyitem, itemname))
            {
                FaceunityWorker.fu_DestroyItem(beautyitemID);
                beautyitem = "";
                beautyitemID = -1;
            }
        }
    }

    public void UnLoadAllItems()
    {
        Debug.Log("UnLoadAllItems");
        FaceunityWorker.fu_DestroyAllItems();
        currentItem = "";
        currentItemID = -1;
        beautyitem = "";
        beautyitemID = -1;
    }

    public void SetItemParamd(string itemname, string paramdname, double value)
    {
        if (itemname.Length > 0)
        {
            if (string.Equals(currentItem, itemname))
            {
                FaceunityWorker.fu_ItemSetParamd(currentItemID, paramdname, value);
            }
            else if(string.Equals(beautyitem, itemname))
            {
                FaceunityWorker.fu_ItemSetParamd(beautyitemID, paramdname, value);
            }
        }
    }

    public double GetItemParamd(string itemname, string paramdname)
    {
        if (itemname.Length > 0)
        {
            if (string.Equals(currentItem, itemname))
            {
                return FaceunityWorker.fu_ItemGetParamd(currentItemID, paramdname);
            }
            else if (string.Equals(beautyitem, itemname))
            {
                return FaceunityWorker.fu_ItemGetParamd(beautyitemID, paramdname);
            }
            return 0;
        }
        return 0;
    }

    public void SetItemParams(string itemname, string paramdname, string value)
    {
        if (itemname.Length > 0)
        {
            if (string.Equals(currentItem, itemname))
            {
                FaceunityWorker.fu_ItemSetParams(currentItemID, paramdname, value);
            }
            else if (string.Equals(beautyitem, itemname))
            {
                FaceunityWorker.fu_ItemSetParams(beautyitemID, paramdname, value);
            }
        }
    }

    public string GetItemParams(string itemname, string paramdname)
    {
        if (itemname.Length > 0)
        {
            if (string.Equals(currentItem, itemname))
            {
                byte[] bytes = new byte[32];
                int i = FaceunityWorker.fu_ItemGetParams(currentItemID, paramdname, bytes, 32);
                return System.Text.Encoding.Default.GetString(bytes).Replace("\0", "");
            }
            else if (string.Equals(beautyitem, itemname))
            {
                byte[] bytes = new byte[32];
                int i = FaceunityWorker.fu_ItemGetParams(beautyitemID, paramdname, bytes, 32);
                return System.Text.Encoding.Default.GetString(bytes).Replace("\0", "");
            }
            return "";
        }
        return "";
    }

    bool flipmark = true;
    public void SetItemMirror()
    {
        SetItemParamd(currentItem, "camera_change", 1.0);
        bool ifMirrored = NatCam.Camera.Facing == Facing.Front;
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        //道具旋转
        if (ifMirrored)
        {
            if (Util.isNexus6())
                FaceunityWorker.fu_SetDefaultRotationMode(3);
            else
                FaceunityWorker.fu_SetDefaultRotationMode(1);
        }
        else
        {
            if (Util.isNexus5X())
                FaceunityWorker.fu_SetDefaultRotationMode(1);
            else
                FaceunityWorker.fu_SetDefaultRotationMode(3);
        }
        ifMirrored = !ifMirrored;
        SetItemParamd(currentItem, "isAndroid", 1.0);
#endif
#if (UNITY_IOS) && (!UNITY_EDITOR)
        FaceunityWorker.fu_SetDefaultRotationMode(2);
        ifMirrored=false;
#endif
        int param = ifMirrored && flipmark ? 1 : 0;

        //is3DFlipH 参数是用于对3D道具的镜像
        SetItemParamd(currentItem, "is3DFlipH", param);
        //isFlipExpr 参数是用于对道具内部的表情系数的镜像
        SetItemParamd(currentItem, "isFlipExpr", param);
    }

    private void OnApplicationQuit()
    {
        UnLoadAllItems();
        //这些数据必须常驻，直到应用结束才能释放
        if (itemid_handle != null && itemid_handle.IsAllocated)
        {
            itemid_handle.Free();
        }
#if UNITY_EDITOR || UNITY_STANDALONE
        if (img_handle != null && img_handle.IsAllocated)
        {
            img_handle.Free();
        }
#endif
    }
}
