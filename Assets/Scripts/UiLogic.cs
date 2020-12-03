using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLogic : MonoBehaviour
{
	public Button resetButton;
	public Button quitButton;
	public GameObject winPanel;

	void Start()
	{
		Button btnReset = resetButton.GetComponent<Button>();
		Button btnQuit = quitButton.GetComponent<Button>();
		btnReset.onClick.AddListener(OnResetClick);
		btnQuit.onClick.AddListener(OnQuitClick);
	}


    private void Update()
    {
		if (Input.GetKey("escape"))
			Application.Quit();
	}

    void OnResetClick() => winPanel.SetActive(false);

	void OnQuitClick()
	{
		Application.Quit();
	}
}
