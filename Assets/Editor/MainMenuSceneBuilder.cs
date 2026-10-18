using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class MainMenuSceneBuilder
{
    private const string ScenePath = "Assets/Scenes/MainMenuTest.unity";

    [MenuItem("GameObject/Main Menu/Create Placeholder Main Menu Scene", false, 10)]
    public static void CreateScene()
    {
        Directory.CreateDirectory("Assets/Scenes");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        CreateCamera();
        CreateCanvas();
        CreateEventSystem();

        var sceneAsset = EditorSceneManager.SaveScene(scene, ScenePath);
        if (sceneAsset)
        {
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (!scenes.Exists(item => item.path == ScenePath))
                scenes.Add(new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            Debug.Log("Placeholder main menu created at " + ScenePath);
        }
    }

    private static void CreateCamera()
    {
        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        cameraObject.transform.position = new Vector3(0f, 0f, -10f);

        var camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;
    }

    private static void CreateCanvas()
    {
        var canvasObject = new GameObject("Main Menu Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        var backgroundObject = new GameObject("White Background", typeof(RectTransform), typeof(Image));
        backgroundObject.transform.SetParent(canvasObject.transform, false);
        var backgroundRect = backgroundObject.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;
        backgroundObject.GetComponent<Image>().color = Color.white;

        var title = CreateLabel("Game Title", "GAME TITLE", canvasObject.transform, 64, FontStyle.Bold);
        SetRect(title.rectTransform, new Vector2(0.5f, 0.72f), new Vector2(0.5f, 0.72f), new Vector2(700, 100));

        string[] options = { "開始遊戲", "設定", "切換存檔", "離開遊戲" };
        for (int i = 0; i < options.Length; i++)
        {
            var buttonObject = new GameObject(options[i], typeof(RectTransform), typeof(Image), typeof(Button), typeof(MainMenuHoverEffect));
            buttonObject.transform.SetParent(canvasObject.transform, false);
            var rect = buttonObject.GetComponent<RectTransform>();
            SetRect(rect, new Vector2(0.5f, 0.56f - i * 0.13f), new Vector2(0.5f, 0.56f - i * 0.13f), new Vector2(440, 82));
            var image = buttonObject.GetComponent<Image>();
            image.color = Color.white;
            image.raycastTarget = true;
            var button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.None;

            var label = CreateLabel("Label", options[i], buttonObject.transform, 34, FontStyle.Normal);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var hover = buttonObject.GetComponent<MainMenuHoverEffect>();
            SetPrivateField(hover, "background", image);
            SetPrivateField(hover, "label", label);
        }
    }

    private static Text CreateLabel(string objectName, string value, Transform parent, int size, FontStyle style)
    {
        var labelObject = new GameObject(objectName, typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(parent, false);
        var text = labelObject.GetComponent<Text>();
        text.text = value;
        // Unity 6 removed Arial.ttf as a built-in resource name.
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.fontStyle = style;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 size)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
    }

    private static void CreateEventSystem()
    {
        var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        eventSystem.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
    }

    private static void SetPrivateField<T>(T target, string fieldName, object value) where T : Component
    {
        var field = typeof(MainMenuHoverEffect).GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field?.SetValue(target, value);
    }
}
