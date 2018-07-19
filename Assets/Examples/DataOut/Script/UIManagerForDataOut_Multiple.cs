using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIManagerForDataOut_Multiple : MonoBehaviour {

    RenderToModel rtm;

    public Button Btn_Switch;
    public GameObject Image_FaceDetect;

    public StdController[] stcs;    //如果有更多模型，可以提高MAXFACE值

    private bool[] marks;
    private string text;

    void Awake()
    {
        rtm = GetComponent<RenderToModel>();
        
    }

    void Start()
    {
        marks = new bool[stcs.Length];
        foreach (StdController stc in stcs)
        {
            stc.gameObject.SetActive(false);
        }
        rtm.ifTrackPos = true;
        FaceunityWorker.instance.OnInitOK += InitApplication;
    }

    void InitApplication(object source, EventArgs e)
    {
        RegisterUIFunc();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (FaceunityWorker.fu_IsTracking() > 0)
            Image_FaceDetect.SetActive(false);
        else
            Image_FaceDetect.SetActive(true);

        for(int i=0;i< stcs.Length;i++)
        {
            marks[i] = false;
        }
        string tmps = "trueid:";
        for (int i = 0; i < FaceunityWorker.instance.m_need_blendshape_update; i++)
        {
            int trueid = (int)Mathf.Log(FaceunityWorker.fu_GetFaceIdentifier(i),2);
            tmps += trueid + ",";
            if (trueid < stcs.Length && trueid >= 0)
            {
                stcs[trueid].faceid = i;
                stcs[trueid].gameObject.SetActive(true);
                marks[trueid] = true;
            }
        }
        for (int i = 0; i < marks.Length; i++)
        {
            if(marks[i]==false)
                stcs[i].gameObject.SetActive(false);
        }

        text = "faceNum=" + FaceunityWorker.instance.m_need_blendshape_update+"\n"+ tmps;
    }

    void RegisterUIFunc()
    {
        Btn_Switch.onClick.AddListener(delegate {
            rtm.SwitchCamera();
        });
    }

    void UnRegisterUIFunc()
    {
        Btn_Switch.onClick.RemoveAllListeners();
    }

    void OnApplicationQuit()
    {
        UnRegisterUIFunc();
    }

    void OnGUI()
    {
        if (text != null)
        {
            int w = Screen.width, h = Screen.height;
            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, h * 2 / 100, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);
            GUI.Label(rect, text, style);
        }
    }
}
