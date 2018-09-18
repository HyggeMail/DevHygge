using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HyggeMail.BLL.Models;
using Newtonsoft.Json;
using HyggeMail.BLL.Validator;

namespace HyggeMail.BLL.Models
{
    /// <summary>
    /// Login Model this will be  used to login in the application
    /// </summary>
    public class LoginModal
    {
        [Required(ErrorMessage = "Email  can't be empty")]
        // [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-??]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Invalid Email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }

    /// <summary>
    /// UserListing Model : This will be used to List all the users in Admin panel
    /// </summary>
    public class UserListingModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ImagePath { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? CardsCount { get; set; }
        public bool IsActivated { get; set; }
        public UserListingModel()
        {

        }

        public UserListingModel(User userObj)
        {
            this.FirstName = userObj.FirstName;
            this.LastName = userObj.LastName;
            this.Email = userObj.Email;
            this.UserId = userObj.UserID;
            this.CreatedOn = userObj.CreatedAt;
            this.CardsCount = userObj.CardsCount;
            this.IsDeleted = userObj.IsDeleted;
            if (userObj.UserDetail != null)
                this.ImagePath = userObj.UserDetail.ProfileImagePath;
            this.IsActivated = userObj.IsActivated.HasValue ? userObj.IsActivated.Value : false;
        }

    }
    public class ForgetPasswordModel
    {
        public int? UserType { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-??]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Invalid Email")]
        public string UserEmail { get; set; }

    }

    public class FacebookUserDetails
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public string BirthDay { get; set; }
        public bool IsMobile { get; set; }
    }

    public class FacebookUserDetailsApp
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Location { get; set; }
        public bool IsMobile { get; set; }
    }

    public class ResetPasswordModel
    {
        public int UserId { get; set; }
        public int? type { get; set; }
        [Required(ErrorMessage = "Required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password should be minimum of 6 and maximum of 20 characters.")]
        public string NewPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Password and Confirm Password does not match")]
        public string ConfirmPassword { get; set; }

    }

    public class ChangePasswordModel
    {
        public int userID { get; set; }
        [Required(ErrorMessage = "Required")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password should be minimum of 6 and maximum of 20 characters.")]
        public string NewPassword { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "New Password and Confirm Password does not match")]
        public string NewConfirmPassword { get; set; }
    }

    public class UserModel
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string FirstName { get; set; }

        //[Required(ErrorMessage = "Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-??]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string CountryID { get; set; }
        public string StateID { get; set; }
        public string CityID { get; set; }
        public int? CardsCount { get; set; }
        public HttpPostedFileBase Image { get; set; }
        public string ImageString { get; set; }
        public string ImagePath { get; set; }

        public string ImageName { get; set; }
        //public string CountryName { get; set; }
        //public string StateName { get; set; }
        //public string CityName { get; set; }
        [JsonIgnore]
        public List<SelectListItem> CountryList { get; set; }
        [JsonIgnore]
        public List<SelectListItem> StateList { get; set; }
        [JsonIgnore]
        public List<SelectListItem> CityList { get; set; }

        public int? UserType { get; set; }
        public bool? IsActivated { get; set; }
        public string Password { get; set; }
        public AuthenticationType eAuthenticationType { get; set; }

        public string facebookID { get; set; }

        public UserModel()
        {
            this.CountryList = new List<SelectListItem>();
            this.CityList = new List<SelectListItem>();
            this.StateList = new List<SelectListItem>();
        }
        public UserModel(User userObj)
        {
            this.UserId = userObj.UserID;
            this.FirstName = userObj.FirstName;
            this.LastName = userObj.LastName;
            if (userObj.UserDetail != null)
            {
                this.Zip = userObj.UserDetail.Zip;
                this.CountryID = userObj.UserDetail.Country;
                this.StateID = userObj.UserDetail.State;
                this.CityID = userObj.UserDetail.City;
                this.facebookID = userObj.UserDetail.FacebookID;
                this.Address = userObj.UserDetail.Address;
                this.Phone = userObj.UserDetail.Phone;
                this.ImagePath = userObj.UserDetail.ProfileImagePath;
                this.ImageName = userObj.UserDetail.ProfileImageName;
            }
            this.UserType = userObj.UserType;
            this.IsActivated = userObj.IsActivated;
            this.CardsCount = userObj.CardsCount;
            if (!string.IsNullOrEmpty(userObj.Password))
            {
                this.Password = Utilities.DecryptPassword(userObj.Password, true);
            }
            this.eAuthenticationType = (AuthenticationType)userObj.AuthenticationType;
            this.Email = userObj.Email;

            this.CountryList = new List<SelectListItem>();
            this.CityList = new List<SelectListItem>();
            this.StateList = new List<SelectListItem>();
        }

    }
    public class AddUserModel : UserModel
    {
        [Required(ErrorMessage = "Required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password should be minimum of 6 and maximum of 20 characters.")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and Confirm Password does not match")]
        public string ConfirmPassword { get; set; }

        public AddUserModel()
        {

        }
    }
    public class ExternalLoginConfirmationViewModel
    {
        public string UserName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password should be minimum of 6 and maximum of 20 characters.")]
        public string Password { get; set; }
    }
    public class UserRegistrationModel
    {
        public UserSignInStep1 Step1 { get; set; }
        public UserSignInStep2 Step2 { get; set; }
        public UserSignInStep3 Step3 { get; set; }

        public UserRegistrationModel()
        {

            this.Step1 = new UserSignInStep1();
            this.Step2 = new UserSignInStep2();
            this.Step3 = new UserSignInStep3();

        }

    }

    public class UserSignInStep1
    {
        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[a-zA-Z][a-zA-Z0-9._ ]+$", ErrorMessage = "Please use valid full name format")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-??]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Please use valid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password should be minimum of 6 and maximum of 20 characters.")]
        public string Password { get; set; }

        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password and Confirm Password does not match")]
        public string ConfirmPassword { get; set; }

    }
    public class UserSignInStep2
    {
        [Required(ErrorMessage = "Country is required")]
        public string CountryID { get; set; }

        [Required(ErrorMessage = "State is required")]
        public string StateID { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string CityID { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Zip Code is required")]
        public string Zip { get; set; }

        //[IsTrueAttribute(ErrorMessage = "Please accept Terms & Conditions")]
        public bool Terms { get; set; }

        public bool ReceiveEmail { get; set; }

        [JsonIgnore]
        public List<SelectListItem> CountryList { get; set; }
        [JsonIgnore]
        public List<SelectListItem> StateList { get; set; }
        [JsonIgnore]
        public List<SelectListItem> CityList { get; set; }
        public UserSignInStep2()
        {
            this.CountryList = new List<SelectListItem>();
            this.CityList = new List<SelectListItem>();
            this.StateList = new List<SelectListItem>();
        }

    }
    public class UserSignInStep3
    {
        public bool Terms { get; set; }
    }

    public class ApiUserModel
    {
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }
        public string PhoneNumber { get; set; }
        public string SessionId { get; set; }
        public int UserId { get; set; }
        public int UserType { get; set; }
    }

    public partial class ContactUsModel
    {
        [JsonIgnore]
        public int ID { get; set; }
        public string Question { get; set; }
        public string Message { get; set; }

        public Nullable<int> RequestType { get; set; }
        [JsonIgnore]
        public int UserID { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> AddedOn { get; set; }
        [JsonIgnore]
        public Nullable<bool> IsResolved { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> ResolvedOn { get; set; }
        [JsonIgnore]
        public Nullable<bool> IsDeleted { get; set; }
        [JsonIgnore]
        public Nullable<System.DateTime> DeletedOn { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }

    public class EditorModel
    {
        public string response { get; set; }
    }
    public class UserNotificationSetting
    {
        public int UserID { get; set; }
        public bool OrderPlaced { get; set; }
        public bool OrderStatus { get; set; }
        public bool IsToolTipShow { get; set; }
    }
}