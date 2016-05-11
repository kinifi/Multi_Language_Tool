using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Audio;
using Object = UnityEngine.Object;

// Item

public class M10NStringTableListViewNode : TreeViewItem
{
	public int stringTableIndex { get; set; }

	public M10NStringTableListViewNode(int instanceID, int depth, TreeViewItem parent, string displayName, int stringTableIndex)
		: base(instanceID, depth, parent, displayName)
	{
		this.stringTableIndex = stringTableIndex;
	}
}

// Dragging
// We want dragging to work from the mixer window to the inspector like the project browser, but also want
// custom dragging behavior (reparent the group sub assets) so we derive from AssetOrGameObjectTreeViewDragging
// and override DoDrag.

//public class AudioGroupTreeViewDragging : AssetsTreeViewDragging
//{
//	private AudioMixerGroupTreeView m_owner;
//
//	public AudioGroupTreeViewDragging (TreeView treeView, AudioMixerGroupTreeView owner)
//		: base (treeView)
//	{
//		m_owner = owner;
//	}
//
//	public override void StartDrag (TreeViewItem draggedItem, List<int> draggedItemIDs)
//	{
//		if (!EditorApplication.isPlaying)
//			base.StartDrag (draggedItem, draggedItemIDs);
//	}
//
//	public override DragAndDropVisualMode DoDrag (TreeViewItem parentNode, TreeViewItem targetNode, bool perform, DropPosition dragPos)
//	{
//		var insertAfterGroupNode = targetNode as M10NStringTableListViewNode;
//		var parentGroupNode = parentNode as M10NStringTableListViewNode;
//		var draggedGroups = new List<Object> (DragAndDrop.objectReferences).OfType<AudioMixerGroupController> ().ToList ();
//		if (parentGroupNode != null && draggedGroups.Count > 0)
//		{
//			var draggedIDs = (from i in draggedGroups select i.GetInstanceID ()).ToList ();
//			bool validDrag = ValidDrag (parentNode, draggedIDs) && !AudioMixerController.WillModificationOfTopologyCauseFeedback (m_owner.Controller.GetAllAudioGroupsSlow (), draggedGroups, parentGroupNode.group, null);
//			if (perform && validDrag)
//			{
//				AudioMixerGroupController parentGroup = parentGroupNode.group;
//				m_owner.Controller.ReparentSelection (parentGroup, insertAfterGroupNode.group, draggedGroups);
//				m_owner.ReloadTree ();
//				m_TreeView.SetSelection (draggedIDs.ToArray (), true); // Ensure dropped item(s) are selected and revealed (fixes selection if click dragging a single item that is not selected when drag was started)
//			}
//			return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.Rejected;
//		}
//		return DragAndDropVisualMode.None;
//	}
//
//	bool ValidDrag (TreeViewItem parent, List<int> draggedInstanceIDs)
//	{
//		TreeViewItem currentParent = parent;
//		while (currentParent != null)
//		{
//			if (draggedInstanceIDs.Contains (currentParent.id))
//				return false;
//			currentParent = currentParent.parent;
//		}
//		return true;
//	}
//}

// Datasource

public class M10NStringTableDataSource : TreeViewDataSource
{

	private M10NStringDatabase m_db;
//	public SystemLanguage m_targetLanguage;
	private List<TreeViewItem> m_items;

	public M10NStringTableDataSource(TreeView treeView, M10NStringDatabase db)
		: base(treeView)
	{
		m_db = db;
//		m_targetLanguage = targetLanguage;
		m_items = new List<TreeViewItem>();
	}

	public M10NStringDatabase database {
		get {
			return m_db;
		}
		set {
			m_db = value;
		}
	}

	private void AddAllNodes()
	{
		var newRoot = new TreeViewItem(-1, -1, null, "Root");
		for (int i = 0; i < m_db.keys.Count; ++i)
		{
//			Debug.Log("[M10N LV]added child " + i + ":" + m_db.keys[i]);
			//int uniqueNodeID = GetUniqueNodeID();
			var node = new M10NStringTableListViewNode(i, 0, m_RootItem, m_db.keys[i], i);
			newRoot.AddChild(node);
		}

		m_RootItem = newRoot;
	}

//	static public int GetUniqueNodeID (AudioMixerGroupController group)
//	{
//		return group.GetInstanceID(); // alternative: group.groupID.GetHashCode();
//	}

	public override void FetchData()
	{
		if (m_db == null)
		{
			if(m_items.Count > 0) {
				m_items.Clear();
			}
			return;
		}

		AddAllNodes();
		m_NeedRefreshRows = true;
	}

	public override bool IsRenamingItemAllowed(TreeViewItem node)
	{
//		var audioNode = node as M10NStringTableListViewNode;
//		if (audioNode.group == m_Controller.masterGroup)
//			return false;
//
//		return true;
		return false;
	}
}

// Node GUI

public class M10NStringTableListViewGUI : TreeViewGUI
{
	readonly float column1Width = 0f;
	readonly Texture2D k_VisibleON = EditorGUIUtility.FindTexture ("VisibilityOn");

	//public Action<M10NStringTableListViewNode, bool> NodeWasToggled;
	//public AudioMixerController m_Controller = null;
	public M10NStringDatabase m_db;
	public SystemLanguage currentEditingLanguage;

	public M10NStringTableListViewGUI(TreeView treeView, M10NStringDatabase db)
		: base(treeView)
	{
		m_db = db;
		k_BaseIndent = column1Width;
		k_IconWidth = 0;
		k_TopRowMargin = k_BottomRowMargin = 2f;
	}

//	void OpenGroupContextMenu (M10NStringTableListViewNode audioNode, bool visible)
//	{
//		GenericMenu menu = new GenericMenu ();
//
//		if (NodeWasToggled != null)
//		{
//			menu.AddItem (new GUIContent (visible ? "Hide group" : "Show Group"), false, () => NodeWasToggled (audioNode, !visible));
//		}
//		menu.AddSeparator (string.Empty);
//
//		AudioMixerGroupController[] groups;
//		if (m_Controller.CachedSelection.Contains (audioNode.group))
//			groups = m_Controller.CachedSelection.ToArray ();
//		else
//			groups = new AudioMixerGroupController[] { audioNode.group };
//		
//		AudioMixerColorCodes.AddColorItemsToGenericMenu (menu, groups);
//		menu.ShowAsContext ();
//	}

	protected override void DrawIconAndLabel (Rect rect, TreeViewItem item, string label, bool selected, bool focused, bool useBoldFont, bool isPinging)
	{
		Rect keyRect = rect;
		Rect valueRect = rect;
		keyRect.xMax = rect.width / 2.0f;
		valueRect.xMin += rect.width / 2.0f;

		base.DrawIconAndLabel (keyRect, item, label, selected, focused, useBoldFont, isPinging);

		var stringTable = item as M10NStringTableListViewNode;
		if (stringTable != null && m_db != null)
		{
			string valueString = m_db.GetStringTable(currentEditingLanguage).values[stringTable.stringTableIndex].text;
			GUI.Label(valueRect, valueString);
		}
	}

//	override public void OnRowGUI (Rect rowRect, TreeViewItem node, int row, bool selected, bool focused)
//	{
//		Event evt = Event.current;
//		DoItemGUI(rowRect, row, node, selected, focused, false);
//	}
//
//	protected override Texture GetIconForItem(TreeViewItem node)
//	{
//		if (node != null && node.icon != null)
//			return node.icon;
//		return null;
//	}

//	protected override void SyncFakeItem()
//	{
//	}

	protected override void RenameEnded()
	{
		bool userAccepted = GetRenameOverlay().userAcceptedRename;
		if (userAccepted)
		{
			string name = string.IsNullOrEmpty(GetRenameOverlay().name) ? GetRenameOverlay().originalName : GetRenameOverlay().name;
			int instanceID = GetRenameOverlay().userData;
			var stringTableNode = m_TreeView.FindItem(instanceID) as M10NStringTableListViewNode;
			if (stringTableNode != null)
			{
				Assert.IsNotNull(m_db);
				m_db.keys[instanceID] = name;
//				ObjectNames.SetNameSmartWithInstanceID(instanceID, name);
//				foreach (var effect in audioNode.group.effects)
//					effect.ClearCachedDisplayName ();
				m_TreeView.ReloadData();
//				if (m_Controller != null)
//					m_Controller.OnSubAssetChanged ();
			}
		}
	}
}


// TreeView

//public class M10NStringTablePopupContext
//{
//	public M10NStringTablePopupContext(AudioMixerController controller, AudioMixerGroupController group)
//	{
//		this.controller = controller;
//		this.groups = new AudioMixerGroupController[] { group };
//	}
//
//	public M10NStringTablePopupContext(AudioMixerController controller, AudioMixerGroupController[] groups)
//	{
//		this.controller = controller;
//		this.groups = groups;
//	}
//
//	public AudioMixerController controller;
//	public AudioMixerGroupController[] groups;
//}

public class M10NStringTableListView
{
	private MultiLanguage m_editorWindow;
	private M10NStringDatabase m_db;
	private M10NStringTableDataSource m_StringTableDataSource;
	private TreeViewState m_StringTableTreeState;
	private TreeView m_StringTableTree;
	private int m_TreeViewKeyboardControlID;
	private M10NStringTableListViewGUI m_StringTableListViewGUI;
	private int m_ScrollToItem;

	class Styles
	{
		public GUIStyle optionsButton = "PaneOptions";
		public GUIContent header = new GUIContent ("Groups", "An Audio Mixer Group is used by e.g Audio Sources to modify the audio output before it reaches the Audio Listener. An Audio Mixer Group will route its output to another Audio Mixer Group if it is made a child of that group. The Master Group will route its output to the Audio Listener if it doesn't route its output into another Mixer.");
		public GUIContent addText = new GUIContent ("+", "Add child group");
		public Texture2D audioMixerGroupIcon = EditorGUIUtility.FindTexture ("AudioMixerGroup Icon");
	}
	static Styles s_Styles;

	public M10NStringTableListView (MultiLanguage editorWindow, TreeViewState treeState, M10NStringDatabase db)
	{
		m_editorWindow = editorWindow;
		m_db = db;
		m_StringTableTreeState = treeState;

		m_StringTableTree = new TreeView(editorWindow, m_StringTableTreeState);
		m_StringTableTree.deselectOnUnhandledMouseDown = false;
		m_StringTableTree.selectionChangedCallback += OnTreeSelectionChanged;
//		m_StringTableTree.contextClickItemCallback += OnTreeViewContextClick;
//		m_StringTableTree.expandedStateChanged += SaveExpandedState;
		
		m_StringTableListViewGUI = new M10NStringTableListViewGUI(m_StringTableTree, m_db);
//		m_StringTableListViewGUI.NodeWasToggled += OnNodeToggled;

		m_StringTableDataSource = new M10NStringTableDataSource(m_StringTableTree, m_db);
		m_StringTableTree.Init(editorWindow.position,
			m_StringTableDataSource, m_StringTableListViewGUI,
			//new AudioGroupTreeViewDragging(m_AudioGroupTree, this)
			null // no dragging allowed
			);
		m_StringTableDataSource.showRootItem = false;
		m_StringTableTree.ReloadData();
	}

	public void UseScrollView(bool useScrollView)
	{
		m_StringTableTree.SetUseScrollView(useScrollView);
	}

	public void ReloadTreeData ()
	{
		m_StringTableTree.ReloadData ();
	}

	public void ReloadTree()
	{
		m_StringTableTree.ReloadData();
//		if (m_Controller != null)
//		{
//			m_Controller.SanitizeGroupViews ();
//			m_Controller.OnSubAssetChanged ();
//		}
	}

	public void SelectItemForKey(string key) {
		if( m_db ) {
			int[] sel = new int[1];
			sel[0] = m_db.keys.IndexOf(key);
			m_StringTableTree.SetSelection(sel, true, true);
			m_StringTableTree.NotifyListenersThatSelectionChanged();
		}
	}

//	public void AddChildGroupPopupCallback(object obj)
//	{
//		AudioMixerGroupPopupContext context = (AudioMixerGroupPopupContext)obj;
//		if (context.groups != null && context.groups.Length > 0)
//			AddAudioMixerGroup(context.groups[0]);
//	}

//	public void AddSiblingGroupPopupCallback (object obj)
//	{
//		AudioMixerGroupPopupContext context = (AudioMixerGroupPopupContext)obj;
//		if (context.groups != null && context.groups.Length > 0)
//		{
//			var item = m_AudioGroupTree.FindItem (context.groups[0].GetInstanceID ()) as M10NStringTableListViewNode;
//			if (item != null)
//			{
//				var parent = item.parent as M10NStringTableListViewNode;
//				AddAudioMixerGroup (parent.group);
//			}
//		}
//	}

	public void AddStringTableEntity ()
	{
		if (m_db == null)
			return;

		// TODO: 

//		Undo.RecordObjects (new UnityEngine.Object[] { m_Controller, parent }, "Add Child Group");
//		var newGroup = m_Controller.CreateNewGroup("New Group", true);
//		m_Controller.AddChildToParent (newGroup, parent);
//		m_Controller.AddGroupToCurrentView (newGroup);
//
//		Selection.objects = new [] {newGroup};
//		m_Controller.OnUnitySelectionChanged ();
//		m_AudioGroupTree.SetSelection(new int[] { newGroup.GetInstanceID() }, true);
//		ReloadTree();
//		m_AudioGroupTree.BeginNameEditing (0f);
	}

//	static string PluralIfNeeded (int count)
//	{
//		return count > 1 ? "s" : "";
//	}
//
//	public void DeleteStringTableEntity (List<AudioMixerGroupController> groups, bool recordUndo)
//	{
//		foreach (AudioMixerGroupController group in groups)
//		{
//			if (group.HasDependentMixers ())
//			{
//				if (!EditorUtility.DisplayDialog ("Referenced Group", "Deleted group is referenced by another AudioMixer, are you sure?", "Delete", "Cancel"))
//					return;
//				break;
//			}
//		}
//
//		Assert.IsNotNull(m_db);
//		//TODO:
//
//		m_db.keys.RemoveAt()

//		if (recordUndo) {
//			Undo.RegisterCompleteObjectUndo (m_Controller, "Delete Group" + PluralIfNeeded (groups.Count));
//		}
//		
//		m_Controller.DeleteGroups (groups.ToArray ());
//		ReloadTree();
//	}

//	public void DuplicateGroups (List<AudioMixerGroupController> groups, bool recordUndo)
//	{
//		if (recordUndo)
//			Undo.RecordObject (m_Controller, "Duplicate group" + PluralIfNeeded (groups.Count));
//
//		var duplicatedRoots = m_Controller.DuplicateGroups (groups.ToArray ());
//		if (duplicatedRoots.Count > 0)
//		{
//			ReloadTree ();
//			var instanceIDs = duplicatedRoots.Select (audioMixerGroup => audioMixerGroup.GetInstanceID ()).ToArray ();
//			m_AudioGroupTree.SetSelection (instanceIDs, false);
//			m_AudioGroupTree.Frame (instanceIDs[instanceIDs.Length - 1], true, false);
//		}
//	}

//	void DeleteGroupsPopupCallback (object obj)
//	{
//		var audioMixerGroupTreeView = (AudioMixerGroupTreeView)obj;
//		audioMixerGroupTreeView.DeleteGroups (GetGroupSelectionWithoutMasterGroup (), true);
//	}
//
//	void DuplicateGroupPopupCallback (object obj)
//	{
//		var audioMixerGroupTreeView = (AudioMixerGroupTreeView)obj;
//		audioMixerGroupTreeView.DuplicateGroups (GetGroupSelectionWithoutMasterGroup (), true);
//	}

//	void RenameGroupCallback (object obj)
//	{
//		var item = (TreeViewItem)obj;
//		m_AudioGroupTree.SetSelection (new int[] {item.id}, false);
//		m_AudioGroupTree.BeginNameEditing (0f);
//	}

//	List<AudioMixerGroupController> GetGroupSelectionWithoutMasterGroup ()
//	{
//		var items = GetAudioMixerGroupsFromNodeIDs (m_AudioGroupTree.GetSelection ());
//		items.Remove (m_Controller.masterGroup);
//		return items;
//	}

//	public void OnTreeViewContextClick(int index)
//	{
//		var node = m_AudioGroupTree.FindItem(index);
//		if (node != null)
//		{
//			M10NStringTableListViewNode mixerNode = node as M10NStringTableListViewNode;
//			if (mixerNode != null && mixerNode.group != null)
//			{
//				GenericMenu pm = new GenericMenu();
//
//				if (!EditorApplication.isPlaying)
//				{
//					pm.AddItem(new GUIContent("Add child group"), false, AddChildGroupPopupCallback, new AudioMixerGroupPopupContext(m_Controller, mixerNode.group));
//					if (mixerNode.group != m_Controller.masterGroup)
//					{
//						pm.AddItem(new GUIContent("Add sibling group"), false, AddSiblingGroupPopupCallback, new AudioMixerGroupPopupContext(m_Controller, mixerNode.group));
//						pm.AddSeparator("");
//						pm.AddItem(new GUIContent("Rename"), false, RenameGroupCallback, node);
//
//						// Mastergroup cannot be deleted nor duplicated
//						var selection = GetGroupSelectionWithoutMasterGroup().ToArray();
//						pm.AddItem(new GUIContent((selection.Length > 1) ? "Duplicate groups (and children)" : "Duplicate group (and children)"), false, DuplicateGroupPopupCallback, this);
//						pm.AddItem(new GUIContent((selection.Length > 1) ? "Remove groups (and children)" : "Remove group (and children)"), false, DeleteGroupsPopupCallback, this);
//					}
//				}
//				else
//				{
//					pm.AddDisabledItem (new GUIContent("Modifying group topology in play mode is not allowed"));
//				}
//				
//				pm.ShowAsContext();
//			}
//		}
//	}

//	void OnNodeToggled (M10NStringTableListViewNode node, bool nodeWasEnabled)
//	{
//		var treeSelection = GetAudioMixerGroupsFromNodeIDs(m_AudioGroupTree.GetSelection());
//		if (!treeSelection.Contains(node.group))
//			treeSelection = new List<AudioMixerGroupController> {node.group};
//		var newSelection = new List<GUID>();
//		var allGroups = m_Controller.GetAllAudioGroupsSlow();
//		foreach (var g in allGroups)
//		{
//			bool inOldSelection = m_Controller.CurrentViewContainsGroup (g.groupID);
//			bool inNewSelection = treeSelection.Contains(g);
//			bool add = inOldSelection && !inNewSelection;
//			if (!inOldSelection && inNewSelection)
//				add = nodeWasEnabled;
//			if(add)
//				newSelection.Add(g.groupID);
//		}
//		m_Controller.SetCurrentViewVisibility (newSelection.ToArray ());
//	}


//	List<AudioMixerGroupController> GetAudioMixerGroupsFromNodeIDs(int[] instanceIDs)
//	{
//		List<AudioMixerGroupController> newSelectedGroups = new List<AudioMixerGroupController>();
//		foreach (var s in instanceIDs)
//		{
//			var node = m_AudioGroupTree.FindItem(s);
//			if (node != null)
//			{
//				M10NStringTableListViewNode mixerNode = node as M10NStringTableListViewNode;
//				if (mixerNode != null)
//					newSelectedGroups.Add(mixerNode.group);
//			}
//		}
//		return newSelectedGroups;
//	}

	public void OnTreeSelectionChanged (int[] selection)
	{
//		var groups = GetAudioMixerGroupsFromNodeIDs(selection);
//		Selection.objects = groups.ToArray ();
//		m_Controller.OnUnitySelectionChanged ();
		if(selection.Length == 1) {
			m_ScrollToItem = selection[0];
		}

		m_editorWindow.OnStringTableSelectionChanged(selection);

		//UnityEditor.InspectorWindow.RepaintAllInspectors();
	}

	public void OnEditingLanguageChanged(SystemLanguage lang) {
		m_StringTableListViewGUI.currentEditingLanguage = lang;
	}

	public void InitSelection (bool revealSelectionAndFrameLastSelected)
	{
		if (m_db == null) {
			return;
		}

//		var groups = m_Controller.CachedSelection;
//		m_AudioGroupTree.SetSelection ((from x in groups select x.GetInstanceID ()).ToArray (), revealSelectionAndFrameLastSelected);
	}

//	public float GetTotalHeight ()
//	{
//		if (m_Controller == null)
//			return 0f;
//		return m_AudioGroupTree.gui.GetTotalSize ().y + AudioMixerDrawUtils.kSectionHeaderHeight;
//	}

	public void OnGUI(Rect rect)
	{
		int treeViewKeyboardControlID = GUIUtility.GetControlID (FocusType.Keyboard);

		m_ScrollToItem = 0;

		if (s_Styles == null)
			s_Styles = new Styles();

		m_StringTableTree.OnEvent();

//		Rect headerRect, contentRect;
		using (new EditorGUI.DisabledScope(m_db == null))
		{
//			AudioMixerDrawUtils.DrawRegionBg (rect, out headerRect, out contentRect);
//			AudioMixerDrawUtils.HeaderLabel (headerRect, s_Styles.header, s_Styles.audioMixerGroupIcon);
		}

		if (m_db != null)
		{
//			AudioMixerGroupController parent = (m_Controller.CachedSelection.Count == 1) ? m_Controller.CachedSelection[0] : m_Controller.masterGroup;
//			using (new EditorGUI.DisabledScope(EditorApplication.isPlaying))
//			{
//				if (GUI.Button (new Rect(headerRect.xMax - 15f, headerRect.y + 3f, 15f, 15f), s_Styles.addText, EditorStyles.label))
//					AddAudioMixerGroup (parent);
//			}

			m_StringTableTree.OnGUI (rect, treeViewKeyboardControlID);
//			AudioMixerDrawUtils.DrawScrollDropShadow (contentRect, m_AudioGroupTree.state.scrollPos.y, m_AudioGroupTree.gui.GetTotalSize ().y);

			HandleKeyboardEvents (treeViewKeyboardControlID);
			HandleCommandEvents (treeViewKeyboardControlID);
		}
	}

	void HandleCommandEvents (int treeViewKeyboardControlID)
	{
		if (GUIUtility.keyboardControl != treeViewKeyboardControlID) {
			return;
		}

		EventType eventType = Event.current.type;
		if (eventType == EventType.ExecuteCommand || eventType == EventType.ValidateCommand)
		{
			bool execute = eventType == EventType.ExecuteCommand;

			if (Event.current.commandName == "Delete" || Event.current.commandName == "SoftDelete")
			{
				Event.current.Use ();
				if (execute)
				{
					//DeleteGroups (GetGroupSelectionWithoutMasterGroup (), true);
					//TODO handle delete
					Debug.Log("[M10NTreeView]Delete command.");
					GUIUtility.ExitGUI (); // Cached groups might have been deleted to so early out of event
				}
			}
			else if (Event.current.commandName == "Duplicate")
			{
				Event.current.Use ();
				if (execute) {
					//TODO:
					Debug.Log("[M10NTreeView]Duplicate command.");
//					DuplicateGroups (GetGroupSelectionWithoutMasterGroup (), true);
				}
			}
		}
	}
	
	void HandleKeyboardEvents (int treeViewKeyboardControlID)
	{
		if (GUIUtility.keyboardControl != treeViewKeyboardControlID)
			return;

		Event evt = Event.current;
		if (evt.keyCode == KeyCode.Space && evt.type == EventType.KeyDown)
		{
			int[] selection = m_StringTableTree.GetSelection();
			if (selection.Length > 0)
			{
				M10NStringTableListViewNode node = m_StringTableTree.FindItem(selection[0]) as M10NStringTableListViewNode;
//				bool shown = m_Controller.CurrentViewContainsGroup (node.group.groupID);
//				OnNodeToggled(node, !shown);
				evt.Use();
			}
		}
	}

	public void OnM10NStringDatabaseChanged(M10NStringDatabase db)
	{
		if (m_db != db)
		{
			m_StringTableListViewGUI.m_db = db;
			m_db = db;
			m_StringTableDataSource.database = db;
			if (db != null)
			{
				ReloadTree();
				InitSelection (false);
//				LoadExpandedState ();
				m_StringTableTree.data.SetExpandedWithChildren (m_StringTableTree.data.root, true);
			}
		}
	}

//	static string GetUniqueAudioMixerName (AudioMixerController controller)
//	{
//		return "AudioMixer_" + controller.GetInstanceID ();
//	}

//	void SaveExpandedState ()
//	{
//		//SessionState.SetIntArray (GetUniqueAudioMixerName (m_Controller), m_AudioGroupTreeState.expandedIDs.ToArray ());
//	}
//
//	void LoadExpandedState ()
//	{
//		int[] cachedExpandedState = SessionState.GetIntArray (GetUniqueAudioMixerName (m_Controller), null);
//		if (cachedExpandedState != null)
//		{
//			m_AudioGroupTreeState.expandedIDs = new List<int> (cachedExpandedState);
//		}
//		else
//		{
//			// Expand whole tree. If no cached data then its the first time tree was loaded in this session
//			m_AudioGroupTree.state.expandedIDs = new List<int> ();
//			m_AudioGroupTree.data.SetExpandedWithChildren (m_AudioGroupTree.data.root, true);
//		}
//	}

	public void EndRenaming()
	{
		m_StringTableTree.EndNameEditing(true);
	}

	public void OnUndoRedoPerformed ()
	{
		ReloadTree ();
	}
}
