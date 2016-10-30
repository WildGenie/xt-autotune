using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoTune.Local {

    [Table("suggestion")]
    public class Suggestion : IEquatable<Suggestion> {

        [Key]
        [Column("id")]
        public long Id { get; set; }
        [Column("title")]
        public string Title { get; set; }
        [Column("type_id")]
        public string TypeId { get; set; }
        [Column("accepted")]
        public bool Accepted { get; set; }
        [Column("declined")]
        public bool Declined { get; set; }
        [Column("video_id")]
        public string VideoId { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
        [Column("image_base64")]
        public string ImageBase64 { get; set; }
        [Column("artist_id")]
        public virtual Artist Artist { get; set; }

        public Suggestion() {
        }

        public Suggestion(string typeId, string videoId) {
            TypeId = typeId;
            VideoId = videoId;
        }

        public override bool Equals(object obj) {
            return Equals((Suggestion)obj);
        }

        public override int GetHashCode() {
            return Id.GetHashCode() + 11 * VideoId.GetHashCode();
        }

        public bool Equals(Suggestion other) {
            return TypeId.Equals(other.TypeId) && VideoId.Equals(other.VideoId);
        }
    }
}
