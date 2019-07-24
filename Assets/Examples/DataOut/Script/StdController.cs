using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using NatCamU.Core;

public class StdController : MonoBehaviour
{
    //这个组件用于人脸的跟踪，包括位移，旋转和表情系数（BlengShape），放在人脸的mesh上
    //！！！仅供参考，没有考虑效率问题和易用性问题！！！

    public RenderToModel rtm;

    Quaternion m_rotation0; //人脸初始旋转
    Vector3 m_position0;    //人脸初始位置

    /////////////////////////////////////
    //unity blendshape
    public SkinnedMeshRenderer[] skinnedMeshRenderers;  //人脸的Render，用来设置表情系数
    bool pauseUpdate = false;   //暂停更新

    public int faceid = 0;  //人脸ID

    //左右调换部分BlendShape数据,使其镜像
    private int[] mirrorBlendShape = new int[56] {1,0, 3,2, 5,4, 7,6, 9,8,
                                                   11,10, 13,12, 15,14, 16,
                                                   18,17, 19,
                                                   22,21,20,
                                                   24,23, 26,25, 28,27, 30,29, 32,31,
                                                   33,34,35,36,37,38,39,40,41,42,43, 45,44,
                                                   46,49,48,47,52,51,50,55,54,53,
                                                 };


    //初始化时记录原始信息
    void Awake()
    {
        m_rotation0 = transform.localRotation;
        m_position0 = transform.localPosition;
    }

    //相机切换完成回调
    void Start()
    {
        //skinnedMeshRenderer.enabled = false;
        rtm.onSwitchCamera += OnSwitchCamera;
    }

    //切换相机时暂停更新，防止乱跑
    void OnSwitchCamera(bool isSwitching)
    {
        pauseUpdate = isSwitching;
    }

    //每帧更新人脸信息
    void Update()
    {
        if (pauseUpdate)
            return;
        if (FaceunityWorker.instance == null || FaceunityWorker.instance.m_plugin_inited == false) { return; }
        if (faceid >= FaceunityWorker.instance.m_need_update_facenum)
        {
            return;
        }
        if (FaceunityWorker.fu_IsTracking() > 0)    //仅在跟踪到人脸的情况下更新
        {
            //skinnedMeshRenderer.enabled = true;
        }
        else
        {
            //skinnedMeshRenderer.enabled = false;
            return;
        }

        float[] R = FaceunityWorker.instance.m_rotation[faceid].m_data; //人脸旋转数据
        float[] RM = FaceunityWorker.instance.m_rotation_mode[faceid].m_data;   //人脸旋转模式，这个是为了在各个方向下跟踪人脸
        float[] P = FaceunityWorker.instance.m_translation[faceid].m_data;  //人脸位移数据

        bool ifMirrored = NatCam.Camera.Facing == Facing.Front; //是否镜像
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        ifMirrored = !ifMirrored;
#elif (UNITY_IOS) && (!UNITY_EDITOR)
        ifMirrored=false;
#endif
        if (ifMirrored)
        {
            float[] data = FaceunityWorker.instance.m_expression_with_tongue[faceid].m_data;
            for (int j = 0; j < skinnedMeshRenderers.Length; j++)
            {
                for (int i = 0; i < skinnedMeshRenderers[j].sharedMesh.blendShapeCount; i++)
                {
                    skinnedMeshRenderers[j].SetBlendShapeWeight(mirrorBlendShape[i], data[i] * 100);    //SDK输出表情系数数据为0~1，一般Unity的BlendShape系数为0~100，因此需要调整
                }
            }
            //本SDK跟踪人脸时，当人脸Z轴旋转角度超过90度时（Z轴即人脸前方），旋转基准会重置，因此需要使用人脸旋转模式来补偿这一重置，根据环境不同补偿方向也不同
            transform.localRotation = m_rotation0 * Quaternion.AngleAxis(getRotateEuler((int)RM[0], ifMirrored), Vector3.back) * new Quaternion(R[0], R[1], -R[2], -R[3]);
            if (rtm.ifTrackPos == true)
                transform.localPosition = getRotatePosition(getRotateEuler((int)RM[0], ifMirrored), -P[0], P[1], P[2]);//new Vector3(-P[0], P[1], P[2]);
            else
                transform.localPosition = m_position0;
        }
        else
        {
            float[] data = FaceunityWorker.instance.m_expression_with_tongue[faceid].m_data;
            for (int j = 0; j < skinnedMeshRenderers.Length; j++)
            {
                for (int i = 0; i < skinnedMeshRenderers[j].sharedMesh.blendShapeCount; i++)
                {
                    skinnedMeshRenderers[j].SetBlendShapeWeight(i, data[i] * 100);
                }
            }
            transform.localRotation = m_rotation0 * Quaternion.AngleAxis(getRotateEuler((int)RM[0], ifMirrored), Vector3.back) * new Quaternion(R[0], -R[1], R[2], -R[3]);
            if (rtm.ifTrackPos == true)
                transform.localPosition = getRotatePosition(getRotateEuler((int)RM[0], ifMirrored), P[0], P[1], P[2]);//new Vector3(P[0], P[1], P[2]);
            else
                transform.localPosition = m_position0;
        }
        //Debug.Log("STDUpdate:localRotation="+ transform.localEulerAngles.x+","+ transform.localEulerAngles.y + "," + transform.localEulerAngles.z);
    }

    //这个为经验数据，具体情况请自行测试
    Vector3 getRotatePosition(int euler, float x, float y, float z)
    {
        switch (euler)
        {
            case -90: return new Vector3(y, -x, z);
            case 0: return new Vector3(x, y, z);
            case 90: return new Vector3(-y, x, z);
            case 180: return new Vector3(-x, -y, z);
            default: return new Vector3(x, y, z);
        }
    }

    //这个为经验数据，具体情况请自行测试
    int getRotateEuler(int mode, bool mirrored)
    {
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        if (mirrored)
            if (Util.isNexus5X())
            {
                switch (mode)
                {

                    case 0: return -90;
                    case 1: return 0;
                    case 2: return 90;
                    case 3: return 180;
                    default: return 0;
                }
            }
            else
            {
                switch (mode)
                {
                    case 0: return -90;
                    case 3: return 0;
                    case 2: return 90;
                    case 1: return 180;
                    default: return 0;
                }
            }
        else
        {
            if (Util.isNexus6())
            {
                switch (mode)
                {

                    case 2: return -90;
                    case 3: return 0;
                    case 0: return 90;
                    case 1: return 180;
                    default: return 0;
                }
            }
            else
            {
                switch (mode)
                {
                    case 0: return -90;
                    case 1: return 0;
                    case 2: return 90;
                    case 3: return 180;
                    default: return 0;
                }
            }
        }
#elif (UNITY_IOS) && (!UNITY_EDITOR)
        switch (mode)
        {
            case 1: return -90;
            case 2: return 0;
            case 3: return 90;
            case 0: return 180;
            default: return 0;
        }
#else
        switch (mode)
        {
            case 3: return -90;
            case 2: return 0;
            case 1: return 90;
            case 0: return 180;
            default: return 0;
        }
#endif
    }

    //重置人脸的位置旋转
    public void ResetTransform()
    {
        transform.localPosition = m_position0;
        transform.localRotation = m_rotation0;
        Debug.Log("ResetTransform:localRotation=" + transform.localEulerAngles.x + "," + transform.localEulerAngles.y + "," + transform.localEulerAngles.z);
    }
}
