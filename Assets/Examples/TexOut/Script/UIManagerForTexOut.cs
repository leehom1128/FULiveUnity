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

    public GameObject Canvas_FrontUI; //前景UI
    public Camera Camera_FrontUI;
    public Button Btn_Switch;
    public Button Btn_TakePic;
    public Button Btn_BeautySkin;
    public Button Btn_Item;

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
    public GameObject ItemSelecter;
    public GameObject[] ItemContentOptions;
    public GameObject Item_Unload;
    public GameObject Item_Content;
    public GameObject Item_UIExample;
    private Transform ItemOptionContentTrans;

    private Coroutine musicfiltercor = null;

    Dictionary<Beauty, GameObject> BeautyGOs = new Dictionary<Beauty, GameObject>();
    GameObject currentSelected;
    string BeautySkinItemName;

    AudioSource audios;

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
        None = 0,
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
    private static bool[] permissions = new bool[12];

    void Awake()
    {
        rtt = GetComponent<RenderToTexture>();
        audios = GetComponent<AudioSource>();
        FaceunityWorker.instance.OnInitOK += InitApplication;
    }

    void Start()
    {
        SwitchMainBtns(true);
        CloseBeautySkinUI();
        CloseItemUI();
    }

    void InitApplication(object source, EventArgs e)
    {
        RegisterUIFunc();
        StartCoroutine(Authentication());
    }

    void Update()
    {
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
        bool enableitem = false;
        Debug.Log("fu_GetModuleCode:" + code);
        for (int i = 0; i < permissions_code.Length; i++)
        {
            if ((code & permissions_code[i]) == permissions_code[i])
            {
                permissions[i] = true;
                if (i == (int)ItemType.None)
                {
                    yield return rtt.LoadItem(ItemConfig.beautySkin[0],
                       new RenderToTexture.LoadItemCallback(delegate (string name)
                       {
                           foreach (Beauty be in BeautyConfig.beautySkin_1)
                           {
                               if (!string.Equals(be.paramword, "RESET"))
                                   rtt.SetItemParamd(name, be.paramword, be.defaultvalue);
                           }
                           foreach (Beauty be in BeautyConfig.beautySkin_2)
                           {
                               if (!string.Equals(be.paramword, "RESET"))
                                   rtt.SetItemParamd(name, be.paramword, be.defaultvalue);
                           }
                       }));

                    Btn_BeautySkin.interactable = true;
                    Btn_BeautySkin.GetComponent<Image>().color = normalColor;
                }
                else
                {
                    SetItemTextEnable(i, true);
                    enableitem = true;
                }
            }
            else
            {
                permissions[i] = false;
                Debug.Log("权限未获取:" + permissions_code[i]);
                if (i== (int)ItemType.None)
                {
                    Btn_BeautySkin.interactable = false;
                    Btn_BeautySkin.GetComponent<Image>().color = disableColor;
                }
                else
                {
                    SetItemTextEnable(i, false);
                }
            }
        }
        if(enableitem)
        {
            Btn_Item.interactable = true;
            Btn_Item.GetComponent<Image>().color = normalColor;
        }
        else
        {
            Btn_Item.interactable = false;
            Btn_Item.GetComponent<Image>().color = disableColor;
        }
    }


    void RegisterUIFunc()
    {
        Btn_Switch.onClick.AddListener(delegate { rtt.SwitchCamera(); });
        Btn_Cancel.onClick.AddListener(OnCancelTakePicture);
        Btn_TakePic.onClick.AddListener(TakePicture);
        Btn_SavePic.onClick.AddListener(OnSavePicture);

        Btn_TakePic_mini_1.onClick.AddListener(TakePicture);
        Btn_TakePic_mini_2.onClick.AddListener(TakePicture);

        Btn_BeautySkin.onClick.AddListener(delegate
        {
            SwitchMainBtns(false);
            CloseItemUI();
            if (currentBeautySkinType == BeautySkinType.None)
                OpenBeautySkinUI(BeautySkinType.BeautySkin);
            else
                OpenBeautySkinUI(currentBeautySkinType);
        });
        Btn_Item.onClick.AddListener(delegate
        {
            SwitchMainBtns(false);
            CloseBeautySkinUI();
            if (currentItemType == ItemType.None)
            {
                bool haspermission = false;
                for (int i=1;i<permissions.Length;i++)
                    if(permissions[i])
                    {
                        OpenItemsUI((ItemType)i);
                        haspermission = true;
                        break;
                    }
                if(haspermission==false)
                    OpenItemsUI(currentItemType);
            }
            else
                OpenItemsUI(currentItemType);
        });

        rtt.RawImg_BackGroud.GetComponent<AddClickEvent>().AddListener(delegate { CloseBeautySkinUI(); CloseItemUI(); SwitchMainBtns(true); });

        BeautySkinSelecterOptions[0].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautySkin); });
        BeautySkinSelecterOptions[1].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautyShape); });
        BeautySkinSelecterOptions[2].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.BeautyFilter); });
        BeautySkinSelecterOptions[3].GetComponent<AddClickEvent>().AddListener(delegate { OpenBeautySkinUI(BeautySkinType.Filter); });


        for (int i = 0; i < ItemContentOptions.Length; i++)
        {
            ItemContentOptions[i].transform.GetComponent<AddClickEvent>().AddListener(delegate (GameObject go)
                                                    {
                                                        LoadItemsOption((ItemType)int.Parse(go.name));
                                                    });
        }

        Item_Unload.GetComponent<AddClickEvent>().AddListener(delegate
        {
            if (string.Equals(rtt.GetCurrentItemName(), ItemConfig.beautySkin[0].name))
                return;
            Debug.Log("卸载当前Item：" + rtt.GetCurrentItemName());
            rtt.UnLoadItem(rtt.GetCurrentItemName());
            UnSelectAllItemOptions();
        });
    }

    void UnRegisterUIFunc()
    {
        Btn_Switch.onClick.RemoveAllListeners();
        Btn_Cancel.onClick.RemoveAllListeners();
        Btn_TakePic.onClick.RemoveAllListeners();
        Btn_SavePic.onClick.RemoveAllListeners();
        Btn_TakePic_mini_1.onClick.RemoveAllListeners();
        Btn_TakePic_mini_2.onClick.RemoveAllListeners();
        Btn_BeautySkin.onClick.RemoveAllListeners();
        Btn_Item.onClick.RemoveAllListeners();

        rtt.RawImg_BackGroud.GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[0].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[1].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[2].GetComponent<AddClickEvent>().RemoveAllListener();
        BeautySkinSelecterOptions[3].GetComponent<AddClickEvent>().RemoveAllListener();

        for (int i = 0; i < ItemContentOptions.Length; i++)
        {
            ItemContentOptions[i].transform.GetComponent<AddClickEvent>().RemoveAllListener();
        }

        Item_Unload.GetComponent<AddClickEvent>().RemoveAllListener();
    }

    void SwitchMainBtns(bool ifenable)
    {
        Btn_TakePic.gameObject.SetActive(ifenable);
        Btn_BeautySkin.gameObject.SetActive(ifenable);
        Btn_Item.gameObject.SetActive(ifenable);
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
        Canvas_BackUI.SetActive(true);
    }

    void OnCancelTakePicture()
    {
        Canvas_BackUI.SetActive(true);
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

        BeautySkinItemName = ItemConfig.beautySkin[0].name;
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
                    GameObject _srgot;
                    if (bi.ifenable)
                    {
                        _srgot = Resources.Load<GameObject>(bi.iconname_0);
                        go.GetComponentInChildren<Text>().text = "朦胧磨皮";
                    }
                    else
                    {
                        _srgot = Resources.Load<GameObject>(bi.iconname_1);
                        go.GetComponentInChildren<Text>().text = "清晰磨皮";
                    }
                    go.GetComponentInChildren<Image>().sprite = _srgot ? _srgot.GetComponent<SpriteRenderer>().sprite : null;
                    rtt.SetItemParamd(BeautySkinItemName, BeautyConfig.beautySkin_1[1].paramword, bi.currentvalue);
                }
            });
            GameObject _srgo;
            if (BeautyConfig.beautySkin_1[1].ifenable)
            {
                _srgo = Resources.Load<GameObject>(BeautyConfig.beautySkin_1[1].iconname_0);
                bgo1.GetComponentInChildren<Text>().text = "朦胧磨皮";
            }
            else
            {
                _srgo = Resources.Load<GameObject>(BeautyConfig.beautySkin_1[1].iconname_1);
                bgo1.GetComponentInChildren<Text>().text = "清晰磨皮";
            }
            bgo1.GetComponentInChildren<Text>().color = highlightColor;
            bgo1.GetComponentInChildren<Image>().sprite = _srgo ? _srgo.GetComponent<SpriteRenderer>().sprite : null;

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
        var srgo = Resources.Load<GameObject>(beautyitem.iconname_0);
        option.GetComponentInChildren<Image>().sprite = srgo ? srgo.GetComponent<SpriteRenderer>().sprite : null;

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
        GameObject _srgo;
        if (bi.ifenable)
        {
            _srgo = Resources.Load<GameObject>(bi.iconname_0);
            go.GetComponentInChildren<Text>().color = highlightColor;
        }
        else
        {
            _srgo = Resources.Load<GameObject>(bi.iconname_1);
            go.GetComponentInChildren<Text>().color = normalColor;
        }
        go.GetComponentInChildren<Image>().sprite = _srgo ? _srgo.GetComponent<SpriteRenderer>().sprite : null;
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
        LoadItemsOption(it);
        ItemSelecter.SetActive(true);
        Item_Content.SetActive(true);
    }

    void LoadItemsOption(ItemType it)
    {
        currentItemType = it;
        Item_Content.GetComponent<ScrollRect>().content.localPosition = Vector3.zero;
        if(ItemOptionContentTrans==null)
            ItemOptionContentTrans = Item_Content.transform.Find("Viewport/Content");
        ClearItemsOptions();
        SetItemTextHighlight((int)it);
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
            case ItemType.None:
                break;
            default:
                break;

        }
    }

    void AddItemOptions(Item[] items)
    {
        foreach (Item item in items)
        {
            AddItemOption(item);
        }
        float uiwidth = rtt.RawImg_BackGroud.canvas.GetComponent<RectTransform>().sizeDelta.x;
        var lg = ItemOptionContentTrans.GetComponent<GridLayoutGroup>();
        var CellSize = lg.cellSize;
        var Spacing = lg.spacing;
        int column = Mathf.CeilToInt((uiwidth-CellSize.x)/(Spacing.x+CellSize.x));
        int row = Mathf.CeilToInt((float)items.Length / column);
        ItemOptionContentTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(0, row * CellSize.y + lg.padding.top);
        lg.padding.left = (int)(uiwidth - column * CellSize.x- (column-1) * Spacing.x) /2;
    }

    GameObject AddItemOption(Item item)
    {
        GameObject option = Instantiate(Item_UIExample);
        option.transform.SetParent(ItemOptionContentTrans,false);
        option.transform.localScale = Vector3.one;
        option.transform.localPosition = Vector3.zero;
        option.name = item.name;
        //option.GetComponentInChildren<Text>().text = item.name; //仅测试用
        var srgo = Resources.Load<GameObject>(item.iconname);
        option.GetComponentInChildren<Image>().sprite = srgo ? srgo.GetComponent<SpriteRenderer>().sprite : null;
        
        if(string.Equals(rtt.GetCurrentItemName(), item.name))
            option.transform.Find("Image_bg").gameObject.SetActive(true);
        else
            option.transform.Find("Image_bg").gameObject.SetActive(false);

        option.GetComponent<AddClickEvent>().AddListener(delegate
        {
            if (musicfiltercor != null)
            {
                StopCoroutine(musicfiltercor);
                musicfiltercor = null;
                audios.Stop();
            }
            StartCoroutine(rtt.LoadItem(item, new RenderToTexture.LoadItemCallback(SwitchItemOptionUIState)));
        });
        return option;
    }

    IEnumerator RunMusicFilter(string name)
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
        musicfiltercor = null;
        audios.Stop();
    }

    void ClearItemsOptions()
    {
        foreach (Transform childTr in ItemOptionContentTrans)
        {
            childTr.GetComponent<AddClickEvent>().RemoveAllListener();
            Destroy(childTr.gameObject);
        }
    }

    void UnSelectAllItemOptions()
    {
        foreach (Transform childTr in ItemOptionContentTrans)
        {
            var bg = childTr.Find("Image_bg");
            if (bg)
                bg.gameObject.SetActive(false);
        }
        if (musicfiltercor != null)
        {
            StopCoroutine(musicfiltercor);
            musicfiltercor = null;
            audios.Stop();
        }
    }

    void SwitchItemOptionUIState(string name)
    {
        foreach (Transform childTr in ItemOptionContentTrans)
        {
            var bg = childTr.Find("Image_bg");
            if (bg)
            {
                if (string.Equals(childTr.gameObject.name, name))
                    bg.gameObject.SetActive(true);
                else
                    bg.gameObject.SetActive(false);
            }
        }
        musicfiltercor = StartCoroutine(RunMusicFilter(name));
    }

    void SetItemTextHighlight(int num)
    {
        for (int i = 0; i<ItemContentOptions.Length;i++)
        {
            if (num-1 == i)
                ItemContentOptions[i].GetComponent<Text>().color = highlightColor;
            else
                ItemContentOptions[i].GetComponent<Text>().color = permissions[i+1]?normalColor : disableColor;
        }
    }

    void SetItemTextEnable(int num,bool enable)
    {
        if (ItemContentOptions[num - 1] != null)
        {
            ItemContentOptions[num - 1].GetComponent<Text>().raycastTarget = enable ? true:false;
            ItemContentOptions[num - 1].GetComponent<Text>().color = enable ? normalColor : disableColor;
        }
    }

    void CloseItemUI()
    {
        ItemSelecter.SetActive(false);
        Item_Content.SetActive(false);
    }
    #endregion

    void OnApplicationQuit()
    {
        UnRegisterUIFunc();
    }
}
