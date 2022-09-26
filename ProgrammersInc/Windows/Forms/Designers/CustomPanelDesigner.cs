using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace ProgrammersInc.Windows.Forms
{
    internal class CustomPanelDesigner : ParentControlDesigner
    {
        #region Constructors
        /// <summary>
        /// Crea una nueva instancia de la clase <see cref="CustomPanelDesigner"/>.
        /// </summary>
        public CustomPanelDesigner() { }
        #endregion

        #region Properties
        /// <summary>
        /// Obtiene las acciones en tiempo de diseño que admite el componente asociado al diseñador.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection actionLists = new DesignerActionListCollection();

                actionLists.Add(new PanelDesignerActionList(Component));

                return actionLists;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Inicializa el diseñador con el componente especificado.
        /// </summary>
        /// <param name="component">Componente a asociar al diseñador.</param>
        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
        }
        #endregion
    }
}
