using UnityEngine;
using System.Collections;
using NatCamU.Core;

public class EyeController : MonoBehaviour {
    //这个组件用于眼球的跟踪，仅旋转，放在眼球的mesh上，SDK输出瞳孔坐标，转化成Quaternion应用在眼球上
    //！！！仅供参考，没有考虑效率问题和易用性问题！！！

    Quaternion m_rotation0; //眼球初始旋转
    Vector3 m_position0;    //眼球初始位置
    Renderer rend;  //眼球的Render

    public int faceid=0;    //人脸ID

    //初始化时记录原始信息
    void Awake()
    {
        rend = GetComponent<Renderer>();
        m_rotation0 = transform.localRotation;
        m_position0 = transform.localPosition;
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
        if (FaceunityWorker.instance.m_need_update_facenum > 0)    //仅在跟踪到人脸的情况下更新
        {
            //rend.enabled = true;
        }
        else
        {
            //rend.enabled = false;
            return;
        }
        
        transform.localRotation=m_rotation0;
        transform.localPosition=m_position0;

        bool ifMirrored = NatCam.Camera.Facing == Facing.Front;
#if (UNITY_IOS) && (!UNITY_EDITOR)
        ifMirrored=false;
#endif

        var m_eye_rotation = FaceunityWorker.instance.m_eye_rotation[faceid].m_data;
        var rotd = new Quaternion(m_eye_rotation[0], (ifMirrored ? 1.0f : -1.0f) * m_eye_rotation[1], m_eye_rotation[2], m_eye_rotation[3]);

        RotateAround(transform, rend.bounds.center, rotd);
    }

    private void RotateAround(Transform t, Vector3 center, Quaternion rot)
    {
        Vector3 pos = t.position;
        Vector3 dir = pos - center; // find current direction relative to center
        dir = rot * dir; // rotate the direction
        t.position = center + dir; // define new position
                                   // rotate object to keep looking at the center:
        Quaternion myRot = t.rotation;
        t.rotation *= Quaternion.Inverse(myRot) * rot * myRot;
    }
}
