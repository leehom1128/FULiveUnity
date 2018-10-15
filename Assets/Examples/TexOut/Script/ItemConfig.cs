using System.Collections;

public struct Item
{
    public string name;
    public string fullname; //路径
    public int iconid;
    public int type;

    Item(string _name="",string _fullname="",int _iconid = -1,int _type=0)
    {
        name = _name;
        fullname = _fullname;
        iconid = _iconid;
        type = _type;
    }

};

public class ItemConfig
{
    public static Item[] beautySkin ={
        new Item { name = "beautification", fullname = "face_beautification", iconid = -1 ,type=0},
    };

    //Animoji
    public static Item[] item_1 = {
        new Item { name = "frog_Animoji", fullname = "items/Animoji/frog_Animoji", iconid = 4 ,type=1},
        new Item { name = "huangya_Animoji", fullname = "items/Animoji/huangya_Animoji", iconid = 7 ,type=1},
        new Item { name = "hetun_Animoji", fullname = "items/Animoji/hetun_Animoji", iconid = 6 ,type=1},
        new Item { name = "buoutuzi_Animoji", fullname = "items/Animoji/buoutuzi_Animoji", iconid = 1 ,type=1},
        new Item { name = "douniuquan_Animoji", fullname = "items/Animoji/douniuquan_Animoji", iconid = 3 ,type=1},
        new Item { name = "hashiqi_Animoji", fullname = "items/Animoji/hashiqi_Animoji", iconid = 5 ,type=1},
        new Item { name = "baimao_Animoji", fullname = "items/Animoji/baimao_Animoji", iconid = 0 ,type=1},
        new Item { name = "chaiquan_Animoji", fullname = "items/Animoji/chaiquan_Animoji", iconid = 2 ,type=1},
        new Item { name = "kuloutou_Animoji", fullname = "items/Animoji/kuloutou_Animoji", iconid = 8 ,type=1},
    };

    //ItemSticker
    public static Item[] item_2 = {
        new Item { name = "fengya_ztt_fu", fullname = "items/ItemSticker/fengya_ztt_fu", iconid = 0 ,type=2},
        new Item { name = "hudie_lm_fu", fullname = "items/ItemSticker/hudie_lm_fu", iconid = 1 ,type=2},
        new Item { name = "juanhuzi_lm_fu", fullname = "items/ItemSticker/juanhuzi_lm_fu", iconid = 2 ,type=2},
        new Item { name = "mask_hat", fullname = "items/ItemSticker/mask_hat", iconid = 3 ,type=2},
        new Item { name = "touhua_ztt_fu", fullname = "items/ItemSticker/touhua_ztt_fu", iconid = 4 ,type=2},
        new Item { name = "yazui", fullname = "items/ItemSticker/yazui", iconid = 5 ,type=2},
        new Item { name = "yuguan", fullname = "items/ItemSticker/yuguan", iconid = 6 ,type=2},
    };

    //ARMask
    public static Item[] item_3 = {
        new Item { name = "afd", fullname = "items/ARMask/afd", iconid = 0 ,type=3},
        new Item { name = "armesh", fullname = "items/ARMask/armesh", iconid = 1 ,type=3},
        new Item { name = "armesh_ex", fullname = "items/ARMask/armesh_ex", iconid = 2 ,type=3},
        new Item { name = "baozi", fullname = "items/ARMask/baozi", iconid = 3 ,type=3},
        new Item { name = "tiger", fullname = "items/ARMask/tiger", iconid = 4 ,type=3},
        new Item { name = "xiongmao", fullname = "items/ARMask/xiongmao", iconid = 5 ,type=3},
    };

    //ChangeFace
    public static Item[] item_4 = {
        new Item { name = "mask_guocaijie", fullname = "items/ChangeFace/mask_guocaijie", iconid = 0 ,type=4},
        new Item { name = "mask_huangxiaoming", fullname = "items/ChangeFace/mask_huangxiaoming", iconid = 1 ,type=4},
        new Item { name = "mask_linzhiling", fullname = "items/ChangeFace/mask_linzhiling", iconid = 2 ,type=4},
        new Item { name = "mask_liudehua", fullname = "items/ChangeFace/mask_liudehua", iconid = 3 ,type=4},
        new Item { name = "mask_luhan", fullname = "items/ChangeFace/mask_luhan", iconid = 4 ,type=4},
        new Item { name = "mask_matianyu", fullname = "items/ChangeFace/mask_matianyu", iconid = 5 ,type=4},
        new Item { name = "mask_tongliya", fullname = "items/ChangeFace/mask_tongliya", iconid = 6 ,type=4},
    };

    //ExpressionRecognition
    public static Item[] item_5 = {
        new Item { name = "future_warrior", fullname = "items/ExpressionRecognition/future_warrior", iconid = 0 ,type=5},
        new Item { name = "jet_mask", fullname = "items/ExpressionRecognition/jet_mask", iconid = 1 ,type=5},
        new Item { name = "luhantongkuan_ztt_fu", fullname = "items/ExpressionRecognition/luhantongkuan_ztt_fu", iconid = 2 ,type=5},
        new Item { name = "qingqing_ztt_fu", fullname = "items/ExpressionRecognition/qingqing_ztt_fu", iconid = 3 ,type=5},
        new Item { name = "sdx2", fullname = "items/ExpressionRecognition/sdx2", iconid = 4 ,type=5},
        new Item { name = "xiaobianzi_zh_fu", fullname = "items/ExpressionRecognition/xiaobianzi_zh_fu", iconid = 5 ,type=5},
        new Item { name = "xiaoxueshen_ztt_fu", fullname = "items/ExpressionRecognition/xiaoxueshen_ztt_fu", iconid = 6 ,type=5},
    };

    //MusicFilter
    public static Item[] item_6 = {
        new Item { name = "douyin_01", fullname = "items/MusicFilter/douyin_01", iconid = 0 ,type=6},
        new Item { name = "douyin_02", fullname = "items/MusicFilter/douyin_02", iconid = 1 ,type=6},
    };

    //BackgroundSegmentation
    public static Item[] item_7 = {
        new Item { name = "chiji_lm_fu", fullname = "items/BackgroundSegmentation/chiji_lm_fu", iconid = 0 ,type=7},
        new Item { name = "gufeng_zh_fu", fullname = "items/BackgroundSegmentation/gufeng_zh_fu", iconid = 1 ,type=7},
        new Item { name = "hez_ztt_fu", fullname = "items/BackgroundSegmentation/hez_ztt_fu", iconid = 2 ,type=7},
        new Item { name = "ice_lm_fu", fullname = "items/BackgroundSegmentation/ice_lm_fu", iconid = 3 ,type=7},
        new Item { name = "jingongmen_zh_fu", fullname = "items/BackgroundSegmentation/jingongmen_zh_fu", iconid = 4 ,type=7},
        new Item { name = "men_ztt_fu", fullname = "items/BackgroundSegmentation/men_ztt_fu", iconid = 5 ,type=7},
        new Item { name = "sea_lm_fu", fullname = "items/BackgroundSegmentation/sea_lm_fu", iconid = 6 ,type=7},
        new Item { name = "xiandai_ztt_fu", fullname = "items/BackgroundSegmentation/xiandai_ztt_fu", iconid = 7 ,type=7},
    };

    //GestureRecognition
    public static Item[] item_8 = {
         new Item { name = "fu_lm_koreaheart", fullname = "items/GestureRecognition/fu_lm_koreaheart", iconid = 0 ,type=8},
         new Item { name = "fu_zh_baoquan", fullname = "items/GestureRecognition/fu_zh_baoquan", iconid = 1 ,type=8},
         new Item { name = "fu_zh_hezxiong", fullname = "items/GestureRecognition/fu_zh_hezxiong", iconid = 2 ,type=8},
         new Item { name = "fu_ztt_live520", fullname = "items/GestureRecognition/fu_ztt_live520", iconid = 3 ,type=8},
         new Item { name = "ssd_thread_cute", fullname = "items/GestureRecognition/ssd_thread_cute", iconid = 4 ,type=8},
         new Item { name = "ssd_thread_six", fullname = "items/GestureRecognition/ssd_thread_six", iconid = 5 ,type=8},
         new Item { name = "ssd_thread_thumb", fullname = "items/GestureRecognition/ssd_thread_thumb", iconid = 6 ,type=8},
    };

    //MagicMirror
    public static Item[] item_9 = {
        new Item { name = "facewarp2", fullname = "items/MagicMirror/facewarp2", iconid = 0 ,type=9},
        new Item { name = "facewarp3", fullname = "items/MagicMirror/facewarp3", iconid = 1 ,type=9},
        new Item { name = "facewarp4", fullname = "items/MagicMirror/facewarp4", iconid = 2 ,type=9},
        new Item { name = "facewarp5", fullname = "items/MagicMirror/facewarp5", iconid = 3 ,type=9},
        new Item { name = "facewarp6", fullname = "items/MagicMirror/facewarp6", iconid = 4 ,type=9},
    };

    //PortraitLightEffect
    public static Item[] item_10 ={
        new Item { name = "PortraitLighting_effect_0", fullname = "items/PortraitLightEffect/PortraitLighting_effect_0", iconid = 0 ,type=10},
        new Item { name = "PortraitLighting_effect_1", fullname = "items/PortraitLightEffect/PortraitLighting_effect_1", iconid = 1 ,type=10},
        new Item { name = "PortraitLighting_effect_2", fullname = "items/PortraitLightEffect/PortraitLighting_effect_2", iconid = 2 ,type=10},
        new Item { name = "PortraitLighting_effect_3", fullname = "items/PortraitLightEffect/PortraitLighting_effect_3", iconid = 3 ,type=10},
        new Item { name = "PortraitLighting_X_rim", fullname = "items/PortraitLightEffect/PortraitLighting_X_rim", iconid = 4 ,type=10},
        new Item { name = "PortraitLighting_X_studio", fullname = "items/PortraitLightEffect/PortraitLighting_X_studio", iconid = 5 ,type=10},
    };

    //PortraitDrive
    public static Item[] item_11 ={
        new Item { name = "picasso_e1", fullname = "items/PortraitDrive/picasso_e1", iconid = 0,type=11},
        new Item { name = "picasso_e2", fullname = "items/PortraitDrive/picasso_e2", iconid = 1 ,type=11},
        new Item { name = "picasso_e3", fullname = "items/PortraitDrive/picasso_e3", iconid = 2 ,type=11},
    };

    //Makeup lipstick
    public static Item[] item_makeup_1 ={
        new Item { name = "MU_Lipstick_01", fullname = "makeup/lipstick/MU_Lipstick_01", iconid = 0 ,type=12},
        new Item { name = "MU_Lipstick_02", fullname = "makeup/lipstick/MU_Lipstick_02", iconid = 1 ,type=12},
        new Item { name = "MU_Lipstick_03", fullname = "makeup/lipstick/MU_Lipstick_03", iconid = 2 ,type=12},
        new Item { name = "MU_Lipstick_04", fullname = "makeup/lipstick/MU_Lipstick_04", iconid = 3 ,type=12},
        new Item { name = "MU_Lipstick_05", fullname = "makeup/lipstick/MU_Lipstick_05", iconid = 4 ,type=12},
        new Item { name = "MU_Lipstick_06", fullname = "makeup/lipstick/MU_Lipstick_06", iconid = 5 ,type=12},
    };

    //Makeup blusher
    public static Item[] item_makeup_2 ={
        new Item { name = "MU_Blush_01", fullname = "makeup/blusher/MU_Blush_01", iconid = 0 ,type=13},
        new Item { name = "MU_Blush_02", fullname = "makeup/blusher/MU_Blush_02", iconid = 1 ,type=13},
        new Item { name = "MU_Blush_03", fullname = "makeup/blusher/MU_Blush_03", iconid = 2 ,type=13},
        new Item { name = "MU_Blush_04", fullname = "makeup/blusher/MU_Blush_04", iconid = 3 ,type=13},
        new Item { name = "MU_Blush_05", fullname = "makeup/blusher/MU_Blush_05", iconid = 4 ,type=13},
        new Item { name = "MU_Blush_06", fullname = "makeup/blusher/MU_Blush_06", iconid = 5 ,type=13},
        new Item { name = "MU_Blush_07", fullname = "makeup/blusher/MU_Blush_07", iconid = 6 ,type=13},
        new Item { name = "MU_Blush_08", fullname = "makeup/blusher/MU_Blush_08", iconid = 7 ,type=13},
        new Item { name = "MU_Blush_09", fullname = "makeup/blusher/MU_Blush_09", iconid = 8 ,type=13},
        new Item { name = "MU_Blush_10", fullname = "makeup/blusher/MU_Blush_10", iconid = 9 ,type=13},
    };

    //Makeup eyebrow
    public static Item[] item_makeup_3 ={
        new Item { name = "MU_Eyebrow_01", fullname = "makeup/eyebrow/MU_Eyebrow_01", iconid = 0 ,type=14},
        new Item { name = "MU_Eyebrow_02", fullname = "makeup/eyebrow/MU_Eyebrow_02", iconid = 1 ,type=14},
        new Item { name = "MU_Eyebrow_03", fullname = "makeup/eyebrow/MU_Eyebrow_03", iconid = 2 ,type=14},
        new Item { name = "MU_Eyebrow_04", fullname = "makeup/eyebrow/MU_Eyebrow_04", iconid = 3 ,type=14},
        new Item { name = "MU_Eyebrow_05", fullname = "makeup/eyebrow/MU_Eyebrow_05", iconid = 4 ,type=14},
        new Item { name = "MU_Eyebrow_06", fullname = "makeup/eyebrow/MU_Eyebrow_06", iconid = 5 ,type=14},
    };

    //Makeup eyeshadow
    public static Item[] item_makeup_4 ={
        new Item { name = "MU_EyeShadow_01", fullname = "makeup/eyeshadow/MU_EyeShadow_01", iconid = 0 ,type=15},
        new Item { name = "MU_EyeShadow_02", fullname = "makeup/eyeshadow/MU_EyeShadow_02", iconid = 1 ,type=15},
        new Item { name = "MU_EyeShadow_03", fullname = "makeup/eyeshadow/MU_EyeShadow_03", iconid = 2 ,type=15},
        new Item { name = "MU_EyeShadow_04", fullname = "makeup/eyeshadow/MU_EyeShadow_04", iconid = 3 ,type=15},
        new Item { name = "MU_EyeShadow_05", fullname = "makeup/eyeshadow/MU_EyeShadow_05", iconid = 4 ,type=15},
        new Item { name = "MU_EyeShadow_06", fullname = "makeup/eyeshadow/MU_EyeShadow_06", iconid = 5 ,type=15},
    };

    //Makeup eyeliner
    public static Item[] item_makeup_5 ={
        new Item { name = "MU_EyeLiner_01", fullname = "makeup/eyeliner/MU_EyeLiner_01", iconid = 0 ,type=16},
        new Item { name = "MU_EyeLiner_02", fullname = "makeup/eyeliner/MU_EyeLiner_02", iconid = 1 ,type=16},
        new Item { name = "MU_EyeLiner_03", fullname = "makeup/eyeliner/MU_EyeLiner_03", iconid = 2 ,type=16},
        new Item { name = "MU_EyeLiner_04", fullname = "makeup/eyeliner/MU_EyeLiner_04", iconid = 3 ,type=16},
        new Item { name = "MU_EyeLiner_05", fullname = "makeup/eyeliner/MU_EyeLiner_05", iconid = 4 ,type=16},
        new Item { name = "MU_EyeLiner_06", fullname = "makeup/eyeliner/MU_EyeLiner_06", iconid = 5 ,type=16},
    };

    //Makeup eyelash
    public static Item[] item_makeup_6 ={
        new Item { name = "MU_Eyelash_01", fullname = "makeup/eyelash/MU_Eyelash_01", iconid = 0 ,type=17},
        new Item { name = "MU_Eyelash_02", fullname = "makeup/eyelash/MU_Eyelash_02", iconid = 1 ,type=17},
        new Item { name = "MU_Eyelash_03", fullname = "makeup/eyelash/MU_Eyelash_03", iconid = 2 ,type=17},
        new Item { name = "MU_Eyelash_04", fullname = "makeup/eyelash/MU_Eyelash_04", iconid = 3 ,type=17},
        new Item { name = "MU_Eyelash_05", fullname = "makeup/eyelash/MU_Eyelash_05", iconid = 4 ,type=17},
        new Item { name = "MU_Eyelash_06", fullname = "makeup/eyelash/MU_Eyelash_06", iconid = 5 ,type=17},
    };

    //Makeup contactlens
    public static Item[] item_makeup_7 ={
        new Item { name = "MU_ContactLenses_01", fullname = "makeup/contactlens/MU_ContactLenses_01", iconid = 0 ,type=18},
        new Item { name = "MU_ContactLenses_02", fullname = "makeup/contactlens/MU_ContactLenses_02", iconid = 1 ,type=18},
        new Item { name = "MU_ContactLenses_03", fullname = "makeup/contactlens/MU_ContactLenses_03", iconid = 2 ,type=18},
        new Item { name = "MU_ContactLenses_04", fullname = "makeup/contactlens/MU_ContactLenses_04", iconid = 3 ,type=18},
        new Item { name = "MU_ContactLenses_05", fullname = "makeup/contactlens/MU_ContactLenses_05", iconid = 4 ,type=18},
        new Item { name = "MU_ContactLenses_06", fullname = "makeup/contactlens/MU_ContactLenses_06", iconid = 5 ,type=18},
        new Item { name = "MU_ContactLenses_07", fullname = "makeup/contactlens/MU_ContactLenses_07", iconid = 6 ,type=18},
        new Item { name = "MU_ContactLenses_08", fullname = "makeup/contactlens/MU_ContactLenses_08", iconid = 7 ,type=18},
        new Item { name = "MU_ContactLenses_09", fullname = "makeup/contactlens/MU_ContactLenses_09", iconid = 8 ,type=18},
    };
}

public class Beauty
{
    public float currentvalue = 0;

    public string name = "";
    public string paramword = "";
    public float maxvalue = 1;
    public float defaultvalue = 0;
    public float disablevalue = 0;
    public int iconid_0 = -1;
    public int iconid_1 = -1;
};

public class BeautyConfig
{
    public static Beauty[] beautySkin_1 = {
        new Beauty { name = "精准美肤", paramword = "skin_detect", maxvalue=1, defaultvalue=1, disablevalue=0, iconid_0 = 0,iconid_1=1 },//0-1 int,d=0
        new Beauty { name = "清晰磨皮", paramword = "heavy_blur", maxvalue=1, defaultvalue=0, disablevalue=0, iconid_0 = 2,iconid_1=3 },//0-1 int,d=0 朦胧磨皮
        new Beauty { name = "磨皮", paramword = "blur_level", maxvalue=6, defaultvalue=6, disablevalue=0, iconid_0 = 2,iconid_1=3 },//0-6 int,
        new Beauty { name = "美白", paramword = "color_level", maxvalue=1, defaultvalue=0.5f, disablevalue=0, iconid_0 = 4,iconid_1=5 },//0-1f
        new Beauty { name = "红润", paramword = "red_level", maxvalue=1, defaultvalue=0.5f, disablevalue=0, iconid_0 = 6,iconid_1=7 },//0-1f
        new Beauty { name = "亮眼", paramword = "eye_bright", maxvalue=1, defaultvalue=0.7f, disablevalue=0, iconid_0 = 8,iconid_1=9 },//0-1f
        new Beauty { name = "美牙", paramword = "tooth_whiten", maxvalue=1, defaultvalue=0.7f, disablevalue=0, iconid_0 = 10,iconid_1=11 },//0-1f
    };

    public static Beauty[] beautySkin_2 = {
        new Beauty { name = "脸型", paramword = "face_shape", maxvalue=4, defaultvalue=4, disablevalue=-1, iconid_0 = 12,iconid_1=13 },//0-4 int ,d=3
        new Beauty { name = "大眼", paramword = "eye_enlarging", maxvalue=1, defaultvalue=0.4f, disablevalue=0, iconid_0 = 14,iconid_1=15 },//0-1f
        new Beauty { name = "瘦脸", paramword = "cheek_thinning", maxvalue=1, defaultvalue=0.4f, disablevalue=0, iconid_0 = 16,iconid_1=17 },//0-1f
        new Beauty { name = "下巴", paramword = "intensity_chin", maxvalue=1, defaultvalue=0.3f, disablevalue=0.5f, iconid_0 = 18,iconid_1=19 },//0-1f
        new Beauty { name = "额头", paramword = "intensity_forehead", maxvalue=1, defaultvalue=0.3f, disablevalue=0.5f, iconid_0 = 20,iconid_1=21 },//0-1f
        new Beauty { name = "瘦鼻", paramword = "intensity_nose", maxvalue=1, defaultvalue=0.5f, disablevalue=0, iconid_0 = 22,iconid_1=23 },//0-1f
        new Beauty { name = "嘴型", paramword = "intensity_mouth", maxvalue=1, defaultvalue=0.4f, disablevalue=0.5f, iconid_0 = 24,iconid_1=25 },//0-1f
    };

    public static Beauty[] beautySkin_3 = {
        new Beauty { name = "原图", paramword = "origin", maxvalue=1, defaultvalue=0.5f, iconid_0 = 31},//0-1f
        new Beauty { name = "自然", paramword = "ziran", maxvalue=1, defaultvalue=0.5f, iconid_0 = 26},
        new Beauty { name = "淡雅", paramword = "danya", maxvalue=1, defaultvalue=0.5f, iconid_0 = 27},
        new Beauty { name = "粉嫩", paramword = "fennen", maxvalue=1, defaultvalue=0.5f, iconid_0 = 28},
        new Beauty { name = "清新", paramword = "qingxin", maxvalue=1, defaultvalue=0.5f, iconid_0 = 29},
        new Beauty { name = "红润", paramword = "hongrun", maxvalue=1, defaultvalue=0.5f, iconid_0 = 30},
    };

    public static Beauty[] beautySkin_4 = {
        new Beauty { name = "Origin", paramword = "origin", maxvalue=1, defaultvalue=0.5f, iconid_0 = 31},//0-1f
        new Beauty { name = "Delta", paramword = "delta", maxvalue=1, defaultvalue=0.5f, iconid_0 = 32},
        new Beauty { name = "Electric", paramword = "electric", maxvalue=1, defaultvalue=0.5f, iconid_0 = 33},
        new Beauty { name = "Slowlived", paramword = "slowlived", maxvalue=1, defaultvalue=0.5f, iconid_0 = 34},
        new Beauty { name = "Tokyo", paramword = "tokyo", maxvalue=1, defaultvalue=0.5f, iconid_0 = 35},
        new Beauty { name = "Warm", paramword = "warm", maxvalue=1, defaultvalue=0.5f, iconid_0 = 36},
    };
}