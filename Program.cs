#region Copyright
//      .NET Sample
//
//      Copyright (c) 2012 by Autodesk, Inc.
//
//      Permission to use, copy, modify, and distribute this software
//      for any purpose and without fee is hereby granted, provided
//      that the above copyright notice appears in all copies and
//      that both that copyright notice and the limited warranty and
//      restricted rights notice below appear in all supporting
//      documentation.
//
//      AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
//      AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
//      MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
//      DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
//      UNINTERRUPTED OR ERROR FREE.
//
//      Use, duplication, or disclosure by the U.S. Government is subject to
//      restrictions set forth in FAR 52.227-19 (Commercial Computer
//      Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
//      (Rights in Technical Data and Computer Software), as applicable.
//
#endregion

#region imports
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Runtime.InteropServices;
using System.Diagnostics;

using UiViewModels.Actions;
using Autodesk.Max;
using ManagedServices;
#endregion

namespace ADNMenuSample
{
    public class AdnMenuSampleStrings
    {
        // just convienence for globals strings. Normally strings would probably be loaded from resources
        public static string actionText01 = "ADN Menu Sample - Mouse Pick";
        public static string actionCategory = "ADN Samples";
    }

    public class AdnMouseCallBack : Autodesk.Max.Plugins.MouseCallBack
    {
        public override int Proc(IntPtr hwnd, int msg, int point, int flags, IIPoint2 m)
        {
            Debug.Print("viewport point: " + m.X + ", " + m.Y);
            switch (msg)
            {
                case 0: // MOUSE_ABORT see .\maxsdk\include\mouseman.h for all possible values
                    {
                        IGlobal global = Autodesk.Max.GlobalInterface.Instance;
                        IInterface14 ip = global.COREInterface14;
                        ip.PopCommandMode();
                        Debug.Print("mouse callback aborted");
                    }
                    break;
                case 1: // MOUSE_POINT see .\maxsdk\include\mouseman.h for all possible values
                    {
                        if (point == 0)
                        {
                            IGlobal global = Autodesk.Max.GlobalInterface.Instance;
                            IInterface14 ip = global.COREInterface14;
                            IViewExp vp = ip.ActiveViewExp;
                            IPoint3 pt = vp.SnapPoint(m, m, null, SnapFlags.InPlane);
                            ip.PopCommandMode();
                            Debug.Print("3d point (in plane): " + pt.X + ", " + pt.Y + ", " + pt.Z);
                        }
                        // else if ... other points if you have more than one...
                    }
                    break;
                default:
                    break;
            }
            return 1;
        }
    }

    public class AdnCommandMode : Autodesk.Max.Plugins.CommandMode
    {
         public override bool ChangeFG(ICommandMode oldMode)
        {
            return false;
        }

        public override IChangeForegroundCallback ChangeFGProc
        {
            get { return null; }
        }

        public override int Class
        {
            get { return 17; } //PICK_COMMAND	17, see .\maxsdk\include\cmdmode.h
        }

        public override void EnterMode()
        {
            IGlobal global = Autodesk.Max.GlobalInterface.Instance;
            IInterface14 ip = global.COREInterface14;
            ip.PushPrompt("Select a point:");
            return;
        }

        public override void ExitMode()
        {
            IGlobal global = Autodesk.Max.GlobalInterface.Instance;
            IInterface14 ip = global.COREInterface14;
            ip.PopPrompt();
            return;
        }

        public override int Id
        {
            get { return 0x6c2e0068; } // used gencid to create this.
        }

        public override IMouseCallBack MouseProc(IntPtr numPoints)
        {
            int npoints = 1; // in this sample we want ONLY 1 point.
            numPoints = new IntPtr(npoints);
            return new AdnMouseCallBack(); // return a new instance of our MouseCallBack class.
        }
    }

    /// <summary>
    /// Example CuiActionCommandAdapter setup
    /// </summary>
    public class AdnCuiSample00 : CuiActionCommandAdapter
    {
        public override void Execute(object parameter)
        {
            IGlobal global = Autodesk.Max.GlobalInterface.Instance;

            IInterface14 ip = global.COREInterface14;

            AdnCommandMode mode = new AdnCommandMode();
            ip.PushCommandMode(mode);

        }

        public override string InternalActionText
        {
            get { return AdnMenuSampleStrings.actionText01; }
        }

        public override string InternalCategory
        {
            get { return AdnMenuSampleStrings.actionCategory; }
        }

        public override string ActionText
        {
            get { return InternalActionText; }
        }

        public override string Category
        {
            get { return InternalCategory; }
        }
    }
}
