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
    [Table("Spectra_Product")]
    public class Product
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        [MaxLength(250, ErrorMessage = "Max of length is 30 characters")]
        [MinLength(2, ErrorMessage = "This field can't least 2 characters")]
        public string Name { get; set; }
        public string TitleDescription { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public float SalePrice { get; set; }
        public string Images { get; set; }
        public string JobId { get; set; }
        public int WarrantyMonth { get; set; }
        // Foreign Key - tblCategory
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public int? GiftId { get; set; }
        public Gift Gift { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public bool Option { get; set; }
        public bool ScheduleStatus { get; set; }
        public string Information { get; set; }
        public string Instruct { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string TitleSeo { get; set; }
        public string MetaKeyWords { get; set; }
        public string MetaDescription { get; set; }
        public string AccountEdit { get; set; }
        public DateTime Start { get; set; }
        public DateTime Ends { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
        public ICollection<Question> Questions { get; set; }
        public ICollection<Item> Items { get; set; }
        public ICollection<SeriProduct> SeriProducts { get; set; }
        public ICollection<Voucher> Vouchers { get; set; }

    }

    public class ProductDisplay : Product
    {

        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public bool CategoryWaranty { get; set; }
        public string GiftName { get; set; }
        public float GiftPrice { get; set; }
        public float RatingFeed { get; set; }
        public float Ratings { get; set; }
        public float Giaphantram { get; set; }
        public string TitleSeoCate { get; set; }
        public string KeyWordsSeoCate { get; set; }
        public string DescriptionSeoCate { get; set; }
        public string ImageName { get; set; }
        private string linkname;
        public string LinkName
        {
            get { return linkname; }
            set { linkname = RemoveVietnameseTone(value); }
        }
        
        public string RemoveVietnameseTone(string text)
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
