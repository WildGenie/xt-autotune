using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTune.Local {

    [Table("favourite")]
    public class Favourite {

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("type_id")]
        public string TypeId { get; set; }
        [Column("video_id")]
        public string VideoId { get; set; }

        public Favourite() {
        }

        public Favourite(string typeId, string videoId) {
            TypeId = typeId;
            VideoId = videoId;
        }
    }
}
