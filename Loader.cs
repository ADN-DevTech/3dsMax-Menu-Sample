using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Max;

namespace ADNMenuSample
{
    /// <summary>
    /// Public managed c# class to load assembly 
    /// </summary>
   public static class Loader
    {
       /// <summary>
       /// Init point for assembly loader
       /// </summary>
        public static void AssemblyMain()
        {
            MenuUtilities.SetupMenuHandlers();
        }       
    }
    /// <summary>
    /// Utilitiy class to handle menus at global level.
    /// </summary>
    public class MenuUtilities
    {
        
        static IActionItem m_mouseAction;
        static IActionItem m_builtinVpConfigAction;

        /// <summary>
        /// Note, this method is iterating all action tbales and actions (for this example to also be able to find built-in actions).
        /// Normally, if you only need your own, you can use actionManager.FindTable(context); but when using the 
        /// CuiActionCommandAdapter, the context is not available, so this method is still the best option. 
        /// To find only your actions, if you make all your CuiActionCommandAdapter actions use the same category(s), 
        /// then you can use the table name to find your table (category), then iterate just your the commands.
        /// </summary>
        static void LookupActions()
        {
            var actionManager = GlobalInterface.Instance.COREInterface.ActionManager;
            //actionManager.FindTable();
            for (var actionTableIndex = 0; actionTableIndex < actionManager.NumActionTables; ++actionTableIndex)
            {
                var actionTable = actionManager.GetTable(actionTableIndex);
                //actionTable.
                for (var actionIndex = 0; actionIndex < actionTable.Count; ++actionIndex)
                {
                    var action = actionTable[actionIndex];
                    // finds our known cui action.
                    if (action != null && action.DescriptionText == AdnMenuSampleStrings.actionText01)
                    {
                        m_mouseAction = action;
                        uint n = actionTable.ContextId;
                    }
                    // finds a known the built-in action by string
                    else if (action != null && action.DescriptionText == "Viewport Configuration")
                    {
                        m_builtinVpConfigAction = action;
                    }
                }
            }
        }

        static GlobalDelegates.Delegate5 m_MenuPostLoadDelegate;
        static GlobalDelegates.Delegate5 m_MenuPreSaveDelegate;
        static GlobalDelegates.Delegate5 m_MenuPostSaveDelegate;
        static GlobalDelegates.Delegate5 m_SystemStartupDelegate;

        static string menuName = "ADN SampleMenu";
        private static void MenuPostLoadHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(menuName);
            if (menu == null)
                InstallMenu();
        }
        private static void MenuPreSaveHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(menuName);
            if (menu != null)
                RemoveMenu(menuName);
        }
        private static void MenuPostSaveHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(menuName);
            if (menu == null)
                InstallMenu();
        }
        private static void MenuSystemStartupHandler(IntPtr objPtr, INotifyInfo infoPtr)
        {
            var global = GlobalInterface.Instance;
            var ip = global.COREInterface13;
            IIMenuManager manager = ip.MenuManager;
            IIMenu menu = manager.FindMenu(menuName);
            if (menu == null)
                InstallMenu();
        }

        /// <summary>
        /// This method setups all possible callbacks to handle menus add/remove in various user actions. For example, 
        /// if the user changes workspaces, the entire menu bar is updated, so this handles adding it in all workspaces as switched.
        /// The drawback is that 3ds Max calls some more than once, so you get some seemingly unnecessary add/remove. But it's safer 
        /// if you always want your menu present.
        /// Of course you could also call the add/remove in other conexts and callbacks depending on the 3ds max state where 
        /// you need your menu to display.
        /// </summary>
        public static void SetupMenuHandlers()
        {
            var global = GlobalInterface.Instance;
            m_MenuPostLoadDelegate = new GlobalDelegates.Delegate5(MenuPostLoadHandler);
            m_MenuPreSaveDelegate = new GlobalDelegates.Delegate5(MenuPreSaveHandler);
            m_MenuPostSaveDelegate = new GlobalDelegates.Delegate5(MenuPostSaveHandler);
            m_SystemStartupDelegate = new GlobalDelegates.Delegate5(MenuSystemStartupHandler);
            global.RegisterNotification(m_MenuPostLoadDelegate, null, SystemNotificationCode.CuiMenusPostLoad);
            global.RegisterNotification(m_MenuPreSaveDelegate, null, SystemNotificationCode.CuiMenusPreSave);
            global.RegisterNotification(m_MenuPostSaveDelegate, null, SystemNotificationCode.CuiMenusPostSave);

            // this will add it at startup and for some scenerios is enough. But a commercial app shuold consider above for workspace switching.
            global.RegisterNotification(m_SystemStartupDelegate, null, SystemNotificationCode.SystemStartup);
        }
        /// <summary>
        /// Installs the menu from scratch
        /// </summary>
        /// <returns>1 when successfully installed, or 0 in error state</returns>
        private static uint InstallMenu()
        {
            try
            {
                LookupActions();

                IGlobal global = GlobalInterface.Instance;
                IIActionManager actionManager = global.COREInterface.ActionManager;
                IIMenuManager menuManager = global.COREInterface.MenuManager;

                // this only needs to be done once
                global.COREInterface.MenuManager.RegisterMenuBarContext(0x58527952, menuName);
                IIMenu mainMenuBar = menuManager.MainMenuBar;
                IIMenu adnSampleMenu = global.IMenu;
                adnSampleMenu.Title = menuName;
                menuManager.RegisterMenu(adnSampleMenu, 0);

                // Launch option
                {
                    IIMenuItem menuItem1 = global.IMenuItem;
                    menuItem1.ActionItem = m_mouseAction; // uses text from ActionItem.DescriptionText
                    adnSampleMenu.AddItem(menuItem1, -1);

                    IIMenuItem menuItem2 = global.IMenuItem;
                    menuItem2.ActionItem = m_builtinVpConfigAction;
                    menuItem2.Title = "ADN Menu Sample - " + menuItem2.ActionItem.DescriptionText; // just to show you can override the text, too.
                    menuItem2.UseCustomTitle = true;
                    adnSampleMenu.AddItem(menuItem2, -1);
                }
                // }
                IIMenuItem adnMenu = global.IMenuItem;
                adnMenu.Title = menuName;
                adnMenu.SubMenu = adnSampleMenu;
                menuManager.MainMenuBar.AddItem(adnMenu, -1);
                global.COREInterface.MenuManager.UpdateMenuBar();

            }
            catch
            {
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// removes the menu
        /// </summary>
        /// <param name="menuName"></param>
        private static void RemoveMenu(string menuName)
        {
            IGlobal global = GlobalInterface.Instance;
            IIActionManager actionManager = global.COREInterface.ActionManager;
            IIMenuManager menuManager = global.COREInterface.MenuManager;
            IIMenu customMenu = menuManager.FindMenu(menuName);

            menuManager.UnRegisterMenu(customMenu);
            global.ReleaseIMenu(customMenu);
            customMenu = null;
        }

    }

}
