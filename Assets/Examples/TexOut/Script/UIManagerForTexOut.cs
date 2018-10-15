using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

public class UIManagerForTexOut : MonoBehaviour
{

    RenderToTexture rtt;

    public GameObject Canvas_BackUI; //后景UI，包括rtt.RawImg_BackGroud
    public Camera Camera_BackUI;
    public GameObject RawImage_Pic;
    public Button Btn_Cancel;
    public Button Btn_SavePic;

    public GameObject Canvas_TopUI; //主界面UI
    public Camera Camera_TopUI;
    public GameObject[] ItemSelecters;

    public GameObject Canvas_FrontUI; //前景UI
    public Camera Camera_FrontUI;
    public Button Btn_Switch;
    public Button Btn_Back;
    public GameObject Image_FaceDetect;

    public Button Btn_TakePic_mini_1;   //美颜相关UI
    public GameObject BeautySkinSelecter;
    public GameObject[] BeautySkinSelecterOptions;
    public GameObject BeautySkinContent;
    public GameObject[] BeautySkinContentPanels;
    public GameObject BeautySkin_UIExample;
    public GameObject BeautyFilter_UIExample;

    public GameObject[] BeautySkin_FaceShape;
    public Slider BeautySkin_Slider;
    public RectTransform BeautyOptionContentTrans;

    public Button Btn_TakePic_mini_2;   //道具相关UI
    public GameObject Item_Content;
    public ToggleGroup Item_ToggleGroup;
    public RectTransform ItemOptionContentTrans;
    public GameObject Item_UIExample;
    public GameObject Item_Disable;

    public Button Btn_TakePic_mini_3;   //美妆相关UI
    public GameObject MakeupSelecter;
    public GameObject[] MakeupSelecterOptions;
    public GameObject MakeupContent;
    public GameObject[] MakeupContentPanels;
    public Slider Makeup_Slider;
    public RectTransform MakeupOptionContentTrans;
    public GameObject Makeup_UIExample;
    public GameObject Makeup_Disable;

    Coroutine musiccor = null;
    AudioSource audios;

    Color highlightColor = new Color(0.337f, 0.792f, 0.957f, 1);
    Color normalColor = Color.white;

    Dictionary<Beauty, GameObject> BeautyGOs = new Dictionary<Beauty, GameObject>();
    GameObject currentSelected;
    string BeautySkinItemName;

    enum BeautySkinType
    {
        None = 0,
        BeautySkin = 1,
        BeautyShape,
        BeautyFilter,
        Filter,
    }
    BeautySkinType currentBeautySkinType = BeautySkinType.None;


    enum ItemType
    {
        None = -1,
        Beauty = 0,
        Animoji = 1,
        ItemSticker,
        ARMask,
        ChangeFace,
        ExpressionRecognition,
        MusicFilter,
        BackgroundSegmentation,
        GestureRecognition,
        MagicMirror,
        PortraitLightEffect,
        PortraitDrive,
        MakeUp,
    }
    ItemType currentItemType = ItemType.None;

    enum MakeupType
    {
        None = -1,
        Lipstick = 0,
        Blusher = 1,
        EyeBrow,
        EyeShadow,
        EyeLiner,
        EyeLash,
        ContactLens,
    }
    MakeupType currentMakeupType = MakeupType.None;

    private static int[] permissions_code = {
            0x1,                    //美颜
            0x10,                   //Animoji
            0x2 | 0x4,              //道具贴纸
            0x20 | 0x40,            //AR面具
            0x80,                   //换脸
            0x800,                  //表情识别
            0x20000,                //音乐滤镜
            0x100,                  //背景分割
            0x200,                  //手势识别
            0x10000,                //哈哈镜
            0x4000,                 //人像光效
            0x8000,                 //人像驱动
            0x80000                 //美妆
    };
    private static bool[] permissions;
    Sprites uisprites;

    enum SlotForItems   //道具的槽位
    {
        Beauty = 0,
        Item = 1,
        EyeShadow,
        EyeLiner,
        EyeLash,
        ContactLens,
        EyeBrow,
        Lipstick,
        Blusher,
    };

    void Awake()
    {
        rtt = GetComponent<RenderToTexture>();
        audios = GetComponent<AudioSource>();
        uisprites = GetComponent<Sprites>();
    }

    void Start()
    {
        FaceunityWorker.instance.OnInitOK += InitApplication;
        Canvas_TopUI.SetActive(true);
        Canvas_FrontUI.SetActive(true);
        Canvas_BackUI.SetActive(true);
        CloseBeautySkinUI();
        CloseItemUI();
        CloseMakeUpUI();
    }

    void InitApplication(object source, EventArgs e)
    {
        StartCoroutine(Authentication());
    }

    void Update()
    {
        //TODO:根据场景处理
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (FaceunityWorker.fu_IsTracking() > 0)
            Image_FaceDetect.SetActive(false);
        else
            Image_FaceDetect.SetActive(true);
    }

    IEnumerator Authentication()
    {
        while (FaceunityWorker.jc_part_inited() == 0)
            yield return Util._endOfFrame;
        int code = FaceunityWorker.fu_GetModuleCode(0);
        Debug.Log("fu_GetModuleCode:" + code);
        permissions = new bool[permissions_code.Length];
        for (int i = 0; i < permissions_code.Length; i++)
        {
            if ((code & permissions_code[i]) == permissions_code[i])
            {
                permissions[i] = true;
                SetItemTypeEnable(i, true);
            }
            else
            {
                permissions[i] = false;
                Debug.Log("权限未获取:" + permissions_code[i]);
                SetItemTypeEnable(i, false);
            }
        }
        RegisterUIFunc();
        if (permissions[0])
        {
            yield return rtt.LoadItem(ItemConfig.beautySkin[0], (int)SlotForItems.Beauty);
            BeautySkinItemName = ItemConfig.beautySkin[0].name;
            for (int i = 0; i < BeautyConfig.beautySkin_1.Length; i++)
            {
                rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[i].paramword, BeautyConfig.beautySkin_1[i].defaultvalue);
            }
            for (int i = 0; i < BeautyConfig.beautySkin_2.Length; i++)
            {
                rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_2[i].paramword, BeautyConfig.beautySkin_2[i].defaultvalue);
            }
        }
    }

    void SetItemTypeEnable(int i,bool ifenable)
    {
        if(i< ItemSelecters.Length && ItemSelecters[i])
        {
            ItemSelecters[i].transform.Find("Image_On").gameObject.SetActive(ifenable);
            ItemSelecters[i].transform.Find("Image_Off").gameObject.SetActive(!ifenable);
            ItemSelecters[i].transform.Find("Image_bg").GetComponent<Image>().raycastTarget = ifenable;
        }
    }

    //for循环配合delegate是个坑，小心。
    void RegisterUIFunc()
    {
        Btn_Switch.onClick.AddListener(delegate { rtt.SwitchCamera(); });
        Btn_Back.onClick.AddListener(delegate {
            if (musiccor != null)
            {
                StopCoroutine(musiccor);
                musiccor = null;
                audios.Stop();
            }
            Canvas_TopUI.SetActive(true);
            //unload除了美颜以外所有道具
            for (int i=1; i<RenderToTexture.SLOTLENGTH; i++)
                rtt.UnLoadItem(i);
            CloseBeautySkinUI();
            CloseItemUI();
            CloseMakeUpUI();
        });
        Btn_Cancel.onClick.AddListener(OnCancelTakePicture);
        Btn_SavePic.onClick.AddListener(OnSavePicture);
        Btn_TakePic_mini_1.onClick.AddListener(TakePicture);
        Btn_TakePic_mini_2.onClick.AddListener(TakePicture);
        Btn_TakePic_mini_3.onClick.AddListener(TakePicture);

        for (int i=0;i< ItemSelecters.Length;i++)
        {
            if (ItemSelecters[i].activeSelf)
                ItemSelecters[i].GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                {
                    int id = int.Parse(go.name);
                    if (id == 0)
                    {
                        OpenBeautySkinUI(BeautySkinType.None);
                    }
                    else if (id == 12)
                    {
                        MakeupSelecter.GetComponent<ScrollRect>().content.localPosition = Vector3.zero;
                        OpenMakeUpUI(MakeupType.None);
                    }
                    else
                    {
                        OpenItemsUI((ItemType)id);
                    }
                    Canvas_TopUI.SetActive(false);
                });
        }

        rtt.RawImg_BackGroud.GetComponent<AddClickEvent>().AddListener(delegate {
            if (currentItemType == ItemType.Beauty)
            {
                CloseAllBeautySkinContent();
                BeautySkinContentPanels[3].SetActive(true);
            }
            else if (currentItemType == ItemType.MakeUp)
            {
                CloseAllMakeUpContentUI();
                MakeupContentPanels[1].SetActive(true);
                MakeupContentPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0,-193.7f);
            }
        });

        BeautySkinSelecterOptions[0].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautySkin); });
        BeautySkinSelecterOptions[1].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautyShape); });
        BeautySkinSelecterOptions[2].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautyFilter); });
        BeautySkinSelecterOptions[3].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.Filter); });

        MakeupSelecterOptions[0].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.Lipstick); });
        MakeupSelecterOptions[1].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.Blusher); });
        MakeupSelecterOptions[2].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.EyeBrow); });
        MakeupSelecterOptions[3].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.EyeShadow); });
        MakeupSelecterOptions[4].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.EyeLiner); });
        MakeupSelecterOptions[5].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.EyeLash); });
        MakeupSelecterOptions[6].GetComponent<AddClickEvent>().AddListener(delegate { OpenMakeUpUI(MakeupType.ContactLens); });
    }

    void UnRegisterUIFunc()
    {
        Btn_Switch.onClick.RemoveAllListeners();
        Btn_Back.onClick.RemoveAllListeners();
        Btn_Cancel.onClick.RemoveAllListeners();
        Btn_SavePic.onClick.RemoveAllListeners();
        Btn_TakePic_mini_1.onClick.RemoveAllListeners();
        Btn_TakePic_mini_2.onClick.RemoveAllListeners();
        Btn_TakePic_mini_3.onClick.RemoveAllListeners();
        for (int i = 0; i < ItemSelecters.Length; i++)
        {
            ItemSelecters[i].GetComponent<AddClickEvent>().RemoveAllListener();
        }

        rtt.RawImg_BackGroud.GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[0].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[1].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[2].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[3].GetComponent<AddClickEvent>().RemoveAllListener();

        MakeupSelecterOptions[0].GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupSelecterOptions[1].GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupSelecterOptions[2].GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupSelecterOptions[3].GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupSelecterOptions[4].GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupSelecterOptions[5].GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupSelecterOptions[6].GetComponent<AddClickEvent>().RemoveAllListener();
    }

    void SwitchPicGos(bool ifenable)
    {
        RawImage_Pic.SetActive(ifenable);
        Btn_Cancel.gameObject.SetActive(ifenable);
        Btn_SavePic.gameObject.SetActive(ifenable);
    }

    void TakePicture()
    {
        RawImage_Pic.GetComponent<RawImage>().texture = rtt.CaptureCamera(new Camera[] { Camera_BackUI }, new Rect(0, 0, Screen.width, Screen.height));
        Canvas_FrontUI.SetActive(false);
        SwitchPicGos(true);
    }

    void OnCancelTakePicture()
    {
        SwitchPicGos(false);
        Canvas_FrontUI.SetActive(true);
    }

    void OnSavePicture()
    {
        rtt.SaveTex2D((Texture2D)RawImage_Pic.GetComponent<RawImage>().texture);
        OnCancelTakePicture();
    }

    #region BeautySkinUI

    void OpenBeautySkinUI(BeautySkinType type)
    {
        currentBeautySkinType = type;
        currentItemType = ItemType.Beauty;

        CloseAllBeautySkinContent();
        GameObject panel = BeautySkinContentPanels[0];
        BeautyOptionContentTrans.localPosition = Vector3.zero;
        var layout = BeautyOptionContentTrans.GetComponent<HorizontalLayoutGroup>();
        layout.padding.left = 20;
        layout.padding.right = 20;
        BeautySkinContentPanels[3].SetActive(true);
        ClearBeautySkinOptions();

        if (type == BeautySkinType.BeautySkin)
        {
            BeautySkinSelecterOptions[0].GetComponent<Text>().color = highlightColor;
            
            AddBeautySkinOptions(0, BeautyConfig.beautySkin_1[0]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
            {
                Beauty bi = BeautyConfig.beautySkin_1[0];
                if (currentSelected != go)
                {
                    currentSelected = go;
                    UnSelectAllBeautySkinOptions();
                    go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_1);
                    go.GetComponentInChildren<Text>().color = highlightColor;
                    BeautySkinContentPanels[2].SetActive(false);
                    BeautySkinContentPanels[1].SetActive(false);
                }
                else
                {
                    bi.currentvalue = bi.currentvalue==bi.disablevalue ? bi.maxvalue : bi.disablevalue;
                    rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[0].paramword, bi.currentvalue);
                    SwitchBeautyOptionUIState(bi, go);
                }
            });
            
            GameObject bgo1 = AddBeautySkinOptions(1, BeautyConfig.beautySkin_1[2]);
            bgo1.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
            {
                Beauty bi1 = BeautyConfig.beautySkin_1[1];
                Beauty bi2 = BeautyConfig.beautySkin_1[2];

                if (currentSelected != go)
                {
                    currentSelected = go;
                    UnSelectAllBeautySkinOptions();
                    go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi2.iconid_1);
                    go.GetComponentInChildren<Text>().color = highlightColor;
                    BeautySkinContentPanels[2].SetActive(false);
                    BeautySkin_Slider.onValueChanged.RemoveAllListeners();
                    BeautySkin_Slider.minValue = 0;
                    BeautySkin_Slider.maxValue = bi2.maxvalue;
                    BeautySkin_Slider.value = bi2.currentvalue;
                    BeautySkin_Slider.onValueChanged.AddListener(delegate
                    {
                        bi2.currentvalue = BeautySkin_Slider.value;
                        rtt.SetItemParamd(BeautySkinItemName, bi2.paramword, bi2.currentvalue);
                        SwitchBeautyOptionUIState(bi2, go);
                    });
                    BeautySkinContentPanels[1].SetActive(true);
                }
                else
                {
                    bi1.currentvalue = bi1.currentvalue == bi1.disablevalue ? bi1.maxvalue : bi1.disablevalue;
                    if (bi1.currentvalue == bi1.maxvalue)
                    {
                        go.GetComponentInChildren<Text>().text = "朦胧磨皮";
                    }
                    else
                    {
                        go.GetComponentInChildren<Text>().text = "清晰磨皮";
                    }
                    rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[1].paramword, bi1.currentvalue);
                }
            });
            if (BeautyConfig.beautySkin_1[1].currentvalue == BeautyConfig.beautySkin_1[1].maxvalue)
            {
                bgo1.GetComponentInChildren<Text>().text = "朦胧磨皮";
            }
            else
            {
                bgo1.GetComponentInChildren<Text>().text = "清晰磨皮";
            }

            for (int i = 3; i < BeautyConfig.beautySkin_1.Length; i++)
            {
                AddBeautySkinOptions(i, BeautyConfig.beautySkin_1[i]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                 {
                     if (currentSelected != go)
                     {
                         Beauty bi = null;
                         foreach (var bgo in BeautyGOs)
                         {
                             if (bgo.Value == go)
                                 bi = bgo.Key;
                         }
                         if (bi == null)
                         {
                             Debug.Log("Undefined BeautyGO!!! name=" + go.name);
                             return;
                         }
                         currentSelected = go;
                         UnSelectAllBeautySkinOptions();
                         go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_1);
                         go.GetComponentInChildren<Text>().color = highlightColor;
                         BeautySkinContentPanels[2].SetActive(false);
                         BeautySkin_Slider.onValueChanged.RemoveAllListeners();
                         BeautySkin_Slider.minValue = 0;
                         BeautySkin_Slider.maxValue = bi.maxvalue;
                         BeautySkin_Slider.value = bi.currentvalue;
                         BeautySkin_Slider.onValueChanged.AddListener(delegate
                             {
                                 bi.currentvalue = BeautySkin_Slider.value;
                                 rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.currentvalue);
                                 SwitchBeautyOptionUIState(bi, go);
                             });
                         BeautySkinContentPanels[1].SetActive(true);
                     }
                 });
            }

            panel.SetActive(true);
            BeautySkinContentPanels[4].SetActive(true);
        }
        else if (type == BeautySkinType.BeautyShape)
        {
            BeautySkinSelecterOptions[1].GetComponent<Text>().color = highlightColor;

            GameObject bgo1 = AddBeautySkinOptions(1, BeautyConfig.beautySkin_2[0]);
            bgo1.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
            {
                if (currentSelected != go)
                {
                    Beauty bi = BeautyConfig.beautySkin_2[0];
                    currentSelected = go;
                    UnSelectAllBeautySkinOptions();
                    go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_1);
                    go.GetComponentInChildren<Text>().color = highlightColor;
                    BeautySkinContentPanels[1].SetActive(false);
                    BeautySkinContentPanels[2].SetActive(true);
                }
            });
            bgo1.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, BeautyConfig.beautySkin_2[0].iconid_1);
            bgo1.GetComponentInChildren<Text>().color = highlightColor;

            for (int i = 1; i < BeautyConfig.beautySkin_2.Length; i++)
            {
                AddBeautySkinOptions(i, BeautyConfig.beautySkin_2[i]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                {
                    if (currentSelected != go)
                    {
                        Beauty bi = null;
                        foreach (var bgo in BeautyGOs)
                        {
                            if (bgo.Value == go)
                                bi = bgo.Key;
                        }
                        if (bi == null)
                        {
                            Debug.Log("Undefined BeautyGO!!! name=" + go.name);
                            return;
                        }
                        currentSelected = go;
                        UnSelectAllBeautySkinOptions();
                        go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_1);
                        go.GetComponentInChildren<Text>().color = highlightColor;
                        BeautySkinContentPanels[2].SetActive(false);
                        BeautySkin_Slider.onValueChanged.RemoveAllListeners();
                        BeautySkin_Slider.minValue = 0;
                        BeautySkin_Slider.maxValue = bi.maxvalue;
                        BeautySkin_Slider.value = bi.currentvalue;
                        BeautySkin_Slider.onValueChanged.AddListener(delegate
                        {
                            bi.currentvalue = BeautySkin_Slider.value;
                            rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.currentvalue);
                            SwitchBeautyOptionUIState(bi, go);
                        });
                        BeautySkinContentPanels[1].SetActive(true);
                    }
                });
            }

            for (int i = 0; i < BeautySkin_FaceShape.Length; i++)
            {
                BeautySkin_FaceShape[i].GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                {
                    BeautyConfig.beautySkin_2[0].currentvalue = int.Parse(go.name);
                    OpenBeautyShapeUI();
                });
            }
            if(BeautyConfig.beautySkin_2[0].currentvalue == -1)
                BeautyConfig.beautySkin_2[0].currentvalue = 4;
            else if (BeautyConfig.beautySkin_2[0].currentvalue == -5)
                BeautyConfig.beautySkin_2[0].currentvalue = 3;
            OpenBeautyShapeUI();

            panel.SetActive(true);
            BeautySkinContentPanels[1].SetActive(false);
            BeautySkinContentPanels[2].SetActive(true);
            BeautySkinContentPanels[4].SetActive(true);
        }
        else if (type == BeautySkinType.BeautyFilter)
        {
            BeautySkinSelecterOptions[2].GetComponent<Text>().color = highlightColor;
            string currentfiltername = rtt.GetItemParams(BeautySkinItemName, "filter_name");
            foreach (var bi in BeautyConfig.beautySkin_3)
            {
                GameObject go = AddBeautyFilterOptions(bi);
                if (string.Compare(bi.paramword, currentfiltername, true) == 0)
                {
                    currentSelected = go;
                    go.transform.Find("Image_bg").gameObject.SetActive(true);
                }
            }

            BeautySkin_Slider.onValueChanged.RemoveAllListeners();
            BeautySkin_Slider.minValue = 0;
            BeautySkin_Slider.maxValue = 1;
            BeautySkin_Slider.value = (float)rtt.GetItemParamd(BeautySkinItemName, "filter_level");
            BeautySkin_Slider.onValueChanged.AddListener(delegate
            {
                rtt.SetItemParamd(BeautySkinItemName, "filter_level", BeautySkin_Slider.value);
            });
            panel.SetActive(true);
            BeautySkinContentPanels[1].SetActive(true);
            BeautySkinContentPanels[2].SetActive(false);
            BeautySkinContentPanels[4].SetActive(true);
        }
        else if (type == BeautySkinType.Filter)
        {
            BeautySkinSelecterOptions[3].GetComponent<Text>().color = highlightColor;
            string currentfiltername = rtt.GetItemParams(BeautySkinItemName, "filter_name");
            foreach (var bi in BeautyConfig.beautySkin_4)
            {
                GameObject go = AddBeautyFilterOptions(bi);
                if (string.Compare(bi.paramword, currentfiltername, true) == 0)
                {
                    currentSelected = go;
                    go.transform.Find("Image_bg").gameObject.SetActive(true);
                }
            }

            BeautySkin_Slider.onValueChanged.RemoveAllListeners();
            BeautySkin_Slider.minValue = 0;
            BeautySkin_Slider.maxValue = 1;
            BeautySkin_Slider.value = (float)rtt.GetItemParamd(BeautySkinItemName, "filter_level");
            BeautySkin_Slider.onValueChanged.AddListener(delegate
            {
                rtt.SetItemParamd(BeautySkinItemName, "filter_level", BeautySkin_Slider.value);
            });

            panel.SetActive(true);
            BeautySkinContentPanels[1].SetActive(true);
            BeautySkinContentPanels[2].SetActive(false);
            BeautySkinContentPanels[4].SetActive(true);
        }

        BeautySkinContent.SetActive(true);
        BeautySkinSelecter.SetActive(true);
    }

    GameObject AddBeautySkinOptions(int name, Beauty beautyitem)
    {
        beautyitem.currentvalue = (float)rtt.GetItemParamd(BeautySkinItemName, beautyitem.paramword);
        GameObject option = Instantiate(BeautySkin_UIExample);
        option.transform.SetParent(BeautyOptionContentTrans, false);
        option.transform.localScale = Vector3.one;
        option.transform.localPosition = Vector3.zero;
        option.name = name.ToString();
        option.GetComponentInChildren<Text>().text = beautyitem.name;
        option.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, beautyitem.iconid_0);

        if (BeautyGOs.ContainsKey(beautyitem))
            BeautyGOs.Remove(beautyitem);
        BeautyGOs.Add(beautyitem, option);
        SwitchBeautyOptionUIState(beautyitem, option);
        return option;
    }

    void OpenBeautyShapeUI()
    {
        for (int i = 0; i < BeautySkin_FaceShape.Length; i++)
        {
            if (i == (int)BeautyConfig.beautySkin_2[0].currentvalue)
            {
                BeautySkin_FaceShape[i].GetComponentInChildren<Text>().color = highlightColor;
                BeautySkin_FaceShape[i].transform.Find("Image_bg").gameObject.SetActive(true);
            }
            else
            {
                BeautySkin_FaceShape[i].GetComponentInChildren<Text>().color = normalColor;
                BeautySkin_FaceShape[i].transform.Find("Image_bg").gameObject.SetActive(false);
            }
        }

        int addnum;
        var layout = BeautyOptionContentTrans.GetComponent<HorizontalLayoutGroup>();
        if (BeautyConfig.beautySkin_2[0].currentvalue == 4)
        {
            layout.padding.left = 20;
            layout.padding.right = 20;
            addnum = BeautyConfig.beautySkin_2.Length;
        }
        else
        {
            int d=(int)((Canvas_FrontUI.GetComponent<RectTransform>().sizeDelta.x-540)*0.5);    //540=150*3+45*2
            layout.padding.left = d;
            layout.padding.right = d;
            addnum = 3;
        }
        
        for (int i = 1; i < BeautyConfig.beautySkin_2.Length; i++)
        {
            if (i < addnum)
                BeautyGOs[BeautyConfig.beautySkin_2[i]].SetActive(true);
            else
                BeautyGOs[BeautyConfig.beautySkin_2[i]].SetActive(false);
        }
        rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_2[0].paramword, BeautyConfig.beautySkin_2[0].currentvalue);
    }

    GameObject AddBeautyFilterOptions(Beauty beautyitem)
    {
        GameObject option = Instantiate(BeautyFilter_UIExample);
        option.transform.SetParent(BeautyOptionContentTrans, false);
        option.transform.localScale = Vector3.one;
        option.transform.localPosition = Vector3.zero;
        option.name = beautyitem.name;
        option.GetComponentInChildren<Text>().text = beautyitem.name;
        option.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, beautyitem.iconid_0);

        if (BeautyGOs.ContainsKey(beautyitem))
            BeautyGOs.Remove(beautyitem);
        BeautyGOs.Add(beautyitem, option);
        option.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
        {
            if (go != currentSelected)
            {
                currentSelected = go;
                foreach (var bgo in BeautyGOs)
                {
                    bgo.Value.transform.Find("Image_bg").gameObject.SetActive(false);
                }
                go.transform.Find("Image_bg").gameObject.SetActive(true);
            }
            rtt.SetItemParams(BeautySkinItemName, "filter_name", beautyitem.paramword);
        });
        return option;
    }

    void SwitchBeautyOptionUIState(Beauty bi, GameObject go)
    {
        var bg = go.transform.Find("Image_bg");
        if (bg)
            if (Math.Abs(bi.currentvalue-bi.disablevalue)<0.01)
            {
                bg.gameObject.SetActive(false);
            }
            else
            {
                bg.gameObject.SetActive(true);
            }
    }

    void ClearBeautySkinOptions()
    {
        foreach (Transform childTr in BeautyOptionContentTrans)
        {
            childTr.GetComponent<AddClickEvent>().RemoveAllListener();
            Destroy(childTr.gameObject);
        }
        BeautyGOs.Clear();
    }

    void UnSelectAllBeautySkinOptions()
    {
        foreach (var bgo in BeautyGOs)
        {
            bgo.Value.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bgo.Key.iconid_0);
            bgo.Value.GetComponentInChildren<Text>().color = normalColor;
        }
    }

    void CloseAllBeautySkinContent()
    {
        foreach (var go in BeautySkinContentPanels)
        {
            go.SetActive(false);
        }
        foreach (var go in BeautySkinSelecterOptions)
        {
            go.GetComponent<Text>().color = normalColor;
        }
    }

    void CloseBeautySkinUI()
    {
        BeautySkinContent.SetActive(false);
        BeautySkinSelecter.SetActive(false);
    }
    #endregion

    #region ItemsUI

    void OpenItemsUI(ItemType it)
    {
        currentItemType = it;
        ItemOptionContentTrans.localPosition = Vector3.zero;
        ClearItemsOptions();
        switch (it)
        {
            case ItemType.Animoji:
                AddItemOptions(ItemConfig.item_1);
                break;
            case ItemType.ItemSticker:
                AddItemOptions(ItemConfig.item_2);
                break;
            case ItemType.ARMask:
                AddItemOptions(ItemConfig.item_3);
                break;
            case ItemType.ChangeFace:
                AddItemOptions(ItemConfig.item_4);
                break;
            case ItemType.ExpressionRecognition:
                AddItemOptions(ItemConfig.item_5);
                break;
            case ItemType.MusicFilter:
                AddItemOptions(ItemConfig.item_6);
                break;
            case ItemType.BackgroundSegmentation:
                AddItemOptions(ItemConfig.item_7);
                break;
            case ItemType.GestureRecognition:
                AddItemOptions(ItemConfig.item_8);
                break;
            case ItemType.MagicMirror:
                AddItemOptions(ItemConfig.item_9);
                break;
            case ItemType.PortraitLightEffect:
                AddItemOptions(ItemConfig.item_10);
                break;
            case ItemType.PortraitDrive:
                AddItemOptions(ItemConfig.item_11);
                break;
            default:
                break;
        }
        
        Item_Content.SetActive(true);
    }

    void AddItemOptions(Item[] items)
    {
        GameObject disableoption = Instantiate(Item_Disable);
        disableoption.transform.SetParent(ItemOptionContentTrans, false);
        disableoption.transform.localScale = Vector3.one;
        disableoption.transform.localPosition = Vector3.zero;
        disableoption.name = "Item_Disable";
        var disabletoggle = disableoption.GetComponent<Toggle>();
        disabletoggle.isOn = true;
        disabletoggle.group = Item_ToggleGroup;
        Item_ToggleGroup.RegisterToggle(disabletoggle);

        disabletoggle.onValueChanged.AddListener(delegate
        {
            if (disabletoggle.isOn)
            {
                if (musiccor != null)
                {
                    StopCoroutine(musiccor);
                    musiccor = null;
                    audios.Stop();
                }
                rtt.UnLoadItem((int)SlotForItems.Item);
            }
        });

        for(int i=0;i< items.Length;i++)
        {
            var itemobj = AddItemOption(items[i]);
            if(i==0)
            {
                itemobj.GetComponent<Toggle>().isOn = true;
                Item_ToggleGroup.NotifyToggleOn(itemobj.GetComponent<Toggle>());
            }
        }
    }

    GameObject AddItemOption(Item item)
    {
        GameObject option = Instantiate(Item_UIExample);
        option.transform.SetParent(ItemOptionContentTrans,false);
        option.transform.localScale = Vector3.one;
        option.transform.localPosition = Vector3.zero;
        option.name = item.name;
        option.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(item.type, item.iconid);
        var toggle = option.GetComponent<Toggle>();
        toggle.isOn = false;
        toggle.group = Item_ToggleGroup;
        Item_ToggleGroup.RegisterToggle(toggle);


        toggle.onValueChanged.AddListener(delegate
        {
            if(toggle.isOn)
            {
                if (musiccor != null)
                {
                    StopCoroutine(musiccor);
                    musiccor = null;
                    audios.Stop();
                }
                StartCoroutine(rtt.LoadItem(item,(int)SlotForItems.Item, new RenderToTexture.LoadItemCallback(OnItemLoaded)));
            }
        });
        return option;
    }

    IEnumerator PlayMusic(string name)
    {
        bool isMusicFilter = false;
        foreach(Item item in ItemConfig.item_6)
        {
            if (string.Equals(name, item.name))
            {
                isMusicFilter = true;
                break;
            }
        }
        if (isMusicFilter)
        {
            var audiodata = Resources.LoadAsync<AudioClip>("items/MusicFilter/douyin");
            yield return audiodata;
            audios.clip = audiodata.asset as AudioClip;
            audios.loop = true;
            audios.Play();
            while (true)
            {
                rtt.SetItemParamd(name, "music_time", audios.time * 1000);
                //Debug.Log(audios.time);
                yield return Util._endOfFrame;
            }
        }
        musiccor = null;
        audios.Stop();
    }

    void ClearItemsOptions()
    {
        foreach (Transform childTr in ItemOptionContentTrans)
        {
            var toggle = childTr.GetComponent<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            Item_ToggleGroup.UnregisterToggle(toggle);
            Destroy(childTr.gameObject);
        }
    }

    void OnItemLoaded(string name)
    {
        musiccor = StartCoroutine(PlayMusic(name));
    }

    void CloseItemUI()
    {
        Item_Content.SetActive(false);
    }
    #endregion

    #region MakeUp
    void OpenMakeUpUI(MakeupType type)
    {
        currentItemType = ItemType.MakeUp;
        currentMakeupType = type;
        MakeupOptionContentTrans.localPosition = Vector3.zero;
        ClearMakeUpOptions();
        CloseAllMakeUpContentUI();
        MakeupContentPanels[1].SetActive(true);
        switch (type)
        {
            case MakeupType.Lipstick:
                AddMakeUpOptions(ItemConfig.item_makeup_1, (int)SlotForItems.Lipstick);
                break;
            case MakeupType.Blusher:
                AddMakeUpOptions(ItemConfig.item_makeup_2, (int)SlotForItems.Blusher);
                break;
            case MakeupType.EyeBrow:
                AddMakeUpOptions(ItemConfig.item_makeup_3, (int)SlotForItems.EyeBrow);
                break;
            case MakeupType.EyeShadow:
                AddMakeUpOptions(ItemConfig.item_makeup_4, (int)SlotForItems.EyeShadow);
                break;
            case MakeupType.EyeLiner:
                AddMakeUpOptions(ItemConfig.item_makeup_5, (int)SlotForItems.EyeLiner);
                break;
            case MakeupType.EyeLash:
                AddMakeUpOptions(ItemConfig.item_makeup_6, (int)SlotForItems.EyeLash);
                break;
            case MakeupType.ContactLens:
                AddMakeUpOptions(ItemConfig.item_makeup_7, (int)SlotForItems.ContactLens);
                break;
            case MakeupType.None:
                for (int i = 0; i < MakeupSelecterOptions.Length; i++)
                {
                    MakeupSelecterOptions[i].GetComponent<Text>().color = normalColor;
                }
                break;
            default:
                break;
        }
        MakeupContent.SetActive(true);
        MakeupSelecter.SetActive(true);
    }

    void AddMakeUpOptions(Item[] items, int slotid)
    {
        for(int i=0;i< MakeupSelecterOptions.Length; i++)
        {
            if (i==(int)currentMakeupType)
                MakeupSelecterOptions[i].GetComponent<Text>().color = highlightColor;
            else
                MakeupSelecterOptions[i].GetComponent<Text>().color = normalColor;
        }

        Makeup_Disable.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
        {
            if (go != currentSelected)
            {
                currentSelected = go;
                UnSelectAllMakeUpOptions();
                go.transform.Find("Image").GetComponent<Image>().color = highlightColor;
                rtt.UnLoadItem(slotid);
                MakeupContentPanels[2].SetActive(false);
            }
        });

        Makeup_Slider.onValueChanged.AddListener(delegate
        {
            rtt.SetItemParamd(slotid, "makeup_intensity", Makeup_Slider.value);
        });
        if (rtt.GetItemNamebySlotID(slotid).Length > 0)
        {
            Makeup_Slider.value = (float)rtt.GetItemParamd(slotid, "makeup_intensity");
            MakeupContentPanels[2].SetActive(true);
        }
        else
            MakeupContentPanels[2].SetActive(false);

        string currentitemname = rtt.GetItemNamebySlotID(slotid);
        if(currentitemname.Length>0)
            Makeup_Disable.transform.Find("Image").GetComponent<Image>().color = normalColor;
        else
            Makeup_Disable.transform.Find("Image").GetComponent<Image>().color = highlightColor;
        for (int i = 0; i < items.Length; i++)
        {
            var go = AddMakeUpOption(items[i], slotid);
            if(string.Equals(currentitemname,items[i].name))
                go.transform.Find("Image_bg").gameObject.SetActive(true);
        }
        MakeupContentPanels[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 125.5f);
        MakeupContentPanels[0].SetActive(true);
    }

    GameObject AddMakeUpOption(Item item, int slotid)
    {
        GameObject option = Instantiate(Makeup_UIExample);
        option.transform.SetParent(MakeupOptionContentTrans, false);
        option.transform.localScale = Vector3.one;
        option.transform.localPosition = Vector3.zero;
        option.name = item.name;
        option.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(item.type, item.iconid);

        option.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
        {
            if (go != currentSelected)
            {
                currentSelected = go;
                UnSelectAllMakeUpOptions();
                go.transform.Find("Image_bg").gameObject.SetActive(true);
                StartCoroutine(rtt.LoadItem(item, slotid,new RenderToTexture.LoadItemCallback(OnMakeUpLoaded)));
            }
        });
        return option;
    }

    void OnMakeUpLoaded(string name)
    {
        Makeup_Slider.value = (float)rtt.GetItemParamd(name, "makeup_intensity");
        MakeupContentPanels[2].SetActive(true);
    }

    void UnSelectAllMakeUpOptions()
    {
        Makeup_Disable.transform.Find("Image").GetComponent<Image>().color = normalColor;
        foreach (Transform childTr in MakeupOptionContentTrans)
        {
            childTr.Find("Image_bg").gameObject.SetActive(false);
        }
    }

    void ClearMakeUpOptions()
    {
        foreach (Transform childTr in MakeupOptionContentTrans)
        {
            childTr.GetComponent<AddClickEvent>().RemoveAllListener();
            Destroy(childTr.gameObject);
        }
    }

    void CloseAllMakeUpContentUI()
    {
        MakeupContentPanels[0].SetActive(false);
        MakeupContentPanels[1].SetActive(false);
        MakeupContentPanels[2].SetActive(false);
        Makeup_Disable.GetComponent<AddClickEvent>().RemoveAllListener();
        MakeupContentPanels[2].GetComponentInChildren<Slider>().onValueChanged.RemoveAllListeners();
    }

    void CloseMakeUpUI()
    {
        MakeupSelecter.SetActive(false);
        MakeupContent.SetActive(false);
    }
    #endregion
    void OnApplicationQuit()
    {
        UnRegisterUIFunc();
    }
}
