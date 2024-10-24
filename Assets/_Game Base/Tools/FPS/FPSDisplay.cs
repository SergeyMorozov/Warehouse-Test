using UnityEngine;

namespace GAME.FPS
{
    public class FPSDisplay : MonoBehaviour
    {
        [SerializeField] private bool show;
        [SerializeField] private bool setTo60FPS;
        [SerializeField] private Color colorFPS = Color.gray;

        private float _deltaTime;
        private int _frame;
        private string _ver;
        private string _fps;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            if (setTo60FPS)
                Application.targetFrameRate = 60;
            else
                Application.targetFrameRate = 1000;


            _ver = Application.version;
            _frame = 0;
            _fps = "0";
        }

        private void LateUpdate()
        {
            _frame++;
            
            _deltaTime += Time.deltaTime;
            if (_deltaTime > 0.5f)
            {
                _deltaTime -= 0.5f;
                _fps = _frame * 2 + "";
                _frame = 0;
            }
        }

        private void OnGUI()
        {
            if (!show)
                return;

            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w / 10, h * 2 / 50);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 70;
            style.normal.textColor = colorFPS;
            float msec = _deltaTime * 1000.0f;
            float fps = 1.0f / _deltaTime;
            string text = string.Format(_ver + " (" + _fps + " fps)");
            GUI.Label(rect, text, style);
        }
    }
}