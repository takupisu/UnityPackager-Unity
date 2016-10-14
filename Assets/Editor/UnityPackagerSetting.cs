using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// UnityPackagerの設定ファイル
/// </summary>
public class UnityPackagerSetting : ScriptableObject {

	/// <summary>
	/// 設定ファイル保存パス
	/// </summary>
	public const string createPath = "Assets/Editor/UnityPackagerSetting.asset";

	/// <summary>
	/// 出力パス
	/// </summary>
	public string outputPath;

	/// <summary>
	/// 設定データ群
	/// </summary>
	public List<Param> list = new List<Param>();

	/// <summary>
	/// UnityPackger設定項目
	/// </summary>
	[System.Serializable]
	public class Param{
		/// <summary>
		/// パッケージ名
		/// </summary>
		public string packageName;
		/// <summary>
		/// 対象パス
		/// </summary>
		public string targetPath;
	}
		
	/// <summary>
	/// ScriptableObject作成
	/// </summary>
	public static void Create()
	{
		UnityPackagerSetting asset = CreateInstance<UnityPackagerSetting> ();
		AssetDatabase.CreateAsset (asset, createPath);
		AssetDatabase.Refresh ();
	}
}
