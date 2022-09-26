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

namespace ProgrammersInc.WinFormsUtility
{
    public sealed class ControlSearch
    {
        public Control FindControl( Control parent, string name )
        {
            if( _found == null )
            {
                _found = new Dictionary<Control, Dictionary<string, Control>>();
            }

            Dictionary<string, Control> map = null;

            if( _found.ContainsKey( parent ) )
            {
                map = _found[parent];
            }
            else
            {
                map = new Dictionary<string, Control>();

                RecurseControls( parent, map );

                _found[parent] = map;
            }

            if( map.ContainsKey( name ) )
            {
                return map[name];
            }
            else
            {
                return null;
            }
        }

        private void RecurseControls( Control control, Dictionary<string, Control> map )
        {
            map[control.Name] = control;

            foreach( Control child in control.Controls )
            {
                RecurseControls( child, map );
            }
        }

        private Dictionary<Control, Dictionary<string, Control>> _found;
    }
}
