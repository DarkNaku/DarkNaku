using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DarkNaku;

namespace DarkNaku {
    public sealed class Director : SingletonBehaviour<Director> {
        public static bool IsChanging { get; private set; }
        public static float MinRetentionTime { get; set; }
        public static string LoadingSceneName { get; set; }

        public static void ChangeScene(string nextSceneName) {
            ChangeScene(LoadingSceneName, nextSceneName);
        }

        public static void ChangeScene(string loadingSceneName, string nextSceneName) {
            Assert.IsFalse(IsChanging,
                "[Director] ChangeScene : This method could not call by continuous.");
            Assert.IsFalse(string.IsNullOrEmpty(LoadingSceneName),
                "[Director] ChangeScene : 'Director.LoadingSceneName' property is null or empty.");
            Instance.StartCoroutine(Instance.CoChangeScene(loadingSceneName, nextSceneName));
        }

        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }

        private IEnumerator CoChangeScene(string loadingSceneName, string nextSceneName) {
            if (IsChanging) yield break;

            IsChanging = true;
            if (EventSystem.current != null) EventSystem.current.enabled = false;
            Scene currentScene = SceneManager.GetActiveScene();
            ISceneHandler currentSceneHandler = FindHandler<ISceneHandler>(currentScene);
            yield return SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);

            Scene loadingScene = SceneManager.GetSceneByName(loadingSceneName);
            while (loadingScene.isLoaded == false) yield return null;

            SceneManager.SetActiveScene(loadingScene);
            ILoader loader = FindHandler<ILoader>(loadingScene);

            if (currentSceneHandler != null) currentSceneHandler.OnStartOutAnimation();
            if (loader != null) yield return StartCoroutine(loader.CoOutAnimation());
            if (currentSceneHandler != null) currentSceneHandler.OnUnloadScene();

            float startTime = Time.time;

            if (loader == null) {
                yield return SceneManager.UnloadSceneAsync(currentScene);
                yield return SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive);
            } else {
                yield return StartCoroutine(CoSceneAsync(SceneManager.UnloadSceneAsync(currentScene),
                        (progress) => { loader.OnProgress((progress / 0.9F) * 0.5F); })
                    );
                Resources.UnloadUnusedAssets();
                loader.OnProgress(0.5F);

                yield return StartCoroutine(CoSceneAsync(SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Additive),
                        (progress) => { loader.OnProgress(0.5F + ((progress / 0.9F) * 0.5F)); })
                    );
                loader.OnProgress(1F);
            }

            currentScene = SceneManager.GetSceneByName(nextSceneName);
            EventSystem eventSystem = GetEventSystemInScene(currentScene);
            if (eventSystem != null) eventSystem.enabled = false;
            while (currentScene.isLoaded == false) yield return null;
            SceneManager.SetActiveScene(currentScene);

            float elapsedTime = Time.time - startTime;

            if (elapsedTime < MinRetentionTime) {
                yield return new WaitForSeconds(MinRetentionTime - elapsedTime);
            }

            currentSceneHandler = FindHandler<ISceneHandler>(currentScene);

            if (currentSceneHandler != null) currentSceneHandler.OnLoadScene();
            if (loader != null) yield return StartCoroutine(loader.CoInAnimation());
            if (currentSceneHandler != null) currentSceneHandler.OnEndInAnimation();

            SceneManager.UnloadSceneAsync(loadingScene); // Need Yield ???
            if (eventSystem != null) eventSystem.enabled = true;

            IsChanging = false;
        }

        private T FindHandler<T>(Scene scene) where T : class {
            GameObject[] goes = scene.GetRootGameObjects();

            for (int i = 0; i < goes.Length; i++) {
                T handler = goes[i].GetComponent(typeof(T)) as T;
                if (handler != null) return handler;
            }

            return null;
        }

        private IEnumerator CoSceneAsync(AsyncOperation ao, System.Action<float> onProgress) {
            Assert.IsNotNull(ao, "[Director] CoSceneAsync : 'AsyncOperation' can not be null.");
            ao.allowSceneActivation = false;

            while (ao.progress < 0.9F) {
                yield return null;
                if (onProgress != null) onProgress(ao.progress);
            }

            ao.allowSceneActivation = true;
            yield return null;
        }

        private EventSystem GetEventSystemInScene(Scene scene) {
            EventSystem[] ess = EventSystem.FindObjectsOfType<EventSystem>();

            for (int i = 0; i < ess.Length; i++) {
                if (ess[i].gameObject.scene == scene) return ess[i];
            }

            return null;
        }
    }
}