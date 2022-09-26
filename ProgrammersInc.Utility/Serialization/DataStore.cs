/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

/////////////////////////////////////////////////////////////////////////////
//
// (c) 2006 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using BinaryComponents.Utility.Serialization.Streaming;

namespace BinaryComponents.Utility.Serialization
{
	/// <summary>
	/// Class that manages on demand loading of objects and saving them. This class
	/// enables us to store a set of objects individually only saving ones that have
	/// changed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class DataStore<T> where T : DataStore<T>.IStorableItem
	{
		/// <summary>
		/// Constructs a data store given a stream. 
		/// Note the stream must support Seeking, 
		/// reading and writing.
		/// </summary>
		/// <param name="stream"></param>
		public DataStore( Stream stream )
		{
			if( stream == null )
				throw new ArgumentNullException( "stream" );
			if( !stream.CanSeek || !stream.CanWrite || !stream.CanRead ) 
				throw new ArgumentException( "stream must support seeking, reading and writing " );

			_stream = stream;
			StreamingReader reader = new StreamingReader( _stream );
			if( reader.ReadInt() != Version )
				throw new ArgumentException( "Invalid version number" );

			ReadIndexItems( reader );
		}


		#region IStorableItem
		/// <summary>
		/// Represents the necessary methods a storable item should implement.
		/// </summary>
		public interface IStorableItem
		{
			string Id { get; }
			bool IsDeleted { get; }

			void Serialize( StreamingWriter writer );
			T Deserialize( StreamingReader reader );
		}
		#endregion

		public void ItemIsToBeDirtied( IStorableItem item )
		{
			if( item == null )
				throw new ArgumentNullException( "Item" );

			TransactionedItem tranasctedItem;
			if( _transactedItems.TryGetValue( item.Id, out tranasctedItem ) )
			{
				//_transactedItem
			}
			else
			{
				throw new InvalidOperationException( "Cannot find transacted item" );
			}

		}

		public void BeginTransaction()
		{
			_transactionLevel++;
		}
		public void RollbackTransaction()
		{
			List<KeyValuePair<string, TransactionedItem>> itemsToRemove = new List<KeyValuePair<string, TransactionedItem>>();
			foreach( KeyValuePair<string, TransactionedItem> kv in _transactedItems )
			{
				if( kv.Value.CurrentTransactionLevel == _transactionLevel )
				{
					itemsToRemove.Add( kv );
				}
			}
			foreach( KeyValuePair<string, TransactionedItem> kv in itemsToRemove )
			{
				if( kv.Value.PopTransactionLevel() == 0 )
				{
					_transactedItems.Remove( kv.Key );
				}
			}
			_transactionLevel--;
		}
		public void CommitTransaction()
		{
			_transactionLevel--;
		}


		#region Implementation
		private void ReadIndexItems( StreamingReader reader )
		{
			long itemsToRead = reader.ReadLong();
			while( itemsToRead-- != 0 )
			{
				_indexItems[reader.Read<string>()] = reader.ReadLong();
			}
		}

		private Dictionary<string, TransactionedItem> _transactedItems = new Dictionary<string, TransactionedItem>();
		private Dictionary<string, long> _indexItems = new Dictionary<string, long>();
		private Stream _stream;
		private const int Version = 1;
		private int _transactionLevel = -1;

		private class TransactionedItem
		{
			public TransactionedItem( DataStore<T> dataStore, IStorableItem item )
			{
				_transactionStack.Push( new TransactionLevelItem( item, dataStore._transactionLevel ) );
			}
			public IStorableItem CurrentItem
			{
				get
				{
					return _transactionStack.Peek().Item;
				}
			}

			public int CurrentTransactionLevel
			{
				get
				{
					return _transactionStack.Peek().TransactionLevel;
				}
			}

			public int PopTransactionLevel()
			{
				_transactionStack.Pop();
				return _transactionStack.Count;
			}

			public void PushTransactionLevel()
			{
				//_transactionStack.Push();
				//MemoryStream ms = new MemoryStream();
				//StreamingWriter sw = new StreamingWriter();
				//this.CurrentItem.Serialize( new StreamingWriter( ms ) );
				//ms.Seek( 0 );
				//IStorableItem copiedItem = this.CurrentItem.Deserialize();
				//TransactionLevelItem item = new TransactionLevelItem();
			}

			public class TransactionLevelItem
			{
				public TransactionLevelItem( IStorableItem item, int transactionLevel )
				{
					this.Item = item;
					this.TransactionLevel = transactionLevel;
				}
				public readonly IStorableItem Item;
				public readonly int TransactionLevel;
			}
			public Stack<TransactionLevelItem> _transactionStack = new Stack<TransactionLevelItem>();
		}


		#endregion
	}
}
