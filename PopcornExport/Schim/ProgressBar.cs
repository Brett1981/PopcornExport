using System;
using System.Collections.Generic;
using System.Text;
using ShellProgressBar;

namespace PopcornExport.Schim
{
    public class ProgressBar
    {
        public static IProgressBar Create(int maxTicks, string message, ProgressBarOptions options)
        {
            if(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPSETTING_WEBSITE_SITE_NAME")))
                return new ShellProgressBar.ProgressBar(maxTicks, message, options);

            return new DummyProgressBar();
        }
    }

    public class DummyChildProgressBar : ChildProgressBar
    {
        public int MaxTicks { get; set; }
        public string Message { get; set; }
        public double Percentage { get; private set; }
        public int CurrentTick { get; private set; }
        public ConsoleColor ForeGroundColor { get; }

        public DummyChildProgressBar(int maxTicks, string message, Action scheduleDraw, ProgressBarOptions options = null, Action<ProgressBarHeight> growth = null):
            base(maxTicks, message, scheduleDraw, options, growth)
        {
            
        }

        public new void Dispose()
        {
            
        }

        public new void Tick(string message = "")
        {
            CurrentTick++;
            Percentage = (double)CurrentTick / (double)MaxTicks * 100d;
            Console.WriteLine($"{Percentage}%");
        }

        public new ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null)
        {
            Console.WriteLine(message);
            MaxTicks = maxTicks;
            CurrentTick = 0;
            Message = message;
            return new DummyChildProgressBar(maxTicks, message, () => { }, options);
        }
    }

    public class DummyProgressBar : IProgressBar
    {
        public void Dispose()
        {
        }

        public ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null)
        {
            Console.WriteLine(message);
            MaxTicks = maxTicks;
            CurrentTick = 0;
            Message = message;
            return new DummyChildProgressBar(maxTicks, message, () => { }, options);
        }

        public void Tick(string message = "")
        {
            CurrentTick++;
            Percentage = (double) CurrentTick / (double) MaxTicks * 100d;
            Console.WriteLine($"{Percentage}%");
        }

        public int MaxTicks { get; set; }
        public string Message { get; set; }
        public double Percentage { get; private set; }
        public int CurrentTick { get; private set; }
        public ConsoleColor ForeGroundColor { get; }
    }
}
