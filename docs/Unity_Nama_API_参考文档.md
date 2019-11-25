# Unity Nama C# API 参考文档
级别：Public
更新日期：2019-09-25 
SDK版本: 6.4.0 

------

## 最新更新内容：

2019-09-25 v6.4.0:

- v6.4.0 接口无变动。

2019-08-14 v6.3.0:

- 新增fuSetFaceTrackParam函数，用于设置人脸跟踪参数。

2019-06-27 v6.2.0:

- fu_SetFaceDetParam函数增加可设置参数。

2019-05-27 v6.1.0:

- 新增fu_SetupLocal函数，支持离线鉴权。
- 新增fu_DestroyLibData函数，支持tracker内存释放。

2019-04-28 v6.0.0:

- 新增fu_SetFaceDetParam函数，用于设置人脸检测参数。
- 更新了fu_Setup函数。

------
### 目录：
本文档内容目录：

[TOC]

------
### 1. 简介 
本文是相芯人脸跟踪及视频特效开发包(Nama SDK)的底层接口文档。该文档中的 Nama API 为 FaceunityWorker.cs 中的接口，可以直接用于 Unity 上的开发。其中，部分Unity演示场景中也提供了一些二次封装好的接口，相比本文中的接口会更贴近实际的开发。

Unity使用的SDK是在原始的Nama SDK上封装了一层GL调用层，这个封装使得Nama SDK的GL环境得以和Unity的GL环境同步，具体可以 [参考这里](https://docs.unity3d.com/Manual/NativePluginInterface.html)。

以下文字中原始Nama SDK简称**Nama**，GL调用层简称**FacePlugin**，合称**UnityNamaSDK**。

SDK相关的所有调用要求在同一个线程中顺序执行，不支持多线程。为了同步UnityNamaSDK和Unity的GL环境，部分接口调用后不会立即生效，而是等到当前Unity生命周期末尾，初始化完成后开启的每帧执行的协程会调用GL事件从而执行当前Unity生命周期中所调用的部分接口，具体会在API内容中说明。

如果需要用到SDK的绘制功能，则需要Unity开启OpenGL渲染模式，没有开启或开启不正确会导致崩溃。我们对OpenGL的环境要求为 GLES 2.0 以上。具体调用方式，可以参考FULiveUnity Demo。如果不需要使用SDK的绘制功能，可以咨询技术支持如何直接调用Nama。

底层接口根据作用逻辑归为五类：初始化、主运行接口、加载销毁道具、功能接口、C#端辅助接口。

------
### 2. APIs
#### 2.1 初始化
##### fu_Setup 函数
初始化系统环境，加载系统数据，并进行网络鉴权。必须在调用UnityNamaSDK其他接口前执行，否则会引发崩溃。

```c#
public static extern int fu_Setup(IntPtr v3buf, int v3buf_sz, IntPtr licbuf, int licbuf_sz);
```

__参数:__

*v3buf*: v3.bytes中读取的二进制数据的指针

*v3buf_sz*:  v3.bytes的长度

*licbuf*：证书的文本数据，用`,`分割，将分割后的数组数据转成sbyte格式

*licbuf_sz[in]*：sbyte数组长度

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

---

##### fu_SetupLocal 函数

功能和fu_Setup类似，但是用于离线鉴权。

这个接口的原理是，首次鉴权时输入一个设备独有的原始签名bundle，这次鉴权要求有网络连接，签名完毕可以通过fu_GetOfflineBundle获取签名后的bundle，如果签名成功，则后续鉴权只需要使用这个bundle代替原始离线bundle，输入fu_SetupLocal即可实现离线鉴权。

更详细的的信息请咨询技术支持。

```c#
public static extern int fu_SetupLocal(IntPtr v3buf, int v3buf_sz, IntPtr licbuf, int licbuf_sz, IntPtr offline_bundle_ptr, int offline_bundle_sz);
```

__参数:__

*v3buf*: v3.bytes中读取的二进制数据的指针

*v3buf_sz*:  v3.bytes的长度

*licbuf*：证书的文本数据，用`,`分割，将分割后的数组数据转成sbyte格式

*licbuf_sz[in]*：sbyte数组长度

offline_bundle_ptr：原始离线bundle的指针

offline_bundle_sz：原始离线bundle的长度

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

---

##### fu_GetOfflineBundle 函数

返回签名完毕的离线bundle（不一定签名成功）。

```c#
public static extern int fu_GetOfflineBundle(ref IntPtr offline_bundle_ptr, IntPtr offline_bundle_sz);
```

__参数:__

*offline_bundle_ptr*: 离线bundle的指针

*offline_bundle_sz*:  离线bundle的长度

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### jc_part_inited 函数
返回值表示UnityNamaSDK初始化PART1是否成功。这个接口主要用于防止二次初始化UnityNamaSDK，这会导致程序崩溃。

```c#
public static extern int jc_part_inited();
```

__参数:__

无

__返回值:__

当返回1表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------

##### fu_GetNamaInited 函数
返回值表示UnityNamaSDK初始化PART2是否成功。PART1和PART2均成功时，UnityNamaSDK才真正初始化成功。

```c#
public static extern int fu_GetNamaInited();
```

__参数:__

无

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------

##### ~~fu_LoadExtendedARData 函数~~

**已弃用**

```c#
public static extern int fu_LoadExtendedARData(IntPtr databuf, int databuf_sz);
```

------

##### ~~fu_LoadAnimModel 函数~~

**已弃用**

```c#
public static extern int fu_LoadAnimModel(IntPtr databuf, int databuf_sz);
```

------

##### fu_LoadTongueModel 函数

加载舌头跟踪需要的数据文件

```c#
public static extern int fu_LoadTongueModel(IntPtr databuf, int databuf_sz);
```

__参数:__

*databuf*:  tongue.bytes中读取的二进制数据的指针
*databuf_sz*:  tongue.bytes的长度

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

---



#### 2.2 主运行接口

##### SetImage 函数

图像数据输入接口之一。输入RGBA格式的buffer数组，通用性最强，各平台均可使用。

```c#
 public static extern int SetImage(IntPtr imgbuf,int flags, bool isbgra, int w, int h);
```

__参数:__

*imgbuf*: RGBA格式的buffer数组指针
*flags*: 数据输入的标志位
*isbgra*: 0表示RGBA格式，1表示BGRA格式
*w*: 图像宽
*h*: 图像高

```
flags: FU_ADM_FLAG_FLIP_X = 32;
       FU_ADM_FLAG_FLIP_Y = 64; 翻转只翻转道具渲染，并不会翻转整个图像
```

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### SetDualInput 函数

图像数据输入接口之一。同时输入NV21格式的buffer数组和纹理ID，通常在安卓设备上，通过原生相机插件获取相应数据来使用，效率最高。

```c#
 public static extern int SetDualInput(IntPtr nv21buf, int texid, int flags, int w, int h);
```

__参数:__

*nv21buf*: NV21格式的buffer数组指针
*texid*: RGBA格式的纹理ID
*flags*: 数据输入的标志位，参数同SetImage
*w*: 图像宽
*h*: 图像高

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### SetNV21Input 函数

图像数据输入接口之一。输入NV21格式的buffer数组，通常在安卓设备上，通过原生相机插件获取相应数据来使用。

```c#
 public static extern int SetNV21Input(IntPtr nv21buf, int flags, int w, int h);
```

__参数:__

*nv21buf*: NV21格式的buffer数组指针
*flags*: 数据输入的标志位，参数同SetImage
*w*: 图像宽
*h*: 图像高

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### SetImageTexId 函数

图像数据输入接口之一。输入GL纹理ID，某些特殊GL环境下无法使用，但是一定程度上性能高于Image。

```c#
 public static extern int SetImageTexId(int texid, int flags, int w, int h);
```

__参数:__

*texid*: RGBA格式的纹理ID
*flags*: 数据输入的标志位，参数同SetImage
*w*: 图像宽
*h*: 图像高

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

---

##### fu_SetRuningMode 函数

设置UnityNamaSDK的运行模式

```c#
public static extern int fu_SetRuningMode(int runningMode);

public enum FURuningMode
    {
        FU_Mode_None = 0,
        FU_Mode_RenderItems, 
        FU_Mode_Beautification,
        FU_Mode_Masked,
        FU_Mode_TrackFace
    };
```

__参数:__

*runningMode*: 运行模式

```
- FU_Mode_None：停止渲染
- FU_Mode_RenderItems：开启人脸跟踪和渲染道具
- FU_Mode_Beautification：只开启美颜
- FU_Mode_Masked ：使用fu_setItemIds设置好Mask后，开启这个模式即可生效
- FU_Mode_TrackFace：只开启人脸跟踪，速度最快
```

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### fu_GetRenderEventFunc 函数

**SDK调用的核心逻辑** 这个函数返回的值配合GL.IssuePluginEvent即可运行SDK，具体原因请[参考这里](https://docs.unity3d.com/Manual/NativePluginInterface.html) 

```c#
 public static extern IntPtr fu_GetRenderEventFunc();
 GL.IssuePluginEvent(fu_GetRenderEventFunc(), 1);	//调用示例
```

__参数:__

无

__返回值:__

SDK运行函数的指针

__备注:__

这个接口会**立即**生效。

---

##### fu_GetNamaTextureId 函数

获取本插件渲染完毕的图像的纹理ID。

```c#
 public static extern int fu_GetNamaTextureId();
```

__参数:__

无

__返回值:__

GL纹理ID

__备注:__

这个接口会**立即**生效。

---

##### fu_GetFaceInfo 函数

获取人脸跟踪信息。

```c#
 public static extern int fu_GetFaceInfo(int face_id, IntPtr ret, int szret, [MarshalAs(UnmanagedType.LPStr)]string name);
```

__参数:__

*face_id*: 当前第几张人脸
*ret*: 用于接收数据的数组的指针
*szret*: 用于接收数据的数组的长度
*name*: 需要获取的参数名字

__返回值:__

无

__备注:__

这个接口会**立即**生效。
这个接口的具体调用方法和可获取参数名请参考FaceunityWorker.cs中的相关代码。

------

##### fu_SetTongueTracking 函数

 FURuningMode为FU_Mode_RenderItems的时候，加载EnableTongueForUnity.bytes，才能开启舌头跟踪。
 FURuningMode为FU_Mode_TrackFace的时候，调用fu_SetTongueTracking(1)，才能开启舌头跟踪。注意，每次切换到FU_Mode_TrackFace之后都需要设置一次！！！

```c#
 public static extern int fu_SetTongueTracking(int enable);
```

__参数:__

*enable*: 0表示关闭，1表示开启

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### fu_SetFaceDetParam 函数

```
- 设置 `name == "use_new_cnn_detection"` ，且 `pvalue == 1` 则使用默认的CNN-Based人脸检测算法，否则 `pvalue == 0`则使用传统人脸检测算法。默认开启该模式。
- 设置 `name == "other_face_detection_frame_step"` ，如果当前状态已经检测到一张人脸后，可以通过设置该参数，每隔`step`帧再进行其他人脸检测，有助于提高性能，设置过大会导致延迟感明显。

如果`name == "use_new_cnn_detection"` ，且 `pvalue == 1` 已经开启：
- `name == "use_cross_frame_speedup"`，`pvalue==1`表示，开启交叉帧执行推理，每帧执行半个网络，下帧执行下半个网格，可提高性能。默认 `pvalue==0`关闭。
- - `name == "enable_large_pose_detection"`，`pvalue==1`表示，开启正脸大角度(45度)检测优化。`pvalue==0`表示关闭。默认 `pvalue==1`开启。
- `name == "small_face_frame_step"`，`pvalue`表示每隔多少帧加强小脸检测。极小脸检测非常耗费性能，不适合每帧都做。默认`pvalue==5`。
- 检测小脸时，小脸也可以定义为范围。范围下限`name == "min_facesize_small"`，默认`pvalue==18`，表示最小脸为屏幕宽度的18%。范围上限`name == "min_facesize_big"`，默认`pvalue==27`，表示最小脸为屏幕宽度的27%。该参数必须在`fuSetup`前设置。

否则，当`name == "use_new_cnn_detection"` ，且 `pvalue == 0`时：
- `name == "scaling_factor"`，设置图像金字塔的缩放比，默认为1.2f。
- `name == "step_size"`，滑动窗口的滑动间隔，默认 2.f。
- `name == "size_min"`，最小人脸大小，多少像素。 默认 50.f 像素，参考640x480分辨率。
- `name == "size_max"`，最大人脸大小，多少像素。 默认最大，参考640x480分辨率。
- `name == "min_neighbors"`，内部参数, 默认 3.f
- `name == "min_required_variance"`， 内部参数, 默认 15.f
- `name == "is_mono"`，设置输入源是否是单目相机。
```

```c#
 public static extern int fu_SetFaceDetParam([MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value);
```

__参数:__

*name*: 参数名
*value*: 参数指针

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### fu_SetFaceTrackParam函数

```
- 设置 `name == "mouth_expression_more_flexible"` ，`pvalue = [0,1]`，默认 `pvalue = 0` ，从0到1，数值越大，嘴部表情越灵活。  
```

```c#
 public static extern int fu_SetFaceTrackParam([MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value);
```

__参数:__

*name*: 参数名
*value*: 参数指针

__返回值:__

无

__备注:__

这个接口会**立即**生效。

---

##### fu_OnDeviceLost 函数

重置Nama的GL渲染环境

```c#
 public static extern void fu_OnDeviceLost();
```

__参数:__

无

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### fu_OnCameraChange 函数

重置Nama中的人脸跟踪功能（不涉及GL）

```c#
 public static extern void fu_OnCameraChange();
```

__参数:__

无

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### ClearImages 函数

重置FacePlugin的GL渲染环境

```c#
 public static extern void ClearImages();
```

__参数:__

无

__返回值:__

无

__备注:__

这个接口会**立即**生效。

---

##### fu_DestroyLibData 函数

特殊函数，当不再需要Nama SDK时，可以释放由 ```fu_Setup```初始化所分配的人脸跟踪模块的内存，约30M左右。调用后，人脸跟踪以及道具绘制功能将失效，相关函数将失败。如需使用，需要重新调用 ```fu_Setup```进行初始化。

```c#
 public static extern void fu_DestroyLibData();
```

__参数:__

无

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

#### 2.3 加载销毁道具

##### fu_setItemDataFromPackage 函数

这个接口用于加载UnityNamaSDK所适配的道具文件，如美颜，贴纸，Animoji等等，但是不建议直接调用这个接口，而是用封装好的fu_CreateItemFromPackage

```c#
private static extern void fu_setItemDataFromPackage(IntPtr databuf, int databuf_sz);
```

__参数:__

*databuf*:  道具文件中读取的二进制数据的指针
*databuf_sz*:  道具文件的长度

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### fu_getItemIdxFromPackage 函数

获取上一个加载成功的道具的Index（又称ItemID）

```c#
public static extern int fu_getItemIdxFromPackage();
```

__参数:__

无

__返回值:__

上一个加载的道具的Index

__备注:__

这个接口会**立即**生效。

---

##### fu_setItemIds 函数

当加载完道具后，道具不会立即生效开始渲染，而是要通过这个接口输入要渲染的道具的ItemID来开启对应道具的渲染，如果不输入相应ItemID，对应的道具就不会渲染。

```c#
public static extern int fu_setItemIds(IntPtr idxbuf, int idxbuf_sz, IntPtr mask);
```

__参数:__

*idxbuf*:  所有需要渲染的道具的ItemID组成的数组
*idxbuf_sz*:  数组的长度
*mask*:  多人脸多道具时每个人脸用不同的道具

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### fu_DestroyItem 函数

销毁指定道具

```c#
public static extern void fu_DestroyItem(int itemid);
```

__参数:__

*itemid*:  道具加载后返回的ItemID

__返回值:__

无

__备注:__

这个接口会**延迟**生效。
如果同一帧内多次调用本函数，只有最后一次调用会生效。

------

##### fu_DestroyAllItems 函数

销毁当前所有加载的道具

```c#
public static extern void fu_DestroyAllItems();
```

__参数:__

无

__返回值:__

无

__备注:__

这个接口会**延迟**生效。
如果同一帧内多次调用本函数，只有最后一次调用会生效。

---

##### fu_ItemSetParamd 函数

给道具设置参数（double）

```c#
public static extern int fu_ItemSetParamd(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, double value);
```

__参数:__

*itemid*: 需要设置参数的道具ID
*name*: 参数的名字
*value*: 参数

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------

##### fu_ItemSetParamdv 函数

给道具设置参数（double数组）

```c#
public static extern int fu_ItemSetParamdv(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int value_sz);
```

__参数:__

*itemid*: 需要设置参数的道具ID
*name*: 参数的名字
*value*: 参数指针
*value_sz*: 参数长度

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------

##### fu_ItemSetParams 函数

给道具设置参数（string）

```c#
public static extern int fu_ItemSetParams(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string value);
```

__参数:__

*itemid*: 需要设置参数的道具ID
*name*: 参数的名字
*value*: 参数

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------

##### fu_ItemGetParamd 函数

获取指定道具的某个参数（double）

```c#
public static extern double fu_ItemGetParamd(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name);
```

__参数:__

*itemid*: 需要获取参数的道具ID
*name*: 参数的名字

__返回值:__

需要获取的参数

__备注:__

这个接口会**立即**生效。

------

##### fu_ItemGetParams 函数

获取指定道具的某个参数（string）

```c#
public static extern int fu_ItemGetParams(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]byte[] buf, int buf_sz);
```

__参数:__

*itemid*: 需要获取参数的道具ID
*name*: 参数的名字
*buf*: 参数指针
*buf_sz*: 参数的长度

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

---

##### fu_CreateTexForItem 函数

给道具设置纹理

```c#
public static extern int fu_CreateTexForItem(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, IntPtr value, int width, int height);
```

__参数:__

*itemid*: 需要设置纹理的道具ID
*name*: 需要设置的纹理的名字
*value*: RGBA格式的纹理的buffer的指针
*width*: 纹理宽
*height*: 纹理高

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------

##### fu_DeleteTexForItem 函数

销毁指定道具的某个纹理

```c#
public static extern int fu_DeleteTexForItem(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name);
```

__参数:__

*itemid*: 需要销毁纹理的道具ID
*name*: 需要销毁的纹理的名字

__返回值:__

当返回1时表示成功，0表示失败。

__备注:__

这个接口会**立即**生效。

------



#### 2.4 功能接口

##### fu_GetModuleCode 函数

获取证书鉴权结果，总共64bit的标志位。

```c#
public static extern int fu_GetModuleCode(int i);
```

__参数:__

*i*: 输入0获取前32bit，输入1获取后32bit

__返回值:__

鉴权结果标志位。

__备注:__

这个接口会**立即**生效。

---

##### fu_SetExpressionCalibration 函数

控制自动表情校正。

```c#
public static extern void fu_SetExpressionCalibration(int enable);
```

__参数:__

*enable*:  0表示关闭自动校正，1表示开启自动校正

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### fu_SetDefaultRotationMode 函数

设置默认的RotationMode，即默认的渲染方向

```c#
public static extern void fu_SetDefaultRotationMode(int i);
```

__参数:__

*i*:  输入0~3的整数，具体应用请参考UnityDemo中的StdController.cs中的getRotateEuler函数

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### fu_SetASYNCTrackFace 函数

开启异步跟踪功能，某些机型可以性能会提升，但是某些机型性能下降。

```c#
public static extern int fu_SetASYNCTrackFace(int i);
```

__参数:__

*i*: 输入0关闭，输入1开启，默认关闭

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### fu_SetDefaultOrientation 函数

设置默认的人脸检测方向，正确设置可以提高检测速度和性能

```c#
 public static extern void fu_SetDefaultOrientation(int rmode);
```

__参数:__

*rmode*: 0~3的整数，含义同fu_SetDefaultRotationMode

__返回值:__

无

__备注:__

这个接口会**立即**生效。

---

##### fu_SetFocalLengthScale 函数

设置Nama渲染FOV的Scale。

```c#
 public static extern void fu_SetFocalLengthScale(float scale);
```

__参数:__

*scale*: 这个数字需要大于0，用于调整Nama的FOV

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### SetUseNatCam 函数

UnityDemo使用了NatCam来提高相机效率，同时修改了其代码以便配合FacePlugin进一步提高效率，但是如果客户需要使用自己的相机插件，则需要调用这个接口来关闭FacePlugin中相关的优化代码，以防止出现异常。这个开关只在安卓平台生效，其他平台无需关心这个问题。

```c#
 public static extern void SetUseNatCam(int enable);
```

__参数:__

*enable*: 0代表关闭，1代表开启，默认开启

__返回值:__

无

__备注:__

这个接口会**立即**生效。

---

##### SetFlipTexMarkX 函数

翻转输入的纹理，仅在使用SetDualInput时生效，有些安卓平台nv21buf和tex的方向不一致，可以用这个接口设置tex的X轴镜像。

```c#
 public static extern int SetFlipTexMarkX(bool mark);
```

__参数:__

*mark*: 0表示不翻转，1表示翻转X轴

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### SetFlipTexMarkY 函数

翻转输入的纹理，仅在使用SetDualInput时生效，有些安卓平台nv21buf和tex的方向不一致，可以用这个接口设置tex的Y轴镜像。

```c#
 public static extern int SetFlipTexMarkY(bool mark);
```

__参数:__

*mark*: 0表示不翻转，1表示翻转Y轴

__返回值:__

无

__备注:__

这个接口会**延迟**生效。

------

##### SetPauseRender 函数

手动暂时屏蔽UnityNamaSDK的渲染，调用这个函数后UnityNamaSDK将暂时停止解析输入的图像数据，即时当前仍有图像数据输入。

```c#
 public static extern void SetPauseRender(bool ifpause);
```

__参数:__

*ifpause*: 0代表不暂停，1代表暂停

__返回值:__

无

__备注:__

这个接口会**立即**生效。

------

##### fu_IsTracking 函数

获取当前解析完毕后图像中有几张人脸

```c#
 public static extern int fu_IsTracking();
```

__参数:__

无

__返回值:__

人脸数，这个值受到fu_SetMaxFaces的影响

__备注:__

这个接口会**立即**生效。

------

##### fu_SetMaxFaces 函数

设置最多检测几张人脸

```c#
 public static extern int fu_SetMaxFaces(int num);
```

__参数:__

*num*: 最多人脸数

__返回值:__

无

__备注:__

这个接口会**立即**生效。
**这个接口在每次调用（GL.IssuePluginEvent(fu_GetRenderEventFunc(), 1);）之前调用一次。**

---

##### fu_GetFaceIdentifier 函数

输入当前第N张脸，获取该张脸的独有ID。

```c#
 public static extern int fu_GetFaceIdentifier(int face_id);
```

__参数:__

*face_id*: fu_IsTracking()会返回当前总共有N张脸，这个数字需要满足(0 <= face_id < N)

__返回值:__

输入数字N，返回当前第N张脸所独有的ID

__备注:__

这个接口会**立即**生效。

------

##### ~~fu_SetQualityTradeoff 函数~~

**已弃用**

```c#
 public static extern void fu_SetQualityTradeoff(float num);
```

------

##### fu_EnableLog 函数

开启FacePlugin层的Log。PC平台需要自行开启Unity控制台，或者配合RegisterDebugCallback开启Unity内Log。

```c#
 public static extern void fu_EnableLog(bool isenable);
```

__参数:__

*isenable*: 0表示关闭，1表示开启

__返回值:__

无

__备注:__

这个接口会**立即**生效。

---

##### RegisterDebugCallback 函数

配合fu_EnableLog使用，注册一个C#委托用于处理返回的Log信息，一般就直接使用Debug.Log打在UnityConsole里。

```c#
 private static extern void RegisterDebugCallback(DebugCallback callback);
```

__参数:__

*callback*: 回调委托

__返回值:__

无

__备注:__

这个接口会**立即**生效。

---

##### fu_GetVersion 函数

获取Nama版本信息

```c#
 public static extern IntPtr fu_GetVersion();
 Marshal.PtrToStringAnsi(fu_GetVersion());	//调用示例
```

__参数:__

无

__返回值:__

Nama版本

__备注:__

这个接口会**立即**生效。

------

##### fu_GetSystemError 函数

获取上一个Nama中发生的错误

```c#
 public static extern int fu_GetSystemError();
```

__参数:__

无

__返回值:__

上一个Nama中发生的错误的代号

__备注:__

这个接口会**立即**生效。

系统错误代码及其含义如下：

| 错误代码 | 错误信息                          |
| :------- | :-------------------------------- |
| 1        | 随机种子生成失败                  |
| 2        | 机构证书解析失败                  |
| 3        | 鉴权服务器连接失败                |
| 4        | 加密连接配置失败                  |
| 5        | 客户证书解析失败                  |
| 6        | 客户密钥解析失败                  |
| 7        | 建立加密连接失败                  |
| 8        | 设置鉴权服务器地址失败            |
| 9        | 加密连接握手失败                  |
| 10       | 加密连接验证失败                  |
| 11       | 请求发送失败                      |
| 12       | 响应接收失败                      |
| 13       | 异常鉴权响应                      |
| 14       | 证书权限信息不完整                |
| 15       | 鉴权功能未初始化                  |
| 16       | 创建鉴权线程失败                  |
| 17       | 鉴权数据被拒绝                    |
| 18       | 无鉴权数据                        |
| 19       | 异常鉴权数据                      |
| 20       | 证书过期                          |
| 21       | 无效证书                          |
| 22       | 系统数据解析失败                  |
| 0x100    | 加载了非正式道具包（debug版道具） |
| 0x200    | 运行平台被证书禁止                |

------

##### fu_GetSystemErrorString 函数

将错误代号转换成对应string

```c#
 public static extern IntPtr fu_GetSystemErrorString(int code);
 Marshal.PtrToStringAnsi(fu_GetSystemErrorString(fu_GetSystemError()));		//示例
```

__参数:__

*code*: 错误代号

__返回值:__

字符串指针

__备注:__

这个接口会**立即**生效。

---



#### 2.5 C#端辅助接口

##### fu_CreateItemFromPackage 函数

开启这个协程来加载Nama道具，这个道具会自动等待两帧，待道具真正加载完毕再返回。

```c#
 public static IEnumerator fu_CreateItemFromPackage(IntPtr databuf, int databuf_sz)
```

__参数:__

*databuf*: 道具文件数组的指针
*databuf_sz*: 道具文件数组的长度

__返回值:__

无

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

------

##### InitCFaceUnityCoefficientSet 函数

实例化所有获取人脸信息的类， 这些实例内含了fu_GetFaceInfo，用于获取各种人脸信息。

```c#
 void InitCFaceUnityCoefficientSet(int maxface)
```

__参数:__

*maxface*: 最多几张人脸

__返回值:__

无

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

------

##### Start 函数

这个协程会初始化整个UnityNamaSDK。初始化完毕会自动开启CallPluginAtEndOfFrames。

```c#
 IEnumerator Start()
```

__参数:__

无

__返回值:__

无

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

------

##### CallPluginAtEndOfFrames 函数

开启这个协程，在每帧的末尾会自动调用UnityNamaSDK来识别人脸并渲染当前图像帧，如果开启相关参数(EnableExpressionLoop)，会同时自动获取识别后的人脸信息。

```c#
 private IEnumerator CallPluginAtEndOfFrames()
```

__参数:__

无

__返回值:__

无

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

------

##### DebugMethod 函数

配合RegisterDebugCallback使用，输入返回的Log信息

```c#
 private static void DebugMethod(string message)
```

__参数:__

*message*: log信息

__返回值:__

无

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

------

##### OnApplicationQuit 函数

在应用退出的时候清理GCHandle及UnityNamaSDK内部的相关数据。

```c#
 private void OnApplicationQuit()
```

__参数:__

无

__返回值:__

无

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

------

##### EnvironmentCheck 函数

检测当前渲染环境是否为OpenGL，本插件只支持在OpenGL的渲染环境下渲染。

```c#
 bool EnvironmentCheck()
```

__参数:__

无

__返回值:__

false表示检测不通过，true表示检测通过

__备注:__

这是一个C#函数，不是UnityNamaSDK接口

---



### 3. 常见问题 

#### 3.1 类型为IntPtr的形参到底该输入什么？

C#中通常不会用到IntPtr这个类型，但是当需要和Native代码交互的时候，可以用这个类型代替指针的作用。

获取IntPtr的方式很多，这里建议使用GCHandle来得到数组的指针，或者用Marshal.AllocHGlobal来新建一个，具体可以参考FULiveUnity Demo。

