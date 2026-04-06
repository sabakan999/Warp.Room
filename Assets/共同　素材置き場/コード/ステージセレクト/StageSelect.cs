using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public void SelectStage(int world, int stage)
    {
        GameSettings.selectedWorld = world;
        GameSettings.selectedStage = stage;

        SceneManager.LoadScene("ワープ・ルーム");
    }
}