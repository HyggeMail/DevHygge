using HyggeMail.DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyggeMail.BLL.Models
{
    public class ImageViewModel
    {
        public string Url { get; set; }
    }
    public class ImagesByCategoryViewModel
    {
        public int CatID { get; set; }
        public string CatName { get; set; }
        public int ImagesCount { get; set; }
        public List<ImagesViewModel> Images { get; set; }
        public bool IsRecent { get; set; }
        public bool HasLoggedInUser { get; set; }
        public ImagesByCategoryViewModel()
        {
            this.Images = new List<ImagesViewModel>();
        }
        public ImagesByCategoryViewModel(ImageCategory dal)
        {
            this.Images = new List<ImagesViewModel>();
            this.CatID = dal.ID;
            this.CatName = dal.CategoryName;
        }
    }
    public class ImagesViewModel
    {
        public int ImageID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public int AddedBy { get; set; }
        public bool ByAdmin { get; set; }
    }

    public class UnSplashImagesListModel
    {
        [JsonProperty("total")]
        public int TotalRecords { get; set; }
        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }
        public int PageNumber { get; set; }
        [JsonProperty("results")]
        public List<UnSplashImageDetailsModel> Results { get; set; }
    }

    public class UnSplashImageDetailsModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonProperty("urls")]
        public UnSplashImageModel Urls { get; set; }
    }

    public class UnSplashImageModel
    {
        [JsonProperty("raw")]
        public string Raw { get; set; }
        [JsonProperty("full")]
        public string Full { get; set; }
        [JsonProperty("regular")]
        public string Regular { get; set; }
        [JsonProperty("small")]
        public string Small { get; set; }
        [JsonProperty("thumb")]
        public string Thumb { get; set; }
    }

}
