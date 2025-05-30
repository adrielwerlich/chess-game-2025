using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    // Call this method from the button's OnClick event in the Inspector
    public void OnNewGameButtonClicked()
    {
        SceneManager.LoadScene("MainChessGame");
    }
}
