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
    [Table("Spectra_Characteristic")]
    public class Characteristic
    {
        [Key]
        public int? Id { get; set; }
        [Required(ErrorMessage = "This field can't blank")]
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DefaultValue(true)]
        public bool Status { get; set; }
        public int? CategoryId { get; set; }
        public Category Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public ICollection<CharacterList> CharacterLists { get; set; }

    }
    public class CharacteristicDisplay : Characteristic
    {

        public string CateNewName { get; set; }
        public string CateName { get; set; }
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
