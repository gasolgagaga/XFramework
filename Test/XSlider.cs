using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace XGUI.XSlider {


    internal class Editor {
        [MenuItem("GameObject/XGUI/Slider", priority = 1)]
        internal static void CreateBlankGraphic(MenuCommand menuCommand) {
            GameObject slider = new GameObject("Slider");
            GameObject background = new GameObject("Background");
            GameObject fill = new GameObject("Fill");
            background.transform.SetParent(slider.transform);
            fill.transform.SetParent(slider.transform);
            Image backgroundImg = background.AddComponent<Image>();
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = Color.black;
            GameObjectUtility.SetParentAndAlign(slider, menuCommand.context as GameObject);
            Slider script = slider.AddComponent<Slider>();
            slider.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 50);
            slider.GetComponent<RectTransform>().pivot = new Vector2(0, 0.5f);
            script.SliderBgRect = background.GetComponent<RectTransform>();
            script.SliderBgRect.sizeDelta = new Vector2(400, 50);
            script.SliderBgRect.pivot = new Vector2(0, 0.5f);
            script.SliderFillRect = fill.GetComponent<RectTransform>();
            script.SliderFillRect.sizeDelta = new Vector2(400, 50);
            script.SliderFillRect.pivot = new Vector2(0, 0.5f);

            GameObjectUtility.SetParentAndAlign(slider, menuCommand.context as GameObject);
        }
    }

    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class Slider : MonoBehaviour {

        public RectTransform SliderBgRect;
        public RectTransform SliderFillRect;
        public RectTransform SliderBufferRect;

        [Range(0,1)]
        public float Value;

        private float m_Length;
        private float m_Width;

        private void Start() {
            if (SliderFillRect != null) {
                m_Length = SliderFillRect.sizeDelta.x;
                m_Width = SliderFillRect.sizeDelta.y;
            }       
        }
        private void Update() {
            if (SliderFillRect != null) {
                SliderFillRect.sizeDelta = new Vector2(m_Length * Value,m_Width);
            }
        }
    }
}


