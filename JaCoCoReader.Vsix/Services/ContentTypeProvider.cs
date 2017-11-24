using System.ComponentModel.Composition;
using JaCoCoReader.Core.Constants;
using Microsoft.VisualStudio.Utilities;

namespace JaCoCoReader.Vsix.Services
{
    /// <summary>
    /// Powershell contenttype provider.
    /// </summary>
    internal sealed class ContentTypeProvider
    {
        [Export]
        [Name(Constant.PowerShell)]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition PowerShellContentType;

        [Export]
        [FileExtension(".psd1")]
        [ContentType(Constant.PowerShell)]
        internal static FileExtensionToContentTypeDefinition Psd1;

        [Export]
        [FileExtension(".psm1")]
        [ContentType(Constant.PowerShell)]
        internal static FileExtensionToContentTypeDefinition Psm1;

        [Export]
        [FileExtension(".ps1")]
        [ContentType(Constant.PowerShell)]
        internal static FileExtensionToContentTypeDefinition Ps1;


    }
}
