using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace View.Loading {
    public class LoadingView : MonoBehaviour
    {
        public static string nextScene;

        [Header("UI")]
        [Space(4)]
        [SerializeField]
        Image image_progress;
        [SerializeField]
        TextMeshProUGUI text_progress;
        [SerializeField]
        Image image;

        #region Unity Method
        void Start() {
            StartCoroutine(LoadScene());
        }
        #endregion

        public static void LoadScene(string scene) {
            nextScene = scene;
            SceneManager.LoadScene("LoadingScene");
        }

        IEnumerator LoadScene() {
            yield return null;

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName: nextScene);
            op.allowSceneActivation = false;

            this.image_progress.transform.localScale = new Vector3(0, 1, 1);
            float timer = 0.0f;
            while(op.isDone == false) {
                yield return null;
                timer += Time.deltaTime;
                if (0.5f > op.progress) {
                    float progress = Mathf.Lerp(this.image_progress.transform.localScale.x, op.progress, timer);
                    this.image_progress.transform.localScale = new Vector3(
                        progress,
                        1,
                        1
                    );
                    this.text_progress.text = $"{Mathf.FloorToInt(progress * 100)}%";
                } else {
                    float progress = Mathf.Lerp(this.image_progress.transform.localScale.x, 1, timer);
                    this.image_progress.transform.localScale = new Vector3(
                        progress,
                        1,
                        1
                    );
                    this.text_progress.text = $"{Mathf.FloorToInt(progress * 100)}%";
                    if (progress >= 1f) {
                        break;
                    }
                }
            }
            this.image
                .DOFade(1f, 0.5f)
                .From(0)
                .OnComplete(() => {
                    op.allowSceneActivation = true;
                });
        }
    }
}

