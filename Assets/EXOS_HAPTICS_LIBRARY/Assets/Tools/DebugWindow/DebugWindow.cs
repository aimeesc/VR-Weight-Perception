using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace exiii.Unity
{
    public class DebugWindow : StaticAccessableMonoBehaviour<DebugWindow>
    {
        #region Inspector

        [Header(nameof(DebugWindow))]

        [SerializeField]
        private bool isShow;

        public bool IsShow
        {
            get { return isShow; }
            set { isShow = value; }
        }

        [SerializeField]
        private GameObject debugWindowObject;

        [SerializeField]
        private bool showUnityLog;

        [SerializeField]
        private bool showStackTrace;

        [SerializeField]
        private int numLineLimit = 20;

        [SerializeField]
        private GameObject flowTextObject;

        #endregion

        GameObject canvas;
        Text fulText;

        List<GameObject> textObjects;

        string returnString = "\n";

        private void Update()
        {
            if (canvas == null)
            {
                CanvasMoveToCamera();
            }
            else
            {
                if (isShow)
                {
                    if (canvas.activeSelf == false)
                        canvas.SetActive(true);
                }
                else
                {
                    if (canvas.activeSelf == true)
                        canvas.SetActive(false);
                }
            }
        }

        protected override void OnEnable()
        {
            Application.logMessageReceived += OnUnityLog;
        }

        protected override void OnDisable()
        {
            Application.logMessageReceived -= OnUnityLog;
        }

        bool CanvasMoveToCamera()
        {
            canvas = Instantiate(debugWindowObject);
            fulText = canvas.transform.Find("FulText").GetComponent<Text>();
            fulText.text = string.Empty;

            canvas.transform.SetParent(transform, false);
            canvas.SetActive(isShow);
            return true;
        }

        void FulTextLimitation()
        {
            int numLines = NumLines(fulText.text);
            if (numLines > numLineLimit)
            {
                string text = fulText.text;
                var lines = new List<string>(fulText.text.Split(returnString.ToCharArray()));
                int numRemove = numLines - numLineLimit;
                bool isInsideTag = false;
                int removeCount = 0;

                for (int i = 0; i < lines.Count; ++i)
                {
                    if (i >= numRemove)
                        break;

                    if (lines[i].Contains("<color="))
                        isInsideTag = true;

                    if (lines[i].Contains("</color>"))
                        isInsideTag = false;

                    if (isInsideTag)
                    {
                        removeCount++;
                    }
                    else
                    {
                        for (int j = 0; j < removeCount + 1; ++j)
                        {
                            int firstReturn = text.IndexOf(returnString);
                            text = text.Remove(0, firstReturn + 1);
                        }
                        removeCount = 0;
                    }
                }
                fulText.text = text;
            }
        }

        int NumLines(string text)
        {
            return text.Split(returnString.ToCharArray()).Length;
        }

        void OnUnityLog(string logText, string stackTrace, LogType type)
        {
            if (showUnityLog || showStackTrace)
            {
                if (showUnityLog)
                {
                    switch (type)
                    {
                        case LogType.Log:
                            WriteLine("[Debug] " + logText); break;
                        case LogType.Warning:
                            WriteLine("[Worning] " + logText, Color.yellow); break;
                        case LogType.Error:
                            WriteLine("[Error] " + logText, Color.red); break;
                        case LogType.Assert:
                            WriteLine("[Assert] " + logText, Color.red); break;
                        case LogType.Exception:
                            WriteLine(logText, Color.red); break;
                    }
                }
                if (showStackTrace)
                {
                    WriteLine("[StackTrace] " + stackTrace, Color.grey);
                }
            }
        }

        string GetColorBegin(Color color)
        {
            return "<color=#" +
                ((int)(color.r * 255)).ToString("x2") +
                ((int)(color.g * 255)).ToString("x2") +
                ((int)(color.b * 255)).ToString("x2") +
                ((int)(color.a * 255)).ToString("x2") +
                ">";
        }

        string GetColorEnd()
        {
            return "</color>";
        }

        public void ChangeShow()
        {
            IsShow = !IsShow;
        }

        public void Write(string message)
        {
            if (fulText == null) return;

            fulText.text += message;
            FulTextLimitation();
        }

        public void WriteLine(string message)
        {
            Write(message + returnString);
        }

        public void Write(string message, Color color)
        {
            if (fulText == null) return;

            fulText.text += GetColorBegin(color) + message + GetColorEnd();
            FulTextLimitation();
        }

        public void WriteLine(string message, Color color)
        {
            Write(message + returnString, color);
        }

        public void Clear()
        {
            fulText.text = string.Empty;
        }

        public UnityEngine.UI.Text AddTextObject(Vector2 position, string message = "")
        {
            var o = Instantiate(flowTextObject);
            o.name = flowTextObject.name;
            o.transform.SetParent(canvas.transform, false);
            o.transform.GetComponent<RectTransform>().offsetMin = position;
            o.transform.GetComponent<RectTransform>().offsetMax = new Vector2(position.x + 160, position.y + 16);

            var text = o.GetComponent<Text>();
            text.text = message;
            return text;
        }

        //StaticManagerBase
        public override void Initialize()
        {
            base.Initialize();

            CanvasMoveToCamera();
        }
    }
}
