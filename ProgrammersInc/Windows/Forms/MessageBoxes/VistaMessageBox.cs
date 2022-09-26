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
        /// Muestra un cuadro un cuadro de di�logo con un mensaje al usuario.
        /// </summary>
        /// <param name="Title">T�tulo del cuadro de mensaje.</param>
        /// <param name="MainInstruction">Instrucci�n principal.</param>
        /// <param name="Content">Contenido.</param>
        /// <param name="ExpandedInfo">Informaci�n oculta a expandir.</param>
        /// <param name="Footer">Texto al pie del cuadro de mensaje.</param>
        /// <param name="VerificationText">Texto para la casilla de verificaci�n.</param>
        /// <param name="Buttons">Botones de acci�n a mostrarse.</param>
        /// <param name="MainIcon">Icono Principal.</param>
        /// <param name="FooterIcon">Icono al pie.</param>
        /// <returns>Un valor que representar� el �cono sobre el que el usuario hizo clic.</returns>
        static public DialogResult Show(string Title, string MainInstruction, string Content, string ExpandedInfo,
                                          string Footer, string VerificationText, TaskDialogButtons Buttons,
                                          SysIcons MainIcon, SysIcons FooterIcon)
        {
            return TaskDialog.ShowTaskDialogBox(Title, MainInstruction, Content, ExpandedInfo, Footer, VerificationText,
                                     "", "", Buttons, MainIcon, FooterIcon);
        }

        /// <summary>
        /// Muestra un cuadro un cuadro de di�logo con un mensaje al usuario.
        /// </summary>
        /// <param name="Title">T�tulo del cuadro de mensaje.</param>
        /// <param name="MainInstruction">Instrucci�n principal.</param>
        /// <param name="Content">Contenido.</param>
        /// <param name="Buttons">Botones de acci�n a mostrarse.</param>
        /// <param name="MainIcon">Icono Principal.</param>
        /// <param name="FooterIcon">Icono al pie.</param>
        /// <returns>Un valor que representar� el �cono sobre el que el usuario hizo clic.</returns>
        static public DialogResult Show(string Title, string MainInstruction, string Content,
                                              TaskDialogButtons Buttons, SysIcons MainIcon)
        {
            return Show(Title, MainInstruction, Content, "", "", "", Buttons, MainIcon, SysIcons.Information);
        }
    }
}