using System.Collections;

public interface ISceneHandler {
    void OnStartOutAnimation();
    void OnUnloadScene();
    void OnLoadScene(object param);
    void OnEndInAnimation(object param);
    IEnumerator CoInAnimation();
    IEnumerator CoOutAnimation();
}