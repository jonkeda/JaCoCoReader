using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JaCoCoReader.Core.ViewModels.Tests;
using JaCoCoReader.Vsix.Extensions;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Language.Intellisense;

namespace JaCoCoReader.Vsix.Tests
{
    internal class RunTestSuggestedAction : ISuggestedAction
    {
        private readonly TestsViewModel _tests;
        private readonly string _filePath;
        private readonly int _lineNumber;

        public RunTestSuggestedAction(TestsViewModel tests, string filePath, int lineNumber)
        {
            _tests = tests;
            _filePath = filePath;
            _lineNumber = lineNumber;
        }

        public string DisplayText
        {
            get
            {
                return "Run pester test";
            }
        }


        public string IconAutomationText
        {
            get
            {
                return null;
            }
        }

        ImageMoniker ISuggestedAction.IconMoniker
        {
            get
            {
                return default(ImageMoniker);
            }
        }

        public string InputGestureText
        {
            get
            {
                return null;
            }
        }

        public bool HasActionSets
        {
            get
            {
                return false;
            }
        }

        public Task<IEnumerable<SuggestedActionSet>> GetActionSetsAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public bool HasPreview
        {
            get
            {
                return false;
            }
        }

        public Task<object> GetPreviewAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public void Dispose()
        {
        }

        public void Invoke(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            VsExtensions.SaveProjectItem(_filePath);
            _tests.RunTests(_filePath, _lineNumber);
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            telemetryId = Guid.Empty;
            return false;
        }
    }
}
