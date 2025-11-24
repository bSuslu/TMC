#if UNITY_2018_3_OR_NEWER
#define USE_PREFAB_MODE
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;
#if USE_PREFAB_MODE

#endif

namespace SHNameSpace
{
	// **************************************************************
	[InitializeOnLoad]
	public class StaticSimpleSelectionMain
	{
		public static int			s_curHidtoryIndex 	= 0;
		public static GameObject[]	s_prevActiveGameObjects	= null;

		public static HistoryData	HistoryData{
			get{ return HistoryDataInstance.instance.historyData;}
			set{ HistoryDataInstance.instance.historyData = value;}
		}

		static SelectionData CurSelectData{
			get{
				return HistoryData.history[s_curHidtoryIndex];
			}
		}

		public static int	filterMask{
			get{ return HistoryDataInstance.instance.filterMask;}
			set{ HistoryDataInstance.instance.filterMask = value;}
		}

		public static int  filterMaskEverything{
			get {
				int ret = 0;
				int i = 0;
				foreach (var filter in HistoryData.filterDatas)
				{
					ret |= 1<<i;
					i++;
				}
				return ret;
			}
		}

		public static string[]	_FilterExtensions;
		public static string[]	FilterExtensions{
			get{
				if( _FilterExtensions == null )
					UpdateFilter();
				return _FilterExtensions;
			}
		}

		// --------------------------------------------------------------
		static StaticSimpleSelectionMain()
		{
			Load();
			Selection.selectionChanged += SelectionChanged;
			EditorApplication.update += EditorUpdate;
			EditorApplication.projectChanged += EditorProjectChanged;
		}

		// --------------------------------------------------------------
		static public void ClearHistory()
		{
			HistoryData.history.Clear();
		}


		// --------------------------------------------------------------
		public static void Save()
		{
			HistoryData.Save();
		}

		// --------------------------------------------------------------
		public static void Load()
		{
			if( HistoryData != null )
				return;
			HistoryData = HistoryData.Load();
			UpdateViewFillter();
		}

		// --------------------------------------------------------------
		static void EditorUpdate()
		{
			if( HistoryData == null )
				return;
			SelectionData findItem;

			// Check path
			findItem = HistoryData.history.FindLast( _ => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( _.Path ) == null );
			if( findItem != null ){
				findItem.Refresh(false);
				if( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( findItem.Path ) == null ){
					HistoryData.history.Remove(findItem);
				}
				SimpleSelectionHistoryWindow.Refresh();
				return;
			}

			// Refresh icon
			findItem = HistoryData.history.FindLast( _ => _.Icon == null );
			if( findItem != null ){
				findItem.RefreshIcon();
				SimpleSelectionHistoryWindow.Refresh();
				return;
			}

			EditorApplication.update -= EditorUpdate;
		}

		// **************************************************************
		static void EditorProjectChanged(){
			EditorApplication.update -= EditorUpdate;
			EditorApplication.update += EditorUpdate;
		}

		// --------------------------------------------------------------
		static void SelectionChanged()
		{
			if( Selection.activeObject == null )
				return;
			// if( Selection.activeTransform != null )
			// 	return;

			if( Selection.activeTransform != null || Selection.assetGUIDs.Length > 0 )
			{
				if( HistoryData == null )
					Load();

				if( s_curHidtoryIndex < 0 || s_curHidtoryIndex >= HistoryData.history.Count )
					s_curHidtoryIndex = HistoryData.history.Count - 1;

				// Operate the history when it is different from the current selection
				bool isAddHistory = HistoryData.history.Count == 0;
				if( !isAddHistory ){
					if( Selection.activeTransform == null ){
						isAddHistory = !HistoryData.history[s_curHidtoryIndex].IsSame( Selection.assetGUIDs );
					}else{
						if( !HistoryData.WithoutHierarchy ){
							// bool changed = s_prevActiveGameObjects == null || Selection.gameObjects == null;
							// if( !changed )
							// 	changed = s_prevActiveGameObjects.Length != Selection.gameObjects.Length;

							// if( !changed ){
							// 	List<GameObject> list = new List<GameObject>( s_prevActiveGameObjects );
							// 	foreach (var go in Selection.gameObjects){
							// 		var index = list.FindIndex( _ => _ == go );
							// 		if( index >= 0 )
							// 			list.RemoveAt( index );
							// 	}
							// 	changed = list.Count != 0;
							// }
							if( s_prevActiveGameObjects != Selection.gameObjects ){
								SelectionData curSel = null;
								if( s_curHidtoryIndex >= 0 && s_curHidtoryIndex < HistoryData.history.Count )
									curSel = HistoryData.history[s_curHidtoryIndex];
								if( curSel != null ){
									if( !curSel.IsSame( Selection.gameObjects.Select(_ => GetHierarchyName(_.transform) ).ToArray() ) )
										isAddHistory = true;
								}else{
									isAddHistory = true;
								}
							}
						}
					}
				}

				// Add history
				if( isAddHistory )
				{
					s_prevActiveGameObjects = null;
					if( Selection.activeTransform != null )
						s_prevActiveGameObjects = Selection.gameObjects;
					var identifiers = Selection.assetGUIDs==null || Selection.assetGUIDs.Length==0 ? null : new string[]{Selection.assetGUIDs[0]};
					if( s_prevActiveGameObjects != null ){
						identifiers = s_prevActiveGameObjects.Select(_ => GetHierarchyName(_.transform) ).ToArray();
					}

					// Change the Index if it is the same as the last selection
					var last = HistoryData.history.Count > 0　? HistoryData.history[HistoryData.history.Count - 1] : null;
					if( HistoryData.history.Count > 0 && last.IsMultiAddSub(identifiers) )
					{
						s_curHidtoryIndex = HistoryData.history.Count - 1;
						if( !last.IsSame(identifiers) ){
							last.SetIdentifiers( identifiers );
						}
					}
					// Add history
					else
					{
						// Add Filter
						var Path = AssetDatabase.GUIDToAssetPath( identifiers[0] );
						var ext = System.IO.Path.GetExtension( Path );
						CheckAddFilter( ext );

						// Remove same history
						if( HistoryData.isAutoRemoveSameFile ){
							for( int i = HistoryData.history.Count - 1; i >= 0; --i )
							{
								if( HistoryData.history[i].IsSame( identifiers ) ){
									HistoryData.history.RemoveAt(i);
								}
							}
						}

						// Over history count
						if( HistoryData.history.Count > HistoryData.historycount ){
							HistoryData.history.RemoveRange(0,HistoryData.history.Count-HistoryData.historycount);
						}
						var newSel = new SelectionData(identifiers);
						newSel.IsView =  filterMask == 0 ? true : HistoryData.filterDatas.Any( x => x.Enabled && x.extensionHash == newSel.ExtensionHash );
						HistoryData.history.Add( newSel );
						s_curHidtoryIndex = HistoryData.history.Count - 1;
						Save();
					}
				}
				SimpleSelectionHistoryWindow.Refresh();
			}
		}

		// --------------------------------------------------------------
		static string GetHierarchyName(Transform trn){
			string ret = trn.name;
			if( trn.parent != null )
				return GetHierarchyName( trn.parent ) + "/" + trn.name;
			else{
#if USE_PREFAB_MODE
				if( UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null ){
					return "*" + trn.name;
				}
				else
#endif
				{
					return "/" + trn.name;
				}
			}
		}

		// --------------------------------------------------------------
		public static Texture GetIcon( IconType type ){
			const string IconPathConfigName = "SimpleSelectionHistoryIconsPath";
			var icons = HistoryDataInstance.instance.icons;
			// get icons tex
			if( icons.Count == 0 ){
				string iconsFolder = UnityEditor.EditorUserSettings.GetConfigValue(IconPathConfigName);
				if( !Directory.Exists(iconsFolder)){
					// get icon directory path
					string appPath = Application.dataPath;
					string[] dirs = Directory.GetDirectories( appPath , "*Icons" , System.IO.SearchOption.AllDirectories  );
					foreach( string dir in dirs )
					{
						string replaceDir = dir.Replace( Application.dataPath , "Assets" ).Replace("\\","/");
						if( replaceDir.EndsWith( "SimpleSelectionHistory/Icons" ) )
						{
							iconsFolder = replaceDir;
							UnityEditor.EditorUserSettings.SetConfigValue (IconPathConfigName,iconsFolder);
							break;
						}
					}
				}
				// get tex
				if( !string.IsNullOrEmpty(iconsFolder) ){
					foreach(IconType item in System.Enum.GetValues(typeof(IconType)) )
					{
						Texture tex = AssetDatabase.LoadAssetAtPath(iconsFolder + "/" + item.ToString() + ".png" , typeof(Texture) ) as Texture;
						icons.Add( item , tex );
					}
				}
			}
			return icons[type];
		}
		// --------------------------------------------------------------
		static Object pingObj;
		static public void SelectData( params Object[] objs ){
			if( objs != null ){
				Selection.objects = objs;
				pingObj = objs[0];
				UnityEditor.EditorApplication.delayCall += PingObject;
			}
		}
		static void PingObject(){
			EditorGUIUtility.PingObject( pingObj );
			UnityEditor.EditorApplication.delayCall -= PingObject;
		}

		// --------------------------------------------------------------
		static void CheckAddFilter( string ext ){
			var hash = ext.GetHashCode();
			if( !HistoryData.filterDatas.Any( x => x.extensionHash == hash )){
				HistoryData.filterDatas.Add( new FilterData( ext, filterMask == -1 ) );
				UpdateFilter();
			}
		}

		// --------------------------------------------------------------
		static public void SetFilterMask( int mask ){
			int i = 0;
			foreach (var filter in HistoryData.filterDatas)
			{
				filter.Enabled = (mask & (1<<i)) != 0;
				i++;
			}
			UpdateViewFillter();
		}

		// --------------------------------------------------------------
		static public void UpdateFilter(){
			var filterDatas = HistoryData.filterDatas;

			filterMask = 0;
			_FilterExtensions = new string[filterDatas.Count];

			int i = 0;
			bool isAll = true;
			foreach (var filter in filterDatas)
			{
				isAll &= filter.Enabled;
				if( filter.Enabled )
					filterMask += 1<<i;
				_FilterExtensions[i] = string.IsNullOrEmpty(filter.extension) ? "[No Extension]" : filter.extension;
				i++;
			}
			if( isAll )
				filterMask = -1;
		}

		// --------------------------------------------------------------
		static public void UpdateViewFillter(){
			UpdateFilter();

			var history = HistoryData.history;
			var fillterEnable = filterMask != 0;
			foreach (var data in history)
			{
				if( fillterEnable ){
					data.IsView = HistoryData.filterDatas.Any( x => x.Enabled && x.extensionHash == data.ExtensionHash );
				}else{
					data.IsView = true;
				}
			}
		}

		public const int Priority = 0;
#region Shortcut
		// --------------------------------------------------------------
		// Shortcut
		// --------------------------------------------------------------

		// --------------------------------------------------------------
		// Back
		[MenuItem("Tools/Simple Selection History/Back %&,",false, Priority + 11)]
		static void PrevSelect()
		{
			if( s_curHidtoryIndex > 0 )
			{
				s_curHidtoryIndex--;
				SelectData(CurSelectData.GetObjects());
				SimpleSelectionHistoryWindow.Refresh();
			}
		}

		// --------------------------------------------------------------
		// Forward
		[MenuItem("Tools/Simple Selection History/Forward %&.",false, Priority + 12)]
		static void ForwardSelect()
		{
			if( s_curHidtoryIndex < HistoryData.history.Count - 1 )
			{
				s_curHidtoryIndex++;
				SelectData(CurSelectData.GetObjects());
				SimpleSelectionHistoryWindow.Refresh();
			}
		}
#endregion Shortcut
	}
}
