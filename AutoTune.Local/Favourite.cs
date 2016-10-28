using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTune.Local {

    [Table("favourite")]
    public class Favourite :IEquatable<Favourite>{

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

        public override bool Equals(object obj) {
            return Equals((Favourite)obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode() + 11 * VideoId.GetHashCode();
        }

        public bool Equals(Favourite other) {
            return TypeId.Equals(other.TypeId) && VideoId.Equals(other.VideoId);
        }
    }
}
