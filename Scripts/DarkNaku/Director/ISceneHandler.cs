using System.Collections;

public interface ISceneHandler {
    void OnStartOutAnimation();
    void OnUnloadScene();
    void OnLoadScene();
    void OnEndInAnimation();
}