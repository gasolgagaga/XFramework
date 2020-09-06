/******************************************************
 * FlieName:游戏摇杆
 * Auth:    Gasol.X
 * Date:    2020.9.6 15:54
 * Purpose: 通过UI模拟摇杆
 * Add:     2020.9.6.18.52 添加摇杆跟随模式
 *          2020.9.6.18.52 添加摇杆固定方向模式
 ******************************************************/
namespace XGUI.XJoystick
{
    using UnityEngine;
    using UnityEditor;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using System;
    using XGUI.XBlankGraphic;
    internal class Editor{
        
        [MenuItem("GameObject/XGUI/Joystick",priority = 0)]
        internal static void CreateJoystick(MenuCommand menuCommand){            
            GameObject joystick = new GameObject("Joystick",typeof(RectTransform),typeof(BlankGraphic));
            GameObject center = new GameObject("Hander");
            GameObject background = new GameObject("Background");
            background.transform.SetParent(joystick.transform);
            center.transform.SetParent(background.transform);     
            Image centerImg = center.AddComponent<Image>();
            centerImg.color = Color.black;
            Image backgroundImg = background.AddComponent<Image>();
            Joystick script = joystick.AddComponent<Joystick>();
            script.JoystickRect = joystick.GetComponent<RectTransform>();
            script.BackgroundRect = background.GetComponent<RectTransform>();
            script.CenterRect = center.GetComponent<RectTransform>();
            script.CenterRect.sizeDelta = script.BackgroundRect.sizeDelta/3;
            script.UICamera = Camera.main;
            GameObjectUtility.SetParentAndAlign(joystick, menuCommand.context as GameObject);
        }
    }

    //摇杆控件
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler{

        //拖动开始
        public Action<JoyData> OnDragStart;
        //拖动中
        public Action<JoyData> OnDraging;
        //拖动结束
        public Action OnDragEnd;
    

        //摇杆跟随 <=> 摇杆固定
        public bool IsFollow;     
        //只移动HV方向 【获得固定的8个方向】
        public bool IsOnlyHV;
        //UI相机
        public Camera UICamera;
        //摇杆响应区域
        public RectTransform JoystickRect;
        //摇杆杆
        public RectTransform CenterRect;
        //摇杆背景
        public RectTransform BackgroundRect;

        //--------UI事件---------
        public void OnDrag(PointerEventData eventData) {
            var pos = GetUIPos(eventData.position);
            if (IsOnlyHV) {
                OnlyHV(ref pos);
            }
            if (pos.magnitude > m_Radius) {
                pos = pos.normalized * m_Radius;
            }           
            if (CenterRect != null) {
                CenterRect.anchoredPosition = pos;
            }
            JoyData joyData = new JoyData {                
                Distance = pos.magnitude / m_Radius,
                Direction = Mathf.RoundToInt((Mathf.Rad2Deg * Mathf.Atan2(pos.y, pos.x) + 360) % 360)
            };
            OnDraging?.Invoke(joyData);
        }
        public void OnPointerDown(PointerEventData eventData) {
            var pos = GetUIPos(eventData.position);
            if (IsOnlyHV) {
                OnlyHV(ref pos);
            }
            if (IsFollow) {
                BackgroundRect.gameObject.SetActive(true);
                if (BackgroundRect != null) {
                    BackgroundRect.localPosition = GetUIPos(eventData.position,JoystickRect);
                }
                if (CenterRect != null) {
                    CenterRect.anchoredPosition = Vector2.zero;
                }
            }
            else {
                if (pos.magnitude > m_Radius) {
                    pos = pos.normalized * m_Radius;
                }
                if (CenterRect != null) {
                    CenterRect.anchoredPosition = pos;
                }
            }         
            JoyData joyData = new JoyData {
                Distance = pos.magnitude / m_Radius,
                Direction = Mathf.RoundToInt((Mathf.Rad2Deg * Mathf.Atan2(pos.y, pos.x) + 360) % 360)
            };
            OnDragStart?.Invoke(joyData);
        }
        public void OnPointerUp(PointerEventData eventData) {
            if (IsFollow) {
                BackgroundRect.gameObject.SetActive(false);
            }
            if (CenterRect != null) {
                CenterRect.anchoredPosition = Vector2.zero;
            }
            OnDragEnd?.Invoke();
        }

        #region 内部实现
        //禁止外部构造
        internal Joystick(){ }
        //中心位置
        private Vector2 m_Position;
        //半径
        private float m_Radius {
            get {
                if (CenterRect != null) {
                    return BackgroundRect.rect.width / 2;
                }
                return 0;
            }
        }
        //固定摇杆方向位置  
        private void OnlyHV(ref Vector2 pos) {
            float distance = pos.magnitude;
            float direction = (Mathf.Rad2Deg * Mathf.Atan2(pos.y, pos.x) + 360) % 360;
            // ⚪
            if (Mathf.Abs(distance) < m_Radius / 2) {
                pos = Vector2.zero;
            }
            // →
            else if (direction >= 337.5 || direction <= 22.5) {
                pos = new Vector2(m_Radius, 0);
            }
            // ↗
            else if(direction > 22.5 && direction <= 67.5) {
                pos = new Vector2(m_Radius * Mathf.Sin(45), m_Radius * Mathf.Sin(45));
            }
            // ↑
            else if (direction > 67.5 && direction <= 112.5) {
                pos = new Vector2(0, m_Radius);
            }
            // ↖
            else if (direction > 112.5 && direction <= 157.5) {
                pos = new Vector2(-m_Radius * Mathf.Sin(45), m_Radius * Mathf.Sin(45));
            }
            // ←
            else if (direction > 157.5 && direction <= 202.5) {
                pos = new Vector2(-m_Radius, 0);
            }
            // ↙
            else if (direction > 202.5 && direction <= 247.5) {
                pos = new Vector2(-m_Radius * Mathf.Sin(45), -m_Radius * Mathf.Sin(45));
            }
            //↓
            else if (direction > 247.5 && direction <= 292.5) {
                pos = new Vector2(0, -m_Radius);
            }
            //↘
            else if (direction > 292.5 && direction <= 337.5) {
                pos = new Vector2(m_Radius * Mathf.Sin(45), -m_Radius * Mathf.Sin(45));
            }
        }
        //获取UI位置
        private Vector2 GetUIPos(Vector2 viewPos,RectTransform rect = null) {
            if (BackgroundRect != null) {
                rect = rect == null ? BackgroundRect : rect;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, viewPos, UICamera, out m_Position);                
            }
            return m_Position;
        }
        #endregion
    }

    //摇杆数据
    public struct JoyData{
        //摇杆距离【 0 - 1 】
        public float Distance;
        //摇杆角度【 0 - 360°】
        public int Direction;       
    }

}
