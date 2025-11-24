using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

namespace SHNameSpace
{
	// --------------------------------------------------------------
	// View
	// --------------------------------------------------------------
	public class SimpleSelectionHistoryWindow :EditorWindow, IHasCustomMenu {

		enum MouseButtonType{
			None,
			Left,
			Right,
			Middle,
			Other,
		}

		static public SimpleSelectionHistoryWindow SelectHistoryWindow = null;
		Vector2 m_scroll;
		GUIStyle m_ButtonStyle = null;
		GUIStyle m_ButtonActiveStyle = null;
		SelectionData activeSelect = null;
		Vector2 mouseDownPos = Vector2.zero;

		GUIContent cont;

		[MenuItem("Tools/Simple Selection History/Show window %&#h",false, StaticSimpleSelectionMain.Priority + 10)]
		static void Open()
		{
			EditorWindow.GetWindow<SimpleSelectionHistoryWindow>(false,"Simple Selection History");
		}

		// --------------------------------------------------------------
		public void AddItemsToMenu (GenericMenu menu)
		{
			menu.AddItem (new GUIContent (GetString(LangString.Setteings)), false, () => {
				SimpleSelectionHistorySettingsWindow.Open();
			});
		}

		// --------------------------------------------------------------
		void OnEnable()
		{
			SelectHistoryWindow = this;
			// EditorApplication.projectChanged
		}

		// --------------------------------------------------------------
		void OnDestroy()
		{
			SelectHistoryWindow = null;
		}

		// --------------------------------------------------------------
		static public void Refresh()
		{
			if( SelectHistoryWindow != null )
				SelectHistoryWindow.Repaint();
		}

		// --------------------------------------------------------------
		SelectionData prevSelData;
		double prevSelTime = 0.0;

		// --------------------------------------------------------------
		void OnGUI()
		{
			// Button style
			if( m_ButtonStyle == null )
			{
				m_ButtonStyle = new GUIStyle( GUI.skin.button );
				m_ButtonStyle.alignment = TextAnchor.MiddleLeft;
				m_ButtonActiveStyle = new GUIStyle( m_ButtonStyle );
				m_ButtonActiveStyle.normal = m_ButtonStyle.active;
			}

			// Load data
			if( StaticSimpleSelectionMain.HistoryData == null )
				StaticSimpleSelectionMain.Load();

			// bool bFileNameOnly = StaticSimpleSelectionMain.s_HistoryData.bFileNameOnly;
			bool needSave = false;

			// --------------------------------------------------------------
			if( cont == null )
				cont = new GUIContent();
			int width = (int)position.width - 40;
			var	HistoryButtonLayout = new GUILayoutOption[]{ GUILayout.Width( width ),GUILayout.Height(20) };

			if( !StaticSimpleSelectionMain.HistoryData.ScrollOnlyHistroy )
				m_scroll = EditorGUILayout.BeginScrollView(m_scroll);

			// --------------------------------------------------------------
			// Filter
			var mask = StaticSimpleSelectionMain.filterMask;
			var extentions = StaticSimpleSelectionMain.FilterExtensions;
			if( extentions.Length > 0 ){
				EditorGUILayout.BeginHorizontal();
				mask = EditorGUILayout.MaskField( GetString(LangString.Filter) , mask , extentions );
				if( StaticSimpleSelectionMain.filterMask != mask ){
					StaticSimpleSelectionMain.SetFilterMask( mask );
				}
				EditorGUILayout.EndHorizontal();
			}

			if( StaticSimpleSelectionMain.HistoryData.ScrollOnlyHistroy )
				m_scroll = EditorGUILayout.BeginScrollView(m_scroll);

			// History
			var history = StaticSimpleSelectionMain.HistoryData.history;
			// StaticSimpleSelectionMain.UpdateViewFillter();
			EditorGUILayout.Space();
			for( int i = history.Count - 1; i >= 0; --i )
			{
				var data = history[i];
				if( !data.IsView )
					continue;

				EditorGUILayout.BeginHorizontal();
				// Current Index
				GUILayout.Label( StaticSimpleSelectionMain.s_curHidtoryIndex == i ? ">" : "" , EditorStyles.miniLabel , GUILayout.Width(10) );

				// Select
				cont.text = data.GetPath();
				cont.image = data.Icon;

				// var rect = GUILayoutUtility.GetRect( cont , data.isActive ? m_ButtonActiveStyle : m_ButtonStyle  , HistoryButtonLayout );
				GUILayout.Label( cont , activeSelect == data ? m_ButtonActiveStyle : m_ButtonStyle , HistoryButtonLayout );
				var rect = GUILayoutUtility.GetLastRect();
				switch( ControllAction( rect , data , false ) ){
					case MouseButtonType.Left:
						// Open Asset
						if( IsPressCtrl() && !data.isGameObject && !data.isPrefab ){
							OpenAction(data);
						}
						else{
							if( prevSelData == data && EditorApplication.timeSinceStartup - prevSelTime < 0.2 ){
								OpenAction(data);
								prevSelData = null;
								prevSelTime = 0;
								break;
							}
							prevSelData = data;
							prevSelTime = EditorApplication.timeSinceStartup;

							var objs = data.GetObjects(true);
							if( StaticSimpleSelectionMain.HistoryData.UpdateWhenSelected ){
								StaticSimpleSelectionMain.s_prevActiveGameObjects = null;
							}else{
								StaticSimpleSelectionMain.s_curHidtoryIndex = i;
								var gos = objs.Where( _ => _ is GameObject ).Select( _ => _ as GameObject ).ToArray();
								StaticSimpleSelectionMain.s_prevActiveGameObjects = gos.Length > 0 ? gos : null;
							}

							StaticSimpleSelectionMain.SelectData(objs);
						}
						break;
				}

				EditorGUILayout.EndHorizontal();
			}

			// --------------------------------------------------------------
			EditorGUILayout.EndScrollView();

			if( activeSelect != null && Event.current.type == EventType.DragUpdated){
				DragAndDrop.visualMode = DragAndDropVisualMode.Move;
			}

			if( needSave )
				StaticSimpleSelectionMain.Save();
		}

		// --------------------------------------------------------------
		void OpenAction( SelectionData selData ){
			if( selData.Identifiers.Length > 1 ){
				GenericMenu menu = new GenericMenu();
				foreach (var path in selData.Paths )
				{
					var filename = System.IO.Path.GetFileName( path );
					menu.AddItem(new GUIContent(filename), false, () => AssetDatabase.OpenAsset( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( path ) ) );
				}
				menu.ShowAsContext();

			}
			else{
				AssetDatabase.OpenAsset( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( selData.Path ) );
			}
		}

		// --------------------------------------------------------------
		MouseButtonType ControllAction( Rect rect, SelectionData selData , bool isFavorite ){
			EventType eventType = Event.current.type;
			// var objectRef = DragAndDrop.objectReferences.Length == 1 ? DragAndDrop.objectReferences[0] : null;
			// var buttonData = DragAndDrop.objectReferences.Length == 1 ? DragAndDrop.objectReferences[0] as DragButtonData : null;
			var mousepos = Event.current.mousePosition;
			switch( eventType )
			{
				case EventType.MouseDown:
					if( rect.Contains(mousepos ) ){
						// Debug.Log("MouseDown");
						activeSelect = selData;
						mouseDownPos = mousepos;
					}
					break;
				case EventType.MouseDrag:
					if( activeSelect == selData && (mouseDownPos-mousepos).sqrMagnitude >= 25 ){
						// Debug.Log("MouseDrag");
						activeSelect = selData;
						DragAndDrop.PrepareStartDrag();

						var objs = new List<UnityEngine.Object>();
						foreach (var path in selData.Paths)
							objs.Add( AssetDatabase.LoadAssetAtPath<UnityEngine.Object>( path ) ) ;
						DragAndDrop.objectReferences = selData.GetObjects();
						DragAndDrop.StartDrag( selData.GetPath() );
						Event.current.Use();
					}
					break;
				case EventType.MouseUp:
					if( activeSelect == selData && rect.Contains(mousepos ) ){
						// Debug.Log("MouseUp");
						activeSelect = null;
						// DragAndDrop.PrepareStartDrag();
						switch( Event.current.button ){
						case 0:	return MouseButtonType.Left;
						case 1:	return MouseButtonType.Right;
						case 2:	return MouseButtonType.Middle;
						default:	return MouseButtonType.Other;
						}
					}
					break;
				case EventType.DragExited:
					if( activeSelect == selData ){
						// Debug.Log("DragExited");
						activeSelect = null;
					}
					break;
				// case EventType.DragPerform:
				// 	if( activeSelect == selData ){
				// 		Debug.Log("DragPerform");
				// 	}
				// 	break;
				case EventType.DragUpdated:
					if( activeSelect == selData ){
                		DragAndDrop.visualMode = DragAndDropVisualMode.Move;
					}
					break;
				default:
					// if( activeSelect == selData ){
					// 	Debug.Log(eventType.ToString());
					// }
					break;
			}
			return MouseButtonType.None;
		}

		//----------------------------------------------------------------------------------------------
		static bool IsPressCtrl(){
#if UNITY_EDITOR_OSX
			return ( Event.current.modifiers & EventModifiers.Command ) != 0;
#else // UNITY_EDITOR_WIN
			return ( Event.current.modifiers & EventModifiers.Control ) != 0;
#endif
		}

		// --------------------------------------------------------------
		enum LangString{
			Filter,
			AddFavorite,
			Delete,
			DontAddFavorite,
			Setteings,
			Open,
			Select,
		}
		static string GetString( LangString type ){

			switch( StaticSimpleSelectionMain.HistoryData.Langage ){
				case LangageType.English:
					switch(type){
					case LangString.Filter:							return "Extension Filter";
					case LangString.AddFavorite:					return "Add Favorite";
					case LangString.Delete:							return "Delete";
					case LangString.Setteings:						return "Settings";
					case LangString.Open:							return "Open";
					case LangString.Select:							return "Select";
					}
					break;
				case LangageType.Japanese:
					switch(type){
					case LangString.Filter:							return "フィルター";
					case LangString.AddFavorite:					return "お気に入りに追加";
					case LangString.Delete:							return "削除";
					case LangString.Setteings:						return "設定";
					case LangString.Open:							return "開く";
					case LangString.Select:							return "選択";
					}
					break;
			}
			return null;
		}
	}
}