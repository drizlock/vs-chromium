// Copyright 2013 The Chromium Authors. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using VsChromium.Core.Logging;
using VsChromium.Core.Utility;
using VsChromium.Features.AutoUpdate;
using VsChromium.Features.BuildOutputAnalyzer;
using VsChromium.Features.IndexServerInfo;
using VsChromium.Package;
using VsChromium.ServerProxy;
using VsChromium.Settings;
using VsChromium.Threads;
using VsChromium.Views;
using VsChromium.Wpf;

namespace VsChromium.Features.ToolWindows.OpenFile {
  /// <summary>
  /// Interaction logic for CodeSearchControl.xaml
  /// </summary>
  public partial class OpenFileControl {

    private IDispatchThreadServerRequestExecutor _dispatchThreadServerRequestExecutor;
    private OpenFileController _controller;

    public OpenFileControl() {
      InitializeComponent();
      // Add the "VsColors" brushes to the WPF resources of the control, so that the
      // resource keys used on the XAML file can be resolved dynamically.
      Resources.MergedDictionaries.Add(VsResources.BuildResourceDictionary());
      DataContext = new OpenFileViewModel();

      SearchFileTextBox.TextChanged += (s, e) => {
        ViewModel.SearchPattern = SearchFileTextBox.Text;
        RefreshSearchResults(false);
      };
    }

    /// <summary>
    /// Called when Visual Studio creates our container ToolWindow.
    /// </summary>
    public void OnVsToolWindowCreated(IServiceProvider serviceProvider) {
      var componentModel = (IComponentModel)serviceProvider.GetService(typeof(SComponentModel));

      _dispatchThreadServerRequestExecutor = componentModel.DefaultExportProvider.GetExportedValue<IDispatchThreadServerRequestExecutor>();

      var standarImageSourceFactory = componentModel.DefaultExportProvider.GetExportedValue<IStandarImageSourceFactory>();
      _controller = new OpenFileController(
        this,
        _dispatchThreadServerRequestExecutor,
        componentModel.DefaultExportProvider.GetExportedValue<IDispatchThreadDelayedOperationExecutor>(),
        componentModel.DefaultExportProvider.GetExportedValue<IFileSystemTreeSource>(),
        componentModel.DefaultExportProvider.GetExportedValue<ITypedRequestProcessProxy>(),
        standarImageSourceFactory,
        componentModel.DefaultExportProvider.GetExportedValue<IWindowsExplorer>(),
        componentModel.DefaultExportProvider.GetExportedValue<IClipboard>(),
        componentModel.DefaultExportProvider.GetExportedValue<ISynchronizationContextProvider>(),
        componentModel.DefaultExportProvider.GetExportedValue<IOpenDocumentHelper>(),
        componentModel.DefaultExportProvider.GetExportedValue<ITextDocumentTable>(),
        componentModel.DefaultExportProvider.GetExportedValue<IDispatchThreadEventBus>(),
        componentModel.DefaultExportProvider.GetExportedValue<IGlobalSettingsProvider>(),
        componentModel.DefaultExportProvider.GetExportedValue<IBuildOutputParser>(),
        componentModel.DefaultExportProvider.GetExportedValue<IVsEditorAdaptersFactoryService>(),
        componentModel.DefaultExportProvider.GetExportedValue<IShowServerInfoService>());
      _controller.Start();
    }

    public OpenFileViewModel ViewModel {
      get {
        return (OpenFileViewModel)DataContext;
      }
    }

    public IOpenFileController Controller {
      get { return _controller; }
    }

#region WPF Event handlers

    private void RefreshSearchResults(bool immediate) {
      Controller.PerformSearch(immediate);
    }

    private void ClearFilePathsPattern_Click(object sender, RoutedEventArgs e) {
      SearchFileTextBox.Text = "";
      RefreshSearchResults(true);
    }

#endregion
  }
}
