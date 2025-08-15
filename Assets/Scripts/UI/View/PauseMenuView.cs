using Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.View {
    public class PauseMenuView : MonoBehaviour {
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;

        private Canvas _thisCanvas;
        
        private void Awake() {
            _thisCanvas = GetComponent<Canvas>();
            DisableCanvas();
        }

        private void OnEnable() {
            _resumeButton.onClick.AddListener(DisableCanvas);
            _restartButton.onClick.AddListener(RestartScene);
            _exitButton.onClick.AddListener(ExitGame);
            PlayerController.PausePressed += SwitchCanvasEnabled;
        }

        private void OnDisable() {
            _resumeButton.onClick.RemoveListener(DisableCanvas);
            _restartButton.onClick.RemoveListener(RestartScene);
            _exitButton.onClick.RemoveListener(ExitGame);
            PlayerController.PausePressed -= SwitchCanvasEnabled;
        }

        public void EnableCanvas() {
            _thisCanvas.enabled = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        
        public void DisableCanvas() {
            _thisCanvas.enabled = false;
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void SwitchCanvasEnabled() {
            if (_thisCanvas.enabled) DisableCanvas();
            else EnableCanvas();
        }

        private void RestartScene() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void ExitGame() {
            Application.Quit();
        }
    }
}