using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SHNameSpace
{
	// --------------------------------------------------------------
	// Window
	// --------------------------------------------------------------
	public class SimpleSelectionHistorySettingsWindow :EditorWindow
	{
		static SimpleSelectionHistorySettingsWindow window = null;

		Vector2 m_scroll;
		GUIStyle m_ButtonStyle = null;
		IMECompositionMode prevMode;
		// bool m_bFileNameOnly = true;

		// --------------------------------------------------------------
		// Back
		[MenuItem("Tools/Simple Selection History/Settings",false, StaticSimpleSelectionMain.Priority + 20)]
		public static void Open()
		{
			EditorWindow.GetWindow<SimpleSelectionHistorySettingsWindow>(true,"Simple Selection History Settings");
		}

		// --------------------------------------------------------------
		void Awake()
		{
			window = this;
			prevMode = Input.imeCompositionMode;
			Input.imeCompositionMode = IMECompositionMode.On;
		}
		// --------------------------------------------------------------
		void OnDestroy()
		{
			window = null;
			Input.imeCompositionMode = prevMode;
		}

		// --------------------------------------------------------------
		static public void Refresh()
		{
			if( window != null )
				window.Repaint();
		}


		// --------------------------------------------------------------
		void OnGUI()
		{
			// if( m_ButtonStyle == null )
			{
				// m_ButtonStyle = new GUIStyle( EditorStyles.miniButtonLeft );
				m_ButtonStyle = new GUIStyle( GUI.skin.button );
				// m_ButtonStyle.fixedHeight = 30;
				m_ButtonStyle.alignment = TextAnchor.MiddleCenter;
				m_ButtonStyle.alignment = TextAnchor.MiddleRight;
			}

			bool needSave = false;

			// --------------------------------------------------------------
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label( GetString(LangString.LangCount) );
			int count = EditorGUILayout.IntSlider( StaticSimpleSelectionMain.HistoryData.historycount, 100 , 1000 );
			if( count !=  StaticSimpleSelectionMain.HistoryData.historycount ){
				needSave = true;
				StaticSimpleSelectionMain.HistoryData.HistoryCount = count;
			}
			EditorGUILayout.EndHorizontal();


			// --------------------------------------------------------------
			bool isAutoRemove = GUILayout.Toggle( StaticSimpleSelectionMain.HistoryData.isAutoRemoveSameFile , GetString(LangString.AutoRemoveHistory) );
			if( isAutoRemove != StaticSimpleSelectionMain.HistoryData.isAutoRemoveSameFile ){
				StaticSimpleSelectionMain.HistoryData.isAutoRemoveSameFile = isAutoRemove;
				needSave = true;
			}

			// --------------------------------------------------------------
			bool withoutHierarchy = GUILayout.Toggle( StaticSimpleSelectionMain.HistoryData.WithoutHierarchy , GetString(LangString.WithoutHierarcy) );
			if( withoutHierarchy != StaticSimpleSelectionMain.HistoryData.WithoutHierarchy ){
				StaticSimpleSelectionMain.HistoryData.WithoutHierarchy = withoutHierarchy;
				needSave = true;
			}

			// --------------------------------------------------------------
			bool ScrollOnlyHistroy = GUILayout.Toggle( StaticSimpleSelectionMain.HistoryData.ScrollOnlyHistroy , GetString(LangString.ScrollOnlyHitory) );
			if( ScrollOnlyHistroy != StaticSimpleSelectionMain.HistoryData.ScrollOnlyHistroy ){
				StaticSimpleSelectionMain.HistoryData.ScrollOnlyHistroy = ScrollOnlyHistroy;
				needSave = true;
			}

			// --------------------------------------------------------------
			bool UpdateWhenSelected = GUILayout.Toggle( StaticSimpleSelectionMain.HistoryData.UpdateWhenSelected , GetString(LangString.UpdateWhenSelected) );
			if( UpdateWhenSelected != StaticSimpleSelectionMain.HistoryData.UpdateWhenSelected ){
				StaticSimpleSelectionMain.HistoryData.UpdateWhenSelected = UpdateWhenSelected;
				needSave = true;
			}

			// --------------------------------------------------------------
			LangageType lang = (LangageType)EditorGUILayout.EnumPopup( GetString(LangString.LangageType) , StaticSimpleSelectionMain.HistoryData.Langage );
			if( lang != StaticSimpleSelectionMain.HistoryData.Langage ){
				StaticSimpleSelectionMain.HistoryData.Langage = lang;
				needSave = true;
				Repaint();
			}

			if( needSave ){
				StaticSimpleSelectionMain.Save();
				SimpleSelectionHistoryWindow.Refresh();
			}
		}

		// --------------------------------------------------------------
		enum LangString{
			LangCount,
			AutoRemoveHistory,
			WithoutHierarcy,
			ScrollOnlyHitory,
			UpdateWhenSelected,
			LangageType,
			Favorite,
		}
		static string GetString( LangString type ){

			switch( StaticSimpleSelectionMain.HistoryData.Langage ){
				case LangageType.English:
					switch(type){
					case LangString.LangCount:						return "History Count";
					case LangString.AutoRemoveHistory:				return "Auto remove same file history";
					case LangString.WithoutHierarcy:				return "Without Hierarchy object";
					case LangString.ScrollOnlyHitory:				return "Scroll only history";
					case LangString.UpdateWhenSelected:				return "Update when selected history";
					case LangString.LangageType:					return "Langage";
					case LangString.Favorite:						return "Favorite";
					}
					break;
				case LangageType.Japanese:
					switch(type){
					case LangString.LangCount:						return "履歴数";
					case LangString.AutoRemoveHistory:				return "同じ履歴は自動で削除する";
					case LangString.WithoutHierarcy:				return "Hierarchyの選択は除外する";
					case LangString.ScrollOnlyHitory:				return "履歴のみスクロールする";
					case LangString.UpdateWhenSelected:				return "選択した時も履歴に追加する";
					case LangString.LangageType:					return "Langage";
					case LangString.Favorite:						return "お気に入り";
					}
					break;
			}
			return null;
		}
	}
}