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
    public Transform BeautyOptionContentTrans;

    public Button Btn_TakePic_mini_2;   //道具相关UI
    public GameObject Item_Content;
    public ToggleGroup Item_ToggleGroup;
    public Transform ItemOptionContentTrans;
    public GameObject Item_UIExample;
    public GameObject Item_Disable;

    private Coroutine musiccor = null;
    AudioSource audios;

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

    private Color highlightColor = new Color(0.337f, 0.792f, 0.957f, 1);
    private Color normalColor = Color.white;
    private Color normalColor_bg = new Color(0.659f, 0.659f, 0.659f, 1);
    private Color disableColor = new Color(0.41f, 0.41f, 0.41f, 1);


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
    }
    ItemType currentItemType = ItemType.None;
    Sprites uisprites;

    private static int[] permissions_code = {
            0x1,                    //美颜
            0x10,                    //Animoji
            0x2 | 0x4,              //道具贴纸
            0x20 | 0x40,            //AR面具
            0x80,                   //换脸
            0x800,                  //表情识别
            0x20000,                //音乐滤镜
            0x100,                  //背景分割
            0x200,                  //手势识别
            0x10000,                //哈哈镜
            0x4000,                 //人像光效
            0x8000                  //人像驱动
    };
    private static bool[] permissions;

    void Awake()
    {
        rtt = GetComponent<RenderToTexture>();
        audios = GetComponent<AudioSource>();
        uisprites = GetComponent<Sprites>();
        FaceunityWorker.instance.OnInitOK += InitApplication;
    }

    void Start()
    {
        Canvas_TopUI.SetActive(true);
        Canvas_FrontUI.SetActive(true);
        Canvas_BackUI.SetActive(true);
        CloseBeautySkinUI();
        CloseItemUI();
    }

    void InitApplication(object source, EventArgs e)
    {
        StartCoroutine(Authentication());
    }

    void Update()
    {
        //根据场景处理
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
            yield return rtt.LoadItem(ItemConfig.beautySkin[0]);
            BeautySkinItemName = ItemConfig.beautySkin[0].name;
            rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[0].paramword, BeautyConfig.beautySkin_1[0].defaultvalue);
            rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[1].paramword, BeautyConfig.beautySkin_1[1].defaultvalue);
            for (int i = 2; i < BeautyConfig.beautySkin_1.Length - 1; i++)
            {
                rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[i].paramword, BeautyConfig.beautySkin_1[i].defaultvalue);
            }

            rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_2[0].paramword, BeautyConfig.beautySkin_2[0].defaultvalue);
            for (int i = 1; i < BeautyConfig.beautySkin_2.Length - 1; i++)
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
            rtt.UnLoadItem(rtt.GetCurrentItemName());
            CloseBeautySkinUI();
            CloseItemUI();
        });
        Btn_Cancel.onClick.AddListener(OnCancelTakePicture);
        Btn_SavePic.onClick.AddListener(OnSavePicture);
        Btn_TakePic_mini_1.onClick.AddListener(TakePicture);
        Btn_TakePic_mini_2.onClick.AddListener(TakePicture);

        for(int i=0;i< ItemSelecters.Length;i++)
        {
            if (ItemSelecters[i].activeSelf)
                ItemSelecters[i].GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                {
                    int id = int.Parse(go.name);
                    if (id == 0)
                    {
                        OpenBeautySkinUI(BeautySkinType.None);
                    }
                    else
                    {
                        OpenItemsUI((ItemType)id);
                    }
                    Canvas_TopUI.SetActive(false);
                });
        }

        rtt.RawImg_BackGroud.GetComponent<AddClickEvent>().AddListener(delegate {
            if(currentItemType==ItemType.Beauty)
            {
                CloseAllBeautySkinContent();
                BeautySkinContentPanels[3].gameObject.SetActive(true);
            }
        });

        BeautySkinSelecterOptions[0].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautySkin); });
        BeautySkinSelecterOptions[1].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautyShape); });
        BeautySkinSelecterOptions[2].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautyFilter); });
        BeautySkinSelecterOptions[3].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.Filter); });


    }

    void UnRegisterUIFunc()
    {
        Btn_Switch.onClick.RemoveAllListeners();
        Btn_Back.onClick.RemoveAllListeners();
        Btn_Cancel.onClick.RemoveAllListeners();
        Btn_SavePic.onClick.RemoveAllListeners();
        Btn_TakePic_mini_1.onClick.RemoveAllListeners();
        Btn_TakePic_mini_2.onClick.RemoveAllListeners();
        for (int i = 0; i < ItemSelecters.Length; i++)
        {
            ItemSelecters[i].GetComponent<AddClickEvent>().RemoveAllListener();
        }

        rtt.RawImg_BackGroud.GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[0].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[1].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[2].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[3].GetComponent<AddClickEvent>().RemoveAllListener();
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
        
        StartCoroutine(rtt.LoadItem(ItemConfig.beautySkin[0]));
        CloseAllBeautySkinContent();
        GameObject panel = BeautySkinContentPanels[0];
        panel.GetComponent<ScrollRect>().content.localPosition = Vector3.zero;
        BeautySkinContentPanels[3].gameObject.SetActive(true);
        ClearBeautySkinOptions();

        if (type == BeautySkinType.BeautySkin)
        {
            BeautySkinSelecterOptions[0].GetComponent<Text>().color = highlightColor;
            
            AddBeautySkinOptions(0, BeautyConfig.beautySkin_1[0]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
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
                if (currentSelected != go)
                {
                    currentSelected = go;
                    UnSelectAllBeautySkinOptions();
                    go.transform.Find("Image_bg").gameObject.SetActive(true);
                    BeautySkinContentPanels[2].SetActive(false);
                    BeautySkinContentPanels[1].SetActive(false);
                }
                //else
                {
                    bi.ifenable = !bi.ifenable;
                    bi.currentvalue = bi.ifenable ? 1 : 0;
                    rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[0].paramword, bi.currentvalue);
                    SwitchBeautyOptionUIState(bi, go);
                }
            });
            
            GameObject bgo1 = AddBeautySkinOptions(1, BeautyConfig.beautySkin_1[1]);
            bgo1.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
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
                if (currentSelected != go)
                {
                    currentSelected = go;
                    UnSelectAllBeautySkinOptions();
                    go.transform.Find("Image_bg").gameObject.SetActive(true);
                    BeautySkinContentPanels[2].SetActive(false);
                    BeautySkinContentPanels[1].SetActive(false);
                }
                //else
                {
                    bi.ifenable = !bi.ifenable;
                    bi.currentvalue = bi.ifenable ? 1 : 0;
                    if (bi.ifenable)
                    {
                        go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_0);
                        go.GetComponentInChildren<Text>().text = "朦胧磨皮";
                    }
                    else
                    {
                        go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_1);
                        go.GetComponentInChildren<Text>().text = "清晰磨皮";
                    }
                    rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[1].paramword, bi.currentvalue);
                }
            });
            if (BeautyConfig.beautySkin_1[1].ifenable)
            {
                bgo1.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, BeautyConfig.beautySkin_1[1].iconid_0);
                bgo1.GetComponentInChildren<Text>().text = "朦胧磨皮";
            }
            else
            {
                bgo1.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, BeautyConfig.beautySkin_1[1].iconid_1);
                bgo1.GetComponentInChildren<Text>().text = "清晰磨皮";
            }
            bgo1.GetComponentInChildren<Text>().color = highlightColor;

            for (int i = 2; i < BeautyConfig.beautySkin_1.Length - 1; i++)
            {
                AddBeautySkinOptions(i, BeautyConfig.beautySkin_1[i]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
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
                     if (currentSelected != go)
                     {
                         currentSelected = go;
                         UnSelectAllBeautySkinOptions();
                         go.transform.Find("Image_bg").gameObject.SetActive(true);
                         BeautySkinContentPanels[2].SetActive(false);
                         BeautySkin_Slider.onValueChanged.RemoveAllListeners();
                         BeautySkin_Slider.minValue = 0;
                         BeautySkin_Slider.maxValue = bi.maxvalue;
                         BeautySkin_Slider.value = bi.currentvalue;
                         BeautySkin_Slider.onValueChanged.AddListener(delegate
                             {
                                 bi.currentvalue = BeautySkin_Slider.value;
                                 rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.currentvalue);
                             });
                         if (!bi.ifenable)
                         {
                             bi.ifenable = true;
                             rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.currentvalue);
                             SwitchBeautyOptionUIState(bi, go);
                         }
                         BeautySkinContentPanels[1].SetActive(true);
                     }
                     else
                     {
                         bi.ifenable = !bi.ifenable;
                         rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.ifenable ? bi.currentvalue : bi.disablevalue);
                         SwitchBeautyOptionUIState(bi, go);
                         BeautySkinContentPanels[1].SetActive(bi.ifenable);
                     }
                 });
            }

            AddBeautySkinOptions(BeautyConfig.beautySkin_1.Length - 1, BeautyConfig.beautySkin_1[BeautyConfig.beautySkin_1.Length - 1]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
            {
                foreach (var bgo in BeautyGOs)
                {
                    bgo.Key.currentvalue = bgo.Key.defaultvalue;
                    rtt.SetItemParamd(BeautySkinItemName, bgo.Key.paramword, bgo.Key.currentvalue);
                    bgo.Key.ifenable = bgo.Key.currentvalue == bgo.Key.disablevalue ? false : true;
                    SwitchBeautyOptionUIState(bgo.Key, bgo.Value);
                }
                currentSelected = null;
                UnSelectAllBeautySkinOptions();
                BeautySkinContentPanels[2].SetActive(false);
                BeautySkinContentPanels[1].SetActive(false);
            });

            panel.SetActive(true);
        }
        else if (type == BeautySkinType.BeautyShape)
        {
            BeautySkinSelecterOptions[1].GetComponent<Text>().color = highlightColor;
            
            AddBeautySkinOptions(0, BeautyConfig.beautySkin_2[0]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
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
                if (currentSelected != go)
                {
                    currentSelected = go;
                    UnSelectAllBeautySkinOptions();
                    go.transform.Find("Image_bg").gameObject.SetActive(true);
                    BeautySkinContentPanels[1].SetActive(false);
                    BeautySkinContentPanels[2].SetActive(true);
                }
            });
            for (int i = 1; i < BeautyConfig.beautySkin_2.Length - 1; i++)
            {
                AddBeautySkinOptions(i, BeautyConfig.beautySkin_2[i]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
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
                    if (currentSelected != go)
                    {
                        currentSelected = go;
                        UnSelectAllBeautySkinOptions();
                        go.transform.Find("Image_bg").gameObject.SetActive(true);
                        BeautySkinContentPanels[2].SetActive(false);
                        BeautySkin_Slider.onValueChanged.RemoveAllListeners();
                        BeautySkin_Slider.minValue = 0;
                        BeautySkin_Slider.maxValue = bi.maxvalue;
                        BeautySkin_Slider.value = bi.currentvalue;
                        BeautySkin_Slider.onValueChanged.AddListener(delegate
                        {
                            bi.currentvalue = BeautySkin_Slider.value;
                            rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.currentvalue);
                        });
                        if (!bi.ifenable)
                        {
                            bi.ifenable = true;
                            rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.currentvalue);
                            SwitchBeautyOptionUIState(bi, go);
                        }
                        BeautySkinContentPanels[1].SetActive(true);
                    }
                    else
                    {
                        bi.ifenable = !bi.ifenable;
                        rtt.SetItemParamd(BeautySkinItemName, bi.paramword, bi.ifenable ? bi.currentvalue : bi.disablevalue);
                        SwitchBeautyOptionUIState(bi, go);
                        BeautySkinContentPanels[1].SetActive(bi.ifenable);
                    }
                });
            }
            AddBeautySkinOptions(BeautyConfig.beautySkin_2.Length - 1, BeautyConfig.beautySkin_2[BeautyConfig.beautySkin_2.Length - 1]).GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
            {
                foreach (var bgo in BeautyGOs)
                {
                    if (bgo.Key.paramword != BeautyConfig.beautySkin_2[0].paramword)
                    {
                        bgo.Key.currentvalue = bgo.Key.defaultvalue;
                        rtt.SetItemParamd(BeautySkinItemName, bgo.Key.paramword, bgo.Key.currentvalue);
                        bgo.Key.ifenable = bgo.Key.currentvalue == bgo.Key.disablevalue ? false : true;
                        SwitchBeautyOptionUIState(bgo.Key, bgo.Value);
                    }
                }
                currentSelected = null;
                UnSelectAllBeautySkinOptions();
                BeautySkinContentPanels[2].SetActive(false);
                BeautySkinContentPanels[1].SetActive(false);
            });

            for (int i = 0; i < BeautySkin_FaceShape.Length; i++)
            {
                BeautySkin_FaceShape[i].GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                {
                    BeautyConfig.beautySkin_2[0].currentvalue = int.Parse(go.name);
                    BeautyConfig.beautySkin_2[0].ifenable= BeautyConfig.beautySkin_2[0].currentvalue == BeautyConfig.beautySkin_2[0].disablevalue ? false : true;
                    SwitchBeautyOptionUIState(BeautyConfig.beautySkin_2[0], BeautyGOs[BeautyConfig.beautySkin_2[0]]);
                    OpenBeautyShapeUI();
                });
            }
            if(BeautyConfig.beautySkin_2[0].currentvalue == -1)
                BeautyConfig.beautySkin_2[0].currentvalue =  4;
            OpenBeautyShapeUI();

            panel.SetActive(true);
            BeautySkinContentPanels[1].SetActive(false);
            BeautySkinContentPanels[2].SetActive(true);
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
        }
        else if (type == BeautySkinType.Filter)
        {
            BeautySkinSelecterOptions[3].GetComponent<Text>().color = highlightColor;
            string currentfiltername = rtt.GetItemParams(BeautySkinItemName, "filter_name");
            foreach (var bi in BeautyConfig.beautySkin_4)
            {
                GameObject go = AddBeautyFilterOptions(bi);
                var test = string.Compare(bi.paramword, currentfiltername, true);
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
        }

        BeautySkinContent.SetActive(true);
        BeautySkinSelecter.SetActive(true);
    }

    GameObject AddBeautySkinOptions(int name, Beauty beautyitem)
    {
        beautyitem.currentvalue = (float)rtt.GetItemParamd(BeautySkinItemName, beautyitem.paramword);
        beautyitem.ifenable = beautyitem.currentvalue == beautyitem.disablevalue ? false : true;
        GameObject option = Instantiate(BeautySkin_UIExample);
        option.transform.SetParent(BeautyOptionContentTrans, false);
        option.transform.localScale = Vector3.one;
        option.transform.localPosition = Vector3.zero;
        option.name = name.ToString();
        var txt = option.GetComponentInChildren<Text>();
        txt.text = beautyitem.name;

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
                BeautySkin_FaceShape[i].GetComponentInChildren<Image>().color = highlightColor;
            else
                BeautySkin_FaceShape[i].GetComponentInChildren<Image>().color = normalColor_bg;
        }

        int addnum = BeautyConfig.beautySkin_2[0].currentvalue == 4 ? BeautyConfig.beautySkin_2.Length - 1 : 3;
        for (int i = 1; i < BeautyConfig.beautySkin_2.Length - 1; i++)
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
                UnSelectAllBeautySkinOptions();
                go.transform.Find("Image_bg").gameObject.SetActive(true);
            }
            rtt.SetItemParams(BeautySkinItemName, "filter_name", beautyitem.paramword);
        });
        return option;
    }

    void SwitchBeautyOptionUIState(Beauty bi, GameObject go)
    {
        if (bi.ifenable)
        {
            go.GetComponentInChildren<Image>().sprite= uisprites.GetSprite(0, bi.iconid_0);
            go.GetComponentInChildren<Text>().color = highlightColor;
        }
        else
        {
            go.GetComponentInChildren<Image>().sprite = uisprites.GetSprite(0, bi.iconid_1);
            go.GetComponentInChildren<Text>().color = normalColor;
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
        foreach (Transform childTr in BeautyOptionContentTrans)
        {
            var bg = childTr.Find("Image_bg");
            if (bg)
                bg.gameObject.SetActive(false);
        }
    }

    void CloseAllBeautySkinContent()
    {
        Transform fct = BeautySkinContent.transform;
        for (int i = 0; i < fct.childCount; i++)
        {
            fct.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < BeautySkinSelecterOptions.Length; i++)
        {
            BeautySkinSelecterOptions[i].GetComponent<Text>().color = normalColor;
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
        Item_Content.GetComponent<ScrollRect>().content.localPosition = Vector3.zero;
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
                Debug.Log("卸载当前Item：" + rtt.GetCurrentItemName());
                rtt.UnLoadItem(rtt.GetCurrentItemName());
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
                StartCoroutine(rtt.LoadItem(item, new RenderToTexture.LoadItemCallback(OnItemLoaded)));
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

    void OnApplicationQuit()
    {
        UnRegisterUIFunc();
    }
}
