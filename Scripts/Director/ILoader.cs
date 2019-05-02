using System.Collections;

public interface ILoader {
    IEnumerator CoInAnimation();
    void OnProgress(float progress);
    IEnumerator CoOutAnimation();
}