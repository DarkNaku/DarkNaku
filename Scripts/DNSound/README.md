# DNSound



#### 설명

Resources에 있는 오디오 클립을 로드하여 플레이하고 재활용 및 관리하는 클레스입니다.


## 클래스



### DNSound

Inherits from : SingletonMonobehaviour\<T>


### 속성

**public static string ResourcePath;**
**public static float DefaultFadeTimeOfBGM;**
**public static int BGMChannelCount;**

### 함수

**public static void Play(string clipName);**
**public static void Play(string clipName, float volume);**
**public static void Play(string clipName, float volume, bool loop);**
**public static void Play(string clipName, float volume, bool loop, float fadeTime);**

오디오 클립을 로드하여 플레이하는 함수입니다.

clipName : 경로를 제외한 클립 이름.
volume : 볼륨 (0 ~ 1)
loop : 반복여부.
fadeTime : 값이 0보다 클경우 Fade 효과가 적용됨.

**public static void PlayBGM(string clipName);**
**public static void PlayBGM(string clipName, float volume);**
**public static void PlayBGM(int channel, string clipName);**
**public static void PlayBGM(string clipName, float volume, float fadeTime);**
**public static void PlayBGM(int channel, string clipName, float volume);**
**public static void PlayBGM(int channel, string clipName, float volume, float fadeTime);**

Play API와 비슷하나 BGM으로 관리하며 Loop로 플레이하는 함수입니다.

channel : BGM 체널 인덱스. 기본 인덱스는 0 입니다.
clipName : 경로를 제외한 클립 이름.
volume : 볼륨 (0 ~ 1)
fadeTime : 값이 0보다 클경우 Fade 효과가 적용됨.

**public static void Stop(string clipName);**
**public static void Stop(string clipName, float fadeTime);**

주어진 클립 이름으로 실행중인 모든 사운드를 중지합니다.

clipName : 경로를 제외한 클립 이름.
fadeTime : 값이 0보다 클경우 Fade 효과가 적용됨.

**public static void StopBGM();**
**public static void StopBGM(float fadeTime);**
**public static void StopBGM(int channel, float fadeTime);**

주어진 BGM 체널의 플레이를 중지합니다.

channel : BGM 체널 인덱스. 기본 인덱스는 0 입니다.
fadeTime : 값이 0보다 클경우 Fade 효과가 적용됨.

**public static void PrevLoadClip(params string[] clipNames);**

사이즈가 크거나 플레이 딜레이 발생을 줄이기 위해 오디오 클립을 미리 로드할 때 사용합니다.

clipNames : 경로를 제외한 클립 목록.

**public static void Clear();**

플레이 중이 아닌 모든 오디오 클립 레퍼런스를 제거하여 GC로 회수가 가능하도록 만듭니다.