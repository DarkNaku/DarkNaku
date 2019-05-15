using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DarkNaku;

namespace DarkNaku {
    public sealed class Director : SingletonBehaviour<Director> {
    private bool _isInTransition = false;
    private bool _useLoadingScene = false;
    private string _loadingSceneName = null;
    private float _minMaintainLoadingTime = 0F;

    public static bool IsInTransition { get { return Instance._isInTransition; } }

    public static string LoadingSceneName { 
        get { return Instance._loadingSceneName; }
        set { 
            Instance._useLoadingScene = (string.IsNullOrEmpty(value) == false);
            Instance._loadingSceneName = value;
        }
    }

    public static float MinMaintainLoadingTime { 
        get { return Instance._minMaintainLoadingTime; }
        set { Instance._minMaintainLoadingTime = Mathf.Max(0F, value); }
    }

    public static void ChangeScene(string nextSceneName) {
        ChangeScene(nextSceneName, null);
    }

    public static void ChangeScene(string nextSceneName, object param) {
        Assert.IsFalse(IsInTransition, 
            "[Director] ChangeScene : This method could not call by continuous.");
        Instance.StartCoroutine(Instance.CoChangeScene(nextSceneName, param));
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
	}

    private IEnumerator CoChangeScene(string nextSceneName, object param) {
        if (_isInTransition) yield break;

        _isInTransition = true;
        AsyncOperation ao = null;
        Scene currentScene = SceneManager.GetActiveScene();
        EventSystem eventSystem = GetEventSystemInScene(currentScene);
        if (eventSystem != null) eventSystem.enabled = false;

        if (_useLoadingScene) {
            ao = SceneManager.LoadSceneAsync(_loadingSceneName);
            ao.allowSceneActivation = false;
        }

        ISceneHandler currentSceneHandler = FindHandler<ISceneHandler>(currentScene);
        currentSceneHandler.OnStartOutAnimation();

        yield return StartCoroutine(currentSceneHandler.CoOutAnimation());

        float outTime = Time.time;
        ILoading loader = null;

        if (_useLoadingScene) {
            while (ao.progress < 0.9F) yield return null;

            currentSceneHandler.OnUnloadScene();
            ao.allowSceneActivation = true;
            Scene loadingScene = SceneManager.GetSceneByName(_loadingSceneName);

            while (loadingScene.isLoaded == false) yield return null;
            loader = FindHandler<ILoading>(loadingScene);
        }

        ao = SceneManager.LoadSceneAsync(nextSceneName);
        ao.allowSceneActivation = false;
        float elapsedTime = Time.time - outTime;

        if (elapsedTime < _minMaintainLoadingTime) {
            yield return new WaitForSeconds(_minMaintainLoadingTime - elapsedTime);
        }

        while (ao.progress < 0.9F) {
            if (loader != null) loader.OnProgress(ao.progress / 0.9F);
            yield return null;
        }
        
        if (_useLoadingScene == false) currentSceneHandler.OnUnloadScene();
        ao.allowSceneActivation = true;
        currentScene = SceneManager.GetSceneByName(nextSceneName);
        eventSystem = GetEventSystemInScene(currentScene);
        if (eventSystem != null) eventSystem.enabled = false;

        while (currentScene.isLoaded == false) yield return null;

        currentSceneHandler = FindHandler<ISceneHandler>(currentScene);
        currentSceneHandler.OnLoadScene(param);

        yield return StartCoroutine(currentSceneHandler.CoInAnimation());

        currentSceneHandler.OnEndInAnimation(param);
        if (eventSystem != null) eventSystem.enabled = true;
        _isInTransition = false;
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