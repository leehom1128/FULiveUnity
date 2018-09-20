using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using NatCamU.Core;

public class StdController : MonoBehaviour
{
    public RenderToModel rtm;
    Quaternion m_rotation0;
    Vector3 m_position0;

    /////////////////////////////////////
    //unity blendshape
    int blendShapeCount;
    SkinnedMeshRenderer skinnedMeshRenderer;
    bool pauseUpdate = false;

    public int faceid = 0;

    //左右调换部分BlendShape数据,使其镜像
    private int[] mirrorBlendShape = new int[46] {1,0, 3,2, 5,4, 7,6, 9,8,
                                                   11,10, 13,12, 15,14, 16,
                                                   18,17, 19,
                                                   22,21,20,
                                                   24,23, 26,25, 28,27, 30,29, 32,31,
                                                   33,34,35,36,37,38,39,40,41,42,43, 45,44,
                                                 };


    void Awake()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        m_rotation0 = transform.localRotation;
        m_position0 = transform.localPosition;
    }

    void Start()
    {
        //skinnedMeshRenderer.enabled = false;
        rtm.onSwitchCamera += OnSwitchCamera;
    }

    void OnSwitchCamera(bool isSwitching)
    {
        pauseUpdate = isSwitching;
    }

    void Update()
    {
        if (pauseUpdate)
            return;
        if (FaceunityWorker.instance == null || FaceunityWorker.instance.m_plugin_inited == false) { return; }
        if (faceid >= FaceunityWorker.instance.m_need_blendshape_update)
        {
            return;
        }
        if (FaceunityWorker.fu_IsTracking() > 0)
        {
            //skinnedMeshRenderer.enabled = true;
        }
        else
        {
            //skinnedMeshRenderer.enabled = false;
            return;
        }

        float[] R = FaceunityWorker.instance.m_rotation[faceid].m_data;
        float[] RM = FaceunityWorker.instance.m_rotation_mode[faceid].m_data;
        float[] P = FaceunityWorker.instance.m_translation[faceid].m_data;

        bool ifMirrored = NatCam.Camera.Facing == Facing.Front;
#if (UNITY_ANDROID) && (!UNITY_EDITOR)
        ifMirrored = !ifMirrored;
#elif (UNITY_IOS) && (!UNITY_EDITOR)
        ifMirrored=false;
#endif
        if (ifMirrored)
        {
            if (blendShapeCount > 1)
            {
                float[] data = FaceunityWorker.instance.m_expression[faceid].m_data;
                for (int i = 0; i < blendShapeCount; i++)
                {
                    skinnedMeshRenderer.SetBlendShapeWeight(mirrorBlendShape[i], data[i] * 100);
                }
            }
            transform.localRotation = m_rotation0 * Quaternion.AngleAxis(getRotateEuler((int)RM[0], ifMirrored), Vector3.back) * new Quaternion(R[0], R[1], -R[2], -R[3]);
            if (rtm.ifTrackPos == true)
                transform.localPosition = getRotatePosition(getRotateEuler((int)RM[0], ifMirrored), -P[0], P[1], P[2]);//new Vector3(-P[0], P[1], P[2]);
            else
                transform.localPosition = m_position0;
        }
        else
        {
            if (blendShapeCount > 1)
            {
                float[] data = FaceunityWorker.instance.m_expression[faceid].m_data;
                for (int i = 0; i < blendShapeCount; i++)
                {
                    skinnedMeshRenderer.SetBlendShapeWeight(i, data[i] * 100);
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

    public void ResetTransform()
    {
        transform.localPosition = m_position0;
        transform.localRotation = m_rotation0;
        Debug.Log("ResetTransform:localRotation=" + transform.localEulerAngles.x + "," + transform.localEulerAngles.y + "," + transform.localEulerAngles.z);
    }
}
