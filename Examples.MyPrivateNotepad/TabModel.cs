namespace MyPrivateNotepad
{
    public class TabModel
    {
        public int Pos { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public TabModel(int pos, string name, string content)
        {
            Pos = pos;
            Name = name;
            Content = content;
        }
    }
}
