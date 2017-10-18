namespace JaCoCoReader.UI
{
    public delegate void CommandDelegate();

    public delegate void CommandDelegate<in T>(T parameter);


}