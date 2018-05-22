using System.Collections;

public struct Item
{
    public string name;
    public string fullname; //路径
    public string iconname;
    public int type;

    Item(string _name="",string _fullname="",string _iconname="",int _type=0)
    {
        name = _name;
        fullname = _fullname;
        iconname = _iconname;
        type = _type;
    }

};

public class ItemConfig
{
    public static Item[] beautySkin ={
        new Item { name = "beautification", fullname = "face_beautification", iconname = "" ,type=0},
    };

    //Animoji
    public static Item[] item_1 = {
        new Item { name = "baimao_Animoji", fullname = "items/Animoji/baimao_Animoji", iconname = "uiprefabs/items/Animoji/baimao_Animoji" ,type=1},
        new Item { name = "chaiquan_Animoji", fullname = "items/Animoji/chaiquan_Animoji", iconname = "uiprefabs/items/Animoji/chaiquan_Animoji" ,type=1},
        new Item { name = "douniuquan_Animoji", fullname = "items/Animoji/douniuquan_Animoji", iconname = "uiprefabs/items/Animoji/douniuquan_Animoji" ,type=1},
        new Item { name = "kulutou_Animoji", fullname = "items/Animoji/kulutou_Animoji", iconname = "uiprefabs/items/Animoji/kulutou_Animoji" ,type=1},
        new Item { name = "maotouying_Animoji", fullname = "items/Animoji/maotouying_Animoji", iconname = "uiprefabs/items/Animoji/maotouying_Animoji" ,type=1},
    };

    //ItemSticker
    public static Item[] item_2 = {
        new Item { name = "fengya_ztt_fu", fullname = "items/ItemSticker/fengya_ztt_fu", iconname = "uiprefabs/items/ItemSticker/fengya_ztt_fu" ,type=2},
        new Item { name = "hudie_lm_fu", fullname = "items/ItemSticker/hudie_lm_fu", iconname = "uiprefabs/items/ItemSticker/hudie_lm_fu" ,type=2},
        new Item { name = "juanhuzi_lm_fu", fullname = "items/ItemSticker/juanhuzi_lm_fu", iconname = "uiprefabs/items/ItemSticker/juanhuzi_lm_fu" ,type=2},
        new Item { name = "mask_hat", fullname = "items/ItemSticker/mask_hat", iconname = "uiprefabs/items/ItemSticker/mask_hat" ,type=2},
        new Item { name = "touhua_ztt_fu", fullname = "items/ItemSticker/touhua_ztt_fu", iconname = "uiprefabs/items/ItemSticker/touhua_ztt_fu" ,type=2},
        new Item { name = "yazui", fullname = "items/ItemSticker/yazui", iconname = "uiprefabs/items/ItemSticker/yazui" ,type=2},
        new Item { name = "yuguan", fullname = "items/ItemSticker/yuguan", iconname = "uiprefabs/items/ItemSticker/yuguan" ,type=2},
    };

    //ARMask
    public static Item[] item_3 = {
        new Item { name = "afd", fullname = "items/ARMask/afd", iconname = "uiprefabs/items/ARMask/afd" ,type=3},
        new Item { name = "armesh", fullname = "items/ARMask/armesh", iconname = "uiprefabs/items/ARMask/armesh" ,type=3},
        new Item { name = "armesh_ex", fullname = "items/ARMask/armesh_ex", iconname = "uiprefabs/items/ARMask/armesh_ex" ,type=3},
        new Item { name = "baozi", fullname = "items/ARMask/baozi", iconname = "uiprefabs/items/ARMask/baozi" ,type=3},
        new Item { name = "tiger", fullname = "items/ARMask/tiger", iconname = "uiprefabs/items/ARMask/tiger" ,type=3},
        new Item { name = "xiongmao", fullname = "items/ARMask/xiongmao", iconname = "uiprefabs/items/ARMask/xiongmao" ,type=3},
    };

    //ChangeFace
    public static Item[] item_4 = {
        new Item { name = "mask_guocaijie", fullname = "items/ChangeFace/mask_guocaijie", iconname = "uiprefabs/items/ChangeFace/mask_guocaijie" ,type=4},
        new Item { name = "mask_huangxiaoming", fullname = "items/ChangeFace/mask_huangxiaoming", iconname = "uiprefabs/items/ChangeFace/mask_huangxiaoming" ,type=4},
        new Item { name = "mask_linzhiling", fullname = "items/ChangeFace/mask_linzhiling", iconname = "uiprefabs/items/ChangeFace/mask_linzhiling" ,type=4},
        new Item { name = "mask_liudehua", fullname = "items/ChangeFace/mask_liudehua", iconname = "uiprefabs/items/ChangeFace/mask_liudehua" ,type=4},
        new Item { name = "mask_luhan", fullname = "items/ChangeFace/mask_luhan", iconname = "uiprefabs/items/ChangeFace/mask_luhan" ,type=4},
        new Item { name = "mask_matianyu", fullname = "items/ChangeFace/mask_matianyu", iconname = "uiprefabs/items/ChangeFace/mask_matianyu" ,type=4},
        new Item { name = "mask_tongliya", fullname = "items/ChangeFace/mask_tongliya", iconname = "uiprefabs/items/ChangeFace/mask_tongliya" ,type=4},
    };

    //ExpressionRecognition
    public static Item[] item_5 = {
        new Item { name = "future_warrior", fullname = "items/ExpressionRecognition/future_warrior", iconname = "uiprefabs/items/ExpressionRecognition/future_warrior" ,type=5},
        new Item { name = "jet_mask", fullname = "items/ExpressionRecognition/jet_mask", iconname = "uiprefabs/items/ExpressionRecognition/jet_mask" ,type=5},
        new Item { name = "luhantongkuan_ztt_fu", fullname = "items/ExpressionRecognition/luhantongkuan_ztt_fu", iconname = "uiprefabs/items/ExpressionRecognition/luhantongkuan_ztt_fu" ,type=5},
        new Item { name = "qingqing_ztt_fu", fullname = "items/ExpressionRecognition/qingqing_ztt_fu", iconname = "uiprefabs/items/ExpressionRecognition/qingqing_ztt_fu" ,type=5},
        new Item { name = "sdx2", fullname = "items/ExpressionRecognition/sdx2", iconname = "uiprefabs/items/ExpressionRecognition/sdx2" ,type=5},
        new Item { name = "xiaobianzi_zh_fu", fullname = "items/ExpressionRecognition/xiaobianzi_zh_fu", iconname = "uiprefabs/items/ExpressionRecognition/xiaobianzi_zh_fu" ,type=5},
        new Item { name = "xiaoxueshen_ztt_fu", fullname = "items/ExpressionRecognition/xiaoxueshen_ztt_fu", iconname = "uiprefabs/items/ExpressionRecognition/xiaoxueshen_ztt_fu" ,type=5},
    };

    //MusicFilter
    public static Item[] item_6 = {
        new Item { name = "douyin_01", fullname = "items/MusicFilter/douyin_01", iconname = "uiprefabs/items/MusicFilter/douyin_01" ,type=6},
        new Item { name = "douyin_02", fullname = "items/MusicFilter/douyin_02", iconname = "uiprefabs/items/MusicFilter/douyin_02" ,type=6},
    };

    //BackgroundSegmentation
    public static Item[] item_7 = {
        new Item { name = "chiji_lm_fu", fullname = "items/BackgroundSegmentation/chiji_lm_fu", iconname = "uiprefabs/items/BackgroundSegmentation/chiji_lm_fu" ,type=7},
        new Item { name = "gufeng_zh_fu", fullname = "items/BackgroundSegmentation/gufeng_zh_fu", iconname = "uiprefabs/items/BackgroundSegmentation/gufeng_zh_fu" ,type=7},
        new Item { name = "hez_ztt_fu", fullname = "items/BackgroundSegmentation/hez_ztt_fu", iconname = "uiprefabs/items/BackgroundSegmentation/hez_ztt_fu" ,type=7},
        new Item { name = "ice_lm_fu", fullname = "items/BackgroundSegmentation/ice_lm_fu", iconname = "uiprefabs/items/BackgroundSegmentation/ice_lm_fu" ,type=7},
        new Item { name = "jingongmen_zh_fu", fullname = "items/BackgroundSegmentation/jingongmen_zh_fu", iconname = "uiprefabs/items/BackgroundSegmentation/jingongmen_zh_fu" ,type=7},
        new Item { name = "men_ztt_fu", fullname = "items/BackgroundSegmentation/men_ztt_fu", iconname = "uiprefabs/items/BackgroundSegmentation/men_ztt_fu" ,type=7},
    };

    //GestureRecognition
    public static Item[] item_8 = {
         new Item { name = "fu_lm_koreaheart", fullname = "items/GestureRecognition/fu_lm_koreaheart", iconname = "uiprefabs/items/GestureRecognition/fu_lm_koreaheart" ,type=8},
         new Item { name = "fu_zh_baoquan", fullname = "items/GestureRecognition/fu_zh_baoquan", iconname = "uiprefabs/items/GestureRecognition/fu_zh_baoquan" ,type=8},
         new Item { name = "fu_zh_hezxiong", fullname = "items/GestureRecognition/fu_zh_hezxiong", iconname = "uiprefabs/items/GestureRecognition/fu_zh_hezxiong" ,type=8},
         new Item { name = "fu_ztt_live520", fullname = "items/GestureRecognition/fu_ztt_live520", iconname = "uiprefabs/items/GestureRecognition/fu_ztt_live520" ,type=8},
         new Item { name = "ssd_thread_cute", fullname = "items/GestureRecognition/ssd_thread_cute", iconname = "uiprefabs/items/GestureRecognition/ssd_thread_cute" ,type=8},
         new Item { name = "ssd_thread_six", fullname = "items/GestureRecognition/ssd_thread_six", iconname = "uiprefabs/items/GestureRecognition/ssd_thread_six" ,type=8},
         new Item { name = "ssd_thread_thumb", fullname = "items/GestureRecognition/ssd_thread_thumb", iconname = "uiprefabs/items/GestureRecognition/ssd_thread_thumb" ,type=8},
    };

    //MagicMirror
    public static Item[] item_9 = {
        new Item { name = "facewarp2", fullname = "items/MagicMirror/facewarp2", iconname = "uiprefabs/items/MagicMirror/facewarp2" ,type=9},
        new Item { name = "facewarp3", fullname = "items/MagicMirror/facewarp3", iconname = "uiprefabs/items/MagicMirror/facewarp3" ,type=9},
        new Item { name = "facewarp4", fullname = "items/MagicMirror/facewarp4", iconname = "uiprefabs/items/MagicMirror/facewarp4" ,type=9},
        new Item { name = "facewarp5", fullname = "items/MagicMirror/facewarp5", iconname = "uiprefabs/items/MagicMirror/facewarp5" ,type=9},
        new Item { name = "facewarp6", fullname = "items/MagicMirror/facewarp6", iconname = "uiprefabs/items/MagicMirror/facewarp6" ,type=9},
    };

    //PortraitLightEffect
    public static Item[] item_10 ={
        new Item { name = "PortraitLighting_effect_0", fullname = "items/PortraitLightEffect/PortraitLighting_effect_0", iconname = "uiprefabs/items/PortraitLightEffect/portrait_lighting_effect_0" ,type=10},
        new Item { name = "PortraitLighting_effect_1", fullname = "items/PortraitLightEffect/PortraitLighting_effect_1", iconname = "uiprefabs/items/PortraitLightEffect/portrait_lighting_effect_1" ,type=10},
        new Item { name = "PortraitLighting_effect_2", fullname = "items/PortraitLightEffect/PortraitLighting_effect_2", iconname = "uiprefabs/items/PortraitLightEffect/portrait_lighting_effect_2" ,type=10},
        new Item { name = "PortraitLighting_effect_3", fullname = "items/PortraitLightEffect/PortraitLighting_effect_3", iconname = "uiprefabs/items/PortraitLightEffect/portrait_lighting_effect_3" ,type=10},
        new Item { name = "PortraitLighting_X_rim", fullname = "items/PortraitLightEffect/PortraitLighting_X_rim", iconname = "uiprefabs/items/PortraitLightEffect/portrait_lighting_x_rim" ,type=10},
        new Item { name = "PortraitLighting_X_studio", fullname = "items/PortraitLightEffect/PortraitLighting_X_studio", iconname = "uiprefabs/items/PortraitLightEffect/portrait_lighting_x_studio" ,type=10},
    };

    //PortraitDrive
    public static Item[] item_11 ={
        new Item { name = "picasso_e1", fullname = "items/PortraitDrive/picasso_e1", iconname = "uiprefabs/items/PortraitDrive/picasso_e1" ,type=11},
        new Item { name = "picasso_e2", fullname = "items/PortraitDrive/picasso_e2", iconname = "uiprefabs/items/PortraitDrive/picasso_e2" ,type=11},
        new Item { name = "picasso_e3", fullname = "items/PortraitDrive/picasso_e3", iconname = "uiprefabs/items/PortraitDrive/picasso_e3" ,type=11},
    };
}

public class Beauty
{
    public bool ifenable = false;
    public float currentvalue = 0;

    public string name = "";
    public string paramword = "";
    public float maxvalue = 1;
    public float defaultvalue = 0;
    public float disablevalue = 0;
    public string iconname_0 = "";
    public string iconname_1 = "";

};

public class BeautyConfig
{
    public static Beauty[] beautySkin_1 = {
        new Beauty { name = "精准美肤", paramword = "skin_detect", maxvalue=1, defaultvalue=1, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/1_jingzhunmeifu_y",iconname_1="uiprefabs/beautySkin/beautyskin/1_jingzhunmeifu_n" },//0-1 int,d=0
        new Beauty { name = "清晰磨皮", paramword = "heavy_blur", maxvalue=1, defaultvalue=0, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/2_meifuleixing_y",iconname_1="uiprefabs/beautySkin/beautyskin/2_meifuleixing_y" },//0-1 int,d=0 朦胧磨皮
        new Beauty { name = "磨皮", paramword = "blur_level", maxvalue=6, defaultvalue=6, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/3_mopi_y",iconname_1="uiprefabs/beautySkin/beautyskin/3_mopi_n" },//0-6 int,
        new Beauty { name = "美白", paramword = "color_level", maxvalue=1, defaultvalue=0.5f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/4_meibai_y",iconname_1="uiprefabs/beautySkin/beautyskin/4_meibai_n" },//0-1f
        new Beauty { name = "红润", paramword = "red_level", maxvalue=1, defaultvalue=0.5f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/5_hongrun_y",iconname_1="uiprefabs/beautySkin/beautyskin/5_hongrun_n" },//0-1f
        new Beauty { name = "亮眼", paramword = "eye_bright", maxvalue=1, defaultvalue=0.7f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/6_liangyan_y",iconname_1="uiprefabs/beautySkin/beautyskin/6_liangyan_n" },//0-1f
        new Beauty { name = "美牙", paramword = "tooth_whiten", maxvalue=1, defaultvalue=0.7f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyskin/7_meiya_y",iconname_1="uiprefabs/beautySkin/beautyskin/7_meiya_n" },//0-1f
        new Beauty { name = "重置", paramword = "RESET", iconname_0 = "uiprefabs/beautySkin/beautyskin/0_reset",iconname_1="uiprefabs/beautySkin/beautyskin/0_reset" },
    };

    public static Beauty[] beautySkin_2 = {
        new Beauty { name = "脸型", paramword = "face_shape", maxvalue=4, defaultvalue=4, disablevalue=3, iconname_0 = "uiprefabs/beautySkin/beautyshape/1_lianxing_y",iconname_1="uiprefabs/beautySkin/beautyshape/1_lianxing_n" },//0-4 int ,d=3
        new Beauty { name = "大眼", paramword = "eye_enlarging", maxvalue=1, defaultvalue=0.4f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyshape/2_dayan_y",iconname_1="uiprefabs/beautySkin/beautyshape/2_dayan_n" },//0-1f
        new Beauty { name = "瘦脸", paramword = "cheek_thinning", maxvalue=1, defaultvalue=0.4f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyshape/3_shoulian_y",iconname_1="uiprefabs/beautySkin/beautyshape/3_shoulian_n" },//0-1f
        new Beauty { name = "下巴", paramword = "intensity_chin", maxvalue=1, defaultvalue=0.3f, disablevalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyshape/4_xiaba_y",iconname_1="uiprefabs/beautySkin/beautyshape/4_xiaba_n" },//0-1f
        new Beauty { name = "额头", paramword = "intensity_forehead", maxvalue=1, defaultvalue=0.3f, disablevalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyshape/5_etou_y",iconname_1="uiprefabs/beautySkin/beautyshape/5_etou_n" },//0-1f
        new Beauty { name = "瘦鼻", paramword = "intensity_nose", maxvalue=1, defaultvalue=0.5f, disablevalue=0, iconname_0 = "uiprefabs/beautySkin/beautyshape/6_shoubi_y",iconname_1="uiprefabs/beautySkin/beautyshape/6_shoubi_n" },//0-1f
        new Beauty { name = "嘴型", paramword = "intensity_mouth", maxvalue=1, defaultvalue=0.4f, disablevalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyshape/7_zuixing_y",iconname_1="uiprefabs/beautySkin/beautyshape/7_zuixing_n" },//0-1f
        new Beauty { name = "重置", paramword = "RESET", iconname_0 = "uiprefabs/beautySkin/beautyshape/0_reset",iconname_1="uiprefabs/beautySkin/beautyshape/0_reset" },
    };

    public static Beauty[] beautySkin_3 = {
        new Beauty { name = "自然", paramword = "ziran", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyfilter/ziran@3x"},//0-1f
        new Beauty { name = "淡雅", paramword = "danya", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyfilter/danya@3x"},
        new Beauty { name = "粉嫩", paramword = "fennen", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyfilter/fennen@3x"},
        new Beauty { name = "清新", paramword = "qingxin", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyfilter/qingxin@3x"},
        new Beauty { name = "红润", paramword = "hongrun", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/beautyfilter/hongrun@3x"},
    };

    public static Beauty[] beautySkin_4 = {
        new Beauty { name = "origin", paramword = "origin", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/filter/origin@3x"},//0-1f
        new Beauty { name = "delta", paramword = "delta", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/filter/delta@3x"},
        new Beauty { name = "electric", paramword = "electric", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/filter/electric@3x"},
        new Beauty { name = "slowlived", paramword = "slowlived", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/filter/slowlived@3x"},
        new Beauty { name = "tokyo", paramword = "tokyo", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/filter/tokyo@3x"},
        new Beauty { name = "warm", paramword = "warm", maxvalue=1, defaultvalue=0.5f, iconname_0 = "uiprefabs/beautySkin/filter/warm@3x"},
    };
}