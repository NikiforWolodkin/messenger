using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessengerModels.Models
{
    public class MessageResponse : INotifyPropertyChanged
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("authorId")]
        public Guid AuthorId { get; set; }
        [JsonPropertyName("authorName")]
        public string AuthorName { get; set; }
        [JsonPropertyName("authorAvatarUrl")]
        public string AuthorAvatarUrl {  get; set; }
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }
        [JsonPropertyName("sendTime")]
        public DateTime SendTime { get; set; }

        [JsonIgnore]
        private string? _text;
        [JsonPropertyName("text")]
        public string? Text
        {
            get => _text;
            set { _text = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        private bool _isLiked;
        [JsonPropertyName("isLiked")]
        public bool IsLiked
        {
            get => _isLiked;
            set { _isLiked = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        private int _likeAmount;
        [JsonPropertyName("likeAmount")]
        public int LikeAmount 
        { 
            get => _likeAmount;
            set { _likeAmount = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
