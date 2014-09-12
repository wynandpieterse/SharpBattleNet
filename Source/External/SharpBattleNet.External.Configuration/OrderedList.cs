using System;
using System.Collections;

namespace SharpBattleNet.External.Configuration.Utilities
{
	//[Serializable]
	/// <include file='OrderedList.xml' path='//Class[@name="OrderedList"]/docs/*' />
	public class OrderedList : ICollection, IDictionary, IEnumerable
	{
		#region Private variables
		private Hashtable _table = new Hashtable ();
		private ArrayList _list = new ArrayList ();
		#endregion

		#region Public properties
		/// <include file='OrderedList.xml' path='//Property[@name="Count"]/docs/*' />
		public int Count 
		{
			get { return _list.Count; }
		}

		/// <include file='OrderedList.xml' path='//Property[@name="IsFixedSize"]/docs/*' />
		public bool IsFixedSize 
		{
			get { return false; }
		}

		/// <include file='OrderedList.xml' path='//Property[@name="IsReadOnly"]/docs/*' />
		public bool IsReadOnly 
		{
			get { return false; }
		}

		/// <include file='OrderedList.xml' path='//Property[@name="IsSynchronized"]/docs/*' />
		public bool IsSynchronized 
		{
			get { return false; }
		}

		/// <include file='OrderedList.xml' path='//Property[@name="ItemIndex"]/docs/*' />
		public object this[int index] 
		{
			get { return ((DictionaryEntry) _list[index]).Value; }
			set 
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException ("index");

				object key = ((DictionaryEntry) _list[index]).Key;
				_list[index] = new DictionaryEntry (key, value);
				_table[key] = value;
			}
		}

		/// <include file='OrderedList.xml' path='//Property[@name="ItemKey"]/docs/*' />
		public object this[object key] 
		{
			get { return _table[key]; }
			set 
			{
				if (_table.Contains (key))
				{
					_table[key] = value;
					_table[IndexOf (key)] = new DictionaryEntry (key, value);
					return;
				}
				Add (key, value);
			}
		}

		/// <include file='OrderedList.xml' path='//Property[@name="Keys"]/docs/*' />
		public ICollection Keys 
		{
			get 
			{ 
				ArrayList retList = new ArrayList ();
				for (int i = 0; i < _list.Count; i++)
				{
					retList.Add ( ((DictionaryEntry)_list[i]).Key );
				}
				return retList;
			}
		}

		/// <include file='OrderedList.xml' path='//Property[@name="Values"]/docs/*' />
		public ICollection Values 
		{
			get 
			{
				ArrayList retList = new ArrayList ();
				for (int i = 0; i < _list.Count; i++)
				{
					retList.Add ( ((DictionaryEntry)_list[i]).Value );
				}
				return retList;
			}
		}

		/// <include file='OrderedList.xml' path='//Property[@name="SyncRoot"]/docs/*' />
		public object SyncRoot 
		{
			get { return this; }
		}
		#endregion

		#region Public methods
		/// <include file='OrderedList.xml' path='//Method[@name="Add"]/docs/*' />
		public void Add (object key, object value)
		{
			_table.Add (key, value);
			_list.Add (new DictionaryEntry (key, value));
		}

		/// <include file='OrderedList.xml' path='//Method[@name="Clear"]/docs/*' />
		public void Clear ()
		{
			_table.Clear ();
			_list.Clear ();
		}

		/// <include file='OrderedList.xml' path='//Method[@name="Contains"]/docs/*' />
		public bool Contains (object key)
		{
			return _table.Contains (key);
		}

		/// <include file='OrderedList.xml' path='//Method[@name="CopyTo"]/docs/*' />
		public void CopyTo (Array array, int index)
		{
			_table.CopyTo (array, index);
		}
		
		/// <include file='OrderedList.xml' path='//Method[@name="CopyToStrong"]/docs/*' />
		public void CopyTo (DictionaryEntry[] array, int index)
		{
			_table.CopyTo (array, index);
		}

		/// <include file='OrderedList.xml' path='//Method[@name="Insert"]/docs/*' />
		public void Insert (int index, object key, object value)
		{
			if (index > Count)
				throw new ArgumentOutOfRangeException ("index");

			_table.Add (key, value);
			_list.Insert (index, new DictionaryEntry (key, value));
		}

		/// <include file='OrderedList.xml' path='//Method[@name="Remove"]/docs/*' />
		public void Remove (object key)
		{
			_table.Remove (key);
			_list.RemoveAt (IndexOf (key));
		}

		/// <include file='OrderedList.xml' path='//Method[@name="RemoveAt"]/docs/*' />
		public void RemoveAt (int index)
		{
			if (index >= Count)
				throw new ArgumentOutOfRangeException ("index");

			_table.Remove ( ((DictionaryEntry)_list[index]).Key );
			_list.RemoveAt (index);
		}

		/// <include file='OrderedList.xml' path='//Method[@name="GetEnumerator"]/docs/*' />
		public IEnumerator GetEnumerator () 
		{
			return new OrderedListEnumerator (_list);
		}

		/// <include file='OrderedList.xml' path='//Method[@name="GetDictionaryEnumerator"]/docs/*' />
		IDictionaryEnumerator IDictionary.GetEnumerator ()
		{
			return new OrderedListEnumerator (_list);
		}

		/// <include file='OrderedList.xml' path='//Method[@name="GetIEnumerator"]/docs/*' />
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return new OrderedListEnumerator (_list);
		}
		#endregion

		#region Private variables
		private int IndexOf (object key)
		{
			for (int i = 0; i < _list.Count; i++)
			{
				if (((DictionaryEntry) _list[i]).Key.Equals (key))
				{
					return i;
				}
			}
			return -1;
		}
		#endregion
	}
}
