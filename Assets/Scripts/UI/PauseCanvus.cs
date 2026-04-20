using System.Diagnostics;
using UnityEngine;
using FMODUnity;

public class PauseCanvus : MonoBehaviour
{
    public GameObject pauseMenu;

    [SerializeField]
    private KeyCode toggleKey = KeyCode.Escape;

    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            TogglePause();
    }

//    void OnDisable()
//    {
//#if UNITY_EDITOR
//        UnityEngine.Debug.LogError(
//            $"`{nameof(PauseCanvus)}` was DISABLED at runtime on '{GetHierarchyPath(transform)}' " +
//            $"(scene: '{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}').\n" +
//            $"Stack:\n{new StackTrace(true)}",
//            this);

//        // If VS is attached, this will break exactly when it happens.
//        if (Debugger.IsAttached)
//            Debugger.Break();
//#endif
//    }

    static string GetHierarchyPath(Transform t)
    {
        if (t == null)
            return "<null>";

        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }

    public void Pause()
    {
        IsPaused = true;

        if (pauseMenu != null)
            pauseMenu.SetActive(true);

        Input.ResetInputAxes();
        Input.multiTouchEnabled = false;
        Input.simulateMouseWithTouches = false;
        Input.backButtonLeavesApp = false;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (PlayerController.instance != null)
            PlayerController.instance.DisablePlayerInput();

        RuntimeManager.PauseAllEvents(true);
    }

    public void Resume()
    {
        IsPaused = false;

        if (pauseMenu != null)
            pauseMenu.SetActive(false);

        Input.multiTouchEnabled = true;
        Input.simulateMouseWithTouches = true;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (PlayerController.instance != null)
            PlayerController.instance.EnablePlayerInput();

        RuntimeManager.PauseAllEvents(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
