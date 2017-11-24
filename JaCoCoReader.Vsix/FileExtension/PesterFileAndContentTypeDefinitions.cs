using JaCoCoReader.Core.Constants;

namespace JaCoCoReader.Vsix.FileExtension
{
    public class FileAndContentTypeDefinitions
    {
        public const string ContentType = Constant.PowerShell;

        //[Export(typeof(ContentTypeDefinition))]
        //[Name(ContentType)]
        //[BaseDefinition(Constant.PowerShell)]
        //public ContentTypeDefinition HidingContentTypeDefinition { get; set; }

        //[Export(typeof(FileExtensionToContentTypeDefinition))]
        //[FileExtension(".xxx")]
        //[ContentType(ContentType)]
        //public FileExtensionToContentTypeDefinition HiddenFileExtensionDefinition { get; set; }
    }
}
