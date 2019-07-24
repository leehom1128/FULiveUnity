using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using NatCamU.Core;
using NatCamU.Dispatch;

public class RenderToModel : MonoBehaviour
{
    //摄像头参数(INPUT)
    public Facing facing = Facing.Rear;
    public ResolutionPreset previewResolution = ResolutionPreset.HD;
    public ResolutionPreset photoResolution = ResolutionPreset.HighestResolution;
    public FrameratePreset framerate = FrameratePreset.Default;

    //Debugging
    public Switch verbose;

#if UNITY_EDITOR || UNITY_STANDALONE
    //以下参数仅在PC或MAC上生效，因为这两个平台上NatCam实际上调用的是Unity自带的WebCam,无法在底层直接向SDK输入数据，因此需要在这里输入数据

    //byte[] img_bytes;
    Color32[] webtexdata;   //用于保存每帧从相机类获取的数据
    GCHandle img_handle;    //webtexdata的GCHandle
    IntPtr p_img_ptr;   //webtexdata的指针

    //SDK返回(OUTPUT)
    private int m_fu_texid = 0; //SDK返回的纹理ID
    private Texture2D m_rendered_tex;   //用SDK返回的纹理ID新建的纹理

    //标记参数
    private bool m_tex_created; //m_rendered_tex是否已被创建，这个不需要每帧创建，纹理ID不变就不要重新创建
#endif

    //渲染显示UI
    public RawImage RawImg_BackGroud;   //用来显示相机结果的UI控件
    private Quaternion baseRotation;    //RawImg_BackGroud的初始旋转
    public Camera camera3d; //渲染3D物体的相机
    private float dde_focallength = 0;  //SDK跟踪时算出来的焦距，需要和Unity的camera3d同步，从而实现真人和3D物体的位移旋转统一
    private Coroutine fovcor = null;    //同步焦距用的协程，因为实际计算中可能没法第一时间拿到跟踪焦距
    public bool ifTrackPos = false;     //是否跟踪人脸位置，选择true时只跟踪人脸旋转

    public Text txt;
    public delegate void OnSwitchCamera(bool isSwitching);  //相机切换委托
    public event OnSwitchCamera onSwitchCamera;

    const int SLOTLENGTH = 1;   //这四个参数的作用请看RenderToTexture，主要是为了满足不同SDK模式下的舌头的跟踪，详细信息请看文档
    int[] itemid_tosdk;
    GCHandle itemid_handle;
    IntPtr p_itemsid;

    //相机开始运行的回调
    public void OnStart()
    {
#if !(UNITY_EDITOR || UNITY_STANDALONE)
        if (FaceunityWorker.currentMode == FaceunityWorker.FURuningMode.FU_Mode_RenderItems)
        {
            // Set the preview RawImage texture once the preview starts
            if (RawImg_BackGroud != null)
            {
                RawImg_BackGroud.texture = NatCam.Preview;
                RawImg_BackGroud.gameObject.SetActive(true);
            }
            else Debug.Log("Preview RawImage has not been set");
        }
#else
        m_tex_created = false;
#endif
        Debug.Log("Preview started with dimensions: " + NatCam.Camera.PreviewResolution.x+","+ NatCam.Camera.PreviewResolution.y);
        if(RawImg_BackGroud.texture!=null)
            Debug.Log("RawImg_BackGroud.TexturePtr= " + RawImg_BackGroud.texture.GetNativeTexturePtr());

        SelfAdjusSize();
        if(onSwitchCamera!=null)
            onSwitchCamera(false);
    }

    //延迟调整相机UI比例旋转，在有些设备上无法第一时间调整
    public IEnumerator delaySet()
    {
        SelfAdjusSize();
        yield return Util._endOfFrame;
        yield return Util._endOfFrame;
        yield return Util._endOfFrame;
#if !(UNITY_EDITOR || UNITY_STANDALONE)
        if (RawImg_BackGroud != null)
        {
            RawImg_BackGroud.texture = NatCam.Preview;
            RawImg_BackGroud.gameObject.SetActive(true);
        }
        else Debug.Log("Preview RawImage has not been set");
#else
        m_tex_created = false;
#endif
    }

    //重置相机UI
    public void ReSetBackGroud()
    {
        if (RawImg_BackGroud != null)
        {
            RawImg_BackGroud.gameObject.SetActive(false);
            RawImg_BackGroud.texture = null;
            ResetFOV();
        }
    }

    //根据SDK实时跟踪给出的焦距调整Unity内部渲染3D物体的相机的焦距（其实是FOV），可能无法在切换摄像机后第一时间调整好，所以需要不停跑直到完成
    IEnumerator SetFOV()
    {
        if(FaceunityWorker.fu_IsTracking() <= 0)
        {
            ResetFOV();
        }
        while (true)
        {
			if (!ifTrackPos) {
				ResetFOV();
				break;
			}
            if (FaceunityWorker.instance.m_focallength != null && FaceunityWorker.fu_IsTracking() > 0)
            {
                var R = FaceunityWorker.instance.m_focallength[0].m_data;
                if (R != null && R[0]>0)
                {
                    if (Mathf.Abs(R[0] - dde_focallength) > 0.0001f)
                    {
                        dde_focallength = R[0];
                        Vector2 currentResolution = NatCam.Camera.PreviewResolution;
                        float l = 0;
                        float rotz = RawImg_BackGroud.rectTransform.eulerAngles.z;
                        if ((rotz > 45 && rotz < 135) || (rotz > 225 && rotz < 315))
                        {
                            if (RawImg_BackGroud.rectTransform.sizeDelta.x < RawImg_BackGroud.rectTransform.sizeDelta.y)
                                l = Mathf.Min(currentResolution.x, currentResolution.y);
                            else
                                l = Mathf.Max(currentResolution.x, currentResolution.y);
                        }
                        else
                        {
                            if (RawImg_BackGroud.rectTransform.sizeDelta.x > RawImg_BackGroud.rectTransform.sizeDelta.y)
                                l = Mathf.Min(currentResolution.x, currentResolution.y);
                            else
                                l = Mathf.Max(currentResolution.x, currentResolution.y);
                        }
                        camera3d.fieldOfView = (float)(Mathf.Atan(l / (2 * R[0])) * Mathf.Rad2Deg * 2.0);
                        //Debug.Log("fieldOfView@@@");
                        //txt.text = "m_focallength=" + R[0] + "\n fieldOfView=" + camera3d.fieldOfView+ "\n VerticalBGLength="+ l;
                    }
                    //txt.text = "m_focallength=" + R[0] + "\n fieldOfView=" + camera3d.fieldOfView;
                    Debug.Log("dde_focallength=" + dde_focallength+ " R[0]=" + R[0]);
                    break;
                }
            }
            //Debug.Log("SetFOV Running!!!");
            yield return Util._endOfFrame;
        }
        fovcor = null;
    }

    //重置渲染相机的FOV
    public void ResetFOV()
    {
        dde_focallength = 0;
        camera3d.fieldOfView = 48.725f;
        //txt.text = "ResetFOV";
        Debug.Log("ReSetFOV");
    }

    //切换相机
    public void SwitchCamera(int newCamera = -1)
    {
        FaceunityWorker.fu_OnCameraChange();

        if (onSwitchCamera != null)
            onSwitchCamera(true);

        if (fovcor != null)
        {
            StopCoroutine(fovcor);
            fovcor = null;
        }
        
        // Select the new camera ID // If no argument is given, switch to the next camera
        newCamera = newCamera < 0 ? (NatCam.Camera + 1) % DeviceCamera.Cameras.Count : newCamera;
        // Set the new active camera
        NatCam.Camera = (DeviceCamera)newCamera;
    }

    //根据运行环境调整相机UI的缩放旋转，这段简单的代码无法覆盖所有情况
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
		RawImg_BackGroud.rectTransform.sizeDelta = new Vector2(targetResolution.y * currentResolution.y / currentResolution.x, targetResolution.y);
#endif

        if (fovcor==null)
            fovcor=StartCoroutine(SetFOV());
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
            // Null checking
            if (!NatCam.Camera)
            {
                // Log
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

    void Awake()
    {
        FaceunityWorker.OnInitOK += InitApplication;

        if (itemid_tosdk == null)
        {
            itemid_tosdk = new int[SLOTLENGTH];
            itemid_handle = GCHandle.Alloc(itemid_tosdk, GCHandleType.Pinned);
            p_itemsid = itemid_handle.AddrOfPinnedObject();
        }
    }

    void InitApplication(object source, EventArgs e)
    {
        baseRotation = RawImg_BackGroud.rectTransform.rotation;
        StartCoroutine("InitCamera");
    }

    //当前环境是PC或者MAC的时候，在这里向SDK输入数据并获取SDK输出的纹理
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
                FaceunityWorker.SetImage(p_img_ptr,0, false, (int)NatCam.Camera.PreviewResolution.x, (int)NatCam.Camera.PreviewResolution.y);   //传输数据方法之一
            }
        }

        if (m_tex_created == false && FaceunityWorker.currentMode == FaceunityWorker.FURuningMode.FU_Mode_RenderItems)
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
            else
                Debug.Log("ERROR!!!m_fu_texid: " + m_fu_texid);
        }
#endif
        //if (FaceunityWorker.instance.m_rotation_mode != null)
        //{
        //    var R = FaceunityWorker.instance.m_rotation_mode[0].m_data;
        //    if (R != null)
        //        txt.text = "rotation=" + R[0];
        //}
    }

    void OnApplicationPause(bool isPause)
    {
        if (isPause)
        {
            Debug.Log("Pause");
#if UNITY_EDITOR || UNITY_STANDALONE
            m_tex_created = false;
#endif
        }
        else
        {
            Debug.Log("Start");
        }
    }

    //加载道具。用于舌头跟踪，详见文档
    public IEnumerator LoadItem(string path, int slotid = 0)
    {
        Debug.Log("LoadItem:" + path);
        WWW bundledata = new WWW(path);
        yield return bundledata;
        byte[] bundle_bytes = bundledata.bytes;
        GCHandle hObject = GCHandle.Alloc(bundle_bytes, GCHandleType.Pinned);
        IntPtr pObject = hObject.AddrOfPinnedObject();

        yield return FaceunityWorker.fu_CreateItemFromPackage(pObject, bundle_bytes.Length);
        hObject.Free();
        int itemid = FaceunityWorker.fu_getItemIdxFromPackage();

        itemid_tosdk[slotid] = itemid;

        FaceunityWorker.fu_setItemIds(p_itemsid, SLOTLENGTH, IntPtr.Zero);
    }

    //卸载道具。用于舌头跟踪，详见文档
    public bool UnLoadItem(int slotid = 0)
    {
        if (slotid >= 0 && slotid < SLOTLENGTH)
        {
            FaceunityWorker.fu_DestroyItem(itemid_tosdk[slotid]);
            itemid_tosdk[slotid] = 0;
            return true;
        }
        Debug.LogError("UnLoadItem Faild!!!");
        return false;
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (img_handle != null && img_handle.IsAllocated)
        {
            img_handle.Free();
        }
#endif
        UnLoadItem();
    }

    //UI上显示当前相机方向
    void OnGUI()
    {
        if (NatCam.Camera != null && !ifTrackPos)
        {
            string text = NatCam.Camera.Facing == Facing.Front ? "前置摄像头" : "后置摄像头";
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, h * 2 / 100, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            GUI.Label(rect, text, style);
        }
    }
}
