using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Deer.UI
{
	public partial class UILoginPanel
	{
		private RawImage m_RImg_bg;
		private InputField m_Input_passWordInputField;
		private Image m_Img_Quan;
		private UIButtonSuper m_Btn_LoginBtn;
		private RawImage m_RImg_Bg;
		private UIButtonSuper m_Btn_LoginBtn1111;
		private TextMeshProUGUI m_TxtM_Time;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_RImg_bg = autoBindTool.GetBindComponent<RawImage>(0);
			m_Input_passWordInputField = autoBindTool.GetBindComponent<InputField>(1);
			m_Img_Quan = autoBindTool.GetBindComponent<Image>(2);
			m_Btn_LoginBtn = autoBindTool.GetBindComponent<UIButtonSuper>(3);
			m_RImg_Bg = autoBindTool.GetBindComponent<RawImage>(4);
			m_Btn_LoginBtn1111 = autoBindTool.GetBindComponent<UIButtonSuper>(5);
			m_TxtM_Time = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
		}
	}
}
