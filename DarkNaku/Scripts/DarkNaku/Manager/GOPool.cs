using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using DarkNaku;

namespace DarkNaku {
    public class GOPool : SingletonBehaviour<GOPool> {
        private Dictionary<string, GameObject> _molds = new Dictionary<string, GameObject>();
        private Dictionary<string, List<GameObject>> _pools = new Dictionary<string, List<GameObject>>();
        private Queue<GameObject> _trashs = new Queue<GameObject>();
        private bool _isRecycleWorking = false;

        public static GameObject GetItem(string path, Transform parent = null) {
            return Instance._GetItem(path, parent);
        }

        public static T GetItem<T>(string path, Transform parent = null) where T : Component {
            return Instance._GetItem<T>(path, parent);
        }

        public static void Abandon(GameObject item) {
            Assert.IsNotNull(item);
            Instance._Abandon(item);
        }

        public static void Clear() {
            Instance._Clear();
        }

        private T _GetItem<T>(string path, Transform parent) where T : Component {
            GameObject go = _GetItem(path, parent);
            T item = go.GetComponent<T>();
            Assert.IsNotNull(item, "[GOPool] GetItem<T> : Can not found type of T.");
            return item;
        }

        private GameObject _GetItem(string path, Transform parent) {
            GameObject item = null;

            if (_molds.ContainsKey(path) == false) CreateMold(path);

            List<GameObject> pool = _pools[path];

            for (int i = 0; i < pool.Count; i++) {
                if (pool[i].activeSelf) continue;
                if (_trashs.Contains(pool[i])) continue;
                item = pool[i];
                break;
            }

            if (item == null) {
                item = CreateItem(path);
            }

            if (parent == null) {
                item.transform.SetParent(transform);
            } else {
                item.transform.SetParent(parent);
            }

            item.transform.localPosition = Vector3.zero;
            item.SetActive(true);

            return item.gameObject;
        }

        private void CreateMold(string path) {
            GameObject mold = Resources.Load<GameObject>(path);
            _molds.Add(path, mold);
            _pools.Add(path, new List<GameObject>());
        }

        private GameObject CreateItem(string path) {
            Assert.IsTrue(_molds.ContainsKey(path));
            GameObject item = Instantiate(_molds[path]) as GameObject;
            _pools[path].Add(item);
            return item;
        }

        private void _Abandon(GameObject item) {
            item.SetActive(false);
            _trashs.Enqueue(item);
            if (_isRecycleWorking) return;
            StartCoroutine(CoRecycle(item));
        }

        private IEnumerator CoRecycle(GameObject item) {
            Assert.IsFalse(_isRecycleWorking);

            _isRecycleWorking = true;

            yield return new WaitForEndOfFrame();

            while (_trashs.Count > 0)
            {
                GameObject trash = _trashs.Dequeue();
                trash.transform.SetParent(transform);
            }

            _isRecycleWorking = false;
        }

        private void _Clear() {
            foreach (List<GameObject> pool in _pools.Values) {
                for (int i = 0; i < pool.Count; i++) {
                    Destroy(pool[i].gameObject);
                }
            }

            _pools.Clear();
            _molds.Clear();
            _trashs.Clear();
        }
    }
}