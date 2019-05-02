# GOPool



#### 설명

GameObject를 Destroy 하지 않고 Active On/Off 해서 재활용 할 수 있도록 관리하는 메니저 입니다.

**주의 : Scene을 전환할 때 Clear를 호출해서 재활용 리스트를 비워줘야 합니다.**



## 클래스



### GOPool

Inherits from : SingletonMonobehaviour\<T>




### 함수



**public static GameObject GetItem(string path, Transform parent = null);**

새로운 GameObject를 생성하여 얻어오는 함수.

path : Resources 하위의 Prefab 패스 입니다.

parent : 생성할 때 GameObject의 부모를 지정할 수 있습니다.



**public static T GetItem<T>(string path, Transform parent = null);**

새로운 GameObject를 생성하여 T 타입의 객체를 얻어 오는 함수입니다.

path : Resources 하위의 Prefab 패스 입니다.

parent : 생성할 때 GameObject의 부모를 지정할 수 있습니다.



**public static void Abandon(GameObject item);**

사용이 끝난 GameObject를 재활용을 위해 버리는 함수입니다.

item : 사용이 끝난 관리되는 GameObject



**public static bool IsManagedItem(GameObject item);**

GameObject가 관리 대상인지 확인하는 함수입니다.

item : 확인할 대상 GameObject.



**public static void Clear();**

관리하고 있는 모든 GameObject들을 제거하는 함수입니다.

Scene을 전환 하기 직전에 반드시 호출해 주어야 메모리 누수가 발생하지 않습니다.

