namespace DeezerPlayer.Model
{
    public class Song
    {
        public int Id { get; set; }
        public bool Readable { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public int Duration { get; set; }
        public int Rank { get; set; }
        public bool Explicit_lyrics { get; set; }
        public Artist Artist { get; set; }
        public Album Album { get; set; }
        public string Type { get; set; }
    }

}
