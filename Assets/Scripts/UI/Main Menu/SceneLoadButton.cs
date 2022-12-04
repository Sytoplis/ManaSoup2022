using UnityEngine.SceneManagement;

public class SceneLoadButton : ButtonTemplate {
    public int loadScene = 0;
    public override void OnClick() {
        SceneManager.LoadScene(loadScene, LoadSceneMode.Single);
    }
}
