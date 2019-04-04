using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using NatCamU.Core;
using System.Runtime.InteropServices;

public class UIManagerForDataOut : MonoBehaviour {

    RenderToModel rtm;

    public Button Btn_Switch;
    public Toggle enableTrack;
    public Toggle[] togglefaces;
    public GameObject Image_FaceDetect;

    public StdController[] stcs;

    void Awake()
    {
        rtm = GetComponent<RenderToModel>();
        FaceunityWorker.OnInitOK += InitApplication;
    }

    void Start () {
        foreach (StdController stc in stcs)
        {
            stc.gameObject.SetActive(false);
        }
        rtm.ifTrackPos = enableTrack.isOn;
    }

    void InitApplication(object source, EventArgs e)
    {
        NatCam.OnStart += OnStart;
        RegisterUIFunc();
        StartCoroutine(rtm.LoadItem(Util.GetStreamingAssetsPath() + "/faceunity/EnableTongueForUnity.bytes"));
    }

    public void OnStart()
    {
        SetHeadActiveByToggle();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (FaceunityWorker.fu_IsTracking() > 0)
            Image_FaceDetect.SetActive(false);
        else
            Image_FaceDetect.SetActive(true);
    }

    void RegisterUIFunc()
    {
        Btn_Switch.onClick.AddListener(delegate {
            rtm.SwitchCamera();
        });
        enableTrack.onValueChanged.AddListener(delegate
        {
            if (enableTrack.isOn)
            {
                rtm.ifTrackPos = true;
                FaceunityWorker.SetRunningMode(FaceunityWorker.FURuningMode.FU_Mode_RenderItems);
                StartCoroutine(rtm.LoadItem(Util.GetStreamingAssetsPath() + "/faceunity/EnableTongueForUnity.bytes"));
                StartCoroutine(rtm.delaySet());
            }
            else
            {
                rtm.ifTrackPos = false;
                FaceunityWorker.SetRunningMode(FaceunityWorker.FURuningMode.FU_Mode_TrackFace);
                FaceunityWorker.fu_SetTongueTracking(1);
                rtm.ReSetBackGroud();
            }
        });

        for (int i = 0; i < togglefaces.Length; i++)
        {
            int id = i;
            togglefaces[i].onValueChanged.AddListener(delegate {
                if (togglefaces[id].isOn)
                {
                    if (id < stcs.Length)
                    {
                        stcs[id].gameObject.SetActive(true);
                        stcs[id].ResetTransform();
                    }
                }
                else
                {
                    if (id < stcs.Length)
                    {
                        stcs[id].gameObject.SetActive(false);
                    }
                }
            });
        }

    }

    void UnRegisterUIFunc()
    {
        Btn_Switch.onClick.RemoveAllListeners();
        enableTrack.onValueChanged.RemoveAllListeners();
    }

    void SetHeadActiveByToggle()
    {
        Debug.Log("SetHeadActiveByToggle!!!!!!!!!!");
        for (int i = 0; i < togglefaces.Length; i++)
        {
            if (togglefaces[i].isOn)
            {
                if (i < stcs.Length)
                {
                    stcs[i].gameObject.SetActive(true);
                    stcs[i].ResetTransform();
                }
            }
            else
            {
                if (i < stcs.Length)
                {
                    stcs[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        UnRegisterUIFunc();
    }
}
