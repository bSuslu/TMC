#if UNITY_2018_3_OR_NEWER
#define USE_PREFAB_MODE
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System.IO;
#if USE_PREFAB_MODE

#endif

namespace SHNameSpace
{
	public enum IconType
	{
		Hierarchy,
		Prefab,
	}

	public enum LangageType{
		English 	= 0,
		Japanese 	= 1,
	}

	// **************************************************************
	[System.Serializable]
	public class SelectionData{
		[SerializeField] private string		identifier = null;
		[SerializeField] private string[]	identifiers;

		private string	Identifier{ get{ return identifiers[0]; }}
		public string[]	Identifiers{ get{ return identifiers; }}
		public string 	Path{ get; private set; }
		public string[] Paths{ get; private set; }
		public string 	FileName{ get; private set; }
		public Texture	Icon{ get; private set; }
		public bool		IsView{ get; set; }
		public string 	Extension{ get; private set; }
		public int		ExtensionHash{ get; private set; }
		public bool 	isActive{get;set;}
		public bool		isGameObject{ get{ return Identifier.StartsWith("/");}}
		public bool		isPrefab{ get{ return Identifier.StartsWith("*");}}

		public SelectionData( params string[] _identifiers )
		{
			identifiers = _identifiers;
			Refresh(true);
		}

		public bool IsSame( params string[] _identifiers ){
			if( _identifiers.Length != identifiers.Length )
				return false;

			List<string> list  = new List<string>(identifiers);
			foreach (var i in _identifiers)
			{
				var index = list.FindIndex( _ => _ == i );
				if( index >= 0 )
					list.RemoveAt(index);
				else
					return false;
			}
			return list.Count == 0;
		}

		public bool IsMultiAddSub( params string[] setIdentifiers ){
			if( identifiers.Length == 1 || setIdentifiers.Length == 1 )
				return false;

			var list = identifiers.Where( _ => setIdentifiers.Any( __ => _ == __ ) );
			return list.Count() == identifiers.Length || list.Count() == setIdentifiers.Length;
		}

		public void SetIdentifiers( params string[] setIdentifiers ){
			List<string> n = new List<string>(setIdentifiers);
			if( n.Any( _ => _ == Identifier ) ){
				n.Remove(Identifier);
				n.Insert(0,Identifier);
			}
			identifiers = n.ToArray();
			Refresh(true);
		}

		public void Refresh( bool refreshIcon ){
			if( !string.IsNullOrEmpty( identifier ) && ( identifiers==null || identifiers.Length == 0 ) ){
				identifiers = new string[]{ identifier };
				identifier = null;
			}

			if( isGameObject ){
				Path = Identifier;
				FileName = System.IO.Path.GetFileName( Path );
			}else if( isPrefab ){
				Path = Identifier;
				FileName = System.IO.Path.GetFileName( Path.Substring(1) );
			}else{
				Path = AssetDatabase.GUIDToAssetPath( Identifier );
				FileName = System.IO.Path.GetFileName( Path );
			}
			if( refreshIcon )
				RefreshIcon();

			Paths = new string[identifiers.Length];
			for( int i = 0; i < identifiers.Length; ++i )
			{
				if( isGameObject ){
					Paths[i] = identifiers[i];
				}else if( isPrefab ){
					Paths[i] = identifiers[i];
				}else{
					Paths[i] = AssetDatabase.GUIDToAssetPath( identifiers[i] );
				}
			}
			Extension = System.IO.Path.GetExtension( Path );
			ExtensionHash = Extension.GetHashCode();
		}

		public void RefreshIcon(){
			if( isGameObject ){
				Icon = StaticSimpleSelectionMain.GetIcon(IconType.Hierarchy);
			}else if( isPrefab ){
				Icon = StaticSimpleSelectionMain.GetIcon(IconType.Prefab);
			}else{
				if( !string.IsNullOrEmpty( Path )  )
				Icon = AssetPreview.GetMiniThumbnail( AssetDatabase.LoadMainAssetAtPath( Path ) );
			}
		}

		public string GetPath( bool bFileNameOnley = true )
		{
			string str = bFileNameOnley ? FileName : Path;
			if( identifiers != null && identifiers.Length > 1 )
				return "["+identifiers.Length+"]" + str;
			return str;
		}

		public Object[] GetObjects( bool showLog = false ){
			if( identifier != null ){
				Refresh(false);
			}

			if( isGameObject ){
#if USE_PREFAB_MODE
				var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
				if( stage != null ){
					if( showLog ) Debug.LogWarning("SimpleSelectionHistory: Don't Find GameObject! Not Prefab Mode!!["+ GetPath() + "]" );
				}
				else
#endif
				{
					List<GameObject> gos = new List<GameObject>();
					foreach(var path in Paths ){
						var go = FindGameObject(path);
						if( go )
							gos.Add( go );
					}
					if( gos.Count > 0 )
						return gos.ToArray();
					if( showLog ) Debug.LogWarning("SimpleSelectionHistory: Don't Find GameObject! ["+ GetPath() + "]" );
				}
			}
#if USE_PREFAB_MODE
			else if( isPrefab ){
				var stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
				if( stage != null ){
					List<GameObject> gos = new List<GameObject>();
					foreach(var path in Paths ){
						var s = Path.Substring(1);
						if( s == stage.prefabContentsRoot.name )
							gos.Add( stage.prefabContentsRoot );
						else if( s.StartsWith( stage.prefabContentsRoot.name ) ){
							s = s.Substring( 1 + stage.prefabContentsRoot.name.Length );
							var findTrans = stage.prefabContentsRoot.transform.Find( s );
							if( findTrans != null )
								gos.Add( findTrans.gameObject );
						}
					}
					if( gos.Count > 0 )
						return gos.ToArray();
					if( showLog ) Debug.LogWarning("SimpleSelectionHistory: Don't Find GameObject! Not in this prefab. ["+ Path.Substring(1) + "]" );
				}else{
					if( showLog ) Debug.LogWarning("SimpleSelectionHistory: Don't Find GameObject! This is in Prefab Mode. ["+ Path.Substring(1) + "]" );
				}
			}
#endif
			else{
				var objs = new List<Object>();
				foreach (var path in Paths)
					objs.Add( AssetDatabase.LoadAssetAtPath<Object>( path ) );
				// if( objs.Any(_=>_ == null ) ){
				// 	Refresh(false);
				// 	foreach (var path in Paths)
				// 		objs.Add( AssetDatabase.LoadAssetAtPath<Object>( path ) );
				// }
				return objs.ToArray();
			}
			return null;
		}

		// --------------------------------------------------------------
		static GameObject FindGameObject( string name ){
			var ret = GameObject.Find( name );
			if( ret != null )
				return ret;

			List<GameObject> roots = new List<GameObject>();
			for( int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; ++i )
			{
				var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
				roots.AddRange(scene.GetRootGameObjects());
			}

			bool isRootSearch = name.StartsWith("/");
			if( isRootSearch )
				name = name.Substring( 1 );
			int hierarchyCount = name.Count(_ => _ == '/' );
			var parentName = hierarchyCount > 0 ? name.Substring(0,name.IndexOf('/')) : name;
			var childName = hierarchyCount > 0 ? name.Substring( name.IndexOf('/')+1 ) : null;

			for( int i = 0; ret == null && i < roots.Count; ++i )
			{
				var t = FindTransform( roots[i].transform, parentName , childName , !isRootSearch );
				if( t != null ){
					ret = t.gameObject;
					break;
				}
			}
			return ret;
		}

		// --------------------------------------------------------------
		static Transform FindTransform( Transform t , string parentName , string childName , bool sarchChild )
		{
			if( t.name == parentName ){
				if( childName == null )
					return t;
				else{
					var trn = t.Find(childName);
					if( trn != null )
						return trn;
				}
			}
			if( sarchChild ){
				foreach (Transform child in t)
				{
					var trn = FindTransform( child , parentName , childName , sarchChild );
					if( trn != null )
						return trn;
				}
			}
			return null;
		}
	}

	// **************************************************************
	public class HistoryDataInstance : ScriptableSingleton<HistoryDataInstance>{
		public HistoryData	historyData 		= null;
		public Dictionary<IconType, Texture> icons 	= new Dictionary<IconType, Texture>();
		public int filterMask;
	}

	// **************************************************************
	// Saved Classes
	[System.Serializable]
	public class FilterData{
		public string				extension = "";
		public int					_enabled;

		private int _extensionHash = 0;
		public int 	extensionHash{ get{ return _extensionHash != 0 ? _extensionHash : ( _extensionHash = extension.GetHashCode() ); }}

		public bool Enabled{ get{ return _enabled != 0;} set { _enabled = value ? 1 : 0; }}

		public FilterData( string e , bool enabled ){
			extension = e;
			this._enabled = enabled ? 1 : 0;
		}
	}


	[System.Serializable]
	public class HistoryData
	{
		const string EditorUserSettings  = "SelectionHistory";

		public int 					historycount 		= 200;
		public List<FilterData>		filterDatas 		= new List<FilterData>();
		public List<SelectionData> 	history				= new List<SelectionData>();
		public int					autoRemoveSameFile	= 1;
		public int					withoutHierarchy	= 1;
		public int					scrollOnlyHistory	= 1;
		public int					updateWhenSelected	= 0;
		public int					langage				= 0;

		// public bool FilterEnable{ get { return filterEnable != 0; } set{ filterEnable = value ? 1 : 0; } }

		public bool isAutoRemoveSameFile{
			get { return autoRemoveSameFile != 0; }
			set {
				autoRemoveSameFile = value ? 1 : 0;
				if( value ){
					List<SelectionData> newHistory = new List<SelectionData>(history.Count);
					for( int i = history.Count - 1; i >= 0; --i )
					{
						var item = history[i];
						if( !newHistory.Any( x => x.IsSame( item.Identifiers ) ) )
							newHistory.Insert(0,item);
					}
					history = newHistory;
				}
			}
		}

		public bool WithoutHierarchy{
			get { return withoutHierarchy != 0; }
			set{
				withoutHierarchy = value ? 1 : 0;
				if( value ){
					for( int i = history.Count - 1; i >= 0; --i )
					{
						var item = history[i];
						if( item.isGameObject || item.isPrefab )
							history.RemoveAt(i);
					}
				}
			}
		}

		public bool ScrollOnlyHistroy{
			get{ return scrollOnlyHistory != 0; }
			set{ scrollOnlyHistory = value ? 1 : 0; }
		}

		public bool UpdateWhenSelected{
			get{ return updateWhenSelected != 0; }
			set{ updateWhenSelected = value ? 1 : 0; }
		}

		public LangageType Langage{
			get{ return (LangageType)langage; }
			set{ langage = (int)value; }
		}
		// --------------------------------------------------------------
		public int HistoryCount{ set {
			historycount = value;
			if( history.Count > historycount ){
				history.RemoveRange( 0 , history.Count - historycount );
			}
		} }
		// --------------------------------------------------------------
		public void Save(){

			string str = JsonUtility.ToJson( this );
			UnityEditor.EditorUserSettings.SetConfigValue (EditorUserSettings, str );
		}

		// --------------------------------------------------------------
		public static HistoryData Load()
		{
			string json = UnityEditor.EditorUserSettings.GetConfigValue (EditorUserSettings);
			if( string.IsNullOrEmpty(json) ){
				return new HistoryData();
			}else{
				var ret = JsonUtility.FromJson<HistoryData>( json );
				ret.Refresh(true);
				return ret;
			}
		}

		// --------------------------------------------------------------
		private void Refresh(bool refreshIcon){

			for( int i = history.Count - 1; i >= 0; --i )
			{
				var item = history[i];
				item.Refresh(refreshIcon);
				if( string.IsNullOrEmpty( item.Path ) )
					history.RemoveAt(i);
				else
					item.IsView = filterDatas.Count == 0 ? true : filterDatas.Any( x => x.Enabled && x.extensionHash == item.ExtensionHash );
			}
		}

		// --------------------------------------------------------------
		public void SetupDefaultFilter(){
			filterDatas.Add( new FilterData( "Scene" , false ));
		}
	}
}
