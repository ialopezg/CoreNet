/*
 
 2008 Jos? Manuel Men?ndez Poo
 * 
 * Please give me credit if you use this code. It's all I ask.
 * 
 * Contact me for more info: menendezpoo@gmail.com
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    public class RibbonDesigner
        : ControlDesigner
    {
        #region Static

        public static int LoWord(int dwValue)
        {
            return dwValue & 0xFFFF;
        }
        public static int HiWord(int dwValue)
        {
            return (dwValue >> 16) & 0xFFFF;
        }


        #endregion

        #region Fields
        
        private IRibbonElement _selectedElement;

        #endregion

        #region Ctor
        public RibbonDesigner()
        {

        } 

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the currently selected RibbonElement
        /// </summary>
        public IRibbonElement SelectedElement
        {
            get { return _selectedElement; }
            set 
            { 
                _selectedElement = value;

                ISelectionService selector = GetService(typeof(ISelectionService)) as ISelectionService;
                
                if(selector != null && value != null)
                    selector.SetSelectedComponents(new Component[] { value as Component }, SelectionTypes.Primary);

                if (value is RibbonButton)
                {
                    (value as RibbonButton).ShowDropDown();
                }

                Ribbon.Refresh();
            }
        }

        /// <summary>
        /// Gets the Ribbon of the designer
        /// </summary>
        public Ribbon Ribbon
        {
            get { return Control as Ribbon; }
        } 

        #endregion

        #region Methods

        private void AssignEventHandler()
        {
            //TODO: Didn't work
            //if (SelectedElement == null) return;

            //IEventBindingService binder = GetService(typeof(IEventBindingService)) as IEventBindingService;

            //EventDescriptorCollection evts = TypeDescriptor.GetEvents(SelectedElement);

            

            ////string id = binder.CreateUniqueMethodName(SelectedElement as Component, evts["Click"]);

            //binder.ShowCode(SelectedElement as Component, evts["Click"]);
        }

        private void SelectRibbon()
        {
            ISelectionService selector = GetService(typeof(ISelectionService)) as ISelectionService;

            if (selector != null)
                selector.SetSelectedComponents(new Component[] { Ribbon }, SelectionTypes.Primary);

        }

        public override System.ComponentModel.Design.DesignerVerbCollection Verbs
        {
            get
            {
                DesignerVerbCollection verbs = new DesignerVerbCollection();

                verbs.Add(new DesignerVerb("Add Tab", new EventHandler(AddTabVerb)));


                return verbs;
            }
        }

        public void AddTabVerb(object sender, EventArgs e)
        {
            Ribbon r = Control as Ribbon;

            if (r != null)
            {
                IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost; if (host == null) return;

                RibbonTab tab = host.CreateComponent(typeof(RibbonTab)) as RibbonTab;

                if (tab == null) return;

                tab.Text = tab.Site.Name;

                Ribbon.Tabs.Add(tab);

                r.Refresh();
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.HWnd == Control.Handle)
            {
                switch (m.Msg)
                {
                    case 0x203: //WM_LBUTTONDBLCLK
                        AssignEventHandler();
                        break;
                    case 0x201: //WM_LBUTTONDOWN
                    case 0x204: //WM_RBUTTONDOWN
                        return;
                    case 0x202: //WM_LBUTTONUP
                    case 0x205: //WM_RBUTTONUP
                        HitOn(LoWord((int)m.LParam), HiWord((int)m.LParam));
                        return;
                    default:
                        break;
                }
            }


            base.WndProc(ref m);

        }

        public void HitOn(int x, int y)
        {
            if (Ribbon.Tabs.Count == 0 || Ribbon.ActiveTab == null)
            {
                SelectRibbon();
                return;
            }

            if (Ribbon != null)
            {
                if (Ribbon.TabHitTest(x, y))
                {
                    SelectedElement = Ribbon.ActiveTab;
                }
                else
                {
                    #region Tab ScrollTest

                    if (Ribbon.ActiveTab.TabContentBounds.Contains(x, y))
                    {
                        if (Ribbon.ActiveTab.ScrollLeftBounds.Contains(x,y) && Ribbon.ActiveTab.ScrollLeftVisible)
                        {
                            Ribbon.ActiveTab.ScrollLeft();
                            SelectedElement = Ribbon.ActiveTab;
                            return;
                        }

                        if (Ribbon.ActiveTab.ScrollRightBounds.Contains(x, y) && Ribbon.ActiveTab.ScrollRightVisible)
                        {
                            Ribbon.ActiveTab.ScrollRight();
                            SelectedElement = Ribbon.ActiveTab;
                            return;
                        }
                    }

                    #endregion

                    //Check Panel
                    if (Ribbon.ActiveTab.TabContentBounds.Contains(x, y))
                    {
                        RibbonPanel hittedPanel = null;

                        foreach (RibbonPanel panel in Ribbon.ActiveTab.Panels)
                            if (panel.Bounds.Contains(x, y))
                            {
                                hittedPanel = panel;
                                break;
                            }

                        if (hittedPanel != null)
                        {
                            //Check item
                            RibbonItem hittedItem = null;

                            foreach (RibbonItem item in hittedPanel.Items)
                                if (item.Bounds.Contains(x, y))
                                {
                                    hittedItem = item;
                                    break;
                                }

                            if (hittedItem != null && hittedItem is IContainsSelectableRibbonItems)
                            {
                                //Check subitem
                                RibbonItem hittedSubItem = null;

                                foreach (RibbonItem subItem in (hittedItem as IContainsSelectableRibbonItems).GetItems())
                                    if (subItem.Bounds.Contains(x, y))
                                    {
                                        hittedSubItem = subItem;
                                        break;
                                    }

                                if (hittedSubItem != null)
                                {
                                    SelectedElement = hittedSubItem;
                                }
                                else
                                {
                                    SelectedElement = hittedItem;
                                }
                            }
                            else if (hittedItem != null)
                            {
                                SelectedElement = hittedItem;
                            }
                            else
                            {
                                SelectedElement = hittedPanel;
                            }
                        }
                        else
                        {
                            SelectedElement = Ribbon.ActiveTab;
                        }
                    }
                    else
                    {
                        SelectRibbon();
                    }
                }
            }
        }

        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);


            using (Pen p = new Pen(Color.Black))
            {
                p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

                ISelectionService host = GetService(typeof(ISelectionService)) as ISelectionService;

                if (host != null)
                {
                    foreach (IComponent comp in host.GetSelectedComponents())
                    {
                        if (comp is IRibbonElement)
                        {
                            pe.Graphics.DrawRectangle(p, (comp as IRibbonElement).Bounds);
                        }
                    }
                }
            }
        } 

        #endregion

        #region Site

      

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            IComponentChangeService changeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            IDesignerEventService desigerEvt = GetService(typeof(IDesignerEventService)) as IDesignerEventService;

            changeService.ComponentRemoved += new ComponentEventHandler(changeService_ComponentRemoved);
        }


        public void changeService_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            RibbonTab tab = e.Component as RibbonTab;
            RibbonPanel panel = e.Component as RibbonPanel;
            RibbonItem item = e.Component as RibbonItem;

            IDesignerHost designerService = GetService(typeof(IDesignerHost)) as IDesignerHost;

            if (tab != null)
            {
                Ribbon.Tabs.Remove(tab);
            }
            else if (panel != null)
            {
                panel.OwnerTab.Panels.Remove(panel);
            }
            else if (item != null)
            {
                if (item.OwnerItem is RibbonItemGroup)
                {
                    (item.OwnerItem as RibbonItemGroup).Items.Remove(item);
                }
                else if (item.OwnerPanel != null)
                {
                    item.OwnerPanel.Items.Remove(item);
                }
            }

            RemoveRecursive(e.Component as IContainsRibbonComponents, designerService);
            
            SelectedElement = null;

            Ribbon.OnRegionsChanged();
        }

        public void RemoveRecursive(IContainsRibbonComponents item, IDesignerHost service)
        {
            if (item == null || service == null) return;

            foreach (Component c in item.GetAllChildComponents())
            {
                if (c is IContainsRibbonComponents)
                {
                    RemoveRecursive(c as IContainsRibbonComponents, service);
                }
                service.DestroyComponent(c);
            }
        }

        #endregion
    }
}
