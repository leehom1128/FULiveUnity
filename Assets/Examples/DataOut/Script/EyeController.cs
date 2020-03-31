using UnityEngine;
using System.Collections;
using NatCamU.Core;

public class EyeController : MonoBehaviour {
    //这个组件用于眼球的跟踪，仅旋转，放在眼球的mesh上，SDK输出瞳孔坐标，转化成Quaternion应用在眼球上
    //！！！仅供参考，没有考虑效率问题和易用性问题！！！

    Quaternion m_rotation0; //眼球初始旋转
    Vector3 m_position0;    //眼球初始位置
    Renderer rend;  //眼球的Render

    public float[] last_pupil_pos = new float[2];   //上一帧瞳孔的坐标
    public int faceid=0;    //人脸ID

    //初始化时记录原始信息
    void Awake()
    {
        rend = GetComponent<Renderer>();
        m_rotation0 = transform.localRotation;
        m_position0 = transform.localPosition;
        last_pupil_pos[0] = 0.0f;
        last_pupil_pos[1] = 0.0f;
    }

    void Start () {
        //rend.enabled = false;
    }

    //每帧更新眼球transform
    void Update() {
        if(FaceunityWorker.instance==null||FaceunityWorker.instance.m_plugin_inited==false){return;}
        if (faceid >= FaceunityWorker.instance.m_need_update_facenum)
        {
            return;
        }
        if (FaceunityWorker.fu_IsTracking() > 0)    //仅在跟踪到人脸的情况下更新
        {
            //rend.enabled = true;
        }
        else
        {
            //rend.enabled = false;
            return;
        }

        float[] pupil_pos = FaceunityWorker.instance.m_pupil_pos[faceid].m_data;
        if(pupil_pos==null){return;}
        //Debug.Log(pupil_pos[0] + "," + pupil_pos[1]);
        transform.localRotation=m_rotation0;
        transform.localPosition=m_position0;

        //计算眼球旋转向量
        Vector3 rotate_from=new Vector3(0.0f,0.0f,-1.0f);
        Vector3 rotate_to = (new Vector3(-pupil_pos[0] * 0.8f, pupil_pos[1] * 0.4f, -1.0f));
        rotate_to.Normalize();
        Vector3 axis=Vector3.Cross(rotate_from,rotate_to);
        float angle = 0;

        bool ifMirrored = NatCam.Camera.Facing == Facing.Front;
#if (UNITY_IOS) && (!UNITY_EDITOR)
        ifMirrored=false;
#endif

        //根据镜像情况计算眼球旋转角度（弧度制）
        if (ifMirrored)
        {
            angle = Mathf.Atan2(axis.magnitude, Vector3.Dot(rotate_from, rotate_to)) / 3.1415926f * 180.0f;
        }
        else
        {
            angle = -Mathf.Atan2(axis.magnitude, Vector3.Dot(rotate_from, rotate_to)) / 3.1415926f * 180.0f;
        }


        //float angle=-Mathf.Atan2(axis.magnitude, Vector3.Dot(rotate_from, rotate_to))/3.1415926f*180.0f;
        //transform.rotation = Quaternion.Euler(pupil_pos[0]*100, -pupil_pos[1]*100, 0) * m_rotation0;
        //transform.RotateAround(rend.bounds.center, new Vector3(1,0,0), (pupil_pos[1] - last_pupil_pos[1]) * 40);
        //transform.RotateAround(rend.bounds.center, new Vector3(0,-1,0), (pupil_pos[0] - last_pupil_pos[0]) * 80);

        //根据眼球mesh的包围盒中心旋转眼球
        transform.RotateAround(rend.bounds.center,axis,angle);

        last_pupil_pos[0] = pupil_pos[0];
        last_pupil_pos[1] = pupil_pos[1];
        //transform.localRotation = Quaternion.Euler(-pupil_pos[0] * 100, -pupil_pos[1] * 100, -1);
    }
}
