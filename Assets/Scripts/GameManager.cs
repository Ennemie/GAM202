using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int killCount;
    public TMP_Text killCountText;
    public GameObject canvas;
    void Start()
    {
        canvas.SetActive(false);
        killCount = 0;
        UpdateKillCountText();
    }
    private void Update()
    {
        UpdateKillCountText();
    }
    void UpdateKillCountText()
    {
        killCountText.text = "Kills: " + killCount.ToString();
    }

    public void Continue()
    {
        canvas.SetActive(false);
        Time.timeScale = 1f;
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
