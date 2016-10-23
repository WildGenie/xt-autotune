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
        [Column("favourite")]
        public bool Favourite { get; set; }
        [Column("genre_id")]
        public virtual Genre Genre { get; set; }
        [Column("artist_id")]
        public virtual Artist Artist { get; set; }

        public Track() {
        }

        public Track(string path, string title, Genre genre, Artist artist) {
            Path = path;
            Title = title;
            Genre = genre;
            Artist = artist;
        }
    }
}
