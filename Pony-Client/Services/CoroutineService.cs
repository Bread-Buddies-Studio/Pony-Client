using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pony_Client.Services;

static class CoroutineService
{
    // Public Methods //
    public static async Task StartCoroutine(IEnumerable coroutine, Func<bool>? boolSupplier = null)
    {
        // Event Resetter to wait 1 frame //
        TaskCompletionSource<bool> taskAwaiter = new();
        // Methods //
        void OnUpdate(object? sender, EventArgs _)
        {
            // Conditions //
            if (taskAwaiter.Task.IsCompleted)
                return;
            // Settings //
            taskAwaiter.SetResult(true);
        }
        // Connections //
        RunService.Update += OnUpdate;
        // Loop Through the Frames //
        foreach (var _ in coroutine)
        {
            // Conditions //
            if (boolSupplier is not null && boolSupplier())
                break;
            // Delay a Frame //
            await taskAwaiter.Task;
        }
        // Disconnect //
        RunService.Update -= OnUpdate;
    }

    public static async Task StartCoroutine(IEnumerable coroutine, CancellationTokenSource cancelSource)
    {
        Task task = Task.Run(async () =>
        {
            // Event Resetter to wait 1 frame //
            TaskCompletionSource<bool> taskAwaiter = new();
            // Methods //
            void OnUpdate(object? sender, EventArgs _)
            {
                // Conditions //
                if (taskAwaiter.Task.IsCompleted)
                    return;
                // Settings //
                taskAwaiter.SetResult(true);
            }
            // Connections //
            RunService.Update += OnUpdate;
            // Loop Through the Frames //
            foreach (var _ in coroutine)
            {
                // Conditions //
                if (cancelSource.IsCancellationRequested)
                    break;
                // Delay a Frame //
                await taskAwaiter.Task;
            }
            // Disconnect //
            RunService.Update -= OnUpdate;
        }, cancelSource.Token);

        await task;
    }
}
