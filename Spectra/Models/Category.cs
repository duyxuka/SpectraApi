using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spectra.Models
{
    [Table("Spectra_Category")]
    public class Category
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }

        [MaxLength(250, ErrorMessage = "Max of length is 250 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [DefaultValue(true)]
        public bool Option { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TitleSeo { get; set; }
        public string MetaKeyWords { get; set; }
        public string MetaDescription { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<Characteristic> Characteristics { get; set; }
        public ICollection<Application> Applications { get; set; }
        public ICollection<NewsDetail> NewsDetails { get; set; }
        
    }
    public class CategoryProductDisplay : Category
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public float ProductPrice { get; set; }
        public float ProductSalePrice { get; set; }
        public int ProductQuantity { get; set; }
        public string ProductImages { get; set; }
        public bool ProductStatus { get; set; }
        private string linkname;
        public string LinkName
        {
            get { return linkname; }
            set { linkname = RemoveVietnameseTone(value); }
        }

        public static string RemoveVietnameseTone(string text)
        {
            string result = text.ToLower();
            result = Regex.Replace(result, "à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ|/g", "a");
            result = Regex.Replace(result, "è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ|/g", "e");
            result = Regex.Replace(result, "ì|í|ị|ỉ|ĩ|/g", "i");
            result = Regex.Replace(result, "ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ|/g", "o");
            result = Regex.Replace(result, "ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ|/g", "u");
            result = Regex.Replace(result, "ỳ|ý|ỵ|ỷ|ỹ|/g", "y");
            result = Regex.Replace(result, "đ", "d");
            return result;
        }
    }
}
