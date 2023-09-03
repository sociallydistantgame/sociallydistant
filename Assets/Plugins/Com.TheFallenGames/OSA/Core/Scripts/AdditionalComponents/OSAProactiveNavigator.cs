//#define DEBUG_OSA_PROACTIVE_NAVIGATOR

using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using frame8.Logic.Misc.Other.Extensions;
using frame8.Logic.Misc.Visual.UI;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Com.TheFallenGames.OSA.Core.SubComponents;
using System.Collections;
using System.Collections.Generic;

namespace Com.TheFallenGames.OSA.AdditionalComponents
{
	/// <summary>
	/// <para>OSA can work with Unity's built-in navigation, at least in simple scenarios, but if you want full control over the navigation, this component provides it.</para>
	/// <para>Attach it to an OSA-containing GameObject.</para>
	/// <para>Set your Item's navigation to Explicit, and leave all its 4 directions as None. You can optionally set the Item's transversal directions (i.e. left/right in a vertical ScrollView) 
	/// to anything, but, unless you need to do it for very specific reasons, it's more convenient to assign them in the Selectables section of this script's Inspector</para>
	/// </summary>
	[RequireComponent(typeof(IOSA))]
	public class OSAProactiveNavigator : Selectable
	{
		[Tooltip("Note that if the OSA has looping active, you'll only be able to reach the transversal selectables (i.e. left/right for a vertical ScrollView)")]
		[SerializeField]
		Selectables _Selectables = new Selectables();

		[Tooltip("What to do when the user navigates to a direction for which you didn't specify a selectable?")]
		[SerializeField]
		OnNoSelectableSpecifiedEnum _OnNoSelectableSpecified = OnNoSelectableSpecifiedEnum.KEEP_CURRENTLY_SELECTED;

		[Tooltip(
			"This is a workaround to prevent the navigation from skipping the first item when entering OSA from outside, " +
			"in case your items also contain an inner navigation logic using Unity's built-in system. " +
			"Default is 0.4"
		)]
		[SerializeField]
		float _JoystickInputMultiplier = .3f;

		[Tooltip("See description of 'JoystickInputMultiplier'. The same thing applies here, but for arrows")]
		[SerializeField]
		float _ArrowsInputMultiplier = 1f;

		[Tooltip("If at extremity and moving futher, the item at the other extremity will be selected. This will supersede the selectables you set in your item or in the Selectables section of this script")]
		[SerializeField]
		bool _LoopAtExtremity = false;

		ViewsHolderFinder ViewsHolderFinder { get { return _NavManager.ViewsHolderFinder; } }
		ScrollStateEnum ScrollState 
		{ 
			get { return _ScrollState; }
			set 
			{
#if DEBUG_OSA_PROACTIVE_NAVIGATOR
				Debug.Log("State from " + _ScrollState + " to " + value);
#endif
				_ScrollState = value;  
			} 
		}

		IOSA _OSA;
		SelectionWatcher _SelectionWatcher;
		BaseNavigationManager _NavManager;
		bool _Initialized;
		float _LastInputProcessingUnscaledTime;
		Vector2 _ScrollStartInputVec;
		float _ScrollStartInputVecUnscaledTime;
		// Contains data about the last command executed by this navigator. If a navigation event happened through Unity's own Navigation system, this won't reflect that.
		// This is useful to distinguish the cases where a new object is selected by this navigator vs by the Unity's system
		NavEvents _LastNav = new NavEvents();
		ScrollStateEnum _ScrollState;


		protected override void Awake()
		{
			base.Awake();

			_OSA = GetComponent(typeof(IOSA)) as IOSA;
			if (_OSA == null)
				throw new OSAException("OSAProactiveNavigatior: OSA component not found on this game object");

			SetNotInteractable();
		}

		void Update()
		{
			// Only executing at runtime
			if (!Application.isPlaying)
				return;

			if (!_Initialized)
			{
				if (_OSA.IsInitialized)
					InitializeOnOSAReady();

				return;
			}

			bool navEnabled = _OSA.BaseParameters.Navigation.Enabled;

			_SelectionWatcher.Enabled = navEnabled;
			_SelectionWatcher.OnUpdate();

			if (navEnabled)
				CheckNav();
			else
				EnterState_NoneIfNeeded();
		}

		void InitializeOnOSAReady()
		{
			SetNotInteractable();
			_NavManager = _OSA.GetBaseNavigationManager();

			if (_Selectables.SyncAllProvidedSelectables)
				_Selectables.SyncProvidedSelectables(this);

			_SelectionWatcher = new SelectionWatcher();
			_SelectionWatcher.NewObjectSelected += SelectionWatcher_NewObjectSelected;

			_Initialized = true;
		}

		void SelectionWatcher_NewObjectSelected(GameObject lastGO, GameObject newGO)
		{
			if (newGO != gameObject)
			{
				// Ignore our own commands
				if (_LastNav.New.Selectable && _LastNav.New.Selectable.gameObject == newGO)
					return;

				//if (_LastNav.New.Type == NavMoveType.ENTER_OSA || _LastNav.New.Type == NavMoveType.EXIT_OSA)
				//{
				//	// This prevents Unity's own nav system from navigating through other selectables after we select the closest VH which also has its own inner navigation bindings
				//	if (!DidEnoughTimePassSinceBoundaryCrossinEventOrPreEvent(_LastNav.New))
				//	{
				//		UndoUnitySelectionSilently(lastGO, newGO);
				//		return;
				//	}
				//}

				HandleUnityBuiltinNavSelection(lastGO, newGO);
				return;
			}

			HandleOnOSASelected(lastGO);
		}

		void HandleOnOSASelected(GameObject lastSelectedGO)
		{
			if (_OSA.VisibleItemsCount == 0)
				return;

			Selectable nextSelectable;
			AbstractViewsHolder nextSelectableContainingVH;
			if (GetClosestSelectableFromOSAsVHs(lastSelectedGO, out nextSelectable, out nextSelectableContainingVH))
			{
				var lastSelectedGOSelectable = lastSelectedGO.GetComponent<Selectable>();
				var lastSelectedGOContainingVH = ViewsHolderFinder.GetViewsHolderFromSelectedGameObject(lastSelectedGO);
				SetOrFindNextSelectable(nextSelectable, Vector3.one, lastSelectedGOSelectable, lastSelectedGOContainingVH);
				EnterState_WaitingForInitial();
			}
		}

		void HandleUnityBuiltinNavSelection(GameObject _, GameObject newGO)
		{
			var newGOSelectable = newGO.GetComponent<Selectable>();
			var newGOVH = ViewsHolderFinder.GetViewsHolderFromSelectedGameObject(newGO);
			_LastNav.OnNewEvent_UnityBuiltinNav(newGOSelectable, newGOVH, NavMoveType.UNKNOWN);
		}

		void EnterState_NoneIfNeeded()
		{
			if (ScrollState == ScrollStateEnum.NONE)
				return;

			_ScrollStartInputVec = Vector3.zero;
			ScrollState = ScrollStateEnum.NONE;
		}

		void EnterState_WaitingForInitial()
		{
			_ScrollStartInputVecUnscaledTime = Time.unscaledTime;
			_ScrollStartInputVec = GetCurrentInputVector();
			ScrollState = ScrollStateEnum.WAITING_FOR_FIRST_DELAY;
		}

		void CheckNav()
		{
			Selectable curSelectable;
			AbstractViewsHolder curSelectableVH;
			if (!NavPreChecks(out curSelectable, out curSelectableVH))
				return;

			var inputVec = GetCurrentInputVector();
			var curSelectableNavMode = curSelectable.navigation.mode;
			bool curSelectableUsesExplicitNav = curSelectableNavMode == Navigation.Mode.Explicit;
			var absVert = Math.Abs(inputVec.y);
			var absHor = Math.Abs(inputVec.x);
			bool isVertDir = absVert >= absHor;
			bool curSelectableIsVH = curSelectableVH != null;

			var lastEv = _LastNav.New;
			if (lastEv.Selectable)
			{
				if (lastEv.Type == NavMoveType.ENTER_OSA || lastEv.Type == NavMoveType.EXIT_OSA)
				{
					if (lastEv.Selectable == curSelectable)
					{
						if (!DidEnoughTimePassSinceBoundaryCrossinEventOrPreEvent(lastEv))
							return;
					}
				}
				else
				{
					if (lastEv.IsUnityBuiltinEvent)
					{
						if (!DidEnoughTimePassSinceBoundaryCrossinEventOrPreEvent(lastEv))
							return;
					}
				}
			}

			if (_OSA.IsVertical)
			{
				float input;
				int inputSign;
				int nextOSAItemDirSign;
				bool isNextOSAItemDirNegative;
				Selectable nextSelectable;
				Vector3 findNextSelectableAuto_Vector;
				if (isVertDir)
				{
					input = inputVec.y;
					inputSign = Math.Sign(input);
					nextOSAItemDirSign = -inputSign;
					isNextOSAItemDirNegative = nextOSAItemDirSign < 0;

					Selectable nextOuterSelectableFromParams;
					Selectable curSelectableNextExplicitSelectable;
					if (isNextOSAItemDirNegative)
					{
						curSelectableNextExplicitSelectable = curSelectable.navigation.selectOnUp;
						nextOuterSelectableFromParams = _Selectables.Up;
						findNextSelectableAuto_Vector = Vector3.up;
					}
					else
					{
						curSelectableNextExplicitSelectable = curSelectable.navigation.selectOnDown;
						nextOuterSelectableFromParams = _Selectables.Down;
						findNextSelectableAuto_Vector = Vector3.down;
					}

					if (curSelectableIsVH)
					{
						if (curSelectableUsesExplicitNav)
						{
							if (curSelectableNextExplicitSelectable)
							{
								// Let Unity system decide
								return;
							}
						}

						int itemIndexOfLastItemInList = _OSA.GetItemsCount() - 1;
						int extremityItemIndex;
						if (isNextOSAItemDirNegative)
						{
							extremityItemIndex = 0;
						}
						else
						{
							extremityItemIndex = itemIndexOfLastItemInList;
						}

						int itemIndex = curSelectableVH.ItemIndex;
						if (itemIndex == extremityItemIndex)
						{
							if (_LoopAtExtremity)
							{
								var otherExtremityItemIndex = itemIndexOfLastItemInList - extremityItemIndex;
								nextSelectable = FindVHSelectableInDirectionOrDefault(otherExtremityItemIndex - nextOSAItemDirSign, nextOSAItemDirSign, extremityItemIndex, this, nextOuterSelectableFromParams);
							}
							else
								nextSelectable = nextOuterSelectableFromParams;

							//SetOrFindNextSelectable(evSystem, nextSelectable, findNextSelectableAuto_Vector);
						}
						else
						{
							nextSelectable = FindVHSelectableInDirectionOrDefault(itemIndex, nextOSAItemDirSign, extremityItemIndex, this, nextOuterSelectableFromParams);
						}
					}
					else
					{
						// Current selectable is an outter Selectable

						if (curSelectableUsesExplicitNav && curSelectableNextExplicitSelectable)
						{
							if (curSelectableNextExplicitSelectable != this)
							{
								// Let Unity system decide
								return;
							}

							// Currently selected is an outer selectable which has <this> pointed as the next selectable, explicitly => select the nearest item instead.
							// Setting to null, and SetOrFindNextSelectable() will decide what to do next, based on params
							nextSelectable = null;
						}
						else
						{
							// Either nav mode is None, or Explicit and no Selectable assigned in that direction.
							// In this case, we don't do anything, as it's required to have <this> selected as the next selectable in order to 'enter' OSA
							return;
						}

						//// Commented: setting to null, and SetOrFindNextSelectable() will decide what to do next, based on params
						////nextSelectable = curSelectable.FindSelectable(findNextSelectableAuto_Vector);
						//nextSelectable = null;
					}
				}
				else
				{
					input = inputVec.x;
					inputSign = Math.Sign(input);
					nextOSAItemDirSign = inputSign;
					isNextOSAItemDirNegative = nextOSAItemDirSign < 0;

					Selectable nextOuterSelectableFromParams;
					Selectable curSelectableNextExplicitSelectable;
					if (isNextOSAItemDirNegative)
					{
						curSelectableNextExplicitSelectable = curSelectable.navigation.selectOnLeft;
						nextOuterSelectableFromParams = _Selectables.Left;
						findNextSelectableAuto_Vector = Vector3.left;
					}
					else
					{
						curSelectableNextExplicitSelectable = curSelectable.navigation.selectOnRight;
						nextOuterSelectableFromParams = _Selectables.Right;
						findNextSelectableAuto_Vector = Vector3.right;
					}

					if (curSelectableIsVH)
					{
						// Let Unity system decide
						if (curSelectableUsesExplicitNav && curSelectableNextExplicitSelectable)
							return;

						nextSelectable = nextOuterSelectableFromParams;
					}
					else
					{
						if (curSelectableUsesExplicitNav && curSelectableNextExplicitSelectable)
						{
							if (curSelectableNextExplicitSelectable != this)
							{
								// Let Unity system decide
								return;
							}

							// Currently selected is an outer selectable which has <this> pointed as the next selectable, explicitly => select the nearest item instead.
							// Setting to null, and SetOrFindNextSelectable() will decide what to do next, based on params
							nextSelectable = null;
						}
						else
						{
							// Either nav mode is None, or Explicit and no Selectable assigned in that direction.
							// In this case, we don't do anything, as it's required to have <this> selected as the next selectable in order to 'enter' OSA
							return;
						}
					}
					//SetOrFindNextSelectable(evSystem, nextSelectableFromParams, findNextSelectableAuto_Vector);
				}
				SetOrFindNextSelectable(nextSelectable, findNextSelectableAuto_Vector, curSelectable, curSelectableVH);
			}
			else
			{

			}
		}

		Selectable FindVHSelectableInDirectionOrDefault(int itemIndex, int nextOSAItemDirSign, int extremityItemIndex, Selectable curSelectable, Selectable defaultSelectable)
		{
			Selectable nextSelectableCandidate = null;
			int maxIterations = 1000;
			int curIterations = 0;
			while (curIterations < maxIterations && !nextSelectableCandidate)
			{
				if (itemIndex == extremityItemIndex)
				{
					// If after <maxIterations> items scrolled through OSA, none of them contains a Selectable, we just fallback to the selectable set in params. 
					// SetOrFindNextSelectable() will further refine the decisions based on params
					nextSelectableCandidate = defaultSelectable;
					break;

				}

				itemIndex += nextOSAItemDirSign;

				_OSA.BringToView(itemIndex);
				_OSA.BringToView(itemIndex); // fix for variable-sized items cases; this needs to be called twice in that case
				var vh = _OSA.GetBaseItemViewsHolderIfVisible(itemIndex);
				if (vh == null)
				{
					nextSelectableCandidate = defaultSelectable;
					break;
				}

				var nextSelectableCandidates = new List<Selectable>();
				vh.root.GetComponentsInChildren(nextSelectableCandidates);
				nextSelectableCandidate = GetClosestActiveSelectable(nextSelectableCandidates, curSelectable.gameObject);

				++curIterations;
			}

			return nextSelectableCandidate;
		}

		bool NavPreChecks(out Selectable curSelectable, out AbstractViewsHolder curSelectableVH)
		{
			if (_OSA.BaseParameters.effects.LoopItems)
				throw new NotImplementedException("loop items nav");

			curSelectable = null;
			curSelectableVH = null;
			if (_OSA.VisibleItemsCount == 0)
			{
				EnterState_NoneIfNeeded();
				return false;
			}

			var evSystem = EventSystem.current;
			if (!evSystem)
			{
				EnterState_NoneIfNeeded();
				return false;
			}

			curSelectable = GetCurrentSelectable();
			if (!curSelectable)
			{
				EnterState_NoneIfNeeded();
				return false;
			}

			var inputVec = GetCurrentInputVector();
			if (!CheckInputStrength(inputVec))
				return false;

			curSelectableVH = ViewsHolderFinder.GetViewsHolderFromSelectedGameObject(curSelectable.gameObject);
			if (!CheckScrollState(curSelectable, inputVec, curSelectableVH))
				return false;

			return true;
		}

		bool CheckInputStrength(Vector2 inputVec)
		{
			var absVert = Math.Abs(inputVec.y);
			var absHor = Math.Abs(inputVec.x);

			if (absVert < .15f && absHor < .15f)
			{
				EnterState_NoneIfNeeded();
				return false;
			}

			return true;
		}

		bool CheckScrollState(Selectable curSelectable, Vector2 inputVec, AbstractViewsHolder curSelectableVH)
		{
			//Debug.Log(GetCurrentInputVector().x + ", " + GetCurrentInputVector().y);

			var curSelectableNavMode = curSelectable.navigation.mode;
			if (curSelectableNavMode == Navigation.Mode.Automatic || curSelectableNavMode == Navigation.Mode.Horizontal || curSelectableNavMode == Navigation.Mode.Vertical)
			{
				EnterState_NoneIfNeeded();
				return false;
			}

			int vertSign = Math.Sign(inputVec.y);
			int horSign = Math.Sign(inputVec.x);

			bool prevIsVert = Math.Abs(_ScrollStartInputVec.y) > Math.Abs(_ScrollStartInputVec.x);
			bool curIsVert = Math.Abs(inputVec.y) > Math.Abs(inputVec.x);

			//bool sameDir = Math.Sign(_ScrollStartInputVec.x) == horSign && Math.Sign(_ScrollStartInputVec.y) == vertSign;
			bool sameOrientation = prevIsVert == curIsVert;
			bool sameSigns;
			if (curIsVert)
				sameSigns = Math.Sign(_ScrollStartInputVec.y) == vertSign;
			else
				sameSigns = Math.Sign(_ScrollStartInputVec.x) == horSign;

			bool sameDir = sameOrientation && sameSigns;
			//bool curSelectableIsVH = curSelectableVH != null;
			//Debug.Log(ScrollState + ", sameDir " + sameDir + ", dt " + (Time.unscaledTime - _ScrollStartInputVecUnscaledTime));
			//bool inputStrengthIsSmaller = inputVec.magnitude < _ScrollStartInputVec.magnitude;
			switch (ScrollState)
			{
				case ScrollStateEnum.NONE:
					EnterState_WaitingForInitial();
					//if (curSelectable != _LastNavCommand.New.Selectable)
					//{
					//	if (curSelectableIsVH && _LastNavCommand.New.SelectableIsVH)
					//	{
					//		if (curSelectableVH.ItemIndex == _LastNavCommand.New.SelectableVHItemIndex)
					//		{
					//			// Don't select immediately, as curSelectable in this case was already selected by the Unity's nav system
					//			return false;
					//		}
					//	}
					//}

					break;

				case ScrollStateEnum.WAITING_FOR_FIRST_DELAY:
					//if (!sameDir || inputStrengthIsSmaller)
					if (!sameDir)
					{
						//ScrollState = ScrollStateEnum.NONE;
						//break;
						EnterState_NoneIfNeeded();
						return false;
					}

					if (Time.unscaledTime - _ScrollStartInputVecUnscaledTime < .4f)
						return false;

					ScrollState = ScrollStateEnum.SCROLLING;
					break;

				case ScrollStateEnum.SCROLLING:
					//if (!sameDir || inputStrengthIsSmaller)
					if (!sameDir)
					{
						ScrollState = ScrollStateEnum.NONE;
						break;
					}

					if (!CheckAndUpdateInputFrequency())
						return false;

					break;
			}
#if DEBUG_OSA_PROACTIVE_NAVIGATOR
			Debug.Log("Aft " + ScrollState);
#endif

			return true;
		}

		bool CheckAndUpdateInputFrequency()
		{
			float actionsPerSec = _NavManager.GetMaxInputModuleActionsPerSecondToExpect();
			if (actionsPerSec == 0)
				actionsPerSec = 10;
			float dtBetweenActions = 1f / actionsPerSec;
			if (Time.unscaledTime - _LastInputProcessingUnscaledTime < dtBetweenActions)
				return false;
			_LastInputProcessingUnscaledTime = Time.unscaledTime;

			return true;
		}

		void SetOrFindNextSelectable(Selectable nextSelectable, Vector3 findNextSelectableAuto_Vector, Selectable curSelectable, AbstractViewsHolder curSelectableVH)
		{
			var evSystem = EventSystem.current;
			bool curSelectableIsVH = curSelectableVH != null;
			AbstractViewsHolder nextSelectableVH;
			if (nextSelectable)
			{
				nextSelectableVH = ViewsHolderFinder.GetViewsHolderFromSelectedGameObject(nextSelectable.gameObject);
			}
			else
			{
				switch (_OnNoSelectableSpecified)
				{
					case OnNoSelectableSpecifiedEnum.KEEP_CURRENTLY_SELECTED:
						goto default;

					case OnNoSelectableSpecifiedEnum.AUTO_FIND:
						if (curSelectableIsVH)
						{
							nextSelectable = curSelectable.FindSelectable(findNextSelectableAuto_Vector);
							if (nextSelectable)
								nextSelectableVH = ViewsHolderFinder.GetViewsHolderFromSelectedGameObject(nextSelectable.gameObject);
							else
								nextSelectableVH = null;
						}
						else
							GetClosestSelectableFromOSAsVHs(curSelectable.gameObject, out nextSelectable, out nextSelectableVH);

						if (!nextSelectable)
							break;

						break;

					default:
						nextSelectable = null;
						nextSelectableVH = null;
						break;

				}
			}

			if (curSelectable != _LastNav.New.Selectable)
			{
				throw new OSAException(
					"[Please report this full error] Expecting curSelectable == _LastNavCommand.New.Selectable; " +
					"curSelectable = " + curSelectable +
					", _LastNav.New.Selectable = " + _LastNav.New.Selectable +
					", _LastNav.New = " + _LastNav.New.ToString()
				);
			}

			if (nextSelectable)
			{
				bool nextSelectableIsVH = nextSelectableVH != null;
				NavMoveType type = NavMoveType.UNKNOWN;
				if (curSelectableIsVH)
				{
					if (!nextSelectableIsVH)
						type = NavMoveType.EXIT_OSA;
				}
				else
				{
					if (nextSelectableIsVH)
					{
						type = NavMoveType.ENTER_OSA;
					}
				}

				bool force = type == NavMoveType.ENTER_OSA;
				if (!force)
				{
					bool checkForTiming = false;
					if (curSelectableIsVH)
					{
						if (_LastNav.New.IsUnityBuiltinEvent)
						{
							if (!nextSelectableIsVH)
							{
								checkForTiming = true;
							}
						}
					}
					else
					{
						if (!_LastNav.New.IsUnityBuiltinEvent)
						{
							if (nextSelectableIsVH)
							{
								checkForTiming = true;
							}
						}
					}

					if (checkForTiming)
					{
						// Going outside => Only allow if enough time has passed or if in the scrolling state
						if (ScrollState != ScrollStateEnum.SCROLLING)
						{
							if (!DidEnoughTimePassSinceBoundaryCrossinEventOrPreEvent(_LastNav.New))
								return;
						}
					}
				}

				evSystem.SetSelectedGameObject(nextSelectable.gameObject);

				// TODO maybe call this from the same place where the builtin event is triggered
				_LastNav.OnNewEvent_OSANav(nextSelectable, nextSelectableVH, type);
			}
		}

		void SetNotInteractable()
		{
			// We don't want this to be selectable. It's just used for Unity's nav system so we can select the navigator as the target of outside selectables. 
			// When they're about to select this object, we select the nearest item instead

			//// Important to set these as null so that the interactable=false won't modify them
			this.targetGraphic = null;
			this.image = null;


			//this.interactable = false;
			var nav = navigation;
			nav.mode = Navigation.Mode.None;
			this.navigation = nav;
		}

		bool GetClosestSelectableFromOSAsVHs(GameObject fromGO, out Selectable selectable, out AbstractViewsHolder containingVH)
		{
			var selectables = new List<Selectable>();
			var mapSelectableToVh = new Dictionary<Selectable, AbstractViewsHolder>();
			for (int i = 0; i < _OSA.VisibleItemsCount; i++)
			{
				var vh = _OSA.GetBaseItemViewsHolder(i);
				var vhSelectables = vh.root.GetComponentsInChildren<Selectable>();
				foreach (var vhSelectable in vhSelectables)
				{
					selectables.Add(vhSelectable);
					mapSelectableToVh[vhSelectable] = vh;
				}
			}

			var closestSelectable = GetClosestActiveSelectable(selectables, fromGO);
			if (closestSelectable)
			{
				selectable = closestSelectable;
				containingVH = mapSelectableToVh[closestSelectable];
				return true;
			}

			selectable = null;
			containingVH = null;
			return false;
		}

		Selectable GetClosestActiveSelectable(List<Selectable> selectables, GameObject fromGO)
		{
			Selectable toSelect;
			if (selectables.Count == 0)
			{
				var inputVec = GetCurrentInputVector();
				toSelect = fromGO.GetComponent<Selectable>().FindSelectable(inputVec);
			}
			else
			{
				var fromPos = fromGO.transform.position;
				selectables.Sort((a, b) => Mathf.RoundToInt((Vector3.Distance(fromPos, a.transform.position) - Vector3.Distance(fromPos, b.transform.position)) * 50));
				toSelect = null;
				// Make sure it's interactable
				for (int i = 0; i < selectables.Count; i++)
				{
					var s = selectables[i];
					if (CanSelect(s))
					{
						toSelect = s;
						break;
					}
				}
			}

			return toSelect;
		}

		bool CanSelect(Selectable s)
		{
			return s.interactable;
		}

		Vector3 GetCurrentInputVector()
		{
			float vertInput;
			float horInput;
			bool down = Input.GetKey(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.DownArrow);
			bool up = Input.GetKey(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.UpArrow);
			bool vertArrowDown = down || up;
			bool left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.LeftArrow);
			bool right = Input.GetKey(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.RightArrow);
			bool horArrowDown = left || right;
			var j = Input.GetJoystickNames();
			bool isJoystickPresent = j != null && j.Length != 0;
			float axisMultiplier = isJoystickPresent ? _JoystickInputMultiplier : _ArrowsInputMultiplier;


			// Old approach: doesn't work well - arrows leave some velocity after they're released
			//if (down)
			//{
			//	if (!up)
			//		vertInput = -1;
			//}
			//else if (up)
			//	vertInput = 1;
			//else
			//{
			//	vertInput = Input.GetAxis("Vertical") * axisMultiplier;
			//}

			//if (left)
			//{
			//	if (!right)
			//		horInput = -1;
			//}
			//else if (down)
			//	horInput = 1;
			//else
			//	horInput = Input.GetAxis("Horizontal") * axisMultiplier;

			// Update: arrows leave some velocity even after they're released, so it's pointless to guess their initial velocities. Just using the axes instead, whether we have a joystick or not
			vertInput = Input.GetAxis("Vertical") * (vertArrowDown ? _ArrowsInputMultiplier : axisMultiplier);
			horInput = Input.GetAxis("Horizontal") * (horArrowDown ? _ArrowsInputMultiplier : axisMultiplier);

			var vecInput = new Vector3(horInput, vertInput);
			return vecInput;
		}

		Selectable GetCurrentSelectable()
		{
			var evSystem = EventSystem.current;
			if (!evSystem.currentSelectedGameObject)
				return null;

			var curSelectable = evSystem.currentSelectedGameObject.GetComponent<Selectable>();
			if (!curSelectable)
				return null;

			return curSelectable;
		}

		bool DidEnoughTimePassSinceBoundaryCrossinEventOrPreEvent(NavEvent ev)
		{
			float minSecondsBase = .05f;
			float minSecondsFromFiveFrames = Time.unscaledDeltaTime * 5;
			float minSeconds = Math.Max(minSecondsBase, minSecondsFromFiveFrames);
			return ev.ElapsedUnscaledTime >= minSeconds;
		}
	}


	[Serializable]
	public class Selectables
	{
		[SerializeField]
		Selectable _Up = null;
		public Selectable Up { get { return _Up; } }

		[SerializeField]
		Selectable _Down = null;
		public Selectable Down { get { return _Down; } }

		[SerializeField]
		Selectable _Left = null;
		public Selectable Left { get { return _Left; } }

		[SerializeField]
		Selectable _Right = null;
		public Selectable Right { get { return _Right; } }

		[Tooltip(
			"Whether to set the corresponding Left/Right/Up/Down Selectables to point back this ScrollView (eg. the Down selectable's Up property will be <this> ScrollView). " +
			"This also overrides the Selectables' navigation mode to 'Explicit'"
		)]
		[SerializeField]
		bool _SyncAllProvidedSelectables = true;
		public bool SyncAllProvidedSelectables { get { return _SyncAllProvidedSelectables; } }


		internal void SyncProvidedSelectables(Selectable target)
		{
			if (Up)
			{
				var nav = Up.navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnDown = target;
				Up.navigation = nav;
			}
			if (Down)
			{
				var nav = Down.navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnUp = target;
				Down.navigation = nav;
			}
			if (Left)
			{
				var nav = Left.navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnRight = target;
				Left.navigation = nav;
			}
			if (Right)
			{
				var nav = Right.navigation;
				nav.mode = Navigation.Mode.Explicit;
				nav.selectOnLeft = target;
				Right.navigation = nav;
			}
		}
	}


	//[Serializable]
	//public class OnNoSelectableSpecifiedInfo
	//{
	//	[SerializeField]
	//	OnNoSelectableSpecifiedEnum _FromItem = OnNoSelectableSpecifiedEnum.KEEP_CURRENTLY_SELECTED;
	//	public OnNoSelectableSpecifiedEnum Up { get { return _Up; } }

	//	[SerializeField]
	//	Selectable _Down = null;
	//	public Selectable Down { get { return _Down; } }

	//	[SerializeField]
	//	Selectable _Left = null;
	//	public Selectable Left { get { return _Left; } }

	//	[SerializeField]
	//	Selectable _Right = null;
	//	public Selectable Right { get { return _Right; } }
	//}


	public enum OnNoSelectableSpecifiedEnum
	{
		KEEP_CURRENTLY_SELECTED,
		AUTO_FIND
	}


	enum ScrollStateEnum
	{
		NONE,
		WAITING_FOR_FIRST_DELAY,
		SCROLLING,
	}


	class NavEvents
	{
		public NavEvent Prev { get; private set; }
		public NavEvent New { get; private set; }


		public NavEvents()
		{
			Prev = new NavEvent();
			New = new NavEvent();
		}

		public void OnNewEvent_OSANav(Selectable newSelectable, AbstractViewsHolder nextSelectableVH, NavMoveType type)
		{
			UpdateInternal(newSelectable, nextSelectableVH, false, type);
		}

		public void OnNewEvent_UnityBuiltinNav(Selectable newSelectable, AbstractViewsHolder nextSelectableVH, NavMoveType type)
		{
			UpdateInternal(newSelectable, nextSelectableVH, true, type);
		}

		void UpdateInternal(Selectable newSelectable, AbstractViewsHolder curSelectableVH, bool isUnityBuiltinEvent, NavMoveType type)
		{
			var p = Prev;
			Prev = New;
			New = p;
			New.Update(newSelectable, curSelectableVH, isUnityBuiltinEvent, type);
		}
	}

	enum NavMoveType
	{
		EXIT_OSA,
		ENTER_OSA,
		UNKNOWN
	}


	class NavEvent
	{
		public Selectable Selectable { get; private set; }
		//public AbstractViewsHolder SelectableVH { get; private set; }
		public int SelectableVHItemIndex { get; private set; }
		public bool IsUnityBuiltinEvent { get; private set; }
		public bool SelectableIsVH { get { return SelectableVHItemIndex != -1; } }
		public float UnscaledTime { get; private set; }
		public int Frame { get; private set; }
		public float ElapsedUnscaledTime { get { return UnityEngine.Time.unscaledTime - UnscaledTime; } }
		public NavMoveType Type { get; private set; }


		public NavEvent()
		{
			SelectableVHItemIndex = -1;
		}


		public void Update(Selectable newSelectable, AbstractViewsHolder curSelectableVH, bool isUnityBuiltinEvent, NavMoveType type)
		{
			Selectable = newSelectable;
			//SelectableVH = curSelectableVH;
			SelectableVHItemIndex = curSelectableVH == null ? -1 : curSelectableVH.ItemIndex;
			IsUnityBuiltinEvent = isUnityBuiltinEvent;
			Frame = UnityEngine.Time.frameCount;
			UnscaledTime = UnityEngine.Time.unscaledTime;
			Type = type;
		}

		public override string ToString()
		{
			return
				"Selectable " + Selectable +
				", SelectableVHItemIndex " + SelectableVHItemIndex +
				", IsUnityBuiltinEvent " + IsUnityBuiltinEvent +
				", UnscaledTime " + UnscaledTime +
				", Frame " + Frame +
				", ElapsedUnscaledTime " + ElapsedUnscaledTime +
				", Type " + Type;
		}
	}
}
