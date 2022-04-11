/* ================================================
 * Introduction：xxxx
 * Creator：xxxx
 * CreationTime：2022/3/17 15:03:00
 * CreateVersion：0.1
================================================ */
using UnityEngine.UI;

namespace Deer.UI
{
    /// <summary>
    /// Please modify the description.
    /// </summary>
    public partial class UILoginPanel : UIBaseForms
	{
		private ProcedureLogin m_ProcedureLogin = null;

		private int m_TimeId;
		private bool m_IsAPas =false;
		void Start()
		{
			GetBindComponents(gameObject);
			m_ProcedureLogin = (ProcedureLogin)GameEntry.Procedure.CurrentProcedure;
			m_Btn_LoginBtn.onClick.AddListener(delegate ()
			{
				m_ProcedureLogin.OnChangeProcedure(ProcedureConfig.ProcedureMainHall);
				Close();
				//GameEntry.NetConnector.Connect("192.168.164.139", 9000);
			});

			m_Btn_LoginBtn1111.onClick.AddListener(delegate () {

				//m_ProcedureLogin.Send();
				if (m_IsAPas)
				{
					GameEntry.Timer.ResumeTimer(m_TimeId);
					m_IsAPas = false;
				}
				else 
				{
					GameEntry.Timer.PauseTimer(m_TimeId);
					m_IsAPas = true;
				}
			});

			m_Img_Quan.SetSprite(AssetUtility.UI.GetSpriteCollectionPath("Login/Login"), AssetUtility.UI.GetSpritePath("Login/01100_41042_sehuan@2x"));
			m_RImg_Bg.SetTextureByResources(AssetUtility.UI.GetTexturePath("BigBG/battle_bg"));
			m_RImg_bg.SetTextureWithVideo("mv",CompleteCallBack);

/*			m_TimeId = GameEntry.Timer.AddRepeatedTimer(3000, 100, delegate ()
            {
                Log.ColorInfo(ColorType.white, "Time");
            });*/
            /*			GameEntry.Timer.AddOnceTimer(2000, delegate () {
                                        Log.ColorInfo(ColorType.white, "Time");
                        });*/
        }

		void OnDisable() {
			m_Btn_LoginBtn.onClick.RemoveAllListeners();
			m_Btn_LoginBtn1111.onClick.RemoveAllListeners();
			m_RImg_bg.SetStopVideo();
		}
		void CompleteCallBack(bool success,RawImage rawImage) 
		{
			//m_RImg_bg.SetTextureWithVideo("mv");
		}
	}
}
