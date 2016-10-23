using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTune.Local {

    [Table("track")]
    public class Track {

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("path")]
        public string Path { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("image")]
        public byte[] Image { get; set; }
        [Column("favourite")]
        public bool Favourite { get; set; }
        [Column("genre_id")]
        public virtual Genre Genre { get; set; }
        [Column("album_id")]
        public virtual Album Album { get; set; }
        [Column("artist_id")]
        public virtual Artist Artist { get; set; }

        public Track() {
        }

        public Track(string path, string title, byte[] image, Genre genre, Album album, Artist artist) {
            Path = path;
            Title = title;
            Genre = genre;
            Album = album;
            Image = image;
            Artist = artist;
        }
    }
}
