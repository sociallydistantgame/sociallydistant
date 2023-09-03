using System;
using System.Collections.Generic;
using UnityEngine;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Extra;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic;
using Com.TheFallenGames.OSA.Demos.Common;
using Com.TheFallenGames.OSA.Demos.Common.SceneEntries;
using System.Collections;
using UnityEngine.UI;

namespace Com.TheFallenGames.OSA.Demos.Table
{
	/// <summary>Hookup between the <see cref="Common.Drawer.DrawerCommandPanel"/> and the adapter to isolate example code from demo-ing and navigation code</summary>
	public class TableViewSceneEntry : DemoSceneEntry<TableViewExample, MyTableParams, MyTupleVH>
	{
		[SerializeField]
		Texture[] _SampleTextures = new Texture[0];

		string[] _RandomStrings = new string[]
		{
				"Lorem Ipsum is simply dummy text of the printing and typesetting industry",
				"dummy text of the printing and typesetting industry",
				"industry",
				"and typesetting industry",
				"Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, " +
					"when an unknown printer took a galley of type and scrambled it to make a type specimen book."
		};


		protected override void Awake()
		{
#if OSA_TV_TMPRO
			Debug.LogError("WARNING: This scene is set up using Non-TMPro TableView, but OSA_TV_TMPRO scripting define symbol is set. Aborting initializing adapters..");
			gameObject.SetActive(false);
#endif

			base.Awake();
		}


		protected override void InitDrawer()
		{
			_Drawer.Init(_Adapters, true, false, false, false, false, false);
			_Drawer.galleryEffectSetting.slider.value = 0f;
			_Drawer.galleryEffectSetting.gameObject.SetActive(false);
			_Drawer.setCountPanel.gameObject.SetActive(false);
			var buttonsPanel = _Drawer.AddButtonsWithOptionalInputPanel("Big buffered\n(sync)", "Big buffered\n(async)", "Small full");
			var b = buttonsPanel.button1.GetComponentInChildren<Text>();
			b.fontSize =
			buttonsPanel.button2.GetComponentInChildren<Text>().fontSize =
			buttonsPanel.button3.GetComponentInChildren<Text>().fontSize = (int)(b.fontSize * .8f);

			buttonsPanel.button1.onClick.AddListener(LoadRandomBigBufferedDataSet);
			buttonsPanel.button2.onClick.AddListener(LoadRandomBigAsyncBufferedDataSet);
			buttonsPanel.button3.onClick.AddListener(LoadRandomSmallWholeDataSet);

			var toggles = _Drawer.AddLabelWithTogglesPanel("Allow resizing", "None", "Auto-fit", "Dragger");
			toggles.subItems[0].toggle.isOn = true; // no sizing by default
			toggles.subItems[0].toggle.onValueChanged.AddListener(isOn => {
				if (isOn)
					ResetWithResizingMode(TableResizingMode.NONE);
			});
			toggles.subItems[1].toggle.onValueChanged.AddListener(isOn => {
				if (isOn)
					ResetWithResizingMode(TableResizingMode.AUTO_FIT_TUPLE_CONTENT);
			});
			toggles.subItems[2].toggle.onValueChanged.AddListener(isOn => {
				if (isOn)
					ResetWithResizingMode(TableResizingMode.MANUAL_COLUMNS_AND_TUPLES);
			});

			buttonsPanel.transform.SetAsFirstSibling();
		}

		protected override void OnAllAdaptersInitialized()
		{
			base.OnAllAdaptersInitialized();

			ResetWithResizingMode(TableResizingMode.NONE); // No resizing by default
		}

		void GetRandomColumnsValueTypes(int numColumns, out List<TableValueType> values, out Type[] enumTypesWhereApplicable)
		{
			values = new List<TableValueType>(numColumns);
			enumTypesWhereApplicable = new Type[numColumns];
			var arr = Enum.GetValues(typeof(TableValueType)) as TableValueType[];
			for (int i = 0; i < numColumns; i++)
			{
				var type = arr[UnityEngine.Random.Range(0, arr.Length)];
				values.Add(type);
				if (type == TableValueType.ENUMERATION)
					enumTypesWhereApplicable[i] = GetRandomEnumType();
			}
		}

		Type GetRandomEnumType()
		{
			var enumTypes = new Type[] {
							typeof(System.DateTimeKind),
							typeof(System.StringComparison),
							typeof(UriHostNameType),
							typeof(PlatformID),
						};
			var enumType = enumTypes[UnityEngine.Random.Range(0, enumTypes.Length)];
			return enumType;
		}

		void GetRandomValuesIntoTuple(ITableColumns columnsModel, ITuple tuple, System.Random random)
		{
			for (int i = 0; i < columnsModel.ColumnsCount; i++)
			{
				var randInt = random.Next();
				var randIntPos = randInt < 0 ? -randInt : randInt;

				object obj = null;
				if (randInt % 10 != 0) // objects can also be null
				{
					var columnState = columnsModel.GetColumnState(i);
					var type = columnState.Info.ValueType;
					var buf8 = new byte[sizeof(double)];
					switch (type)
					{
						case TableValueType.RAW:
							if (randInt % 2 == 0)
								obj = new object();
							else
								obj = new Vector3((randInt % 4), (randInt % (1+i)), (randInt % (2 + i)));
							break;

						case TableValueType.STRING:
							obj = _RandomStrings[randIntPos % _RandomStrings.Length];
							break;

						case TableValueType.INT:
							obj = randInt;
							break;

						case TableValueType.LONG_INT:
							obj = (((long)(randInt) << 32) + (~randInt));
							break;

						case TableValueType.FLOAT:
							obj = randInt / 134325.4134E+23f;
							break;

						case TableValueType.DOUBLE:
							// _Random.NextDouble() is useless because it doesn't produce the entire range of doubles
							// This also can yield NaN and Infinity, but we want that, for demo purposes
							random.NextBytes(buf8);
							obj = BitConverter.ToDouble(buf8, 0);
							break;

						case TableValueType.ENUMERATION:
							var enumType = columnState.Info.EnumValueType;
							var enumValues = Enum.GetValues(enumType);
							obj = enumValues.GetValue(randIntPos % enumValues.Length);
							break;

						case TableValueType.BOOL:
							obj = randInt % 2 == 0;
							break;

						case TableValueType.TEXTURE:
							obj = _SampleTextures[randIntPos % _SampleTextures.Length];
							break;
					}

				}
				tuple.SetValue(i, obj);
			}
		}

#region Random data generation
		void LoadRandomBigBufferedDataSet()
		{
			Debug.Log("Loading data on demand synchronously using " + typeof(BufferredTableData).Name + " (big dataset)");
			var columns = CreateRandomColumnsModel(20);

			int tuplesCount = OSAConst.MAX_ITEMS;
			var adapter = _Adapters[0];

			BufferredTableData.TuplesChunkReader chunkReader = (into, firstIndex, count) =>
			{
				// We don't use firstIndex, because we're just generating random data, 
				// but in a real case scenario you'll use it to locate the first item in your database from which to read <count> items, including itself
				GetRandomTuples(into, count, columns);
			};

			// Setting a smaller chunk size, because we're accessing the values instantly. 
			// If read from disk or similar, you'd probably want to experiment with bigger values to keep scrolling smooth and only have FPS drops on loading a new chunk
			int chunkSize = 5;

			var data = new BufferredTableData(columns, tuplesCount, chunkReader, chunkSize, false);

			adapter.ResetTable(data.Columns, data);
		}

		void LoadRandomBigAsyncBufferedDataSet()
		{
			Debug.Log("Loading data on demand Asynchronously using " + typeof(AsyncBufferredTableData<>).Name + " (big dataset)");
			var columns = CreateRandomColumnsModel(20);

			int tuplesCount = OSAConst.MAX_ITEMS;
			var adapter = _Adapters[0];

			// Async data reading operations imply less frequent calls, but with larger data retrieved per call
			int chunkSize = 500;

			var data = new AsyncBufferredTableData<BasicTuple>(columns, tuplesCount, chunkSize, ReadDataFromServerInto);
			//data.Source.ShowLogs = true;

			// See the description of AsyncLoadingUIController inside its file for more info
			var uiController = new AsyncLoadingUIController<BasicTuple>(adapter, data);

			adapter.ResetTable(data.Columns, data);

			uiController.BeginListeningForSelfDisposal();
		}

		void LoadRandomSmallWholeDataSet()
		{
			Debug.Log("Loading all data at once using " + typeof(BasicTableData).Name + " (small dataset)");
			var columns = CreateRandomColumnsModel(20);
			int tuplesCount = 500;
			var tuples = new ITuple[tuplesCount];
			GetRandomTuples(tuples, tuplesCount, columns);

			var data = new BasicTableData(columns, tuples, true);

			var adapter = _Adapters[0];
			adapter.ResetTable(data.Columns, data);
		}
#endregion

		BasicTableColumns CreateRandomColumnsModel(int numColumns)
		{
			List<TableValueType> valueTypes;
			Type[] enumTypesWhereApplicable;
			GetRandomColumnsValueTypes(numColumns, out valueTypes, out enumTypesWhereApplicable);

			var columnsList = new List<IColumnInfo>(numColumns);
			for (int i = 0; i < numColumns; i++)
				columnsList.Add(new BasicColumnInfo("Col " + i, valueTypes[i], enumTypesWhereApplicable[i]));
			var columnsModel = new BasicTableColumns(columnsList);

			for (int i = 0; i < numColumns; i++)
			{
				// Also add some read-only columns
				if (UnityEngine.Random.Range(0, 10) == 0)
					columnsModel.GetColumnState(i).CurrentlyReadOnly = true;
			}

			return columnsModel;
		}

		void GetRandomTuples(ITuple[] into, int numTuples, ITableColumns columns)
		{
			// If this mehtod is called from different threads, each thead needs its own random generator instance
			var random = new System.Random((int)DateTime.Now.Ticks);
			for (int i = 0; i < numTuples; i++)
			{
				var tuple = TableViewUtil.CreateTupleWithEmptyValues<BasicTuple>(columns.ColumnsCount);
				GetRandomValuesIntoTuple(columns, tuple, random);
				into[i] = tuple;
			}
		}

		void ResetWithResizingMode(TableResizingMode mode)
		{
			var adapter = _Adapters[0];
			adapter.ResetItems(0);
			var tupleAdapterOnPrefab = adapter.Parameters.Table.TuplePrefab.GetComponent(typeof(ITupleAdapter)) as ITupleAdapter;
			tupleAdapterOnPrefab.TupleParameters.ResizingMode = mode;

			// To avoid warning in console
			tupleAdapterOnPrefab.TupleParameters.ItemTransversalSize = 
				mode == TableResizingMode.AUTO_FIT_TUPLE_CONTENT ? -1f
				: 0f;

			tupleAdapterOnPrefab.TupleParameters.InitIfNeeded(tupleAdapterOnPrefab);
			LoadRandomBigBufferedDataSet();
		}

		void ReadDataFromServerInto(BasicTuple[] into, int firstItemIndex, int countToRead, Action onDone)
		{
			StartCoroutine(
				SimulateReadDataFromServerIntoExistingTuples_Coroutine(
					into,
					firstItemIndex,
					countToRead,
					onDone
				)
			);
		}

		IEnumerator SimulateReadDataFromServerIntoExistingTuples_Coroutine(BasicTuple[] into, int firstItemIndex, int countToRead, Action onDone)
		{
			yield return null;

			var adapter = _Adapters[0];
			var columns = adapter.Columns;
			// WebGL doesn't support threads, so we simulate it directly through this coroutine (which is slower, but there's no other choice)
#if UNITY_WEBGL
			// If this mehtod is called from different threads, each thead needs its own random generator instance
			var random = new System.Random((int)DateTime.Now.Ticks);
			for (int i = 0; i < countToRead; i++)
			{
				if (i % 20 == 0)
					yield return null; // wait 1 frame

				var tuple = TableViewUtil.CreateTupleWithEmptyValues<BasicTuple>(columns.ColumnsCount);
				GetRandomValuesIntoTuple(columns, tuple, random);
				into[i] = tuple;
			}
#else
			bool abort = false;
			bool done = false;
			new System.Threading.Thread(
				() =>
				{
					System.Threading.Thread.Sleep(500); // simulate delay

					// Abort if the adapter was disposed or changed its data externally
					if (abort)
						return;

					GetRandomTuples(into, countToRead, columns);

					// Abort if the adapter was disposed or changed its data externally
					if (abort)
						return;

					done = true;
				}
			).Start();

			while (!done)
			{
				if (adapter == null || !adapter.IsInitialized) // adapter was disposed/destroyed => abort
				{
					abort = true;
					yield break;
				}

				if (adapter.Columns != columns) // data was changed externally => abort
				{
					abort = true;
					yield break;
				}

				// Wait a bit until next check
				yield return new UnityEngine.WaitForSeconds(.2f);
			}
#endif

			// onDone should be called on main thread. A coroutine is perfect for this
			if (onDone != null)
				onDone();
		}
	}
}
