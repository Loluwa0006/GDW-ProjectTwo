using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayPressed()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
