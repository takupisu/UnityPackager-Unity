#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;

/// <summary>
/// UnityPackage作成
/// https://docs.unity3d.com/ja/current/ScriptReference/ExportPackageOptions.html
/// </summary>
public static class UnityPackager
{
	/// <summary>
	/// Create the specified outPath and targetPath.
	/// </summary>
	/// Pathに関してはUnityProjectFolderから計算。
	/// <param name="targetPath">出力ターゲットFolderパス。Assets/ooooo</param>
	/// <param name="outputPath">出力先。xxxx/oooo</param>
	/// <param name="packageName">パッケージ名。xxxx.unitypackage</param>
	public static bool Create(string targetPath, string outputPath, string packageName)
	{
		if(!targetPath.StartsWith("Assets/")){
			targetPath = "Assets/" + targetPath;
		}
		if(!packageName.EndsWith(".unitypackage")) {
			packageName += ".unitypackage";
		}

		try{
			AssetDatabase.ExportPackage(targetPath, outputPath + "/" + packageName, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
		}catch{
			return false;
		}

		return true;
	}
}

/// <summary>
/// Unity packager window.
/// </summary>
public class UnityPackagerWindow : EditorWindow
{
	/// <summary>
	/// Windowを開く
	/// </summary>
	[MenuItem ("Window/UnityPackager")]
	static void Open () {
		GetWindow<UnityPackagerWindow>();
	}

	const int OneColumnHeight = 16;

	ReorderableList ro;
	Vector2 scrollPos = new Vector2();
	UnityPackagerSetting setting;

	void OnEnable ()
	{
		if(!System.IO.File.Exists(UnityPackagerSetting.createPath)) {
			UnityPackagerSetting.Create();
		}
		setting = AssetDatabase.LoadAssetAtPath<UnityPackagerSetting>(UnityPackagerSetting.createPath);
		ro = new ReorderableList(new List<UnityPackagerSetting.Param>(), typeof(UnityPackagerSetting.Param));
		ro.list = setting.list;
		ro.drawElementCallback = DrawElement;
		ro.elementHeight = 60;
		ro.drawHeaderCallback = rect => GUI.Label(rect, "Package List");
	}

	/// <summary>
	/// 通常シンボルを表示.
	/// </summary>
	void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
	{
		if(0 > index || ro.list.Count <= index) {
			return;
		}
		UnityPackagerSetting.Param current = ro.list[index] as UnityPackagerSetting.Param;
		if(null == current) {
			return;
		}

		float posY = rect.y + 5;

		GUI.color = Color.white;
		GUI.Label(rect, GUIContent.none,EditorStyles.helpBox);

		// TargetPath
		EditorGUI.LabelField(new Rect(rect.x + 10, posY, 80, OneColumnHeight), "Target Path");
		current.targetPath = EditorGUI.TextField(new Rect(rect.x + 21 + 80, posY, rect.width - 110 - 30, OneColumnHeight), current.targetPath);
		GUIContent contentOpen = new GUIContent(EditorGUIUtility.FindTexture("project"));
		if (GUI.Button(new Rect(rect.x + 21 + 80 + rect.width - 100 - 30, posY, 20, 17), contentOpen, EditorStyles.label))
		{
			EditorApplication.delayCall += () => {
				current.targetPath = FileUtil.GetProjectRelativePath(EditorUtility.OpenFolderPanel("Select Target Path.", "", ""));
			};
		}

		// PackageName
		posY += OneColumnHeight;
		EditorGUI.LabelField(new Rect(rect.x + 10, posY, 80, OneColumnHeight), "Package Name");
		current.packageName = EditorGUI.TextField(new Rect(rect.x + 21 + 80, posY, rect.width - 110, OneColumnHeight), current.packageName);

		// CreateButton
		posY += OneColumnHeight;
		if(GUI.Button(new Rect(rect.x + 10, posY, 50, OneColumnHeight), "Create")) {
			EditorApplication.delayCall += () => {
				bool ret = UnityPackager.Create(current.targetPath, setting.outputPath, current.packageName);
				string message = "Error:\n";
				if(ret) {
					message = "Success";
				}else{
					message += current.packageName;
				}
				EditorUtility.DisplayDialog("Result", message, "OK");
				Debug.Log("Message Info : " + message);
			};
		}
	}

	/// <summary>
	/// Window全体の描画
	/// </summary>
	void OnGUI () {
		// Space
		GUILayout.Space(10);

		// OutputPath
		EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
		setting.outputPath = EditorGUILayout.TextField("Output Path",setting.outputPath);
		GUIContent contentOpen = new GUIContent(EditorGUIUtility.FindTexture("project"));
		if (GUILayout.Button(contentOpen, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(17)))
		{
			EditorApplication.delayCall += () => {
				setting.outputPath = FileUtil.GetProjectRelativePath(EditorUtility.OpenFolderPanel("Select Output Path.", "", ""));
			};
		}
		EditorGUILayout.EndHorizontal();

		// Space
		GUILayout.Space(10);

		// List
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		ro.DoLayoutList();
		EditorGUILayout.EndScrollView();

		// AllCreate Button
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("All Create",GUILayout.Width(200))) {
			EditorApplication.delayCall += () => {
				string errorPackageNames = "";
				for(int i = 0; i < setting.list.Count; i++){
					if(!UnityPackager.Create(setting.list[i].targetPath, setting.outputPath, setting.list[i].packageName)){
						errorPackageNames += setting.list[i].packageName + "\n";
					}
				}
				string message = "Error:\n";
				if(string.IsNullOrEmpty(errorPackageNames)) {
					message = "Success";
				}else{
					message += errorPackageNames;
				}
				EditorUtility.DisplayDialog("Result", message, "OK");
				Debug.Log("Message Info : " + message);
			};
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		// Space
		GUILayout.Space(10);
	}
}
#endif