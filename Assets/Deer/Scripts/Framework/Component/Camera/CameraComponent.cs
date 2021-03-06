// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2021-08-28 13-57-10  
//修改作者 : 王闻浩(小黑) 
//修改时间 : 2022-03-18 19-39-10  
//版 本 : 0.1 
// ===============================================
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityGameFramework.Runtime;

public enum CameraType 
{
    FollowCamera = 0,
    RotateCamera = 1,
}

public class CameraModel 
{
    public CinemachineVirtualCamera CinemachineVirtual;
    public CameraType CameraType = CameraType.FollowCamera;
}
[DisallowMultipleComponent]
[AddComponentMenu("Deer/Camera")]
public class CameraComponent : GameFrameworkComponent
{
    /// <summary>
    /// 主相机
    /// </summary>
    private Camera m_MainCamera;
    public Camera MainCamera 
    {
        get { return m_MainCamera; }
        set { m_MainCamera = value; }
    }
    /// <summary>
    /// 第三人称跟随相机
    /// </summary>
    [SerializeField]
    private CinemachineVirtualCamera m_FollowLockCamera;
    /// <summary>
    /// 第三人称自由跟随相机
    /// </summary>
    [SerializeField]
    private CinemachineFreeLook m_FollowFreeCamera;
    [SerializeField]
    private CinemachineStateDrivenCamera m_FollowStateDrivenCamera;

    #region 小地图
    /// <summary>
    /// 小地图跟随相机
    /// </summary>
    [SerializeField]
    private Camera m_FollowMiniMapCamera;
    private Transform m_MiniMapFollowTarget;
    private Vector3 m_OffsetPosition;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        m_MainCamera = transform.Find("MainCamera").GetComponent<Camera>();
        m_FollowLockCamera = transform.Find("FollowLockViewVirtual").GetComponent<CinemachineVirtualCamera>();
        m_FollowFreeCamera = transform.Find("FollowFreeViewVirtual").GetComponent<CinemachineFreeLook>();
        m_FollowStateDrivenCamera = transform.Find("CMStateDrivenCamera").GetComponent<CinemachineStateDrivenCamera>();
        m_FollowMiniMapCamera = transform.Find("MiniMapCamera").GetComponent<Camera>();

        CinemachineCore.GetInputAxis = GetAxisCustom;
    }

    void Update()
    {
        if (m_MiniMapFollowTarget!= null)
        {
            m_FollowMiniMapCamera.transform.position = m_OffsetPosition + m_MiniMapFollowTarget.transform.position;
        }
    }

    public void OpenCameraType()
    {
        //m_FollowCamera.
    }
    public void LookAtTarget(Transform transform) 
    {
        m_FollowLockCamera.LookAt = transform;  
    }

    public void FollowTarget(Transform transform)
    {
        m_FollowLockCamera.Follow = transform;
    }

    public void FollowTarget(Transform transform, Vector3 position)
    {
        m_FollowStateDrivenCamera.Follow = transform;
        m_FollowLockCamera.transform.localPosition = position;
    }

    public void FollowTarget(Transform transform,Vector3 position, Quaternion quaternion)
    {
        m_FollowLockCamera.Follow = transform;
        m_FollowLockCamera.transform.localPosition = position;
        m_FollowLockCamera.transform.localRotation = quaternion;
    }
    
    public void FollowAndLockViewTarget(Transform followTrans,Transform lookAtTrans)
    {
        //m_FollowLockCamera.Follow = followTrans;
        //m_FollowLockCamera.LookAt = lookAtTrans;
        //m_FollowLockCamera.gameObject.SetActive(true);
        FollowAndFreeViewTarget(followTrans, lookAtTrans);
    }
    
    public void FollowAndFreeViewTarget(Transform followTrans,Transform lookAtTrans)
    {
        m_FollowFreeCamera.Follow = followTrans;
        m_FollowFreeCamera.LookAt = lookAtTrans;        
        m_FollowFreeCamera.gameObject.SetActive(true);
    }
    
    public void CameraActive(bool isActive)
    {
        m_MainCamera.gameObject.SetActive(isActive);   
    }

    public float GetAxisCustom(string axisName)
    {
        if (axisName == "Mouse X")
        {
            if (Input.GetMouseButton(0))
            {
                return Input.GetAxis("Mouse X");
            }
            else
            {
                return 0;
            }
        }
        else if (axisName == "Mouse Y")
        {
            if (Input.GetMouseButton(0))
            {
                return Input.GetAxis("Mouse Y");
            }
            else
            {
                return 0;
            }
        }
        return 0;
    }


    #region 小地图相机管理
    /// <summary>
    /// 设置小地图跟随主角
    /// </summary>
    /// <param name="transform"></param>
    public void SetMiniMapFollowTarget(Transform transform) 
    {
        m_MiniMapFollowTarget = transform;
        m_FollowMiniMapCamera.transform.position = transform.position + new Vector3(0,10,0);
        m_FollowMiniMapCamera.transform.LookAt(transform);
        m_OffsetPosition = m_FollowMiniMapCamera.transform.position - transform.position;
    }
    /// <summary>
    /// 小地图变焦放大
    /// </summary>
    public void MiniMapZoomIn()
    {
        m_FollowMiniMapCamera.fieldOfView += 40;
    }
    /// <summary>
    /// 小地图变焦缩小
    /// </summary>
    public void MiniMapZoomOut()
    {
        m_FollowMiniMapCamera.fieldOfView -= 40;
    }
    #endregion

}