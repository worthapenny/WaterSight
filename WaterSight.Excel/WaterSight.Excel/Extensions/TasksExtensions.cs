using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WaterSight.Excel.Extensions;

public static class TasksExtensions
{
    public static async Task<TResult> TimeoutAfterResult<TResult>(this Task<TResult> task, TimeSpan timeout, string taskName)
    {

        using (var timeoutCancellationTokenSource = new CancellationTokenSource())
        {

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                return await task;  // Very important in order to propagate exceptions
                Log.Information($"Given task to '{taskName}' completed");
            }
            else
            {
                //throw new TimeoutException("The operation has timed out.");
                Log.Error($"The operation to '{taskName}' has timed out. Period: {timeout}");
                return default;
            }
        }
    }

    public static async Task TimeoutAfter(this Task task, TimeSpan timeout, string taskName)
    {

        using (var timeoutCancellationTokenSource = new CancellationTokenSource())
        {

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                await task;  // Very important in order to propagate exceptions
                Log.Information($"Given task to '{taskName}' completed");
            }
            else
            {
                //throw new TimeoutException("The operation has timed out.");
                Log.Error($"The operation to '{taskName}' has timed out. Period: {timeout}");
            }
        }
    }
}
