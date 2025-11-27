using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Werehorse.Runtime.Utility.CommonObjects {
    public class PrefabPool {
        private int _cursor;
        private readonly GameObject _prefab;
        private readonly Transform _parent;
        private readonly GameObject[] _pool;
        
        public PrefabPool(GameObject prefab, Transform parent, int capacity = 100) {
            _prefab = prefab;
            _parent = parent;
            _pool = new GameObject[capacity];
            _cursor = 0;
        }

        public IEnumerable<GameObject> GetAllActiveObjects() {
            return _pool.Where(Active);
        }
        
        public bool GetObject(out GameObject obj) {
            obj = _pool.Where(Available).FirstOrDefault();
            return obj || TryCreateNewObject(out obj);
        }

        private bool TryCreateNewObject(out GameObject obj) {
            if (_cursor >= _pool.Length) {
                obj = null;
                return false;
            }

            _pool[_cursor] = Object.Instantiate(_prefab, _parent);
            _pool[_cursor].SetActive(false);
            obj = _pool[_cursor];
            
            _cursor++;
            return true;
        }

        private bool Available(GameObject poolObject) {
            return !poolObject?.activeSelf ?? false;
        }

        private bool Active(GameObject poolObject) {
            return poolObject?.activeSelf ?? false;
        }
    }
}
