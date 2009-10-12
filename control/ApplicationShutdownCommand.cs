using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;

namespace SlimTimer.control
{
    class ApplicationShutdownCommand : MacroCommand
    {
        protected override void InitializeMacroCommand()
        {
            base.InitializeMacroCommand();
            AddSubCommand(typeof(SaveTimeEntryCommand));
            AddSubCommand(typeof(SaveSettingsCommand));
        }
    }
}
