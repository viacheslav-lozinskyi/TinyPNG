using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using resource.package;
using System;
using System.Collections.Generic;
using System.IO;
using static atom.Trace;
using static atom.Trace.NAME;

namespace resource.tool
{
    internal class VSTool
    {
        private static uint s_Events = 0;
        private static bool s_IsLoaded = false;
        private static bool s_IsTerminated = false;
        private static List<string> s_Files = new List<string>();

        private static string s_PreviewHtml =
            "<body style='owerflow:hidden; margin:5px; margin-bottom:0px; padding:0px;'>" +
            "  <h2 style='background-color:silver; margin:0px; padding-left:10px;'>TinyPNG</h2>" +
            "  <table>" +
            "    <tr>" +
            "    <td>" +
            "      <img style='margin:0px; padding:0px;' src='data:image/png;base64,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' height='145px'>" +
            "    </td>" +
            "    <td style='vertical-align:top; width:100%'>" +
            "      <h3 style='margin-top:5px; margin-bottom:10px; color:darkred;'>[[[Not optimized pictures are found in your solution]]].</h3>" +
            "      <hr style='margin:0px;'>" +
            "      <div style='margin-top:5px;'>[[[List of found pictures are below]]].</div>" +
            "      <div style='margin-top:5px;'>[[[You can reduce size of all those files in one click]]].</div>" +
            "      <div style='margin-top:30px; font-size:12px; color:silver;'>[[[Also information about optimized pictures is stored in]]] <i>$(SOLUTION_FOLDER)/tinypng.json</i></div>" +
            "    </td>" +
            "    </tr>" +
            "  </table>" +
            "</body>";

        private class SolutionEvents : IVsSolutionEvents
        {
            public int OnAfterOpenProject(IVsHierarchy hierarchy, int fAdded)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryCloseProject(IVsHierarchy hierarchy, int fRemoving, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeCloseProject(IVsHierarchy hierarchy, int fRemoved)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
            {
                return VSConstants.S_OK;
            }

            public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
            {
                {
                    s_IsTerminated = false;
                }
                {
                    __UpdateProjects(s_Files, 0);
                }
                return VSConstants.S_OK;
            }

            public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
            {
                return VSConstants.S_OK;
            }

            public int OnBeforeCloseSolution(object pUnkReserved)
            {
                return VSConstants.S_OK;
            }

            public int OnAfterCloseSolution(object pUnkReserved)
            {
                {
                    s_IsLoaded = false;
                    s_IsTerminated = true;
                }
                {
                    atom.Trace.GetInstance().
                        SetCommand(NAME.COMMAND.MESSAGE_REMOVE).
                        SetCommand(NAME.COMMAND.MESSAGE_REMOVE, "urn:tinypng:id:ROOT").
                        Send(NAME.SOURCE.REFACTORING, NAME.EVENT.INFO, 0);
                }
                return VSConstants.S_OK;
            }
        }

        public static void Connect()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            {
                var a_Context = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (a_Context != null)
                {
                    if (a_Context.AdviseSolutionEvents(new SolutionEvents(), out s_Events) == VSConstants.S_OK)
                    {
                        __UpdateProjects(s_Files, 0);
                    }
                }
            }
        }

        public static void Disconnect()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            {
                var a_Context = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (a_Context != null)
                {
                    if (s_Events != 0)
                    {
                        a_Context.UnadviseSolutionEvents(s_Events);
                    }
                }
            }
        }

        public static void __UpdateProjects(List<string> files, int deep)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if ((s_Events != 0) && (s_IsLoaded == false) && (s_IsTerminated == false))
            {
                try
                {
                    var a_Context = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution2;
                    if (a_Context != null)
                    {
                        var a_Context1 = (IEnumHierarchies)null;
                        var a_Name = string.Empty;
                        {
                            var a_Context2 = Guid.Empty;
                            {
                                a_Context.GetProjectEnum((uint)__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION, ref a_Context2, out a_Context1);
                            }
                        }
                        if (a_Context.GetSolutionInfo(out a_Name, out var a_Context3, out var a_Context4) != VSConstants.S_OK)
                        {
                            return;
                        }
                        else
                        {
                            a_Name = a_Name == null ? string.Empty : a_Name.Trim().ToLower();
                            files.Clear();
                        }
                        if (a_Context1 != null)
                        {
                            var a_Context2 = new IVsHierarchy[1];
                            var a_Count = (uint)0;
                            while ((s_IsTerminated == false) && (a_Context1.Next(1, a_Context2, out a_Count) == VSConstants.S_OK) && (a_Count == 1))
                            {
                                TinyPNG.Instance.JoinableTaskFactory.RunAsync(async () =>
                                {
                                    try
                                    {
                                        {
                                            deep++;
                                            await __UpdateFilesAsync(a_Context2[0], VSConstants.VSITEMID_ROOT, files);
                                            deep--;
                                        }
                                        if (deep > 0)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            s_IsLoaded = true;
                                        }
                                        if (files.Count > 0)
                                        {
                                            __ShowCaption(new atom.Trace());
                                            __ShowFiles(new atom.Trace(), files, a_Name);
                                        }
                                        else
                                        {
                                            __RemoveCaption(new atom.Trace());
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        // This exception can be ignored
                                    }
                                });
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // This exception can be ignored
                }
            }
        }

        public static async System.Threading.Tasks.Task __UpdateFilesAsync(IVsHierarchy hierarchy, uint itemId, List<string> files)
        {
            if ((s_IsTerminated == false) && (hierarchy != null) && (itemId != VSConstants.VSITEMID_NIL))
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TinyPNG.Instance.DisposalToken);
                var a_Context = __GetItemValue(hierarchy, itemId, (int)__VSHPROPID.VSHPROPID_HasEnumerationSideEffects);
                if ((a_Context != null) && ((bool)a_Context))
                {
                    return;
                }
            }
            if ((s_IsTerminated == false) && (hierarchy != null) && (itemId != VSConstants.VSITEMID_NIL))
            {
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TinyPNG.Instance.DisposalToken);
                    itemId = __GetItemId(__GetItemValue(hierarchy, itemId, (int)__VSHPROPID.VSHPROPID_FirstChild));
                }
                while ((s_IsTerminated == false) && (itemId != VSConstants.VSITEMID_NIL))
                {
                    var a_Context = (object)null;
                    var a_Name = string.Empty;
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TinyPNG.Instance.DisposalToken);
                        if ((hierarchy.GetCanonicalName(itemId, out a_Name) == VSConstants.S_OK) && __IsItemEnabled(a_Name))
                        {
                            files.Add(a_Name == null ? string.Empty : a_Name.Trim());
                        }
                    }
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TinyPNG.Instance.DisposalToken);
                        a_Context = __GetItemValue(hierarchy, itemId, (int)__VSHPROPID2.VSHPROPID_ChildrenEnumerated);
                    }
                    if ((a_Context == null) || (a_Context.ToString() == "true"))
                    {
                        await __UpdateFilesAsync(hierarchy, itemId, files);
                    }
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(TinyPNG.Instance.DisposalToken);
                        itemId = __GetItemId(__GetItemValue(hierarchy, itemId, (int)__VSHPROPID.VSHPROPID_NextSibling));
                    }
                }
            }
        }

        private static void __RemoveCaption(atom.Trace context)
        {
            context.
                SetCommand(NAME.COMMAND.MESSAGE_REMOVE, "urn:tinypng:id:ROOT").
                Send(NAME.SOURCE.REFACTORING, NAME.EVENT.WARNING, 0);
        }

        private static void __ShowCaption(atom.Trace context)
        {
            context.
                SetFontState(NAME.FONT_STATE.BLINK).
                SetCommand(NAME.COMMAND.MESSAGE_EXPAND).
                SetCommand(NAME.COMMAND.MESSAGE_SHOW).
                SetCommand(NAME.COMMAND.MESSAGE_CLEAR, "urn:tinypng:id:ROOT").
                SetCommand(NAME.COMMAND.MESSAGE_RESEND, "urn:tinypng:id:ROOT").
                SetCommand(NAME.COMMAND.MESSAGE_PIN).
                SetControl(NAME.CONTROL.BROWSER).
                SetValue(s_PreviewHtml).
                Send(NAME.SOURCE.REFACTORING, NAME.EVENT.WARNING, 0, "TinyPNG found not optimized pictures");
            {
                context.Send(NAME.SOURCE.REFACTORING, NAME.EVENT.HEADER, 1, "[[[Info]]]");
                {
                    context.Send(NAME.SOURCE.REFACTORING, NAME.EVENT.PARAMETER, 2, "TinyPNG [[[uses smart lossy compression techniques to reduce the file size of your]]] WEBP, JPEG [[[and]]] PNG [[[files]]].");
                    context.Send(NAME.SOURCE.REFACTORING, NAME.EVENT.PARAMETER, 2, "[[[By selectively decreasing the number of colors in the image, fewer bytes are required to store the data]]].");
                    context.Send(NAME.SOURCE.REFACTORING, NAME.EVENT.PARAMETER, 2, "[[[The effect is nearly invisible but it makes a very large difference in file size]]]!");
                }
            }
            {
                context.
                    SetCommand(NAME.COMMAND.MESSAGE_UPDATE, "urn:tinypng:hmi:ROOT").
                    SetControl(NAME.CONTROL.PANEL).
                    SetBackground(NAME.COLOR.WHITE).
                    SetPadding(CONSTANT.PADDING.DEFAULT).
                    SetPipe("urn:tinypng:pipe:DEFAULT").
                    SetCount(10).
                    Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 1);
                {
                    context.
                        SetControl(NAME.CONTROL.LABEL).
                        SetAlignment(NAME.ALIGNMENT.TOP).
                        SetBackground(NAME.COLOR.SILVER).
                        SetFontSize(16).
                        Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 2, "TinyPNG");
                    context.
                        SetControl(NAME.CONTROL.PICTURE).
                        SetAlignment(NAME.ALIGNMENT.LEFT).
                        SetSize(150, 150).
                        SetContent("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").
                        Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 2);
                    context.
                        SetControl(NAME.CONTROL.PANEL).
                        SetAlignment(NAME.ALIGNMENT.CLIENT).
                        Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 2);
                    {
                        context.
                            SetControl(NAME.CONTROL.LABEL).
                            SetAlignment(NAME.ALIGNMENT.TOP).
                            SetForeground(NAME.COLOR.DARK_RED).
                            SetFontSize(12).
                            Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 3, "[[[List of found pictures are below]]].");
                        context.
                            SetControl(NAME.CONTROL.LABEL).
                            SetAlignment(NAME.ALIGNMENT.TOP).
                            SetFontSize(12).
                            Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 3, "[[[You can reduce size of all those files in one click]]].");
                        context.
                            SetControl(NAME.CONTROL.LABEL).
                            SetAlignment(NAME.ALIGNMENT.BOTTOM).
                            SetForeground(NAME.COLOR.SILVER).
                            SetFontSize(10).
                            Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 3, "[[[Also, information about optimized pictures is stored in]]] $(SOLUTION_FOLDER)/tinypng.json");
                    }
                    context.
                        SetCommand(NAME.COMMAND.MESSAGE_UPDATE, "urn:tinypng:hmi:OPTIMIZE").
                        SetControl(NAME.CONTROL.BUTTON, "[[[Run optimization of all not optimized files]]]").
                        SetAlignment(NAME.ALIGNMENT.RIGHT, NAME.ALIGNMENT.BOTTOM).
                        SetForeground(NAME.COLOR.DARK_GRAY).
                        SetBackground(NAME.COLOR.LIGHT_GRAY).
                        SetBorder(2).
                        SetPosition(0, 0).
                        SetSize(200, 40).
                        Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 2, "[[[OPTIMIZE ALL]]]");
                }
            }
        }

        private static void __ShowFiles(atom.Trace context, List<string> files, string folder)
        {
            var a_Name = string.Empty;
            {
                files.Sort();
            }
            {
                context.
                    SetCommand(NAME.COMMAND.MESSAGE_UPDATE, "urn:tinypng:all:").
                    SetComment("[[[Found]]]: " + files.Count, "[[[Number of found not optimized pictures]]]").
                    Send(NAME.SOURCE.REFACTORING, NAME.EVENT.FOOTER, 1, "[[[Found pictures]]]");
            }
            foreach (var a_Context in files)
            {
                var a_Context1 = Path.GetDirectoryName(a_Context);
                var a_Name1 = a_Context1;
                var a_Index = a_Context.ToLower().IndexOf(folder);
                if (a_Index == 0)
                {
                    a_Name1 = a_Name1.Substring(folder.Length, a_Name1.Length - folder.Length);
                }
                if (a_Name1.Length > 0)
                {
                    if (a_Name != a_Name1)
                    {
                        {
                            a_Name = a_Name1;
                        }
                        {
                            context.
                                SetCommand(NAME.COMMAND.MESSAGE_UPDATE, "urn:tinypng:folder:" + a_Name).
                                SetComment("[[[Found]]]: " + __GetCount(files, a_Context1), "[[[Number of found not optimized pictures]]]").
                                SetUrlInfo(Path.GetDirectoryName(a_Context), "[[[Open folder]]]").
                                Send(NAME.SOURCE.REFACTORING, NAME.EVENT.FOLDER, 2, a_Name);
                        }
                    }
                    {
                        context.
                            SetCommand(NAME.COMMAND.MESSAGE_UPDATE, "urn:tinypng:file:" + a_Context).
                            SetUrl(a_Context, "[[[Open picture]]]").
                            Send(NAME.SOURCE.REFACTORING, NAME.EVENT.FILE, 3, Path.GetFileName(a_Context));
                        {
                            context.
                                SetControl(NAME.CONTROL.PICTURE).
                                SetCount(5).
                                Send(NAME.SOURCE.REFACTORING, NAME.EVENT.CONTROL, 4);
                        }
                    }
                }
            }
        }

        public static int __GetCount(List<string> files, string folder)
        {
            var a_Result = 0;
            {
                folder = folder.ToLower();
            }
            foreach (var a_Context in files)
            {
                if (a_Context.ToLower().IndexOf(folder) == 0)
                {
                    a_Result++;
                }
            }
            return a_Result;
        }

        //public static object __GetProperty(IVsHierarchy context, int propId)
        //{
        //    object a_Result;
        //    if (context.GetProperty((uint)VSConstants.VSITEMID.Root, propId, out a_Result) == VSConstants.S_OK)
        //    {
        //        return a_Result;
        //    }
        //    return null;
        //}

        public static bool __IsItemEnabled(string context)
        {
            if (Path.HasExtension(context))
            {
                var a_Name = Path.GetExtension(context)?.ToLower();
                {
                    if (a_Name == ".png") return true;
                    if (a_Name == ".jpg") return true;
                    if (a_Name == ".jpeg") return true;
                    if (a_Name == ".webp") return true;
                }
            }
            return false;
        }

        public static object __GetItemValue(IVsHierarchy context, uint itemId, int propId)
        {
            if (itemId != VSConstants.VSITEMID_NIL)
            {
                object a_Result;
                if (context.GetProperty(itemId, propId, out a_Result) == VSConstants.S_OK)
                {
                    return a_Result;
                }
            }
            return null;
        }

        public static uint __GetItemId(object context)
        {
            if (context != null)
            {
                if (context is int) return (uint)(int)context;
                if (context is uint) return (uint)context;
                if (context is short) return (uint)(short)context;
                if (context is ushort) return (uint)(ushort)context;
                if (context is long) return (uint)(long)context;
            }
            return VSConstants.VSITEMID_NIL;
        }

        public static void __ShowSettings()
        {
            // VsShellUtilities.ShowToolsOptionsPage(); // TODO: implement property page
        }
    }
}
