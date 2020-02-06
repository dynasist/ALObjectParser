namespace ALObjectParser.Library
{
    public class ALVariable
    {
        public string Name { get; set; }
        public ALTypeDefinition TypeDefinition { get; set; }
        public bool IsTemporary { get; set; }
    }
}