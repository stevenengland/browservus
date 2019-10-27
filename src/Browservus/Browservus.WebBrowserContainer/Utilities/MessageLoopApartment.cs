using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StEn.Browservus.WebBrowserContainer.Utilities
{
	/// <summary>
	/// Reusable <see cref="MessageLoopApartment"/> class is used to start and run a Windows.Forms STA thread with its own message pump.
	/// It can be used from a console application. This class exposes a TPL Task Scheduler (FromCurrentSynchronizationContext) and a set of Task.Factory.StartNew wrappers to use this task scheduler.
	/// Example usage: https://github.com/noserati/WebBrowser/blob/master/AsyncWebBrowserScrapper.cs
	/// </summary>
	/// <seealso cref="IDisposable" />
	public class MessageLoopApartment : IDisposable
	{
		private Thread staThread;

		public MessageLoopApartment()
		{
			var tcs = new TaskCompletionSource<TaskScheduler>();

			// start an STA thread and gets a task scheduler
			this.staThread = new Thread(startArg =>
			{
				EventHandler idleHandler = null;

				idleHandler = (s, e) =>
				{
					// handle Application.Idle just once
					Application.Idle -= idleHandler;

					// return the task scheduler
					tcs.SetResult(TaskScheduler.FromCurrentSynchronizationContext());
				};

				// handle Application.Idle just once
				// to make sure we're inside the message loop
				// and SynchronizationContext has been correctly installed
				Application.Idle += idleHandler;
				Application.Run();
			});

			this.staThread.SetApartmentState(ApartmentState.STA);
			this.staThread.IsBackground = true;
			this.staThread.Start();
			this.TaskScheduler = tcs.Task.Result;
		}

		public TaskScheduler TaskScheduler { get; private set; }

		public void Dispose()
		{
			if (this.TaskScheduler == null)
			{
				return;
			}

			var taskScheduler = this.TaskScheduler;
			this.TaskScheduler = null;

			// execute Application.ExitThread() on the STA thread
			Task.Factory.StartNew(
				Application.ExitThread,
				CancellationToken.None,
				TaskCreationOptions.None,
				taskScheduler).Wait();

			this.staThread.Join();
			this.staThread = null;
		}

		public void Invoke(Action action)
		{
			Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.TaskScheduler).Wait();
		}

		public TResult Invoke<TResult>(Func<TResult> action)
		{
			return Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.None, this.TaskScheduler).Result;
		}

		public Task Run(Action action, CancellationToken token)
		{
			return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this.TaskScheduler);
		}

		public Task<TResult> Run<TResult>(Func<TResult> action, CancellationToken token)
		{
			return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this.TaskScheduler);
		}

		public Task Run(Func<Task> action, CancellationToken token)
		{
			return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this.TaskScheduler).Unwrap();
		}

		public Task<TResult> Run<TResult>(Func<Task<TResult>> action, CancellationToken token)
		{
			return Task.Factory.StartNew(action, token, TaskCreationOptions.None, this.TaskScheduler).Unwrap();
		}
	}
}
