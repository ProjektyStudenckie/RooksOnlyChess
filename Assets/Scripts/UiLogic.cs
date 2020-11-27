using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLogic : MonoBehaviour
{
	public Button yourButton;
	public GameObject winPanel;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(OnResetClick);
	}

	void OnResetClick()
	{
		winPanel.SetActive(false);
	}
}
