# Unity Nama SDK 集成指导文档  
级别：Public
更新日期：2019-08-14

------
## 最新更新内容：

2019-08-14 v6.3.0：

- 优化人脸美妆功能，提高性能，降低功耗。
- 新增fuSetFaceTrackParam接口，用于设置人脸表情跟踪参数。 

- 新增人脸美颜精细磨皮效果。

------
## 目录：
本文档内容目录：

[TOC]

------
## 1. 简介 
本文档旨在说明如何将Faceunity Nama SDK集成您的Unity工程中。  

------


## 2. SDK文件结构

本小节，描述Demo文件结构，各个目录，以及重要文件的功能。

```
+FULiveUnity
  +Assets 			  	//Unity资源目录
    +Examples				//示例目录
      +Common					//Demo的公共资源。
      	+Materials：一些公共材质。
        +NatCam：高效率的摄像机插件，支持安卓和iOS，而PC则是封装了Unity的WebCamTexture，效率一般。
        +Script：一些公共脚本。
        +Shader：渲染模型使用的Shader，仅供参考。
        +Textures：Demo的Logo图片。
        +UIImgs：一些公共UI图片。
      +DataOut					// FacePlugin的数据输出模式，使用Unity进行内容渲染，使用了NatCam以提高效率。仅输出人脸的位置、旋转、表情系数等，以供Unity渲染。
      	+Models: 人头模型和对应材质纹理。
      	+Scene: Demo场景，demoDataOut 是人头模型渲染，demoDataOut_Multiple是多人模型渲染。
      	+Script: Demo的相关脚本。
		 -RenderToModel.cs: 负责对接相机插件，输入输出图像数据，管理输出纹理的旋转缩放。
		 -StdController.cs: 负责控制人头的位置、旋转和表情系数。
		 -EyeController.cs: 负责控制眼睛的位置和旋转。
		 -UIManagerForDataOut.cs: DataOut场景的UI控制器。
		 -UIManagerForDataOut_Multiple.cs: DataOut_Multiple场景的UI控制器，也负责多人模型调度。
      +Simple					//最简单的FacePlugin的使用案例，直接使用Unity自带的WebCamTexture以简化代码结构。
       +Scene: demoSimple是本例的场景。
	   +Script: Demo的相关脚本。
		 -RenderSimple.cs: 如果你需要输入其他渠道获得的图像数据，请参考这个函数。
		 -UIManagerSimple.cs: 简单场景的UI控制器，注册了切换相机按钮，管理人脸检测标志。
      +TexOut					//FacePlugin的纹理输出模式，使用Faceunity Nama SDK进行内容渲染，使用了NatCam以提高效率。直接输出本插件渲染好的数据，可以使用附带的二进制道具文件。
      	+Resources: 所有道具的二进制文件和对应的UI文件。
	    +Scene: demoTexOut是本例的场景。
	    +Script: Demo的相关脚本。
		 -RenderToTexture.cs: 负责对接相机插件，输入输出图像数据，加载卸载道具。
		 -UIManagerForTexOut.cs: 纹理输出模式的UI控制器，和RenderToTexture配合以展现所有道具的功能。
		 -ItemConfig.cs: 道具的二进制文件和UI文件的路径等信息的配置文件。
    +Plugins				//SDK文件目录
    +Script					//核心代码文件目录
      -FaceunityWorker.cs：负责初始化faceunity插件并引入C++接口，初始化完成后每帧更新人脸跟踪数据
    +StreamingAssets		//数据文件目录
      -v3.bytes：SDK的数据文件，缺少该文件会导致初始化失败
      -tongue.bytes：舌头跟踪必须的文件
      -EnableTongueForUnity.bytes：某种情况下获取舌头跟踪数据需要加载的文件
  +docs					//文档目录
  +ProjectSettings   	//Unity工程配置目录
  -readme.md			//工程总文档
  
```

------
## 3. 集成指引


### 3.1 开发环境
#### 3.1.1 支持平台
```
Windows、Android、iOS（9.0以上系统）、Mac
```
#### 3.1.2 开发环境
```
Unity5.4.6f3 及以上
```

### 3.2 准备工作 
- [下载demo代码](https://github.com/Faceunity/FULiveUnity)
- 获取证书:
  1. 拨打电话 **0571-88069272** 
  2. 发送邮件至 **marketing@faceunity.com** 进行咨询。

### 3.3 相关配置
- 将证书文件中的数据复制到场景中FaceunityWorker物体的Inspector面板的LICENSE输入框内，并保存场景（其他场景同理）。

![](imgs/img0.jpg)

- 如需要将本SDK集成到你的工程中去：
  1. 请将demo中的Assets文件夹下除Examples文件夹以外的所有文件及文件夹复制到你的工程中去。
  2. 配置渲染环境为OpenGL。
  3. 在场景中新建一个物体，挂载FaceunityWorker组件，参照上图填写证书信息。
  4. 编写你自己的图像输入的代码组件。
  5. 参考demo获取跟踪数据，或者加载道具进行渲染。

### 3.4 初始化

**本条案例可查看demoSimple场景。** 

查看demoSimple场景中的物体Faceunity Worker，这个物体挂载了4个代码组件，其中FaceunityWorker是所有使用本SDK的场景都必须要有的组件，这个组件引入了本SDK的所有接口，初始化代码、每帧进行SDK渲染的协程(CallPluginAtEndOfFrames)以及相关代码，剩下组件的为本案例的示例代码。

每个场景开始的时候FaceunityWorker会调用DontDestroyOnLoad使宿主物体不在切换场景的时候被销毁，同时确保场景中只有一个FaceunityWorker组件实例。本Demo中的所有案例都不支持相互切换场景，如有需求请确保在切换场景的时候这个组件不被销毁，同时有且只有这一个组件实例。

FaceunityWorker的Start函数中会初始化本SDK，初始化完成后会执行初始化完成事件，同时开启每帧进行SDK渲染的协程(CallPluginAtEndOfFrames)。切换场景的时候CallPluginAtEndOfFrames可能会停止运行，可能需要再次调用相关协程启动函数。

### 3.5 输入图像数据

**本条案例可查看demoSimple场景。** 

这个案例用最少的代码运行本SDK，为使本例能运行，在RenderSimple里直接调用了Unity自带的WebCamTexture。**如果你想输入自己的图像数据，请参考本案例。**

RenderSimple里最关键的函数：

```c#
public void UpdateData(IntPtr ptr,int texid,int w,int h, UpdateDataMode mode)
```

这个函数展示了如何输入图像数据进本插件，以及怎样从插件中取出渲染完毕的数据。

```
    public enum UpdateDataMode
    {
        NV21,
        Dual,
        Image,
        ImageTexId
    }
```

以上四种UpdateDataMode为本SDK输入图像数据的四种格式。

`NV21` NV21格式的buffer数组，通常在安卓设备上，通过原生相机插件使用

`Dual`  同时输入NV21格式的buffer数组和纹理ID，通常在安卓设备上，通过原生相机插件使用，效率最高

`Image` RGBA格式的buffer数组，通用性最强，各平台均可使用

`ImageTexId` GL纹理ID，某些特殊GL环境下无法使用，但是一定程度上性能高于Image

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

fu_SetRuningMode可以设置本插件运行模式，针对需求设置运行模式可以大大提高效率。FU_Mode_RenderItems为默认运行模式，可以在FaceunityWorker.cs中自行更改初始值，也可在运行时更改。具体各项效果请查看API文档。

**查看demoDataOut场景中的RenderToTexture或demoTexOut场景中的RenderToModel。** 

这两个场景中使用了Natcam来加速视频功能，同时要处理视频旋转镜像，因此数据输入过程较为复杂。

***UNITY_ANDROID的环境下：*** 

在RenderToModel/RenderToTexture中，会在初始化时开启NatCam，NatCam会在Java层直接向本SDK输入数据，因此不必再C#层重复操作。输出的时候请自行处理镜像旋转缩放。

***UNITY_IOS的环境下：*** 

初始化步骤同ANDROID，NatCam会在底层直接向本SDK输入数据。但是输出的时候IOS环境下插件自动处理镜像和旋转，输出后只需处理缩放。

### 3.6 输出跟踪数据

**本条案例可查看demoDataOut场景。** 

demoDataOut场景中点击UI上的TrackPositon可以切换渲染模式，点击头像Icon可以切换模型。

初始化完成后，FaceunityWorker会在每帧调用的CallPluginAtEndOfFrames中更新人脸跟踪数据（如果EnableExpressionLoop为true），每张人脸的每个数据都以CFaceUnityCoefficientSet这个类的形式存储着，外界需要获取数据的时候只需要直接调用这个数据对应的这个类实例即可，具体可以参考这个场景中的StdController和EyeController。

**demoDataOut场景中：** 

StdController在每帧通过FaceunityWorker中预存的地址获取旋转和表情数据，并根据镜像情况设置模型对应参数。其中表情数据是包含了56个blendshape值，和预先制作的带有56个blendshape的模型配合，使用SkinnedMeshRenderer.SetBlendShapeWeight设置。

EyeController在每帧通过FaceunityWorker中预存的地址获取眼睛的旋转数据，并根据镜像情况设置眼睛旋转。

点击UI上的TrackPositon可以开启/关闭位置跟踪，同时切换RuningMode。TrackPositon开启时设置运行模式为FU_Mode_RenderItems，同时渲染摄像机图像到UI上。关闭时设置运行模式为FU_Mode_TrackFace，同时关闭图像渲染以提高性能。

**demoDataOut_Multiple场景中：** 

demoDataOut场景AR模式（开启TrackPositon）的多人版本，在场景中Faceunity Worker物体的Inspector中设置MAXFACE可以修改最多同时跟踪的人脸数量。

**关于舌头跟踪：**

1.必须加载tongue.bytes

2.Texout场景中加载带舌头功能的道具，比如青蛙，即可展现出舌头跟踪效果。

3.在Dataout场景中，FURuningMode为FU_Mode_RenderItems的时候，加载EnableTongueForUnity.bytes，才能开启舌头跟踪。FURuningMode为FU_Mode_TrackFace的时候，调用fu_SetTongueTracking(1)，才能开启舌头跟踪。注意，每次切换到FU_Mode_TrackFace之后都需要设置一次！！！

### 3.7 输出图像数据（道具创建、销毁、切换）

**本条案例可查看demoTexOut场景。**

 在demoTexOut场景中，挂载了RenderToTexture，这个组件不仅负责控制相机并输入图像数据，同时也封装了一些道具加载卸载的接口，如果你还不是很了解原生的道具接口以及相关特性，可以参考这个案例。

```c#
public IEnumerator LoadItem(Item item, int slotid = 0, LoadItemCallback cb=null)
```

这个接口封装了道具的加载和启用，同时使用slot的概念控制多个道具的使用。

```c#
int[] itemid_tosdk;
GCHandle itemid_handle;
IntPtr p_itemsid;
```

itemid_tosdk即slot数组，里面保存了每个slot需要渲染的itemid。如果为0或其他无效id则会被跳过，不用担心输错数据。

```c#
var bundledata = Resources.LoadAsync<TextAsset>(item.fullname);
yield return FaceunityWorker.fu_CreateItemFromPackage(pObject, bundle_bytes.Length);
int itemid = FaceunityWorker.fu_getItemIdxFromPackage();
UnLoadItem(slotid); //卸载上一个在这个slot槽内的道具
FaceunityWorker.fu_setItemIds(p_itemsid, SLOTLENGTH, IntPtr.Zero);
```

这5行代码组成了这个接口的主要功能（这里只是伪代码），第一行调用unity的IO接口读取道具文件，第二行调用fu_CreateItemFromPackage加载道具，这个接口是个协程，它内部调用原生接口加载道具后并不会立即生效，因此会等待两帧直到道具真正加载完毕才返回，然后第三行代码去获取刚刚加载完毕的道具的itemid，因为加载完毕的道具并不会直接被渲染，这里要借助slot数组，第四行先卸载在slot数组的第N位道具，然后将刚刚得到的itemid填入slot数组的第N位，第五行设置接下来真正渲染的所有道具。

------
## 4. 功能模块
**本条案例可查看demoTexOut场景。**

除特殊文件如v3.bytes、tongue.bytes以外的道具（*.bytes或 *.bundle）都可直接用LoadItem或者原生接口加载。

以下功能模块均可以使用这个方法加载，以下只详细描述对应道具所独有的参数。

```c#
//rtt即RenderToTexture的实例，这个为实际代码中封装后的加载方法
yield return rtt.LoadItem(ItemConfig.beautySkin[0], (int)SlotForItems.Beauty);

//原生方法
var bundledata = Resources.LoadAsync<TextAsset>(itempath);
yield return bundledata;
var data = bundledata.asset as TextAsset;
byte[] bundle_bytes = data != null ? data.bytes : null;
GCHandle hObject = GCHandle.Alloc(bundle_bytes, GCHandleType.Pinned);
IntPtr pObject = hObject.AddrOfPinnedObject();
yield return FaceunityWorker.fu_CreateItemFromPackage(pObject, bundle_bytes.Length);
hObject.Free();
int itemid = FaceunityWorker.fu_getItemIdxFromPackage();
var itemid_tosdk = new int[1];
var itemid_handle = GCHandle.Alloc(itemid_tosdk, GCHandleType.Pinned);
var p_itemsid = itemid_handle.AddrOfPinnedObject();
FaceunityWorker.fu_setItemIds(p_itemsid, 1, IntPtr.Zero);
```

### 4.1 视频美颜

视频美颜配置方法与视频加特效道具类似，首先创建美颜道具句柄，并保存在句柄数组中:

```c#
//rtt即RenderToTexture的实例，这个为实际代码中封装后的加载方法
yield return rtt.LoadItem(ItemConfig.beautySkin[0], (int)SlotForItems.Beauty);
```

在处理视频时，美颜道具句柄会通过句柄数组传入图像处理接口，处理完成后美颜效果将会被作用到图像中。示例如下：

```c#
var m_fu_texid = FaceunityWorker.fu_GetNamaTextureId();
var m_rendered_tex = Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, true, (IntPtr)m_fu_texid);
//SDK会返回纹理ID，即m_fu_texid
//使用纹理ID创建纹理，即m_rendered_tex，将它用在你的材质上，即可输出渲染结果
//所有道具的渲染结果都在这张纹理上
```

#### 参数设置

美颜道具主要包含七个模块的内容：滤镜、美白、红润、磨皮、亮眼、美牙、美型。

美颜道具的所有功能都通过设置参数来进行设置，以下为设置参数的代码示例：

```c#
//封装过的代码，以下代码功能为设置各美颜参数为默认值
BeautySkinItemName = ItemConfig.beautySkin[0].name;
for (int i = 0; i < BeautyConfig.beautySkin_1.Length; i++)
	{
		rtt.SetItemParamd(BeautySkinItemName, 			BeautyConfig.beautySkin_1[i].paramword, BeautyConfig.beautySkin_1[i].defaultvalue);
	}
for (int i = 0; i < BeautyConfig.beautySkin_2.Length; i++)
	{
		rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_2[i].paramword, BeautyConfig.beautySkin_2[i].defaultvalue);
	}

//原生方法
public static extern int fu_ItemSetParamd(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, double value);
public static extern int fu_ItemSetParams(int itemid, [MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string value);
//例：
FaceunityWorker.fu_ItemSetParamd(1, "color_level", 1.0f);
```

每个模块都有默认效果，它们可以调节的参数如下。

#### 4.1.1 滤镜

滤镜功能主要通过参数filter_level 和 filter_name来控制

```
filter_level 取值范围 0.0-1.0,0.0为无效果，1.0为最大效果，默认值1.0
filter_name 取值为一个字符串，默认值为 “origin” ，origin即为使用原图效果
```

filter_name参数的取值和相关介绍详见：[美颜道具功能文档](美颜道具功能文档.md) ，在 **滤镜对应key值** 部分有详细介绍，对于老版本（6.0之前）的用户，可以参考 **新老滤镜对应关系** 部分。

#### 4.1.2 美白和红润

##### 美白

美白功能主要通过参数color_level来控制

```
color_level 取值范围 0.0-1.0,0.0为无效果，1.0为最大效果，默认值0.2
```

##### 红润

红润功能主要通过参数red_level 来控制

```
red_level 取值范围 0.0-1.0,0.0为无效果，1.0为最大效果，默认值0.5
```

#### 4.1.3 磨皮

控制磨皮的参数有四个：blur_level，skin_detect，nonskin_blur_scale，heavy_blur

```
blur_level: 磨皮程度，取值范围0.0-6.0，默认6.0
skin_detect:肤色检测开关，0为关，1为开
nonskin_blur_scale:肤色检测之后非肤色区域的融合程度，取值范围0.0-1.0，默认0.45
heavy_blur: 重度磨皮开关，0为清晰磨皮，1为重度磨皮
```

注意：重度磨皮为高级美颜功能，需要相应证书权限才能使用

#### 4.1.4 亮眼

亮眼功能主要通过参数eye_bright 来控制

```
eye_bright   取值范围 0.0-1.0,0.0为无效果，1.0为最大效果，默认值1.0
```

注意：亮眼为高级美颜功能，需要相应证书权限才能使用

#### 4.1.5美牙

美牙功能主要通过参数tooth_whiten来控制

```
tooth_whiten   取值范围 0.0-1.0,0.0为无效果，1.0为最大效果，默认值1.0
```

注意：美牙为高级美颜功能，需要相应证书权限才能使用

#### 4.1.6 美型

美型的整体程度由face_shape_level参数控制

```
face_shape_level   取值范围 0.0-1.0,0.0为无效果，1.0为最大效果，默认值1.0
```

美型的渐变由change_frames参数控制

```
change_frames       0为关闭 ，大于0开启渐变，值为渐变所需要的帧数
```

美型的种类主要由face_shape 参数控制

```
face_shape: 变形取值 0:女神变形 1:网红变形 2:自然变形 3:默认变形 4:精细变形
```

在face_shape选取不同参数时，对应可以使用的参数也不同：

##### face_shape参数详解

```
1.
face_shape 为0 1 2 3时
对应0：女神 1：网红 2：自然 3：默认
可以使用参数
eye_enlarging: 默认0.5, //大眼程度范围0.0-1.0
cheek_thinning: 默认0.0,   //v脸程度范围0.0-1.0
2.
face_shape 为4时，为精细变形，添加了鼻子额头嘴巴下巴的调整
可以使用参数
eye_enlarging: 默认0.5, //大眼程度范围0.0-1.0
cheek_thinning: 默认0.0,   //v脸程度范围0.0-1.0
intensity_nose: 默认0.0,       //瘦鼻程度范围0.0-1.0
intensity_forehead: 默认0.5,   //额头调整程度范围0.0-1.0
intensity_mouth:默认0.5,       //嘴巴调整程度范围0.0-1.0
intensity_chin: 默认0.5,       //下巴调整程度范围0.0-1.0
```

注意：变形为高级美颜功能，需要相应证书权限才能使用

### 4.2 Animoji

```c#
yield return rtt.LoadItem(ItemConfig.item_1[0], (int)SlotForItems.Item);
```

Animoji默认渲染时不会跟踪人脸位置，但是可以通过开启AR模式来开启跟踪和背景图像显示：

```c#
rtt.SetItemParamd(item.name, "{\"thing\":\"<global>\",\"param\":\"follow\"}", 1);
```

镜像相关参数：

```c#
//is3DFlipH 参数是用于对3D道具的顶点镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "is3DFlipH", param);
//isFlipExpr 参数是用于对道具内部的表情系数的镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipExpr", param);
//isFlipTrack 参数是用于对道具的人脸跟踪位置旋转的镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipTrack", param);
//isFlipLight 参数是用于对道具内部的灯光的镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipLight", param);
```

默认人脸识别方向，会影响Animoji默认方向：

```c#
FaceunityWorker.fu_SetDefaultRotationMode(3);
```

### 4.3 动漫滤镜

```c#
yield return rtt.LoadItem(ItemConfig.commonFilter[0], (int)SlotForItems.CommonFilter);
```

设置参数来切换滤镜风格：

```c#
rtt.SetItemParamd((int)SlotForItems.CommonFilter, "style", 0);
//你可以尝试设置0以外的整数
```

### 4.4 AR面具

```c#
yield return rtt.LoadItem(ItemConfig.item_3[0], (int)SlotForItems.Item);
```

### 4.5 背景分割

```c#
yield return rtt.LoadItem(ItemConfig.item_7[0], (int)SlotForItems.Item);
```

### 4.6 换脸

```c#
yield return rtt.LoadItem(ItemConfig.item_4[0], (int)SlotForItems.Item);
```

### 4.7 表情识别

```c#
yield return rtt.LoadItem(ItemConfig.item_5[0], (int)SlotForItems.Item);
```

### 4.8 手势识别

```c#
yield return rtt.LoadItem(ItemConfig.item_8[0], (int)SlotForItems.Item);
```

手势识别的旋转镜像参数为独有：

```c#
//rotMode为手势识别方向
FaceunityWorker.fu_ItemSetParamd(itemid, "rotMode", 2);
//loc_x_flip为渲染X轴镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "loc_x_flip", 1.0);
```

### 4.9 贴纸

```c#
yield return rtt.LoadItem(ItemConfig.item_2[0], (int)SlotForItems.Item);
```

### 4.10 人脸夸张变形功能

```c#
yield return rtt.LoadItem(ItemConfig.item_9[0], (int)SlotForItems.Item);
```

### 4.11 音乐滤镜

```c#
yield return rtt.LoadItem(ItemConfig.item_6[0], (int)SlotForItems.Item);
```

通过随着音乐设置播放时间才能让滤镜“动”一起

```c#
rtt.SetItemParamd(name, "music_time", audios.time * 1000);
```

### 4.12 照片驱动功能

```c#
yield return rtt.LoadItem(ItemConfig.item_11[0], (int)SlotForItems.Item);
```

### 4.13 质感美颜

这个选项本质不是某类道具，而是美颜和美妆的组合，其中美颜部分请参考上述内容，美妆部分请参考[美妆道具功能文档](美妆道具功能文档.md)。美妆道具依赖一个独立的bundle（MakeupAssist）这个bundle要一起加载。

```c#
yield return rtt.LoadItem(ItemConfig.makeup[1], (int)SlotForItems.MakeupAssist);
yield return rtt.LoadItem(ItemConfig.makeup[0], (int)SlotForItems.Makeup);
```

这个道具的调用方法相对比较复杂：

```c#
rtt.SetItemParamd((int)SlotForItems.Makeup, "is_makeup_on", 1);
rtt.SetItemParamd((int)SlotForItems.Makeup, "makeup_intensity", makeupitem.intensity);
                
rtt.SetItemParamdv((int)SlotForItems.Makeup, "makeup_lip_color", makeupitem.Lipstick_color);
rtt.SetItemParamd((int)SlotForItems.Makeup, "makeup_intensity_lip", makeupitem.Lipstick_intensity);
rtt.SetItemParamd((int)SlotForItems.Makeup, "makeup_lip_mask", 1.0);
 
CreateTexForItem(uisprites.GetTexture(MakeupType.Blush, makeupitem.Blush_id), "tex_blusher");
rtt.SetItemParamd((int)SlotForItems.Makeup, "makeup_intensity_blusher", makeupitem.Blush_intensity);
 
CreateTexForItem(uisprites.GetTexture(MakeupType.Eyebrow, makeupitem.Eyebrow_id), "tex_brow");
rtt.SetItemParamd((int)SlotForItems.Makeup, "makeup_intensity_eyeBrow", makeupitem.Eyebrow_intensity);
 
CreateTexForItem(uisprites.GetTexture(MakeupType.Eyeshadow, makeupitem.Eyeshadow_id), "tex_eye");
rtt.SetItemParamd((int)SlotForItems.Makeup, "makeup_intensity_eye", makeupitem.Eyeshadow_intensity);
```

------
## 5. 常见问题 

### 5.1 编译相关

- iOS编译时请选择**9.0**以上系统版本。

- **因Github不支持上传100MB以上的文件，iOS的库经过压缩，使用时请自行解压！**

- Unity工程导入可能会导致部分native库的平台信息丢失，如果运行或者编译时报错提示找不到库，请手动修改相应库的平台信息。本案例中的native库有:
  - Assets\Plugins
  - Assets\Examples\Common\NatCam\Core\Plugins
![](imgs\img2.jpg)
  
### 5.2 关于镜像/旋转
整个工程中镜像/旋转的概念在多个地方被多次提到，这里解释一下不同地方的镜像/旋转的概念。
#### 5.2.1 输入图像数据镜像/旋转
如果输入的是纹理ID，你可以自己先算好镜像渲染参数再输入SDK，这样逻辑上会方便很多。

```c#
//翻转输入的纹理，仅对使用了natcam的安卓平台有效
//natcam的安卓平台使用了SetDualInput，有些安卓平台nv21buf和tex的方向不一致，可以用这个接口设置tex的镜像。
public static extern int SetFlipTexMarkX(bool mark);
public static extern int SetFlipTexMarkY(bool mark);
```

#### 5.2.2 跟踪数据的镜像/旋转

跟踪数据的镜像是后期处理，控制权在你的手中。参考StdController和EyeController。

```c#
//镜像Blendshape参数用的表
mirrorBlendShape
//镜像跟踪的旋转和位移
m_rotation
m_translation
//镜像AR模式的旋转参数
m_rotation_mode
//镜像眼球旋转
pupil_pos
```
#### 5.2.3 道具渲染的镜像/旋转
道具的镜像旋转部分由以下参数控制，或者某些独有参数。

```c#
//flags: FU_ADM_FLAG_FLIP_X = 32;
//		 FU_ADM_FLAG_FLIP_Y = 64; 
//翻转只翻转2D道具渲染，并不会翻转整个图像
FaceunityWorker.SetImage(ptr, flags, false, w, h);

//is3DFlipH 参数是用于对3D道具的顶点镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "is3DFlipH", param);
//isFlipExpr 参数是用于对道具内部的表情系数的镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipExpr", param);
//isFlipTrack 参数是用于对道具的人脸跟踪位置旋转的镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipTrack", param);
//isFlipLight 参数是用于对道具内部的灯光的镜像
FaceunityWorker.fu_ItemSetParamd(itemid, "isFlipLight", param);
```

#### 5.2.3 输出图像的镜像/旋转

```c#
//输出的图像的镜像/旋转算是后期处理，参考这个函数调整UI
public void SelfAdjusSize();
//或者参考这个调整纹理
public Texture2D AdjustTex(Texture2D tex_origin,int SwichXY, int flipx, int flipy);
```
