# Demo运行说明文档-Unity 
级别：Public
更新日期：2020-03-19
SDK版本: 6.7.0 

------

## 最新更新内容：

2020-03-19 v6.7.0:

1. 美颜效果
   - 新增去黑眼圈、去法令纹功能
   - 优化磨皮效果，新增只磨皮人脸区域接口功能
   - 优化原有美型效果

2. 优化表情跟踪效果，解决了6.6.0版表情系数表情灵活度问题——FaceProcessor模块优化
   - 解决Animoji表情灵活度问题，基本与原有SDK v6.4.0效果相近
   - 解决优化了表情动图的鼻子跟踪效果问题

3. 优化美妆效果，人脸点位优化，提高准确性
   - 优化口红点位与效果，解决张嘴、正脸、低抬头、左右转头、抿嘴动作的口红溢色
   - 优化美瞳点位效果，美瞳效果稳定
   - 美妆素材效果优化，增加卧蚕提升了眼影层次感，优化腮红拉扯问题

4. 新增接口支持图像裁剪，解决瘦脸边缘变形问题（边缘变形剪裁）
5. 新增接口判断初始化完成状态
7. Unity版本新增离线鉴权

2020-01-19 v6.6.0：

__版本整体说明:__ SDK 6.6.0 主要针对美颜、美妆进行效果优化，性能优化，稳定性优化，同时新增部分特性，使得美颜、美妆效果进入行业顶尖水平。建议对美颜、美妆需求较高的B端用户更新SDK。  
__注意!!!__：此版本由于底层替换原因，表情识别跟踪能力稍有降低，特别是Animoji、表情触发道具的整体表情表现力稍有减弱。Animoji的皱眉、鼓嘴、嘟嘴等动作表现效果比之较差，表情触发道具的发怒（皱眉）、鼓嘴、嘟嘴的表情触发道具较难驱动。其余ARMesh、哈哈镜、明星换脸、动态人像（活照片）的面部跟踪整体稍有10%的效果减弱。故用到表情驱动的功能重度B端用户，仍建议使用SDK6.4.0版，使用其余功能（美颜叠加贴纸等其余功能）的场景不受影响，表情识别跟踪能力将在下一版进行优化更新。   

- 美颜优化：  
  1). 新增美型6款功能，包括开眼角、眼距、眼睛角度、长鼻、缩人中、微笑嘴角。
   2). 新增17款滤镜，其中包含8款自然系列滤镜、8款质感灰系列滤镜、1款个性滤镜。
   3). 优化美颜中亮眼、美牙效果。
   4). 优化美颜中3个脸型，调整优化使得V脸、窄脸、小脸效果更自然。
   5). 优化美白红润强度，美白、红润功能开放2倍参数，详见美颜文档。
- 美妆优化：  
  1). 新增13套自然系组合妆，13套组合妆是滤镜+美妆的整体效果，可自定义。
   2). 新增3款口红质地：润泽、珠光、咬唇。
   3). 提升美妆点位准确度 ，人脸点位由209点增加至 239点。
   4). 优化美妆素材叠加方式，使得妆容效果更加服帖自然。
   5). 优化粉底效果，更加贴合人脸轮廓。
- 提升人脸点位跟踪灵敏度，快速移动时跟踪良好，使美颜美妆效果跟随更紧密。
- 提升人脸点位的稳定性，解决了半张脸屏幕、大角度、遮挡等场景的阈值抖动问题，点位抖动问题也明显优化。
- 提升人脸跟踪角度，人脸最大左右偏转角提升至70度，低抬头检测偏转角也明显提升。
- 优化美发道具CPU占有率，Android/iOS提升约30%
- 新增MSAA抗锯齿接口，fuSetMultiSamples，解决虚拟形象（animoji与捏脸功能）边缘锯齿问题，详见接口文档。
- 架构升级，支持底层AI算法能力和业务逻辑拆分，优化性能，使得系统更加容易扩展和更新迭代：  
  1). 新增加接口 fuLoadAIModelFromPackage 用于加载AI能力模型。
   2). 新增加接口 fuReleaseAIModel 用于释放AI能力模型。
   3). 新增加接口 fuIsAIModelLoaded 用于判断AI能力是否已经加载。

__注1__：从SDK 6.6.0 开始，为了更新以及迭代更加方便，由原先一个nama.so拆分成两个库nama.so以及fuai.so，其中nama.so为轻量级渲染引擎，fuai.so为算法引擎。升级6.6.0时，需添加fuai库。  
__注2__: 更新SDK 6.6.0时，在fuSetup之后，需要马上调用 fuLoadAIModelFromPackage 加载 ai_faceprocessor.bundle !!!  
__注3__: SDK 6.6.0 进行较大的架构调整 , 架构上拆分底层算法能力和业务场景，使得SDK更能够按需复用算法模块，节省内存开销，算法能力模块后期更容易维护升级，使用方式详见新增加的一组接口定义fuLoadAIModelFromPackage / fuReleaseAIModel / fuIsAIModelLoaded 。  

2019-09-25 v6.4.0:

- 新增美体瘦身功能，支持瘦身、长腿、美臀、细腰、肩部调整，一键美体。
- 优化美颜功能中精细磨皮，性能以及效果提升，提升皮肤细腻程度，更好保留边缘细节。
- 优化美发功能，边缘稳定性及性能提升。
- 优化美妆功能，性能提升，CPU占有率降低，Android中低端机表现明显。
- 优化手势识别功能，性能提升，CPU占有率降低，在Android机型表现明显。
- 修复人脸检测多人脸偶现crash问题。
- 修复捏脸功能中模型截断问题。
- 关闭美颜道具打印冗余log。

2019-08-14 v6.3.0：

- 优化人脸美妆功能，提高性能，降低功耗。
- 新增fuSetFaceTrackParam接口，用于设置人脸表情跟踪参数。 

- 新增人脸美颜精细磨皮效果。

文档：

   - [美颜道具功能文档](美颜道具功能文档.md)

   - [美妆道具功能文档](美妆道具功能文档.md)


工程案例更新：

- 由于Nama 6.6的内部机制更新，AI和渲染分离，现在Nama运行在FU_Mode_RenderItems模式下（渲染Nama道具）时，如果不加载任何道具，Nama也不会运行任何AI逻辑，此时无法进行人脸检测等操作，也无法拿到相关数据！！！因此本工程案例里在DataOut场景和Simple场景中都添加了自动加载一个空道具的逻辑，以应对出现的问题。
- 当Nama运行在FU_Mode_TrackFace模式下时，无需加载任何道具，会自动跑人脸识别的AI逻辑
- Nama6.6同时也带来了道具加载卸载机制的更新，新的道具加载卸载接口已经全部都是同步接口，调用后立即执行，没有异步没有协程，简化了道具加载卸载的逻辑复杂度。
- 本次更新添加了一个C#封装函数以更新默认道具/跟踪方向，这个函数会根据当前平台环境、相机是否镜像以及重力感应方向，自动设置道具和跟踪的默认方向，在Texout场景中需要每帧调用以适应重力感应，Dataout场景只需相机切换时调用。具体描述请看API文档，具体应用请看本函数在Demo中的引用。

------
### 目录：
本文档内容目录：

[TOC]

------
### 1. 简介 
本文档旨在说明如何将Faceunity Nama SDK的Unity Demo运行起来，体验Faceunity Nama SDK的功能。FULiveUnity 是集成了 Faceunity 面部跟踪、美颜、Animoji、道具贴纸、AR面具、换脸、表情识别、音乐滤镜、背景分割、手势识别、哈哈镜、人像光照以及人像驱动功能的Demo。Demo新增了一个展示Faceunity产品列表的主界面，新版Demo将根据客户证书权限来控制用户可以使用哪些产品。  

------
### 2. Demo文件结构
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
      -ai_face_processor.bytes：初始化完成后必须加载的AI数据文件
      -ai_bgseg.bytes：背景分割AI数据文件
      -ai_bgseg_green.bytes：带绿幕的背景分割AI数据文件
      -ai_gesture.bytes：手势跟踪AI数据文件
      -ai_hairseg.bytes：头发分割AI数据文件
      -ai_humanpose.bytes：人体姿态跟踪AI数据文件
      -tongue.bytes：舌头跟踪AI数据文件
      -EmptyItem.bytes：空道具，FU_Mode_RenderItems模式下，如果不想加载其他道具，则加载这个，以获取人脸跟踪数据
  +docs					//文档目录
  +ProjectSettings   	//Unity工程配置目录
  -readme.md			//工程总文档
  
```

------
### 3. 运行Demo 

#### 3.1 开发环境
##### 3.1.1 支持平台
```
Windows、Android、iOS（9.0以上系统）、Mac
```
##### 3.1.2 开发环境
```
Unity5.4.6f3 及以上
```

#### 3.2 准备工作 
- [下载demo代码](https://github.com/Faceunity/FULiveUnity)
- 获取证书:
  1. 拨打电话 **0571-88069272** 
  2. 发送邮件至 **marketing@faceunity.com** 进行咨询。

#### 3.3 相关配置

- 将证书文件中的数据复制到场景中FaceunityWorker物体的Inspector面板的LICENSE输入框内，并保存场景（其他场景同理）。

![](imgs/img0.jpg)

#### 3.4 编译运行

- 点击播放按钮在UnityEditor里运行，或者打开BuildSettings，选择你想看的场景，点击Build或者Build And Run编译出你想要的平台的安装包。

![](imgs\img1.jpg)

------
### 4. 常见问题 
- iOS编译时请选择**9.0**以上系统版本。
- **因Github不支持上传100MB以上的文件，iOS的库经过压缩，使用时请自行解压！**
- Unity工程导入可能会导致部分native库的平台信息丢失，如果运行或者编译时报错提示找不到库，请手动修改相应库的平台信息。本案例中的native库有:
  - Assets\Plugins
  - Assets\Examples\Common\NatCam\Core\Plugins
![](imgs\img2.jpg)
