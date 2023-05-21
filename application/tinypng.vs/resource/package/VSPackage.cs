using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
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
            public const string VERSION = "1.1.0";
            public const string PIPE = "urn:metaoutput:pipe:TinyPNG";
        }

        internal static AsyncPackage Instance { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            {
                Instance = this;
            }
            {
                extension.AnyPipe.Connect();
                //extension.AnyPipe.Connect(CONSTANT.APPLICATION, CONSTANT.NAME);
                extension.AnyPipe.Register(CONSTANT.PIPE, new pipe.VSPipe());
            }
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            }
            try
            {
                tool.VSTool.Connect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        protected override int QueryClose(out bool canClose)
        {
            try
            {
                tool.VSTool.Disconnect();
                extension.AnyPipe.Disconnect();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            {
                canClose = true;
            }
            return VSConstants.S_OK;
        }
    }
}
