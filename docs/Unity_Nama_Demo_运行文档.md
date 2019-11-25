# Demo运行说明文档-Unity 
级别：Public
更新日期：2019-09-25 
SDK版本: 6.4.0 

------

## 最新更新内容：

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

- 美颜道具部分接口已改变，请注意同步代码！
- 舌头跟踪相关请查看本文档及代码注释！
- anim_model.bytes以及ardata_ex.bytes已弃用，请删除相关数据及代码

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
      -tongue.bytes：舌头跟踪必须的文件
      -EnableTongueForUnity.bytes：某种情况下获取舌头跟踪数据需要加载的文件
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
