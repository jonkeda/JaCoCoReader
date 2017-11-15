namespace JaCoCoReader.Vsix.FileExtension
{
    public class FileAndContentTypeDefinitions
    {
        public const string ContentType = "Powershell";

        //[Export(typeof(ContentTypeDefinition))]
        //[Name(ContentType)]
        //[BaseDefinition("Powershell")]
        //public ContentTypeDefinition HidingContentTypeDefinition { get; set; }

        //[Export(typeof(FileExtensionToContentTypeDefinition))]
        //[FileExtension(".xxx")]
        //[ContentType(ContentType)]
        //public FileExtensionToContentTypeDefinition HiddenFileExtensionDefinition { get; set; }
    }
}
