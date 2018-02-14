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

    public class DummyProgressBar : IProgressBar
    {
        public void Dispose()
        {
            
        }

        public ChildProgressBar Spawn(int maxTicks, string message, ProgressBarOptions options = null)
        {
            return null;
        }

        public void Tick(string message = "")
        {
        }

        public int MaxTicks { get; set; }
        public string Message { get; set; }
        public double Percentage { get; }
        public int CurrentTick { get; }
        public ConsoleColor ForeGroundColor { get; }
    }
}
