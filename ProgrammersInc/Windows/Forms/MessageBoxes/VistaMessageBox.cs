using System;
using System.Windows.Forms;

namespace ProgrammersInc.Windows.Forms
{
    /// <summary>
    /// Clase que emula los MessageBox de Windows Vista.
    /// </summary>
    public class VistaMessageBox
    {
        /// <summary>
        /// Muestra un cuadro un cuadro de diálogo con un mensaje al usuario.
        /// </summary>
        /// <param name="Title">Título del cuadro de mensaje.</param>
        /// <param name="MainInstruction">Instrucción principal.</param>
        /// <param name="Content">Contenido.</param>
        /// <param name="ExpandedInfo">Información oculta a expandir.</param>
        /// <param name="Footer">Texto al pie del cuadro de mensaje.</param>
        /// <param name="VerificationText">Texto para la casilla de verificación.</param>
        /// <param name="Buttons">Botones de acción a mostrarse.</param>
        /// <param name="MainIcon">Icono Principal.</param>
        /// <param name="FooterIcon">Icono al pie.</param>
        /// <returns>Un valor que representará el ícono sobre el que el usuario hizo clic.</returns>
        static public DialogResult Show(string Title, string MainInstruction, string Content, string ExpandedInfo,
                                          string Footer, string VerificationText, TaskDialogButtons Buttons,
                                          SysIcons MainIcon, SysIcons FooterIcon)
        {
            return TaskDialog.ShowTaskDialogBox(Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                     "", "", Buttons, MainIcon, FooterIcon);
        }

        /// <summary>
        /// Muestra un cuadro un cuadro de diálogo con un mensaje al usuario.
        /// </summary>
        /// <param name="Title">Título del cuadro de mensaje.</param>
        /// <param name="MainInstruction">Instrucción principal.</param>
        /// <param name="Content">Contenido.</param>
        /// <param name="Buttons">Botones de acción a mostrarse.</param>
        /// <param name="MainIcon">Icono Principal.</param>
        /// <param name="FooterIcon">Icono al pie.</param>
        /// <returns>Un valor que representará el ícono sobre el que el usuario hizo clic.</returns>
        static public DialogResult Show(string Title, string MainInstruction, string Content,
                                              TaskDialogButtons Buttons, SysIcons MainIcon)
        {
            return Show(Title, MainInstruction, Content, "", "", "", Buttons, MainIcon, SysIcons.Information);
        }
    }
}