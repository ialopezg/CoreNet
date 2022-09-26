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
using System.Windows.Forms;
using System.Xml;
using System.Globalization;

namespace ProgrammersInc.WinFormsUtility
{
	public abstract class ControlPreferences : IDisposable
	{
		#region Watch

		public abstract class Watch
		{
			protected Watch( Control control, string id )
			{
				if( control == null )
				{
					throw new ArgumentNullException( "control" );
				}
				if( id == null )
				{
					throw new ArgumentNullException( "id" );
				}

				_control = control;
				_id = id;
			}

			public ControlPreferences ControlPreferences
			{
				get
				{
					return _controlPreferences;
				}
				set
				{
					if( _controlPreferences != null )
					{
						Detach();
					}

					_controlPreferences = value;

					if( _controlPreferences != null )
					{
						Attach();
					}
				}
			}

			public Control Control
			{
				get
				{
					return _control;
				}
			}

			public void DoRead()
			{
				if( _controlPreferences._updating.IsActive )
				{
					return;
				}

				using( _controlPreferences._updating.Apply() )
				{
					Read();
				}

				_hasRead = true;
			}

			public void DoWrite()
			{
				if( !_hasRead || _controlPreferences._updating.IsActive )
				{
					return;
				}

				using( _controlPreferences._updating.Apply() )
				{
					Write();
				}
			}

			protected string Name
			{
				get
				{
					return string.Format( "{0}.{1}{2}", _id, _control.Name, ScreenSig );
				}
			}

			public static string ScreenSigOverride
			{
				get
				{
					return _screenSigOverride;
				}
				set
				{
					_screenSigOverride = value;
				}
			}

			public static string ScreenSig
			{
				get 
				{
					if( ScreenSigOverride == null )
					{
						string screenSig = string.Empty;

						foreach( Screen screen in Screen.AllScreens )
						{
							screenSig += string.Format( CultureInfo.InvariantCulture, "-{0},{1},{2},{3}", screen.Bounds.X, screen.Bounds.Y, screen.Bounds.Width, screen.Bounds.Height );
						}
						return screenSig;
					}
					else
					{
						return ScreenSigOverride;
					}
				}
			}

			protected abstract void Read();
			protected abstract void Write();

			protected virtual void OnRegistered()
			{
				_control.Disposed += new EventHandler( _control_Disposed );
				_control.HandleCreated += new EventHandler( _control_HandleCreated );
				_control.HandleDestroyed += new EventHandler( _control_HandleDestroyed );
				_control.VisibleChanged += new EventHandler( _control_VisibleChanged );
			}

			protected virtual void OnUnregistered()
			{
				DoWrite();

				_control.Disposed -= new EventHandler( _control_Disposed );
				_control.HandleCreated -= new EventHandler( _control_HandleCreated );
				_control.HandleDestroyed -= new EventHandler( _control_HandleDestroyed );
				_control.VisibleChanged -= new EventHandler( _control_VisibleChanged );
			}

			protected virtual void OnHandleCreated()
			{
				DoRead();
			}

			protected virtual void OnHandleDestroyed()
			{
				DoWrite();
			}

			protected virtual void OnVisibleSet()
			{
				DoRead();
			}

			protected virtual void OnVisibleUnset()
			{
				DoWrite();
			}

			private void _control_HandleCreated( object sender, EventArgs e )
			{
				OnHandleCreated();
			}

			private void _control_HandleDestroyed( object sender, EventArgs e )
			{
				OnHandleDestroyed();
			}

			private void _control_VisibleChanged( object sender, EventArgs e )
			{
				if( _control.Visible )
				{
					OnVisibleSet();
				}
				else
				{
					OnVisibleUnset();

					if( _controlPreferences != null )
					{
						RecurseSendNonVisibleEvents( _control );
					}
				}
			}

			private void RecurseSendNonVisibleEvents( Control control )
			{
				foreach( Control child in control.Controls )
				{
					Watch w = _controlPreferences.GetWatch( child );

					if( w != null )
					{
						w.OnVisibleUnset();
					}

					RecurseSendNonVisibleEvents( child );
				}
			}

			private void _control_Disposed( object sender, EventArgs e )
			{
				if( _controlPreferences.IsControlRegistered( _control ) )
				{
					_controlPreferences.UnregisterWatch( _control );
				}

				Detach();
			}

			private void Attach()
			{
				if( !_attached )
				{
					OnRegistered();

					_attached = true;
				}
			}

			private void Detach()
			{
				if( _attached )
				{
					OnUnregistered();

					_attached = false;
				}
			}

			private static string _screenSigOverride;

			private Control _control;
			private ControlPreferences _controlPreferences;
			private string _id;
			private bool _attached, _hasRead;
		}

		#endregion

		#region FormWatch

		public class FormWatch : Watch
		{
			public FormWatch( Form form )
				: base( form, "FORM" )
			{
				_form = form;
			}


			protected override void Read()
			{
				FormWindowState? formWindowState = null;

				string windowState = ControlPreferences.GetValue( Name, "State" );

				if( windowState != null )
				{
					formWindowState = (FormWindowState) Enum.Parse( typeof( FormWindowState ), windowState );
				}

				bool sizable = _form.FormBorderStyle == FormBorderStyle.Sizable || _form.FormBorderStyle == FormBorderStyle.SizableToolWindow;

				string xs = ControlPreferences.GetValue( Name, "X" );
				string ys = ControlPreferences.GetValue( Name, "Y" );
				string widths = ControlPreferences.GetValue( Name, "Width" );
				string heights = ControlPreferences.GetValue( Name, "Height" );

				if( xs != null && ys != null )
				{
					int x = int.Parse( xs, Culture );
					int y = int.Parse( ys, Culture );
					int width;
					int height;


					if( x < -30000 )
					{
						x = 50;
					}
					if( y < -30000 )
					{
						y = 0;
					}

					if( sizable && widths != null && heights != null )
					{
						width = int.Parse( widths, Culture );
						height = int.Parse( heights, Culture );
					}
					else
					{
						width = _form.Width;
						height = _form.Height;
					}

					System.Drawing.Rectangle formBounds = new System.Drawing.Rectangle( x, y, width, height );

					if( this.IsRectangleVisible( formBounds ) )
					{
						_form.DesktopBounds = formBounds;

						if( _form.WindowState == FormWindowState.Normal )
						{
							Utility.Win32.User.SetWindowPos
								( _form.Handle, IntPtr.Zero, x + SystemInformation.WorkingArea.X, y + SystemInformation.WorkingArea.Y, width, height
								, Utility.Win32.SetWindowPosOptions.SWP_NOSIZE );
						}
					}
				}

				if( formWindowState != null )
				{
					SetWindowState( _form, formWindowState.Value );
				}
				else
				{
					SetWindowDefaultState( _form );
				}
			}

			protected override void Write()
			{
				if( _form.WindowState != FormWindowState.Minimized )
				{
					ControlPreferences.SetValue( Name, "State", _form.WindowState.ToString() );
				}

				if( _form.WindowState == FormWindowState.Normal )
				{
					int x = _form.DesktopBounds.X;
					int y = _form.DesktopBounds.Y;

					if( x < -30000 )
					{
						x = 50;
					}
					if( y < -30000 )
					{
						y = 50;
					}

					ControlPreferences.SetValue( Name, "X", x.ToString( Culture ) );
					ControlPreferences.SetValue( Name, "Y", y.ToString( Culture ) );

					if( _form.FormBorderStyle == FormBorderStyle.Sizable || _form.FormBorderStyle == FormBorderStyle.SizableToolWindow )
					{
						ControlPreferences.SetValue( Name, "Width", _form.DesktopBounds.Width.ToString( Culture ) );
						ControlPreferences.SetValue( Name, "Height", _form.DesktopBounds.Height.ToString( Culture ) );
					}
				}
				else
				{
					ControlPreferences.SetValue( Name, "X", _form.RestoreBounds.X.ToString( Culture ) );
					ControlPreferences.SetValue( Name, "Y", _form.RestoreBounds.Y.ToString( Culture ) );

					if( _form.FormBorderStyle == FormBorderStyle.Sizable || _form.FormBorderStyle == FormBorderStyle.SizableToolWindow )
					{
						ControlPreferences.SetValue( Name, "Width", _form.RestoreBounds.Width.ToString( Culture ) );
						ControlPreferences.SetValue( Name, "Height", _form.RestoreBounds.Height.ToString( Culture ) );
					}
				}
			}

			private bool IsRectangleVisible( System.Drawing.Rectangle rc )
			{
				foreach( Screen screen in Screen.AllScreens )
				{
					if( screen.WorkingArea.IntersectsWith( rc ) )
					{
						return true;
					}
				}
				return false;
			}

			protected virtual void SetWindowState( Form form, FormWindowState formWindowState )
			{
				if( _form.WindowState != formWindowState )
				{
					_form.WindowState = formWindowState;
				}
			}

			protected virtual void SetWindowDefaultState( Form form )
			{
			}

			private Form _form;
		}

		#endregion

		#region SplitContainerWatch

		public class SplitContainerWatch : Watch
		{
			public SplitContainerWatch( SplitContainer splitContainer, string id )
				: base( splitContainer, id )
			{
				_splitContainer = splitContainer;
			}

			protected override void Read()
			{
				string splitterDistance = ControlPreferences.GetValue( Name, "SplitterDistance" );

				if( splitterDistance != null )
				{
					try
					{
						_splitContainer.SplitterDistance = int.Parse( splitterDistance, Culture );
					}
					catch
					{
					}
				}
			}

			protected override void Write()
			{
				ControlPreferences.SetValue( Name, "SplitterDistance", _splitContainer.SplitterDistance.ToString( Culture ) );
			}

			private SplitContainer _splitContainer;
		}

		#endregion

		#region TabControlWatch

		public class TabControlWatch : Watch
		{
			public TabControlWatch( TabControl tabControl, string id )
				: base( tabControl, id )
			{
				_tabControl = tabControl;
			}

			protected override void Read()
			{
				string selectedIndex = ControlPreferences.GetValue( Name, "SelectedIndex" );

				if( selectedIndex != null )
				{
					_tabControl.SelectedIndex = int.Parse( selectedIndex, Culture );
				}
			}

			protected override void Write()
			{
				if( _tabControl.SelectedIndex >= 0 )
				{
					ControlPreferences.SetValue( Name, "SelectedIndex", _tabControl.SelectedIndex.ToString( Culture ) );
				}
			}

			private TabControl _tabControl;
		}

		#endregion

		#region TreeViewWatch

		public class TreeViewWatch : Watch
		{
			public TreeViewWatch( System.Windows.Forms.TreeView treeView, string id )
				: base( treeView, id )
			{
				_treeView = treeView;
				_timer = new Timer();

				_timer.Interval = 10;
			}

			protected override void Read()
			{
				if( !_treeView.IsHandleCreated || !_treeView.Visible )
				{
					if(_timer.Enabled)
					{
						return;
					}

					_timer.Tick += new EventHandler( _timer_Tick );
					_timer.Enabled = true;
				}

				string selectedPath = ControlPreferences.GetValue( Name, "Selection" );

				ReadNodes( string.Empty, _treeView.Nodes, selectedPath );
			}

			protected override void Write()
			{
				WriteNodes( string.Empty, _treeView.Nodes );
			}

			protected virtual string GetNodeText( TreeNode node )
			{
				return node.Text;
			}

			private void _timer_Tick( object sender, EventArgs e )
			{
				_timer.Enabled = true;
				_timer.Tick -= new EventHandler( _timer_Tick );

				string selectedPath = ControlPreferences.GetValue( Name, "Selection" );

				ReadNodes( string.Empty, _treeView.Nodes, selectedPath );
			}

			private void ReadNodes( string path, System.Windows.Forms.TreeNodeCollection nodes, string selectedPath )
			{
				foreach( System.Windows.Forms.TreeNode node in nodes )
				{
					string childPath = path + "/" + GetNodeText( node ).Replace( '/', '_' );
					bool expand = ControlPreferences.GetValue( Name, childPath ) == "Expanded";
					bool collapse = ControlPreferences.GetValue( Name, childPath ) == "Collapsed";
					bool select = (childPath == selectedPath);

					if( expand )
					{
						node.Expand();

						ReadNodes( childPath, node.Nodes, selectedPath );
					}
					else if( collapse )
					{
						node.Collapse();
					}
					if( select )
					{
						_treeView.SelectedNode = node;
					}
				}
			}

			private void WriteNodes( string path, System.Windows.Forms.TreeNodeCollection nodes )
			{
				foreach( System.Windows.Forms.TreeNode node in nodes )
				{
					string childPath = path + "/" + GetNodeText( node ).Replace( '/', '_' );

					if( node.IsSelected )
					{
						ControlPreferences.SetValue( Name, "Selection", childPath );
					}

					if( node.Nodes.Count > 0 )
					{
						if( node.IsExpanded )
						{
							ControlPreferences.SetValue( Name, childPath, "Expanded" );

							WriteNodes( childPath, node.Nodes );
						}
						else
						{
							ControlPreferences.SetValue( Name, childPath, "Collapsed" );
						}
					}
				}
			}

			private System.Windows.Forms.TreeView _treeView;
			private Timer _timer;
		}

		#endregion

		#region ListViewWatch

		public class ListViewWatch : Watch
		{
			public ListViewWatch( ListView listView, string id )
				: base( listView, id )
			{
				_listView = listView;
			}

			protected override void Read()
			{
				foreach( ColumnHeader header in _listView.Columns )
				{
					string width = ControlPreferences.GetValue( Name, header.Name );

					if( width != null )
					{
						header.Width = int.Parse( width );
					}
				}
			}

			protected override void Write()
			{
				foreach( ColumnHeader header in _listView.Columns )
				{
					//
					// Column must have a name for info to be written.
					System.Diagnostics.Debug.Assert( !string.IsNullOrEmpty( header.Name ) );
					ControlPreferences.SetValue( Name, header.Name, header.Width.ToString() );
				}
			}

			private ListView _listView;
		}

		#endregion

		protected ControlPreferences()
		{
		}

		public bool IsControlRegistered( Control c )
		{
			return _watches.ContainsKey( c );
		}

		public Watch GetWatch( Control c )
		{
			Watch w;

			if( _watches.TryGetValue( c, out w ) )
			{
				return w;
			}
			else
			{
				return null;
			}
		}

		public void RegisterWatches( ICollection<Watch> watches )
		{
			foreach( Watch watch in watches )
			{
				RegisterWatch( watch );
			}
		}

		public void RegisterWatch( Watch watch )
		{
			if( watch == null )
			{
				throw new ArgumentNullException( "watch" );
			}
			if( _watches.ContainsKey( watch.Control ) )
			{
				throw new InvalidOperationException( "Watch already registered with control preferences." );
			}

			watch.ControlPreferences = this;

			_watches.Add( watch.Control, watch );

			ReadControlState( watch.Control );
		}

		public void UnregisterWatches( ICollection<Control> controls )
		{
			foreach( Control control in controls )
			{
				UnregisterWatch( control );
			}
		}

		public void UnregisterWatch( Control control )
		{
			if( control == null )
			{
				throw new ArgumentNullException( "control" );
			}

			if( !_watches.ContainsKey( control ) )
			{
				throw new InvalidOperationException( "Control not registered with control preferences." );
			}

			Watch watch = _watches[control];

			_watches.Remove( control );

			watch.ControlPreferences = null;
		}

		public void ReadControlState( Control control )
		{
			if( control == null )
			{
				throw new ArgumentNullException( "control" );
			}
			if( !_watches.ContainsKey( control ) )
			{
				throw new InvalidOperationException( "Control not registered with control preferences." );
			}

			if( _updating.IsActive )
			{
				return;
			}

			_watches[control].DoRead();
		}

		public void WriteControlState( Control control )
		{
			if( control == null )
			{
				throw new ArgumentNullException( "control" );
			}
			if( !_watches.ContainsKey( control ) )
			{
				throw new InvalidOperationException( "Control not registered with control preferences." );
			}

			if( _updating.IsActive )
			{
				return;
			}

			_watches[control].DoWrite();
		}

		public void Load()
		{
			List<KeyValuePair<Control, Watch>> watches = new List<KeyValuePair<Control, Watch>>();

			watches.AddRange( _watches );

			foreach( KeyValuePair<Control, Watch> kvp in watches )
			{
				if( kvp.Key.FindForm() != null )
				{
					kvp.Value.DoRead();
				}
			}
		}

		public abstract void Save();

		#region IDisposable Members

		public virtual void Dispose()
		{
			foreach( KeyValuePair<Control, Watch> kvp in _watches )
			{
				if( kvp.Key.FindForm() != null )
				{
					kvp.Value.DoWrite();
				}
			}

			_watches.Clear();
		}

		#endregion

		public static System.Globalization.CultureInfo Culture
		{
			get
			{
				return System.Globalization.CultureInfo.InvariantCulture;
			}
		}

		public string GetValue( string name, string key )
		{
			if( _values.ContainsKey( name ) )
			{
				Dictionary<string, string> map = _values[name];

				if( map.ContainsKey( key ) )
				{
					string v = map[key];

					if( v == string.Empty )
					{
						v = null;
					}

					return v;
				}
			}

			return null;
		}

		public virtual void SetValue( string name, string key, string value )
		{
			Dictionary<string, string> map;

			if( _values.ContainsKey( name ) )
			{
				map = _values[name];
			}
			else
			{
				map = new Dictionary<string, string>();
				_values[name] = map;
			}

			map[key] = value;
		}

		protected System.Collections.IEnumerable Names
		{
			get
			{
				return _values.Keys;
			}
		}

		protected System.Collections.IEnumerable GetKeysForName( string name )
		{
			return _values[name].Keys;
		}

		private Dictionary<string, Dictionary<string, string>> _values = new Dictionary<string, Dictionary<string, string>>();
		private Dictionary<Control, Watch> _watches = new Dictionary<Control, Watch>();
		private Utility.Control.Flag _updating = new Utility.Control.Flag();
	}

	public class FileControlPreferences : ControlPreferences
	{
		public FileControlPreferences( string filename )
		{
			if( filename == null )
			{
				throw new ArgumentNullException( "filename" );
			}

			_filename = filename;

			if( System.IO.File.Exists( _filename ) )
			{
				try
				{
					XmlDocument xmlDoc = new XmlDocument();

					xmlDoc.Load( filename );

					foreach( XmlNode itemNode in xmlDoc.SelectNodes( "/ControlPreferences/Item" ) )
					{
						string name = itemNode.Attributes["Name"].Value;

						foreach( XmlNode kvpNode in itemNode.SelectNodes( "Setting" ) )
						{
							SetValue( name, kvpNode.Attributes["Key"].Value, kvpNode.InnerText );
						}
					}
				}
				catch
				{
				}
			}
		}

		public override void SetValue( string name, string key, string value )
		{
			base.SetValue( name, key, value );

			_changed = true;
		}

		public override void Dispose()
		{
			base.Dispose();

			Save();
		}

		public override void Save()
		{
			if( !_changed )
			{
				return;
			}

			try
			{
				System.IO.Directory.CreateDirectory( System.IO.Path.GetDirectoryName( _filename ) );

				using( XmlTextWriter tw = new XmlTextWriter( _filename, Encoding.UTF8 ) )
				{
					tw.Formatting = Formatting.Indented;

					tw.WriteStartDocument();
					tw.WriteStartElement( "ControlPreferences" );

					foreach( string name in Names )
					{
						tw.WriteStartElement( "Item" );
						tw.WriteAttributeString( "Name", name );

						foreach( string key in GetKeysForName( name ) )
						{
							tw.WriteStartElement( "Setting" );
							tw.WriteAttributeString( "Key", key );
							tw.WriteString( GetValue( name, key ) );
							tw.WriteEndElement();
						}

						tw.WriteEndElement();
					}

					tw.WriteEndElement();
				}

				_changed = false;
			}
			catch
			{
			}
		}

		private string _filename;
		private bool _changed;
	}
}
