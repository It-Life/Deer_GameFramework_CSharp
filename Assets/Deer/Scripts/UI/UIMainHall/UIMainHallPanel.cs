// ================================================
//描 述 :  
//作 者 : 杜鑫 
//创建时间 : 2022-03-15 16-33-44  
//修改作者 : 杜鑫 
//修改时间 : 2022-03-15 16-33-44  
//版 本 : 0.1 
// ===============================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using zFrame.UI;
using GameFramework;
using UnityEngine.UI;

public class UIMainHallPanel : UIBaseForms
{
	public Joystick joystick;
	public UIButtonSuper close;
	public UIButtonSuper zoomIn;
	public UIButtonSuper zoomOut;
	public RawImage miniMap;
	public UIButtonSuper zoom;

	#region Can Delete
	Transform MainCamera;
	Animator playerAnim;
	Transform player;
	#endregion

	private bool m_isMoveing;
	private ProcedureMainHall m_ProcedureMainHall = null;

	private bool curIsZoomIn = false;
	void Start()
	{
        joystick.OnValueChanged.AddListener(delegate (Vector2 v) {
			if (v.magnitude != 0)
			{
				MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
				messengerInfo.param1 = v.x;
				messengerInfo.param2 = v.y;
				m_isMoveing = true;
				GameEntry.Messenger.SendEvent(EventName.EVENT_CS_GAME_MOVE_DIRECTION, messengerInfo);
			}
			else 
			{
                if (m_isMoveing)
				{
					m_isMoveing = false;
					MessengerInfo messengerInfo = ReferencePool.Acquire<MessengerInfo>();
					messengerInfo.param1 = false;
					GameEntry.Messenger.SendEvent(EventName.EVENT_CS_GAME_MOVE_END, messengerInfo);
				}
			}
		});

		close.onClick.AddListener(delegate () {
			m_ProcedureMainHall = (ProcedureMainHall)GameEntry.Procedure.CurrentProcedure;
			m_ProcedureMainHall.OnChangeProcedure(ProcedureConfig.ProcedureLogin);
		});
        zoomIn.onClick.AddListener(delegate () {
			ChangeMiniMapZoom(1);
		});
		zoomOut.onClick.AddListener(delegate () {
			ChangeMiniMapZoom(2);
		});
		zoom.onDoubleClick.AddListener(delegate ()
		{
			ChangeMiniMapZoom();
		});
	}
	private void ChangeMiniMapZoom(int isZoomIn = 0) 
	{
		if (curIsZoomIn)
		{
            if (isZoomIn == 1)
            {
				return;
            }
			curIsZoomIn = false;
			GameEntry.Camera.MiniMapZoomOut();
			miniMap.rectTransform.sizeDelta = new Vector2(500f, 400f);
		}
		else 
		{
			if (isZoomIn == 2)
			{
				return;
			}
			curIsZoomIn = true;
			GameEntry.Camera.MiniMapZoomIn();
			miniMap.rectTransform.sizeDelta = new Vector2(600f, 500f);
		}
	}
}