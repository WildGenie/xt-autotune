using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTune.Local {

    [Table("artist")]
    public class Artist {

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }

        public Artist() {
        }

        public Artist(string name) {
            Name = name;
        }
    }
}
