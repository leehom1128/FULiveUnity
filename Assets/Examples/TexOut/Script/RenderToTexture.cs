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

    public const int SLOTLENGTH = 10;
    private struct slot_item
    {
        public string name;
        public int id;
    };
    private slot_item[] slot_items;



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
        for(int i=0;i< SLOTLENGTH;i++)
        {
            SetItemMirror(i);
        }
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
            //默认slot槽长度为SLOTLENGTH=10
            itemid_tosdk = new int[SLOTLENGTH];
            itemid_handle = GCHandle.Alloc(itemid_tosdk, GCHandleType.Pinned);
            p_itemsid = itemid_handle.AddrOfPinnedObject();

            slot_items = new slot_item[SLOTLENGTH];
            for(int i=0;i< SLOTLENGTH;i++)
            {
                slot_items[i].id = 0;
                slot_items[i].name = "";
            }
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
    public IEnumerator LoadItem(Item item, int slotid = 0, LoadItemCallback cb=null)
    {
        if (LoadingItem == false && item.fullname != null && item.fullname.Length != 0 && slotid >= 0 && slotid < SLOTLENGTH)
        {
            LoadingItem = true;
            int tempslot = GetSlotIDbyName(item.name);
            if (tempslot < 0)   //如果尚未载入道具数据
            {
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

                UnLoadItem(slotid); //卸载上一个在这个slot槽内的道具，并非必要，但是为了节省内存还是清一下

                itemid_tosdk[slotid] = itemid;
                slot_items[slotid].id = itemid;
                slot_items[slotid].name = item.name;

                FaceunityWorker.fu_setItemIds(p_itemsid, SLOTLENGTH, IntPtr.Zero);
                Debug.Log("载入Item：" + item.name + " @slotid=" + slotid);
            }
            else if(tempslot != slotid)    //道具已载入，但是不在请求的slot槽内
            {
                UnLoadItem(slotid);

                itemid_tosdk[slotid] = slot_items[tempslot].id;
                slot_items[slotid].id = slot_items[tempslot].id;
                slot_items[slotid].name = slot_items[tempslot].name;

                itemid_tosdk[tempslot] = 0;
                slot_items[tempslot].id = 0;
                slot_items[tempslot].name = "";

                FaceunityWorker.fu_setItemIds(p_itemsid, SLOTLENGTH, IntPtr.Zero);
                Debug.Log("移动Item：" + item.name + " from tempslot=" + tempslot + " to slotid="+ slotid);
            }
            else    //tempslot == slotid 即重复载入同一个道具进同一个slot槽，直接跳过
            {
                Debug.Log("重复载入Item："+ item.name +"  slotid="+ slotid);
            }
            if (item.type == 1)
                flipmark = false;
            else
                flipmark = true;
            SetItemMirror(slotid);

            if (cb != null)
                cb(item.name);//触发载入道具完成事件
            
            LoadingItem = false;
        }
    }

    public int GetItemIDbyName(string name)
    {
        for(int i=0;i< SLOTLENGTH; i++)
        {
            if (string.Equals(slot_items[i].name, name))
                return slot_items[i].id;
        }
        return 0;
    }

    public string GetItemNamebySlotID(int slotid)
    {
        if (slotid >= 0 && slotid < SLOTLENGTH)
        {
            return slot_items[slotid].name;
        }
        return "";
    }

    public int GetSlotIDbyName(string name)
    {
        for (int i = 0; i < SLOTLENGTH; i++)
        {
            if (string.Equals(slot_items[i].name, name))
                return i;
        }
        return -1;
    }

    public bool UnLoadItem(string itemname)
    {
        return UnLoadItem(GetSlotIDbyName(itemname));
    }

    public bool UnLoadItem(int slotid)
    {
        if (slotid >= 0 && slotid< SLOTLENGTH)
        {
            if (slot_items[slotid].id == 0)
                return true;
            Debug.Log("UnLoadItem name=" + slot_items[slotid].name+ " slotid="+ slotid);

            FaceunityWorker.fu_DestroyItem(slot_items[slotid].id);
            itemid_tosdk[slotid] = 0;
            slot_items[slotid].id = 0;
            slot_items[slotid].name = "";
            return true;
        }
        Debug.LogWarning("UnLoadItem Faild!!!");
        return false;
    }

    public void UnLoadAllItems()
    {
        Debug.Log("UnLoadAllItems");
        FaceunityWorker.fu_DestroyAllItems();

        for (int i = 0; i < SLOTLENGTH; i++)
        {
            itemid_tosdk[i] = 0;
            slot_items[i] = new slot_item { name = "", id = 0 };
        }
    }

    public void SetItemParamd(string itemname, string paramdname, double value)
    {
        SetItemParamd(GetSlotIDbyName(itemname), paramdname, value);
    }

    public void SetItemParamd(int slotid, string paramdname, double value)
    {
        if (slotid >= 0 && slotid< SLOTLENGTH)
        {
            FaceunityWorker.fu_ItemSetParamd(slot_items[slotid].id, paramdname, value);
        }
    }

    public double GetItemParamd(string itemname, string paramdname)
    {

        return GetItemParamd(GetSlotIDbyName(itemname), paramdname);
    }

    public double GetItemParamd(int slotid, string paramdname)
    {
        if (slotid >= 0 && slotid < SLOTLENGTH)
        {
            return FaceunityWorker.fu_ItemGetParamd(slot_items[slotid].id, paramdname);
        }
        return 0;
    }

    public void SetItemParams(string itemname, string paramdname, string value)
    {
        SetItemParams(GetSlotIDbyName(itemname), paramdname, value);
    }

    public void SetItemParams(int slotid, string paramdname, string value)
    {
        if (slotid >= 0 && slotid < SLOTLENGTH)
        {
            FaceunityWorker.fu_ItemSetParams(slot_items[slotid].id, paramdname, value);
        }
    }

    public string GetItemParams(string itemname, string paramdname)
    {
        return GetItemParams(GetSlotIDbyName(itemname),paramdname);
    }

    public string GetItemParams(int slotid, string paramdname)
    {
        if (slotid >= 0 && slotid < SLOTLENGTH)
        {
            byte[] bytes = new byte[32];
            int i = FaceunityWorker.fu_ItemGetParams(slot_items[slotid].id, paramdname, bytes, 32);
            return System.Text.Encoding.Default.GetString(bytes).Replace("\0", "");
        }
        return "";
    }

    bool flipmark = true;
    public void SetItemMirror(int slotid)
    {
        if (slotid < 0 || slotid >= SLOTLENGTH)
        {
            return;
        }
        int itemid = slot_items[slotid].id;
        if (itemid <= 0)
            return;
        FaceunityWorker.fu_ItemSetParamd(itemid, "camera_change", 1.0);
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
        FaceunityWorker.fu_ItemSetParamd(itemid, "isAndroid", 1.0);
#endif
#if (UNITY_IOS) && (!UNITY_EDITOR)
        FaceunityWorker.fu_SetDefaultRotationMode(2);
        ifMirrored=false;
#endif
        int param = ifMirrored && flipmark ? 1 : 0;

        //is3DFlipH 参数是用于对3D道具的镜像
        FaceunityWorker.fu_ItemSetParamd(itemid, "is3DFlipH", param);
        //isFlipExpr 参数是用于对道具内部的表情系数的镜像
        FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipExpr", param);
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
