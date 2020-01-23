﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour
{
    // ボタンをクリックするとTitleSceneに移動します
    public void ButtonClicked()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
