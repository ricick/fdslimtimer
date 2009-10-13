using System;
using System.Collections.Generic;
using System.Text;
using PureMVC.Patterns;

namespace SlimTimer.control
{
    class ApplicationStartupCommand : MacroCommand
    {
        protected override void InitializeMacroCommand()
        {
            base.InitializeMacroCommand();
            AddSubCommand(typeof(ModelPrepCommand));
            AddSubCommand(typeof(ViewPrepCommand));
            AddSubCommand(typeof(LoadSettingsCommand));
            AddSubCommand(typeof(LoginCommand));
        }
    }
}
