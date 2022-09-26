/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace ProgrammersInc.Utility.Threading
{
	/// <summary>
	/// Similar to lock but only will try to lock.
	/// </summary>
	public static class TryLock
	{
		/// <summary>
		/// Action to follow depending on success or failure context.
		/// </summary>
		public delegate void LockAction();

		/// <summary>
		/// Trys to lock <paramref name="lockObject"/>, if successful it will call <paramref name="successAction"/>
		/// </summary>
		/// <param name="lockObject">Object to lock</param>
		/// <param name="successAction">Action to call if locked</param>
		/// <returns>True if locked, otherwise false</returns>
		public static bool Lock( object lockObject, LockAction successAction )
		{
			if( successAction == null )
				throw new ArgumentNullException( "successAction" );

			return Lock( lockObject, successAction, null );
		}

		/// <summary>
		/// Trys to lock <paramref name="lockObject"/>, if successful it will call <paramref name="successAction"/>
		/// otherwise it will call it will call <paramref name="failureAction"/>
		/// </summary>
		/// <param name="lockObject">Object to lock</param>
		/// <param name="successAction">Action to call if locked</param>
		/// <param name="failureAction">Action to call if lock failed</param>
		/// <returns>True if locked, otherwise false</returns>
		public static bool Lock( object lockObject, LockAction successAction, LockAction failureAction )
		{
			if( lockObject == null )
				throw new ArgumentNullException( "lockObject" ); 

			if( Monitor.TryEnter( lockObject ) )
			{
				try
				{
					if( successAction != null )
					{
						successAction();
					}
					return true;
				}
				finally
				{
					Monitor.Exit( lockObject );
				}
			}
			else
			{
				if( failureAction != null )
				{
					failureAction();
				}
			}
			return false;
		}
	}
}
