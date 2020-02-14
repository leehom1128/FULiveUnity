using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class FaceunityWorker : MonoBehaviour
{
    //封装好的用于获取当前跟踪信息的类
    public class CFaceUnityCoefficientSet
    {
        public float[] m_data;  //跟踪数据，float类型，一旦实例化就不要改变
        public int[] m_data_int;    //跟踪数据，int类型，一旦实例化就不要改变
        public GCHandle m_handle;   //跟踪数据句柄，通过这个类向SDK内部传输指针
        public IntPtr m_addr;   //跟踪数据的指针
        public string m_name;   //跟踪数据的名字，SDK通过这个来判断返回哪种数据
        public int m_addr_size; //跟踪数据的长度，可变
        public int m_faceId = 0;    //跟踪的人脸ID
        public int m_datatype = 0;  //0为float，1为int

        /**
   \brief 构造函数
   \param name 跟踪数据的名字，SDK通过这个来判断返回哪种数据
   \param size 跟踪数据的长度
   \param faceId 跟踪的人脸ID，默认值为0，第一个值为
   \param datatype 跟踪数据类型，有些为int有些为float，请参照相关文档设置，否则会出错，默认值为float
   \return 类实例
            */
        public CFaceUnityCoefficientSet(string name, int size, int faceId = 0,int datatype=0)
       {
           m_name = name;
           m_addr_size = size;
           m_faceId = faceId;
           m_datatype = datatype;

           if (m_datatype == 0)
           {
               m_data = new float[m_addr_size];
               m_handle = GCHandle.Alloc(m_data, GCHandleType.Pinned);
               m_addr = m_handle.AddrOfPinnedObject();
           }
           else if (m_datatype == 1)
           {
               m_data_int = new int[m_addr_size];
               m_handle = GCHandle.Alloc(m_data_int, GCHandleType.Pinned);
               m_addr = m_handle.AddrOfPinnedObject();
           }
           else
           {
               Debug.LogError("CFaceUnityCoefficientSet Error! Unknown datatype");
               return;
           }
       }
        /**
\brief 析构函数，GCHandle钉住的变量需要手动解除GC限制
\return 无
    */
        ~CFaceUnityCoefficientSet()
       {
           if (m_handle != null && m_handle.IsAllocated)
           {
               m_handle.Free();
               m_data = default(float[]);
               m_data_int = default(int[]);
           }
       }
        /**
\brief 需要逐帧，逐个跟踪信息调用，从而更新对应的数据
\return 无
    */
        public void Update()
       {
           fu_GetFaceInfo(m_faceId, m_addr, m_addr_size, m_name);
       }
        /**
\brief 如果数据长度发生变化，需要调用一下这个函数
\param num 跟踪数据的长度
\return 无
    */
        public void Update(int num)
       {
           if(num!= m_addr_size)
           {
               if (m_handle != null && m_handle.IsAllocated)
               {
                   m_handle.Free();
               }
               m_addr_size = num;
               if (m_datatype == 0)
               {
                   m_data = new float[m_addr_size];
                   m_handle = GCHandle.Alloc(m_data, GCHandleType.Pinned);
                   m_addr = m_handle.AddrOfPinnedObject();
               }
               else if (m_datatype == 1)
               {
                   m_data_int = new int[m_addr_size];
                   m_handle = GCHandle.Alloc(m_data_int, GCHandleType.Pinned);
                   m_addr = m_handle.AddrOfPinnedObject();
               }
               else
                   return;
           }
           Update();
       }
   }

   // Unity editor doesn't unload dlls after 'preview'

   #region DllImport

   /////////////////////////////////////
   //native interfaces

        //详细的接口描述请查看API文档！！！

   /**
   \brief Initialize and authenticate your SDK instance to the FaceUnity server, must be called exactly once before all other functions.
       The buffers should NEVER be freed while the other functions are still being called.
       You can call this function multiple times to "switch pointers".
   \param v3buf should point to contents of the "v3.bin" we provide
   \param v3buf_sz should point to num-of-bytes of the "v3.bin" we provide
   \param licbuf is the pointer to the authentication data pack we provide. You must avoid storing the data in a file.
       Normally you can just `#include "authpack.h"` and put `g_auth_package` here.
   \param licbuf_sz is the authenticafu_Cleartion data size, we use plain int to avoid cross-language compilation issues.
       Normally you can just `#include "authpack.h"` and put `sizeof(g_auth_package)` here.
   \return non-zero for success, zero for failure
   */
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
        [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_Setup(IntPtr v3buf, int v3buf_sz, IntPtr licbuf, int licbuf_sz);


    /**
    \brief offline authentication
	    Initialize and authenticate your SDK instance to the FaceUnity server, must be called exactly once before all other functions.
	    The buffers should NEVER be freed while the other functions are still being called.
	    You can call this function multiple times to "switch pointers".
    \param v3buf should point to contents of the "v3.bin" we provide
    \param v3buf_sz should point to num-of-bytes of the "v3.bin" we provide
    \param licbuf is the pointer to the authentication data pack we provide. You must avoid storing the data in a file.
	    Normally you can just `#include "authpack.h"` and put `g_auth_package` here.
    \param licbuf_sz is the authentication data size, we use plain int to avoid cross-language compilation issues.
	    Normally you can just `#include "authpack.h"` and put `sizeof(g_auth_package)` here.
    \param offline_bundle_ptr is the pointer to offline bundle from FaceUnity server
    \param offline_bundle_sz is size of offline bundle
    \return non-zero for success, zero for failure
    */
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetupLocal(IntPtr v3buf, int v3buf_sz, IntPtr licbuf, int licbuf_sz, IntPtr offline_bundle_ptr, int offline_bundle_sz);


    /**
    \brief 鉴权真正运行完毕后调用这个接口得到对应结果
         fu_SetupLocal并不是运行完就立即执行鉴权的，要等GL.IssuePluginEvent(fu_GetRenderEventFunc(), 1);注册后在GL线程真正执行相应代码
         具体跟离线鉴权相关的信息请询问技术支持
    \param 通过这个指针保存返回的签名完毕的离线bundle，后续用着bundle即可不联网鉴权
    \param 离线bundle长度
    \return 0为鉴权失败，1为成功
    */
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetOfflineBundle(ref IntPtr offline_bundle_ptr, IntPtr offline_bundle_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetNamaInited();

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_LoadTongueModel(IntPtr databuf, int databuf_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_LoadAIModelFromPackage(IntPtr databuf, int databuf_sz, int _type);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ReleaseAIModel(int _type);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_IsAIModelLoaded(int _type);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetExpressionCalibration(int enable);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_CreateItemFromPackage(IntPtr databuf, int databuf_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_DestroyItem(int itemid);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_DestroyAllItems();

    /**
\brief Destroy all internal data, resources, threads, etc.
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_DestroyLibData();

    /**
	\brief Render a list of items on top of a GLES texture or a memory buffer.
		This function needs a GLES 2.0+ context. 
		Render will do in PluginEvent fu_GetRenderEventFunc
	\param idxbuf points to the list of items
	\param idxbuf_sz is the number of items
	*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_setItemIds(IntPtr idxbuf, int idxbuf_sz, IntPtr mask);//mask can be null

    /**
\brief Set the default orientation for face detection. The correct orientation would make the initial detection much faster.
\param rotate_mode is the default orientation to be set to, one of 0..3 should work.
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetDefaultOrientation(int rotate_mode);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetDefaultRotationMode(int rotate_mode);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetDeviceOrientation(int rotate_mode);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetInputCameraMatrix(int flip_x, int flip_y, int rotate_mode);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetOutputResolution(int w, int h);

    /**
\brief Get certificate permission code for modules
\param i - get i-th code, currently available for 0 and 1
\return The permission code
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetModuleCode(int i);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetASYNCTrackFace(int i);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetMultiSamples(int samples);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetRuningMode(int runningMode);//refer to FURuningMode 

    /**
\brief Create Tex For Item
\param item specifies the item
\param name is the tex name
\param value is the tex rgba buffer to be set ,use GCHandle to get ptr
\param width is the tex width
\param height is the tex height
\return zero for failure, non-zero for success
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_CreateTexForItem(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int width, int height);

    /**
\brief Delete Tex For Item
\param item specifies the item
\param name is the parameter name
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_DeleteTexForItem(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name);

    /**
\brief Set an item parameter to a double value
\param item specifies the item
\param name is the parameter name
\param value is the parameter value to be set
\return zero for failure, non-zero for success
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemSetParamd(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, double value);

    /**
\brief Set an item parameter to a string value
\param item specifies the item
\param name is the parameter name
\param value is the parameter value to be set
\return zero for failure, non-zero for success
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemSetParams(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string value);

    /**
\brief Set an item parameter to a double array
\param item specifies the item
\param name is the parameter name
\param value points to an array of doubles
\param n specifies the number of elements in value
\return zero for failure, non-zero for success
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemSetParamdv(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemSetParamu8v(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemSetParamu64(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);



    /**
\brief Set an item parameter to a string value
\param item specifies the item
\param name is the parameter name
\param value is the parameter value to be set
\return zero for failure, non-zero for success
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern double fu_ItemGetParamd(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name);

    /**
\brief Get an item parameter as a string
\param item specifies the item
\param name is the parameter name
\param buf receives the string value
\param sz is the number of bytes available at buf
\return the length of the string value, or -1 if the parameter is not a string.
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemGetParams(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]byte[] buf, int buf_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemGetParamdv(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemGetParamu8v(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_ItemGetParamfv(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern IntPtr fu_GetRenderEventFunc();

    /**
* if plugin inited
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int jc_part_inited();

    /**
* SetUseNatCam(1);
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void SetUseNatCam(int enable);

    /**
* if true,Pause the render pipeline
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void SetPauseRender(bool ifpause);

    /**
\brief Get the face tracking status
\return The number of valid faces currently being tracked
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_IsTracking();

    /**
\brief Set the maximum number of faces we track. The default value is 1.
\param n is the new maximum number of faces to track
\return The previous maximum number of faces tracked
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetMaxFaces(int num);

    /**
\brief Set the quality-performance tradeoff. 
\param quality is the new quality value. 
       It's a floating point number between 0 and 1.
       Use 0 for maximum performance and 1 for maximum quality.
       The default quality is 1 (maximum quality).
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetQualityTradeoff(float num);

    /**
\brief Get Nama version string
\return Nama version string in const char*
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern IntPtr fu_GetVersion(); // Marshal.PtrToStringAnsi(fu_GetVersion());

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetVersionMajor();

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetVersionMinor();

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetVersionFix();

    /**
\brief Get Faceplugin version string
\return Faceplugin version string in const char*
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern IntPtr fu_GetFacepluginVersion(); // Marshal.PtrToStringAnsi(fu_GetFacepluginVersion());

    /**
\brief Get system error, which causes system shutting down
\return System error code represents one or more errors	
	Error code can be checked against following bitmasks, non-zero result means certain error
	This interface is not a callback, needs to be called on every frame and check result, no cost
	Inside authentication error (NAMA_ERROR_BITMASK_AUTHENTICATION), meanings for each error code are listed below:
	1 failed to seed the RNG
	2 failed to parse the CA cert
	3 failed to connect to the server
	4 failed to configure TLS
	5 failed to parse the client cert
	6 failed to parse the client key
	7 failed to setup TLS
	8 failed to setup the server hostname
	9 TLS handshake failed
	10 TLS verification failed
	11 failed to send the request
	12 failed to read the response
	13 bad authentication response
	14 incomplete authentication palette info
	15 not inited yet
	16 failed to create a thread
	17 authentication package rejected
	18 void authentication data
	19 bad authentication package
	20 certificate expired
	21 invalid certificate
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetSystemError();

    /**
\brief Interpret system error code
\param code - System error code returned by fuGetSystemError()
\return One error message from the code
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern IntPtr fu_GetSystemErrorString(int code); // Marshal.PtrToStringAnsi();

    /**
\brief Call this function when the GLES context has been lost and recreated.
    That isn't a normal thing, so this function could leak resources on each call.
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_OnDeviceLost();

    /**
\brief Call this function to reset the face tracker on camera switches
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_OnCameraChange();


    /**
\brief clear camera frame data
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void ClearImages();

    /**
     * 翻转输入的纹理，仅对使用了natcam的安卓平台有效
     * natcam的安卓平台使用了SetDualInput，有些安卓平台nv21buf和tex的方向不一致，可以用这个接口设置tex的镜像。
     */
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int SetFlipTexMarkX(bool mark);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int SetFlipTexMarkY(bool mark);

    /**
\brief provide camera frame data
        flags: FU_ADM_FLAG_FLIP_X = 32;
               FU_ADM_FLAG_FLIP_Y = 64; 翻转只翻转道具渲染，并不会翻转整个图像
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int SetImage(IntPtr imgbuf,int flags, bool isbgra, int w, int h);

    /**
\brief provide camera frame data android nv21 and texture id
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int SetDualInput(IntPtr nv21buf, int texid, int flags, int w, int h);

    /**
\brief provide camera frame data android nv21,only support Android.
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int SetNV21Input(IntPtr nv21buf, int flags, int w, int h);

    /**
\brief provide camera frame data via texid
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int SetImageTexId(int texid, int flags, int w, int h);

    /**
\brief Enable internal Log
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_EnableLog(bool isenable);

    /**
\brief get Rendered texture id, can be recreated in unity
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetNamaTextureId();

    /**
  \brief Get the unique identifier for each face during current tracking
    Lost face tracking will change the identifier, even for a quick retrack
  \param face_id is the id of face, index is smaller than which is set in fuSetMaxFaces
    If this face_id is x, it means x-th face currently tracking
  \return the unique identifier for each face
  */
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetFaceIdentifier(int face_id);

    /**
\brief Scale the rendering perspectivity (focal length, or FOV)
   Larger scale means less projection distortion
   This scale should better be tuned offline, and set it only once in runtime
\param scale - default is 1.f, keep perspectivity invariant
   <= 0.f would be treated as illegal input and discard	
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern void fu_SetFocalLengthScale(float scale);

#if !UNITY_IOS
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    private static extern void RegisterDebugCallback(DebugCallback callback);
#endif

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_GetFaceInfo(int face_id, IntPtr ret, int szret, [MarshalAs(UnmanagedType.LPStr)]string name);

    /**
     * FURuningMode为FU_Mode_RenderItems的时候，加载EmptyItem.bytes，才能开启人脸跟踪。
     * FURuningMode为FU_Mode_TrackFace的时候，调用fu_SetTongueTracking(1)，才能开启舌头跟踪。注意，每次切换到FU_Mode_TrackFace之后都需要设置一次！！！
\brief Turn on or turn off Tongue Tracking, used in trackface.
\param enable > 0 means turning on, enable <= 0 means turning off
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetTongueTracking(int enable);

    /**
\brief Set a face detector parameter.
\param detector is the detector context, currently it is allowed to set to NULL, i.e., globally set all contexts.
\param name is the parameter name, it can be:
        "use_new_cnn_detection": 1 if the new cnn-based detection method is used, 0 else
        "other_face_detection_frame_step": if one face already exists, then we detect other faces not each frame, but with a step,default 10 frames
        if use_new_cnn_detection == 1, then
            "min_facesize_small", int[default=18]: minimum size to detect a small face; must be called **BEFORE** fuSetup
            "min_facesize_big", int[default=27]: minimum size to detect a big face; must be called **BEFORE** fuSetup
            "small_face_frame_step", int[default=5]: the frame step to detect a small face; it is time cost, thus we do not detect each frame
            "use_cross_frame_speedup", int[default=0]: perform a half-cnn inference each frame to speedup
            "enable_large_pose_detection", int[default=1]: enable rotated face detection up to 45^deg roll in each rotation mode.
        else
            "scaling_factor": the scaling across image pyramids, default 1.2f
            "step_size": the step of each sliding window, default 2.f
            "size_min": minimal face supported on 640x480 image, default 50.f
            "size_max": maximal face supported on 640x480 image, default is a large value
            "min_neighbors": algorithm internal, default 3.f
            "min_required_variance": algorithm internal, default 15.f
\param value points to the new parameter value, e.g., 
        float scaling_factor=1.2f;
        dde_facedet_set(ctx, "scaling_factor", &scaling_factor);
        float size_min=100.f;
        dde_facedet_set(ctx, "size_min", &size_min);
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetFaceDetParam([MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value);


    /**
\brief Set the global face tracker parameter.
\param name is the parameter name, it can be:
	"mouth_expression_more_flexible": \in [0, 1], default=0; additionally make mouth expression more flexible.
	"expression_calibration_strength": \in [0, 1], default=0.2; strenght of expression soft calibration.
\param value points to the new parameter value, e.g., 
	float mouth_expression_more_flexible=0.6f;
	dde_facetrack_set("mouth_expression_more_flexible", &mouth_expression_more_flexible);
*/
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("faceplugin", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int fu_SetFaceTrackParam([MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value);

    #endregion

    public enum FURuningMode
    {
        FU_Mode_None = 0,
        FU_Mode_RenderItems, //face tracking and render item (beautify is one type of item) ,item means 'daoju'
        FU_Mode_Beautification,//non face tracking, beautification only.
        FU_Mode_Masked,//when tracking multi-people, different perple　can use different item, give mask in function fu_setItemIds  
        FU_Mode_TrackFace//tracking face only, get face infomation, but do not render item.it's very fast.
    };

    public static FURuningMode currentMode = FURuningMode.FU_Mode_None;

    public static void SetRunningMode(FURuningMode mode)
    {
        currentMode = mode;
        fu_SetRuningMode((int)mode);
    }

    public enum FUAITYPE
    {
        FUAITYPE_BACKGROUNDSEGMENTATION = 1 << 1,
        FUAITYPE_HAIRSEGMENTATION = 1 << 2,
        FUAITYPE_HANDGESTURE = 1 << 3,
        FUAITYPE_TONGUETRACKING = 1 << 4,
        FUAITYPE_FACELANDMARKS75 = 1 << 5,
        FUAITYPE_FACELANDMARKS209 = 1 << 6,
        FUAITYPE_FACELANDMARKS239 = 1 << 7,
        FUAITYPE_HUMANPOSE2D = 1 << 8,
        FUAITYPE_BACKGROUNDSEGMENTATION_GREEN = 1 << 9,
        FUAITYPE_FACEPROCESSOR = 1 << 10
    }

    public enum FUAI_CAMERA_VIEW
    {
        ROT_0 = 0,
        ROT_90 = 1,
        ROT_180 = 2,
        ROT_270 = 3,
    }

    public static void FixRotation(bool ifMirrored = false, FUAI_CAMERA_VIEW eyeViewRot = FUAI_CAMERA_VIEW.ROT_0)
    {
        var rot = FUAI_CAMERA_VIEW.ROT_0;
#if UNITY_EDITOR || UNITY_STANDALONE
        rot = FUAI_CAMERA_VIEW.ROT_180;
#endif
#if !UNITY_EDITOR && UNITY_ANDROID
        if (ifMirrored)
        {
            switch (eyeViewRot)
            {
                case FUAI_CAMERA_VIEW.ROT_0:
                    rot = FUAI_CAMERA_VIEW.ROT_270;
                    break;
                case FUAI_CAMERA_VIEW.ROT_90:
                    rot = FUAI_CAMERA_VIEW.ROT_180;
                    break;
                case FUAI_CAMERA_VIEW.ROT_180:
                    rot = FUAI_CAMERA_VIEW.ROT_90;
                    break;
                case FUAI_CAMERA_VIEW.ROT_270:
                    rot = FUAI_CAMERA_VIEW.ROT_0;
                    break;
            }
        }
        else
        {
            switch (eyeViewRot)
            {
                case FUAI_CAMERA_VIEW.ROT_0:
                    rot = FUAI_CAMERA_VIEW.ROT_90;
                    break;
                case FUAI_CAMERA_VIEW.ROT_90:
                    rot = FUAI_CAMERA_VIEW.ROT_180;
                    break;
                case FUAI_CAMERA_VIEW.ROT_180:
                    rot = FUAI_CAMERA_VIEW.ROT_270;
                    break;
                case FUAI_CAMERA_VIEW.ROT_270:
                    rot = FUAI_CAMERA_VIEW.ROT_0;
                    break;
            }
        }
#endif
#if !UNITY_EDITOR && UNITY_IOS
        switch (eyeViewRot)
        {
            case FUAI_CAMERA_VIEW.ROT_0:
                rot = FUAI_CAMERA_VIEW.ROT_180;
                break;
            case FUAI_CAMERA_VIEW.ROT_90:
                rot = FUAI_CAMERA_VIEW.ROT_270;
                break;
            case FUAI_CAMERA_VIEW.ROT_180:
                rot = FUAI_CAMERA_VIEW.ROT_0;
                break;
            case FUAI_CAMERA_VIEW.ROT_270:
                rot = FUAI_CAMERA_VIEW.ROT_90;
                break;
        }
#endif
        fu_SetDefaultRotationMode((int)rot);
    }

    ///////////////////////////////
    //public int m_camera_native_texid = 0;
    //singleton checks
    public static FaceunityWorker instance = null;
    void Awake()
    {
        //singleton checks
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

    }
    ///////////////////////////////
    //persistent data, DO NOT EVER FREE ANY OF THESE!
    //we must keep the GC handles to keep the arrays pinned to the same addresses
    [HideInInspector]
    public bool m_plugin_inited = false;

    public int MAXFACE = 1;
    public bool EnableExpressionLoop = true;
    int MaxExpression = 0;
    public string LICENSE = "";

    [HideInInspector]
    public int m_need_update_facenum = 0;
    public List<CFaceUnityCoefficientSet> m_translation = new List<CFaceUnityCoefficientSet>();//("translation", 3); //3D translation of face in camera space - 3 float
    public List<CFaceUnityCoefficientSet> m_rotation = new List<CFaceUnityCoefficientSet>();//("rotation", 4); //rotation quaternion - 4 float
    public List<CFaceUnityCoefficientSet> m_rotation_mode = new List<CFaceUnityCoefficientSet>();//("rotation_mode", 1); //the relative orientaion of face agains phone, 0-3 - 1 float
    //public List<CFaceUnityCoefficientSet> m_expression = new List<CFaceUnityCoefficientSet>();//("expression", 46);
    public List<CFaceUnityCoefficientSet> m_expression_with_tongue = new List<CFaceUnityCoefficientSet>();//("expression_with_tongue", 56);
    //public List<CFaceUnityCoefficientSet> m_landmarks = new List<CFaceUnityCoefficientSet>();//("landmarks",75*2); //2D landmarks coordinates in image space - 75*2 float
    //public List<CFaceUnityCoefficientSet> m_landmarks_ar = new List<CFaceUnityCoefficientSet>();//("landmarks_ar",75*3); //3D landmarks coordinates in camera space - 75*3 float
    //public List<CFaceUnityCoefficientSet> m_projection_matrix = new List<CFaceUnityCoefficientSet>();//("projection_matrix",16); //the transform matrix from camera space to image space - 16 float
    //public List<CFaceUnityCoefficientSet> m_eye_rotation = new List<CFaceUnityCoefficientSet>();//("eye_rotation",4); //eye rotation quaternion - 4 float
    //public List<CFaceUnityCoefficientSet> m_face_rect = new List<CFaceUnityCoefficientSet>();//("face_rect",4); //the rectangle of tracked face in image space, (xmin,ymin,xmax,ymax) - 4 float
    //public List<CFaceUnityCoefficientSet> m_failure_rate = new List<CFaceUnityCoefficientSet>();//("failure_rate",1); //the failure rate of face tracking, the less the more confident about tracking result - 1 float
    public List<CFaceUnityCoefficientSet> m_pupil_pos = new List<CFaceUnityCoefficientSet>();//("pupil_pos", 4);
    public List<CFaceUnityCoefficientSet> m_focallength = new List<CFaceUnityCoefficientSet>();//("focal_length", 1);

    //public List<CFaceUnityCoefficientSet> m_armesh_vertex_num = new List<CFaceUnityCoefficientSet>();
    //public List<CFaceUnityCoefficientSet> m_armesh_vertices = new List<CFaceUnityCoefficientSet>();
    //public List<CFaceUnityCoefficientSet> m_armesh_uvs = new List<CFaceUnityCoefficientSet>();
    //public List<CFaceUnityCoefficientSet> m_armesh_face_num = new List<CFaceUnityCoefficientSet>();
    //public List<CFaceUnityCoefficientSet> m_armesh_faces = new List<CFaceUnityCoefficientSet>();

    public static float FocalLengthScale = 1f;

    public static event EventHandler OnInitOK;
    private delegate void DebugCallback(string message);

    GCHandle m_licdata_handle;
    GCHandle m_v3data_handle;

    ///////////////////////////////
    /**
\brief 初始化所有跟踪信息
\param maxface 最多几张脸
\return 无
        */
    void InitCFaceUnityCoefficientSet(int maxface)
    {
        if (MaxExpression < maxface)
            for (int i = MaxExpression; i < maxface; i++)
            {
                m_translation.Add(new CFaceUnityCoefficientSet("translation", 3, i));
                m_rotation.Add(new CFaceUnityCoefficientSet("rotation", 4, i));
                m_rotation_mode.Add(new CFaceUnityCoefficientSet("rotation_mode", 1, i));
                //m_expression.Add(new CFaceUnityCoefficientSet("expression", 46, i));
                m_expression_with_tongue.Add(new CFaceUnityCoefficientSet("expression_with_tongue", 56, i));
                m_pupil_pos.Add(new CFaceUnityCoefficientSet("pupil_pos", 4, i));
                m_focallength.Add(new CFaceUnityCoefficientSet("focal_length", 1, i));
                //m_landmarks.Add(new CFaceUnityCoefficientSet("landmarks", 75 * 2, i));

                //m_armesh_vertex_num.Add(new CFaceUnityCoefficientSet("armesh_vertex_num", 1, i, 1));
                //m_armesh_vertices.Add(new CFaceUnityCoefficientSet("armesh_vertices", 1, i));   //这个长度值需要动态更新
                //m_armesh_uvs.Add(new CFaceUnityCoefficientSet("armesh_uvs", 1, i));
                //m_armesh_face_num.Add(new CFaceUnityCoefficientSet("armesh_face_num", 1, i, 1));
                //m_armesh_faces.Add(new CFaceUnityCoefficientSet("armesh_faces", 1, i, 1));
            }
        else if (MaxExpression > maxface)
            for (int i = maxface; i < MaxExpression; i++)
            {
                m_translation.RemoveAt(i);
                m_rotation.RemoveAt(i);
                m_rotation_mode.RemoveAt(i);
                //m_expression.RemoveAt(i);
                m_expression_with_tongue.RemoveAt(i);
                m_pupil_pos.RemoveAt(i);
                m_focallength.RemoveAt(i);
                //m_landmarks.RemoveAt(i);

                //m_armesh_vertex_num.RemoveAt(i);
                //m_armesh_vertices.RemoveAt(i);
                //m_armesh_uvs.RemoveAt(i);
                //m_armesh_face_num.RemoveAt(i);
                //m_armesh_faces.RemoveAt(i);
            }
        MaxExpression = maxface;
    }

    /**
\brief 初始化SDK并设置部分参数，同时开启驱动SDK渲染的GL循环协程
\return 无
    */
    IEnumerator Start()
    {
        if (EnvironmentCheck())
        {
            Debug.Log("jc_part_inited:   " + jc_part_inited());
            if (m_plugin_inited == false)
            {
                Debug.LogFormat("FaceunityWorker Init");
#if UNITY_EDITOR && !UNITY_IOS
                RegisterDebugCallback(new DebugCallback(DebugMethod));
#endif
                fu_EnableLog(false);
                ClearImages();

                //fu_Setup init nama sdk
                if (jc_part_inited() == 0)  //防止Editor下二次Play导致崩溃的bug
                {
                    //load license data
                    if (LICENSE == null || LICENSE == "")
                    {
                        Debug.LogError("LICENSE is null! please paste the license data to the TextField named \"LICENSE\" in FaceunityWorker");
                    }
                    else
                    {
                        sbyte[] m_licdata_bytes;
                        string[] sbytes = LICENSE.Split(',');
                        if (sbytes.Length <= 7)
                        {
                            Debug.LogError("License Format Error");
                        }
                        else
                        {
                            m_licdata_bytes = new sbyte[sbytes.Length];
                            Debug.LogFormat("length:{0}", sbytes.Length);
                            for (int i = 0; i < sbytes.Length; i++)
                            {
                                //Debug.Log(sbytes[i]);
                                m_licdata_bytes[i] = sbyte.Parse(sbytes[i]);
                                //Debug.Log(m_licdata_bytes[i]);
                            }
                            if (m_licdata_handle.IsAllocated)
                                m_licdata_handle.Free();
                            m_licdata_handle = GCHandle.Alloc(m_licdata_bytes, GCHandleType.Pinned);
                            IntPtr licptr = m_licdata_handle.AddrOfPinnedObject();

                            //load nama sdk data
                            string fnv3 = Util.GetStreamingAssetsPath() + "/faceunity/v3.bytes";
                            WWW v3data = new WWW(fnv3);
                            yield return v3data;
                            byte[] m_v3data_bytes = v3data.bytes;
                            if (m_v3data_handle.IsAllocated)
                                m_v3data_handle.Free();
                            m_v3data_handle = GCHandle.Alloc(m_v3data_bytes, GCHandleType.Pinned); //pinned avoid GC
                            IntPtr v3ptr = m_v3data_handle.AddrOfPinnedObject(); //pinned addr
                            fu_Setup(v3ptr, m_v3data_bytes.Length, licptr, sbytes.Length); //要查看license是否有效请打开插件log（fu_EnableLog(true);）
                            m_plugin_inited = true;
                        }
                    }
                }
                else
                {
                    fu_OnDeviceLost();  //清理残余，防止崩溃
                    m_plugin_inited = true;
                }

                if (m_plugin_inited == true)
                {
                    var v = GCHandle.Alloc(0.5f, GCHandleType.Pinned);
                    IntPtr vptr = v.AddrOfPinnedObject();
                    fu_SetFaceTrackParam("mouth_expression_more_flexible", vptr);
                    v.Free();

                    //fu_SetASYNCTrackFace(0);    //异步人脸跟踪，部分平台能提升性能，默认关闭
                    //fu_SetMultiSamples(4);
                    SetRunningMode(FURuningMode.FU_Mode_RenderItems);   //默认模式，随时可以改
                    SetUseNatCam(1);  //默认选项，仅安卓有效
                    fu_SetFocalLengthScale(FocalLengthScale);   //默认值是1
                    Debug.LogFormat("fu_SetFocalLengthScale({0})", FocalLengthScale);

                    StartCoroutine(GLLoop());
                }
            }
        }
        else
        {
            Debug.LogError("please check your Graphics API,this plugin only supports OpenGL!");
            yield break;
        }
    }



    private IEnumerator GLLoop()
    {
        yield return Util._endOfFrame;
        ////////////////////////////////
        fu_SetMaxFaces(MAXFACE);
        GL.IssuePluginEvent(fu_GetRenderEventFunc(), 1);// cal for sdk render

        if (m_licdata_handle.IsAllocated)
            m_licdata_handle.Free();
        if (m_v3data_handle.IsAllocated)
            m_v3data_handle.Free();

        yield return LoadAIBundle("ai_face_processor.bytes", FUAITYPE.FUAITYPE_FACEPROCESSOR);
        yield return LoadAIBundle("ai_bgseg.bytes", FUAITYPE.FUAITYPE_BACKGROUNDSEGMENTATION);
        //yield return LoadAIBundle("ai_bgseg_green.bytes", FUAITYPE.FUAITYPE_BACKGROUNDSEGMENTATION_GREEN);
        yield return LoadAIBundle("ai_facelandmarks75.bytes", FUAITYPE.FUAITYPE_FACELANDMARKS75);
        yield return LoadAIBundle("ai_facelandmarks209.bytes", FUAITYPE.FUAITYPE_FACELANDMARKS209);
        yield return LoadAIBundle("ai_facelandmarks239.bytes", FUAITYPE.FUAITYPE_FACELANDMARKS239);
        yield return LoadAIBundle("ai_gesture.bytes", FUAITYPE.FUAITYPE_HANDGESTURE);
        //yield return LoadAIBundle("ai_hairseg.bytes", FUAITYPE.FUAITYPE_HAIRSEGMENTATION); 
        //yield return LoadAIBundle("ai_humanpose.bytes", FUAITYPE.FUAITYPE_HUMANPOSE2D);
        yield return LoadTongueBundle("tongue.bytes");

        if (OnInitOK != null)
            OnInitOK(this, null);//触发初始化完成事件

        //Debug.Log("错误:" + fu_GetSystemError() +","+ Marshal.PtrToStringAnsi(fu_GetSystemErrorString(fu_GetSystemError())));
        Debug.Log("Nama Version:" + Marshal.PtrToStringAnsi(fu_GetVersion()));
        Debug.Log("Faceplugin Version:" + Marshal.PtrToStringAnsi(fu_GetFacepluginVersion()));

        yield return CallPluginAtEndOfFrames();
    }

    /**\brief SDK渲染的GL循环协程，每一帧的末尾，调用GL.IssuePluginEvent使Unity执行SDK内部的渲染代码，同时获取并保存跟踪信息\return 无    */
    private IEnumerator CallPluginAtEndOfFrames()
    {
        while (true)
        {
            yield return Util._endOfFrame;
            ////////////////////////////////
            fu_SetMaxFaces(MAXFACE);
            GL.IssuePluginEvent(fu_GetRenderEventFunc(), 1);// cal for sdk render
            if (EnableExpressionLoop)
            {
                if (MaxExpression != MAXFACE)
                    InitCFaceUnityCoefficientSet(MAXFACE);
                //only update other stuff when there is new data
                int num = fu_IsTracking();
                m_need_update_facenum = num < MAXFACE ? num : MAXFACE;
                for (int i = 0; i < m_need_update_facenum; i++)
                {
                    //m_armesh_vertex_num[i].Update();
                    //m_armesh_vertices[i].Update(m_armesh_vertex_num[i].m_data_int[0] * 3);
                    //m_armesh_uvs[i].Update(m_armesh_vertex_num[i].m_data_int[0] * 2);
                    //m_armesh_face_num[i].Update();
                    //m_armesh_faces[i].Update(m_armesh_face_num[i].m_data_int[0] * 3);

                    m_translation[i].Update();
                    m_rotation[i].Update();
                    m_rotation_mode[i].Update();
                    //m_landmarks[i].Update();
                    m_pupil_pos[i].Update();
                    m_focallength[i].Update();
                    ////////////////////////

                    m_expression_with_tongue[i].Update();
                    m_expression_with_tongue[i].m_data[6] = m_expression_with_tongue[i].m_data[7] = m_pupil_pos[i].m_data[0];
                    m_expression_with_tongue[i].m_data[10] = m_expression_with_tongue[i].m_data[11] = -m_pupil_pos[i].m_data[0];
                    m_expression_with_tongue[i].m_data[12] = m_expression_with_tongue[i].m_data[13] = m_pupil_pos[i].m_data[1];
                    m_expression_with_tongue[i].m_data[4] = m_expression_with_tongue[i].m_data[5] = -m_pupil_pos[i].m_data[1];

                    //m_expression[i].Update();
                    //m_expression[i].m_data[6] = m_expression[i].m_data[7] = m_pupil_pos[i].m_data[0];
                    //m_expression[i].m_data[10] = m_expression[i].m_data[11] = -m_pupil_pos[i].m_data[0];
                    //m_expression[i].m_data[12] = m_expression[i].m_data[13] = m_pupil_pos[i].m_data[1];
                    //m_expression[i].m_data[4] = m_expression[i].m_data[5] = -m_pupil_pos[i].m_data[1];
                }
            }
        }
    }





    /**\brief 用来注册SDK的LOG回调，SDK中间层可以用这个来在Unity内部打log\param message 要打的log\return 无    */
    private void DebugMethod(string message)
    {
        Debug.Log("From Dll: " + message);
    }


    private IEnumerator LoadAIBundle(string name,FUAITYPE type)
    {
        if (fu_IsAIModelLoaded((int)type) == 0)
        {
            string bundle = Util.GetStreamingAssetsPath() + "/faceunity/" + name;
            WWW bundledata = new WWW(bundle);
            yield return bundledata;
            byte[] bundle_bytes = bundledata.bytes;
            var bundle_handle = GCHandle.Alloc(bundle_bytes, GCHandleType.Pinned);
            IntPtr bundledataptr = bundle_handle.AddrOfPinnedObject();

            fu_LoadAIModelFromPackage(bundledataptr, bundle_bytes.Length, (int)type);

            bundle_handle.Free();
        }
    }

    private IEnumerator LoadTongueBundle(string name)
    {
        string bundle = Util.GetStreamingAssetsPath() + "/faceunity/" + name;
        WWW bundledata = new WWW(bundle);
        yield return bundledata;
        byte[] bundle_bytes = bundledata.bytes;
        var bundle_handle = GCHandle.Alloc(bundle_bytes, GCHandleType.Pinned);
        IntPtr bundledataptr = bundle_handle.AddrOfPinnedObject();

        fu_LoadTongueModel(bundledataptr, bundle_bytes.Length);

        bundle_handle.Free();
    }


    /**\brief 应用退出是清理相关GCHandle和SDK相关数据\return 无*/
    private void OnApplicationQuit()
    {
        if (m_licdata_handle.IsAllocated)
            m_licdata_handle.Free();
        if (m_v3data_handle.IsAllocated)
            m_v3data_handle.Free();
        if (m_plugin_inited == true)
            fu_OnDeviceLost();
        ClearImages();
    }


    /**\brief 检测当前渲染环境是否符合要求，本SDK仅支持在OpenGL下运行\return true为检测通过，false为不通过*/
    bool EnvironmentCheck()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore
            || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGL2)
            return true;
        else
            return false;
#elif UNITY_STANDALONE_OSX||UNITY_EDITOR_OSX
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore
            || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGL2)
            return true;
        else
            return false;
#elif UNITY_ANDROID
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3
            || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
            return true;
        else
            return false;
#elif UNITY_IOS
        if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3
            || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2)
            return true;
        else
            return false;
#endif
    }
}
