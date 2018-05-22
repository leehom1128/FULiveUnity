using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using NatCamU.Core;

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
        FaceunityWorker.instance.OnInitOK += InitApplication;
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
            SetHeadActiveByToggle();
            rtm.SwitchCamera();
        });
        enableTrack.onValueChanged.AddListener(delegate
        {
            if (enableTrack.isOn)
            {
                rtm.ifTrackPos = true;
                FaceunityWorker.SetRunningMode(FaceunityWorker.FURuningMode.FU_Mode_RenderItems);
                StartCoroutine(rtm.delaySet());
            }
            else
            {
                rtm.ifTrackPos = false;
                FaceunityWorker.SetRunningMode(FaceunityWorker.FURuningMode.FU_Mode_TrackFace);
                rtm.ReSetBackGroud();
            }
        });

        for (int i = 0; i < togglefaces.Length; i++)
        {
            togglefaces[i].onValueChanged.AddListener(delegate { SetHeadActiveByToggle(); });
        }

    }

    void UnRegisterUIFunc()
    {
        Btn_Switch.onClick.RemoveAllListeners();
        enableTrack.onValueChanged.RemoveAllListeners();
    }

    void SetHeadActiveByToggle()
    {
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
