# FULiveDemoUnity

FULiveDemoUnity是集成了Faceunity面部跟踪，智能美颜，贴纸道具功能的Unity工程示例。

## SDK v5.0 (2018.4.8) 更新

本次更新主要包含以下改动：

- 新增高级美颜功能
- 新增精细脸型调整功能
- 新增3D绘制抗锯齿功能
- 新增照片驱动功能
- 新增人脸夸张变形功能
- 新增音乐节奏滤镜
- 新增被动表情校准模式
- 优化手势识别
- 人脸跟踪底层性能进一步优化
- 性能优化
- 其他累积问题修复、接口调整

具体更新内容可以到[这里](https://github.com/Faceunity/FULiveDemo/blob/master/docs/FUNama%20SDK%20v4.6%20%E6%9B%B4%E6%96%B0%E6%96%87%E6%A1%A3.md)查看详细文档。

## 开发环境

> Unity5.3.0f4 及以上

## 文件说明

### Assets文件夹：



* ***Examples*** 

  > 各种案例Demo，如果不需要可以直接删除。
  >
  > **----Common:** Demo的公共资源。
  >
  > ​	**|----Materials:** 一些公共材质。
  >
  > ​	**|----NatCam:** 高效率的摄像机插件，支持安卓和iOS，而PC平台则是封装了Unity的WebCamTexture，效率一般。
  >
  > ​	**|----Script:** 一些公共脚本。
  >
  > ​	**|----Shader:** 渲染模型使用的Shader，仅供参考。
  >
  > ​	**|----Textures:** Demo的Logo图片。
  >
  > **----DataOut:** FacePlugin的数据输出模式，使用Unity进行内容渲染，使用了NatCam以提高效率。仅输出人脸的位置、旋转、表情系数等，以供Unity渲染。
  >
  > ​	**|----Models:** 人头模型和对应材质。
  >
  > ​	**|----Scene:** Demo场景，**demoDataOut** 是人头模型渲染，**demoDataOut_Multiple**是多人模型渲染。
  >
  > ​	**|----Script: **Demo的相关脚本。
  >
  > ​		**|----RenderToModel.cs:** 负责对接相机插件，输入输出图像数据，管理输出材质的旋转缩放。
  >
  > ​		**|----StdController.cs:** 负责控制人头的位置、旋转和表情系数。
  >
  > ​		**|----EyeController.cs:** 负责控制眼睛的位置和旋转。
  >
  > ​		**|----UIManagerForDataOut.cs:** DataOut场景的UI控制器。
  >
  > ​		**|----UIManagerForDataOut_Multiple.cs:** DataOut_Multiple场景的UI控制器，也负责多人模型调度。
  >
  > **----Simple:** 最简单的FacePlugin的使用案例，直接使用Unity自带的WebCamTexture以简化代码结构。
  >
  > ​	**|----Scene:** demoSimple是本例的场景。
  >
  > ​	**|----Script:** Demo的相关脚本。
  >
  > ​		**|----RenderSimple.cs:** 使用UpdateData函数，输入输出图像数据。如果你需要输入自己的数据，请参考这个函数。
  >
  > ​		**|----UIManagerSimple.cs:** 简单场景的UI控制器，注册了切换相机按钮，管理人脸检测标志。
  >

* ***Plugins*** 

  >各平台的faceunity插件



* ***Script*** 

  > 关键脚本文件
  >
  > FaceunityWorker.cs：负责初始化faceunity插件并引入C++接口，初始化完成后每帧更新人脸跟踪数据



* ***StreamingAssets*** 

  > license是证书文件，缺少该文件会导致初始化失败
  >
  > v3.bytes是SDK的数据文件，缺少该文件会导致初始化失败
  >
  > anim_model.bytes是优化人脸跟踪表情数据的文件
  >
  > ardata_ex.bytes是提供高精度AR功能的文件






## 运行流程

###  一、获取证书

您需要拥有我司颁发的证书才能使用我们的SDK的功能，获取证书方法：

1. 拨打电话 **0571-88069272** 
2. 发送邮件至 **marketing@faceunity.com** 进行咨询。




### 二、初始化faceunity插件

***将license文件放在Assets\StreamingAssets\faceunity下。***  

场景中挂载FaceunityWorker.cs，FaceunityWorker提供API接口。

FaceunityWorker会自动载入license文件和v3.bytes，并调用fu_Setup进行初始化。

```C#
public static extern int fu_Setup(IntPtr databuf, IntPtr licbuf, int licbuf_sz);
```

`databuf` v3.bytes中读取的二进制数据的指针。

`licbuf` license文件中读取的文本数据，用`,`分割，将分割后的数组数据转成sbyte格式。

`licbuf_sz` sbyte数组长度。



### 三、 输入图像数据

```C#
public static extern void SetUseNativeCameraData(int enable);
```

SetUseNativeCameraData可以把插件设置成本地输入图像数据模式。

```c#
public static extern int fu_SetRuningMode(int runningMode);

public enum FURuningMode
    {
        FU_Mode_None = 0,
        FU_Mode_RenderItems, //face tracking and render item (beautify is one type of item) ,item means 'daoju'
        FU_Mode_Beautification,//non face tracking, beautification only.
        FU_Mode_Masked,//when tracking multi-people, different perple　can use different item, give mask in function fu_setItemIds  
        FU_Mode_TrackFace//tracking face only, get face infomation, but do not render item.it's very fast.
    };
```

fu_SetRuningMode可以设置本插件运行模式，针对需求设置运行模式可以大大提高效率。FU_Mode_RenderItems为默认运行模式，可以在FaceunityWorker.cs中自行更改，也可在运行中更改。

初始化完成会开启GL循环 [参考这里](https://docs.unity3d.com/Manual/NativePluginInterface.html) ，并更新人脸跟踪数据。


***UNITY_EDITOR或UNITY_STANDALONE的环境下：*** 

在RenderToModel/RenderSimple中，会在初始化对应相机并调整UI自适应后，在Update中使用GetPixels32获取原生图像buffer，并将其指针传入SetImage。

```C#
public static extern int SetImage(IntPtr imgbuf,int flags, bool isbgra, int w, int h);
```

`imgbuf` buffer数组指针

`flags` 传入选项标志位，如XY轴反转等，具体请参考源代码

`isbgra` buffer数据顺序是否为bgra

`w` 图像宽度

`h` 图像高度

除了SetImage，输入函数还有：

```c#
public static extern int SetDaulInput(IntPtr nv21buf, int texid, int flags, int w, int h);
public static extern int SetNV21Input(IntPtr nv21buf, int flags, int w, int h);
public static extern int SetImageTexId(int texid, int flags, int w, int h);
```

**SetNV21Input仅支持ANDROID。** 

***UNITY_ANDROID的环境下：*** 

在RenderToModel/RenderToTexture中，会在初始化时开启NatCam，NatCam会在Java层直接向FacePlugin输入数据，因此不必再C#层重复操作。输出的时候请自行处理镜像旋转缩放。

或者参考RenderSimple输入你自己的相机数据。

***UNITY_IOS的环境下：*** 

初始化步骤同ANDROID，NatCam会在底层直接向FacePlugin输入数据。但是输出的时候IOS环境下插件自动处理镜像和旋转，C#层只需处理缩放。

或者参考RenderSimple输入你自己的相机数据。



### 四、 输出跟踪数据

**本条案例可查看demoDataOut或UIManagerForDataOut场景。** 

demoDataOut场景中点击UI上的TrackPositon可以切换渲染模式，点击头像Icon可以切换模型。

初始化完成并开启GL循环后，FaceunityWorker会在每帧使用fu_GetFaceInfo更新人脸跟踪数据：

```C#
public static extern int fu_GetFaceInfo(int face_id, IntPtr ret, int szret, [MarshalAs(UnmanagedType.LPStr)]string name);
```

`face_id` 人脸ID，默认为0

`ret` 回传指针，函数会把回传数据放入指针所指的地址

`szret` 回传指针大小

`name` 跟踪数据名字，包括但不限于translation（位置）、rotation（旋转）、expression（表情）、pupil_pos（瞳孔位置）、focal_length（焦距）。

并存入预先定义的地址，如位置：

```C#
public CFaceUnityCoefficientSet m_translation = new CFaceUnityCoefficientSet("translation", 3);
```

`CFaceUnityCoefficientSet` 这是一个自定义类，可以自动创建GCHandle并存储对应数据。

**demoDataOut场景中：** 

StdController在每帧通过FaceunityWorker中预存的地址获取旋转和表情数据，并根据镜像情况设置模型对应参数。其中表情数据是包含了46个blendshape值，和预先制作的带有46个blendshape的模型配合，使用SkinnedMeshRenderer.SetBlendShapeWeight设置。

EyeController在每帧通过FaceunityWorker中预存的地址获取眼睛的旋转数据，并根据镜像情况设置眼睛旋转。

点击UI上的TrackPositon可以开启/关闭位置跟踪，同时切换RuningMode。TrackPositon开启时设置运行模式为FU_Mode_RenderItems，同时渲染摄像机图像到UI上。关闭时设置运行模式为FU_Mode_TrackFace，同时关闭图像渲染以提高性能。

**demoDataOut_Multiple场景中：** 

demoDataOut场景AR模式（开启TrackPositon）的多人版本，在场景中Faceunity Worker物体的Inspector中设置MAXFACE可以修改最多同时跟踪的人脸数量。



### 五、 最简案例

**本条案例可查看demoSimple场景。** 

这个案例用最少的代码运行faceplugin，为使本例能运行，在RenderSimple里直接调用了Unity自带的WebCamTexture。

RenderSimple里的函数：

```c#
public void UpdateData(IntPtr ptr,int texid,int w,int h)
```

这个函数展示了如何输入图像数据进本插件，以及怎样从插件中取出渲染完毕的数据。

如果想快速接入你自己的图像数据，请参考本例。



## 详细接口信息
见FaceunityWorker.cs代码注释。



## 鉴权

我们的系统通过标准TLS证书进行鉴权。客户在使用时先从发证机构申请证书，之后将证书数据写在客户端代码中，客户端运行时发回我司服务器进行验证。在证书有效期内，可以正常使用库函数所提供的各种功能。没有证书或者证书失效等鉴权失败的情况会限制库函数的功能，在开始运行一段时间后自动终止。

证书类型分为**两种**，分别为**发证机构证书**和**终端用户证书**。

#### - 发证机构证书

**适用对象**：此类证书适合需批量生成终端证书的机构或公司，比如软件代理商，大客户等。

发证机构的二级CA证书必须由我司颁发，具体流程如下。

1. 机构生成私钥 机构调用以下命令在本地生成私钥 CERT_NAME.key ，其中 CERT_NAME 为机构名称。

```
openssl ecparam -name prime256v1 -genkey -out CERT_NAME.key
```

1. 机构根据私钥生成证书签发请求 机构根据本地生成的私钥，调用以下命令生成证书签发请求 CERT_NAME.csr 。在生成证书签发请求的过程中注意在 Common Name 字段中填写机构的正式名称。

```
openssl req -new -sha256 -key CERT_NAME.key -out CERT_NAME.csr
```

1. 将证书签发请求发回我司颁发机构证书

之后发证机构就可以独立进行终端用户的证书发行工作，不再需要我司的配合。

如果需要在终端用户证书有效期内终止证书，可以由机构自行用OpenSSL吊销，然后生成pem格式的吊销列表文件发给我们。例如如果要吊销先前误发的 "bad_client.crt"，可以如下操作：

```
openssl ca -config ca.conf -revoke bad_client.crt -keyfile CERT_NAME.key -cert CERT_NAME.crt
openssl ca -config ca.conf -gencrl -keyfile CERT_NAME.key -cert CERT_NAME.crt -out CERT_NAME.crl.pem
```

然后将生成的 CERT_NAME.crl.pem 发回给我司。

#### - 终端用户证书

**适用对象**：直接的终端证书使用者。比如，直接客户或个人等。

终端用户由我司或者其他发证机构颁发证书，并通过我司的证书工具生成一个代码头文件交给用户。该文件中是一个常量数组，内容是加密之后的证书数据，形式如下。

```
static char g_auth_package[]={ ... }
```

用户在库环境初始化时，需要提供该数组进行鉴权，具体参考 fuSetup 接口。没有证书、证书失效、网络连接失败等情况下，会造成鉴权失败，在控制台或者Android平台的log里面打出 "not authenticated" 信息，并在运行一段时间后停止渲染道具。

任何其他关于授权问题，请email：[support@faceunity.com](mailto:support@faceunity.com)