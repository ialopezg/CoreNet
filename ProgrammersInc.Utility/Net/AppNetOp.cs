/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProgrammersInc.Utility.Net
{
	/// <summary>
	/// Used to prioritise network calls
	/// </summary>
	public static class AppNetOps
	{
		public enum Priority{ Low = 0, Medium, High };

		/// <summary>
		/// Creates a disposable instance object that represents some space in
		/// time that your going to do some internet activity. Note if the current
		/// threads IsBackground property is true then this context will be given
		/// a low priority, otherwise it will be set by default to high.
		/// </summary>
		/// <returns></returns>
		public static InstanceContext StartInstanceContext()
		{
			return new InstanceContext( Thread.CurrentThread.IsBackground ? Priority.Low : Priority.High );
		}


		/// <summary>
		/// Creates a disposable instance object that represents some space in
		/// time that your going to do some internet activity. 
		/// </summary>
		/// <param name="priority">What priority to give this context</param>
		/// <returns></returns>
		public static InstanceContext StartInstanceContext( Priority priority )
		{
				return new InstanceContext( priority );
		}

		/// <summary>
		/// Returns true if no contexts are greater than this (in different threads).
		/// Note that if somewhere in your thread you have a higher priority context
		/// then your priority will be elevated to it.
		/// </summary>
		/// <param name="priority"></param>
		/// <returns></returns>
		public static bool CanContinue( Priority priority )
		{
			return CanContinue( Thread.CurrentThread, priority );
		}

		#region Class Instance
		public sealed class InstanceContext: IDisposable
		{
#if DEBUG
			private System.Diagnostics.StackTrace _stackTrace = new System.Diagnostics.StackTrace();
#endif
			internal InstanceContext( Priority priority )
			{
				_conceptionThread = Thread.CurrentThread;
				_priority = priority;
				AddInstanceToList( this );
			}

			/// <summary>
			/// Waits until all other contexts in other threads are lower or equal priority
			/// </summary>
			public void SpinWaitUntilAllowed()
			{
				while( !CanContinue )
				{
					Thread.Sleep( 100 );
				}
			}

			/// <summary>
			/// Returns true if all other contexts in other threads are lower or equal priority
			/// otherwise false.
			/// </summary>
			public bool CanContinue
			{
				get
				{
					CheckThreadState();
					if( !_priority.HasValue )
					{
						throw new InvalidOperationException( "Object has already been disposed" );
					}

					return AppNetOps.CanContinue( _priority.Value );
				}
			}
			public void Dispose()
			{
				CheckThreadState();

				if( _priority.HasValue )
				{
					RemoveInstanceToList( this );
					_priority = null;
				}
			}

			private void CheckThreadState()
			{
				if( _conceptionThread != Thread.CurrentThread )
				{
					throw new InvalidOperationException( "This object can only be used in the thread it was created" );
				}
			}

			internal Thread ConceptionThread
			{
				get
				{
					return _conceptionThread;
				}
			}
			internal Priority Priority
			{
				get
				{
					if( !_priority.HasValue )
					{
						throw new InvalidOperationException( "Object has already been disposed" );
					}
					return _priority.Value;
				}
			}

			private Priority ?_priority;
			private Thread _conceptionThread;
		}
		#endregion

		private static bool CanContinue( Thread thread, Priority priority )
		{
			lock( _instancesByThread )
			{
				//
				// Bump up priority if there is a higher one for this thread.
				List<InstanceContext> threadList;
				if( _instancesByThread.TryGetValue( thread, out threadList ) )
				{
					for( int i = 0; i < threadList.Count; i++ )
					{
						if( (int)threadList[i].Priority > (int)priority )
						{
							priority = threadList[i].Priority;
						}
					}
				}

				//
				// Check to see if anything is higher priortiy over us.
				foreach( KeyValuePair<Thread, List<InstanceContext>> kv in _instancesByThread )
				{
					for( int i = 0; i < kv.Value.Count; i++ )
					{
						if( thread != kv.Key && (int)kv.Value[i].Priority > (int)priority )
						{
							//System.Diagnostics.Debug.WriteLine( string.Format( "'{0}' Can't continue", Thread.CurrentThread.Name ) );
							return false;
						}
					}
				}
			}
			//System.Diagnostics.Debug.WriteLine( string.Format( "'{0}' Can continue", Thread.CurrentThread.Name ) );
			return true;
		}

		private static void AddInstanceToList( InstanceContext instance )
		{
			lock( _instancesByThread )
			{
				List<InstanceContext> threadList;
				if( !_instancesByThread.TryGetValue( instance.ConceptionThread, out threadList ) )
				{
					threadList = new List<InstanceContext>();
					_instancesByThread[instance.ConceptionThread] = threadList;
				}
				threadList.Add( instance );
			}
		}
		private static void RemoveInstanceToList( InstanceContext instance )
		{
			lock( _instancesByThread )
			{
				List<InstanceContext> threadList = _instancesByThread[instance.ConceptionThread];

				bool removed = threadList.Remove( instance );

				if( !removed )
				{
					throw new Exception( "Should have deleted instance" );
				}

				if( threadList.Count == 0 )
				{
					_instancesByThread.Remove( instance.ConceptionThread );
				}
			}
		}
		private static Dictionary<Thread, List<InstanceContext>> _instancesByThread = new Dictionary<Thread,List<InstanceContext>>();
	}
}
