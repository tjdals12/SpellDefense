using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartManager : MonoBehaviour
{
    #region Unity Method
    void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DontDestroyOnLoad");
        foreach (var obj in objs) {
            Destroy(obj);
        }
    }
    void Start() {
        SceneManager.LoadScene("LoginScene");
    }
    #endregion
}
