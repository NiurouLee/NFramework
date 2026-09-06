using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OM.Editor
{
    public abstract class OMBaseEditor : UnityEditor.Editor
    {
        public VisualElement Root { get; protected set; }
        
        public override VisualElement CreateInspectorGUI()
        {
            Root = new VisualElement();
            InitializeEditor(Root);
            return Root;
        }

        #region Draw Inspector Funcs

        protected virtual void InitializeEditor(VisualElement root)
        {
            var visualTree = OMEditorUtility.GetBaseEditorTree();
            var styleSheet = OMEditorUtility.GetBaseEditorStyleSheet();

            visualTree.CloneTree(root);
            root.styleSheets.Add(styleSheet);

            root.Q<VisualElement>("header").Q<Button>("header-button").clicked += CreateHeaderGenericMenu;
            root.Q<VisualElement>("header").Q<Button>("edit-script-btn").clicked += OpenScriptToEdit;
            
            Root.Q<VisualElement>("header").Q<Label>("title").text = GetTitle();
            
            Header(root);
            DrawInspector(root);
            Footer(root);
        }
        
        protected virtual void DrawInspector(VisualElement root)
        {

        }

        protected virtual void Header(VisualElement root)
        {
            
        }

        protected virtual void Footer(VisualElement root)
        {
            TryAddRenameButton(root);
        }
        
        #endregion

        #region GenericMenu

        protected void CreateHeaderGenericMenu()
        {
            var genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Edit"),false, OpenScriptToEdit);
            
            OnCreateHeaderGenericItems(ref genericMenu);
            genericMenu.ShowAsContext();
        }
        
        protected virtual void OnCreateHeaderGenericItems(ref GenericMenu genericMenu)
        {

        }

        protected void OpenScriptToEdit()
        {
            var serializedProperty = serializedObject.FindProperty("m_Script");
            AssetDatabase.OpenAsset(serializedProperty.objectReferenceValue);
        }
        
        #endregion
        
        protected virtual string GetTitle()
        {
            return OMEditorUtility.TryGetOMTitle(target,out var title) ? title : target.GetType().Name;
        }

        protected virtual bool TryAddRenameButton(VisualElement container)
        {
            var renamable = target as IOMGameObjectRenameable;
            if (renamable == null) return false;
            
            var button = new Button(() =>
            {
                var component = target as Component;
                Undo.RecordObject(component.gameObject,$"Rename {component.name}");
                component.name = renamable.GetCustomGameObjectName();
            });
            button.text = "Rename";
            button.AddToClassList("om-btn");
            container.Add(button);
            
            return true;
        }
        
    }
}