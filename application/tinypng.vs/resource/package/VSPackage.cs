
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using resource.tool;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace resource.package
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(CONSTANT.GUID)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.ShellInitialized_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(VSOptions), CONSTANT.HOST, CONSTANT.NAME, 0, 0, true)]
    public sealed class TinyPNG : AsyncPackage
    {
        internal static class CONSTANT
        {
            public const string APPLICATION = "Visual Studio";
            public const string COMPANY = "Viacheslav Lozinskyi";
            public const string COPYRIGHT = "Copyright (c) 2022-2023 by Viacheslav Lozinskyi. All rights reserved.";
            public const string DESCRIPTION = "Lossy optimization for any PNG, JPEG, APNG, and WEBP images by TinyPNG service.";
            public const string GUID = "68C2787B-4F41-4145-BD12-17BEC8832539";
            public const string HOST = "MetaOutput";
            public const string NAME = "TinyPNG";
            public const string VERSION = "1.0.1";
            public const string PIPE = "urn:metaoutput:pipe:TinyPNG";
        }

        internal static AsyncPackage Instance { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            {
                Instance = this;
                extension.AnyPipe.Connect();
                extension.AnyPipe.Register(CONSTANT.PIPE, new pipe.VSPipe());
            }
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }
            try
            {
                if (string.IsNullOrEmpty(atom.Trace.GetFailState(CONSTANT.APPLICATION)) == false)
                {
                    var a_Context = Package.GetGlobalService(typeof(SDTE)) as DTE2;
                    if (a_Context != null)
                    {
                        var a_Context1 = (OutputWindowPane)null;
                        for (var i = a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Count; i >= 1; i--)
                        {
                            if (a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Item(i).Name == CONSTANT.HOST)
                            {
                                a_Context1 = a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Item(i);
                                break;
                            }
                        }
                        if (a_Context1 == null)
                        {
                            a_Context1 = a_Context.ToolWindows.OutputWindow.OutputWindowPanes.Add(CONSTANT.HOST);
                        }
                        if (a_Context1 != null)
                        {
                            a_Context1.OutputString("\r\n" + CONSTANT.NAME + " extension doesn't work without MetaOutput.\r\n    Please install it --> https://www.metaoutput.net/download\r\n");
                            a_Context1.Activate();
                        }
                    }
                }
                else
                {
                    tool.VSTool.Connect();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            {
                tool.VSTool.Disconnect();
                extension.AnyPipe.Disconnect();
                canClose = true;
            }
            return VSConstants.S_OK;
        }
    }
}
