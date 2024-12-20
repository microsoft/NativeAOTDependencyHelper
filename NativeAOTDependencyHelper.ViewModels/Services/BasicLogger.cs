// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.Messaging;
using NativeAOTDependencyHelper.Core;
using NativeAOTDependencyHelper.Core.Services;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace NativeAOTDependencyHelper.ViewModels.Services;

public class BasicLogger(TaskScheduler _uiScheduler) : ILogger
{
    public ObservableCollection<LogItemData> LogItems { get; private set; } = new();

    public void Clear()
    {
        Task.Factory.StartNew(() =>
        {
            LogItems.Clear();
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
    }

    public void Information(string message, [CallerMemberName] string member = "")
    {
        Task.Factory.StartNew(() =>
        {
            LogItems.Add(new($"{DateTime.Now} - {member} - {message}", LogItemLevel.Info));
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
    }

    public void Warning(string message, [CallerMemberName] string member = "")
    {
        Task.Factory.StartNew(() =>
        {
            LogItems.Add(new($"{DateTime.Now} - {member} - {message}", LogItemLevel.Warning));
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
    }

    public void Error(Exception exception, string message, [CallerMemberName] string member = "")
    {
        Task.Factory.StartNew(() =>
        {
            LogItems.Add(new($"{DateTime.Now} - {member} - {message}:\n\t{exception.Message}\n\t{exception.StackTrace}", LogItemLevel.Error));

            WeakReferenceMessenger.Default.Send<LoggedErrorMessage>(new(message));
        }, CancellationToken.None, TaskCreationOptions.None, _uiScheduler);
    }
}
