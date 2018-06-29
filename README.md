3dsMax-Menu-Sample
=======================

3ds Max Menu Sample .NET API plug-in

This sample code shows how to install/remove a menu from the .NET API. It runs at 3ds Max startup and also handles workspace switching.

Getting Started
============
The plugin source code and project was built and tested against 3ds Max 2019. In the project file you will find 
environment variable $(ADSK_3DSMAX_SDK_2019) referring to the 3ds Max .NET assemblies. You an change this to match 
the version of 3ds Max you are working with. It should work back to 3ds Max 2015. You will also have to adjust the 
.NET Framework version to match the 3ds Max version.

Note that it handles several menu related callbacks to handle several different situations, including the switching of 
workspaces to ensure menu is always present when the plugin is present, but it also removes itself when plugin is not 
present (so if plugin is uninstalled, the menu is also no longer present). You may want to test and experiment with 
your specific use cases (for example if this is in-house and uninstall is not an issue, you may not want to "remove" 
the menu ever.

Additional Information
=================
This plug-in was written by Kevin Vandecar - Autodesk Developer Network.

Known Issues
===========
The code is generic to cover as many cases as possible. Please review and adjust for your specific scenario. For example, 
it is using callbacks to handle all possibilities for workspace switching (which will also change the menu bar and menus).
This code handles that and will insert the menu during the workspace switch. It also removes the menu at shutdown, so that 
if the plugin is uninstalled, it will not reappear the next time in the 3ds Max CUI setup.

Contact
======
For more information on developing with 3ds Max, please visit the 3ds Max Developer Center.
http://www.autodesk.com/develop3dsmax

Version
=======
1.0 - Initial Release

License
=======
MIT License.
