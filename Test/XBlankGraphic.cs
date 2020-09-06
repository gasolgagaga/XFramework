/******************************************************
 * FlieName:空白图像
 * Auth:    Gasol.X
 * Date:    2020.9.6 15:54
 * Purpose: UI接受射线检测但不参与渲染。
 ******************************************************/

namespace XGUI.XBlankGraphic{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEditor;

    internal class Editor {
        [MenuItem("GameObject/XGUI/BlankGraphic", priority = 1)]
        internal static void CreateBlankGraphic(MenuCommand menuCommand) {
            GameObject blankGraphic = new GameObject("BlankGraphic",typeof(BlankGraphic));
            GameObjectUtility.SetParentAndAlign(blankGraphic, menuCommand.context as GameObject);
        }
    }


    [RequireComponent(typeof(CanvasRenderer))]
    public class BlankGraphic : Graphic {

        internal BlankGraphic() {
            useLegacyMeshGeneration = false;
        }

        //构成网格时调用
        protected override void OnPopulateMesh(VertexHelper toFill) {
            //清除顶点信息
            toFill.Clear();
        }
    }
}

