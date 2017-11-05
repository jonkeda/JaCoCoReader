using System;
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace JaCoCoReader.Core.Services
{
    public class TestAdapterHost : PSHost, IHostSupportsInteractiveSession
    {
        private readonly Guid _instanceGuid;
        private readonly HostUi _hostUi;

        public TestAdapterHost()
        {
            _instanceGuid = Guid.NewGuid();
            _hostUi = new HostUi();
        }

        public override void SetShouldExit(int exitCode)
        {

        }

        public override void EnterNestedPrompt()
        {

        }

        public override void ExitNestedPrompt()
        {

        }

        public override void NotifyBeginApplication()
        {

        }

        public override void NotifyEndApplication()
        {

        }

        public override string Name
        {
            get { return "PowerShell Tools for Visual Studio Test Adapter"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0); }
        }

        public override Guid InstanceId
        {
            get { return _instanceGuid; }
        }

        public override PSHostUserInterface UI
        {
            get { return _hostUi; }
        }

        public HostUi HostUi
        {
            get { return _hostUi; }
        }

        public override CultureInfo CurrentCulture
        {
            get { return CultureInfo.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return CultureInfo.CurrentUICulture; }
        }

        public void PushRunspace(Runspace runspace)
        {

        }

        public void PopRunspace()
        {

        }

        public bool IsRunspacePushed { get; private set; }
        public Runspace Runspace { get; private set; }
    }
}
