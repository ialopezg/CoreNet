/////////////////////////////////////////////////////////////////////////////
//
// (c) 2007 BinaryComponents Ltd.  All Rights Reserved.
//
// http://www.binarycomponents.com/
//
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Runtime.InteropServices;

namespace ProgrammersInc.Utility.Win32.Common
{
    /// <summary>
    /// Define un rectágulo usado por la API de Windows.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="RECT"/>.
        /// </summary>
        /// <param name="rect">Rectángulo que identificará esta estructura.</param>
        public RECT(Rectangle rect)
        {
            Left = rect.Left;
            Top = rect.Top;
            Right = rect.Right;
            Bottom = rect.Bottom;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene el área del rectángulo que define esta instancia.
        /// </summary>
        public Rectangle Rect
        {
            get { return new Rectangle(Left, Top, Right - Left, Bottom - Top); }
        }

        /// <summary>
        /// Obtiene el punto coordenado que identifica esta estructura.
        /// </summary>
        public Point Location
        {
            get { return new Point(Left, Top); }
        }
        #endregion

        #region Fields
        /// <summary>
        /// Punto coordenado izquierdo.
        /// </summary>
        public int Left;
        /// <summary>
        /// Punto coordenado superior.
        /// </summary>
        public int Top;
        /// <summary>
        /// Punto coordenado derecho.
        /// </summary>
        public int Right;
        /// <summary>
        /// Punto coordenado inferior.
        /// </summary>
        public int Bottom;
        #endregion
    }

    /// <summary>
    /// Define un punto coordenado que puede ser utilizado por el API de Windows.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        #region Constructors
        /// <summary>
        /// Inicializa una nueva instancia de la estructura <see cref="POINT"/>.
        /// </summary>
        /// <param name="x">Coordenada del eje X.</param>
        /// <param name="y">Coordenada del eje Y.</param>
        public POINT(Int32 x, Int32 y)
        {
            X = x;
            Y = y;
        }
        #endregion

        #region Fields
        /// <summary>
        /// Coordenada del eje X.
        /// </summary>
        public Int32 X;
        /// <summary>
        /// Coordenada del eje Y.
        /// </summary>
        public Int32 Y;
        #endregion
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public SIZE(Int32 cx, Int32 cy)
        {
            CX = cx;
            CY = cy;
        }

        public Int32 CX;
        public Int32 CY;
    }
}
