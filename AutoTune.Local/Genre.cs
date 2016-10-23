using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTune.Local {

    [Table("genre")]
    public class Genre {

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        public Genre() {
        }

        public Genre(string name) {
            Name = name;
        }
    }
}
