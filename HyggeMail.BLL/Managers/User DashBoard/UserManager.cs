using HyggeMail.BLL.Interfaces;
using HyggeMail.BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic;
using HyggeMail.DAL;
using HyggeMail.BLL.Common;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using AutoMapper;
using System.IO;
using System.Web;
using HyggeMail.MailChimp;

namespace HyggeMail.BLL.Managers
{
    public class UserManager : BaseManager, IUserManager
    {
        public UserManager()
        {
        }

        string IUserManager.GetWelcomeMessage()
        {
            return "Welcome To Base Project Demo";
        }

        PagingResult<UserListingModel> IUserManager.GetUserPagedList(PagingModel model)
        {
            var result = new PagingResult<UserListingModel>();
            var query = Context.Users.OrderBy(model.SortBy + " " + model.SortOrder);
            if (!string.IsNullOrEmpty(model.Search))
            {
                query = query.Where(z => z.FirstName.Contains(model.Search) || z.LastName.Contains(model.Search) || z.Email.Contains(model.Search));
            }
            var list = query
               .Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage)
               .ToList().Select(x => new UserListingModel(x)).ToList();

            result.List = list;
            result.Status = ActionStatus.Successfull;
            result.Message = "User List";
            result.TotalCount = query.Count();
            return result;
        }


        public ActionOutput<apiUserDetail> ValidateSession(string sessionId)
        {
            Guid token;
            if (!Guid.TryParse(sessionId, out token))
            {
                return ReturnError<apiUserDetail>("Invalid Session");
            }
            var found = Context.Sessions.SingleOrDefault(p => p.Token == token);
            if (found != null)
            {
                if (found.IsExpired != true)
                {
                    return ReturnSuccess<apiUserDetail>(PopulateUD(found.User, sessionId));
                }
                return ReturnError<apiUserDetail>("Expired Session");
            }
            return ReturnError<apiUserDetail>("Invalid Session");
        }


        public ActionOutput<apiUserDetailShort> ValidateSessionAuth(string sessionId)
        {
            Guid token;
            if (!Guid.TryParse(sessionId, out token))
            {
                return ReturnError<apiUserDetailShort>("Invalid Session");
            }
            var found = Context.Sessions.SingleOrDefault(p => p.Token == token);
            if (found != null)
            {
                if (found.IsExpired != true)
                {
                    return ReturnSuccess<apiUserDetailShort>(PopulateUDFb(found.User, sessionId));
                }
                return ReturnError<apiUserDetailShort>("Expired Session");
            }
            return ReturnError<apiUserDetailShort>("Invalid Session");
        }


        protected apiUserDetail PopulateUD(User user, string sessionId)
        {
            //return new apiUserDetailShort()
            //{
            //    UserId = user.UserID,
            //    Email = user.Email,
            //    SessionId = sessionId,
            //    Name = string.Format("{0} {1}", user.FirstName, user.LastName),
            //    Role = ((UserTypes)user.UserType).ToString(),
            //    DeviceToken = user.DeviceToken,
            //    UserType = (int)user.UserType,
            //    Phone = user.UserDetail != null ? (user.UserDetail.Phone != null ? user.UserDetail.Phone : "") : null,
            //};
            var allcountries = Context.countries.ToList();
            var allstates = Context.states.ToList();
            var allcities = Context.cities.ToList();
            return new apiUserDetail(user, allcountries, allstates, allcities, sessionId);
        }

        protected apiUserDetailShort PopulateUDFb(User user, string sessionId)
        {
            return new apiUserDetailShort()
            {
                UserId = user.UserID,
                Email = user.Email,
                SessionId = sessionId,
                Name = string.Format("{0} {1}", user.FirstName, user.LastName),
                Role = ((UserTypes)user.UserType).ToString(),
                DeviceToken = user.DeviceToken,
                UserType = (int)user.UserType,
                Phone = user.UserDetail != null ? (user.UserDetail.Phone != null ? user.UserDetail.Phone : "") : null,
            };
        }

        ActionOutput IUserManager.UpdateUserDetails(UserModel userDetails, bool fromDashboard = false)
        {
            var user = Context.Users.Where(z => z.UserID == userDetails.UserId && !z.IsDeleted).FirstOrDefault();
            if (user == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "User Not Exist."
                };
            }
            var existngUser = Context.Users.Where(z => z.Email.Trim().ToLower() == userDetails.Email.Trim().ToLower() && z.UserID != userDetails.UserId && z.IsDeleted != true).FirstOrDefault();
            if (existngUser != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This email-id already exists for another user."
                };
            }
            else
            {
                if (user.Email == null)
                    user.Email = userDetails.Email.Trim().ToLower();
                user.FirstName = userDetails.FirstName;
                if (user.UserDetail == null)
                    user.UserDetail = new UserDetail();
                user.UserDetail.Country = userDetails.CountryID;
                user.UserDetail.City = userDetails.CityID;
                user.UserDetail.State = userDetails.StateID;
                user.UserDetail.Zip = userDetails.Zip;
                user.UserDetail.Address = userDetails.Address;
                user.UserDetail.Phone = userDetails.Phone;
                if (userDetails.Image != null)
                {
                    var rootPath = AttacmentsPath.UserProfileImages + user.FirstName.Replace(" ", "") + "-" + user.UserID + "/ProfileImage";
                    if (!Directory.Exists(rootPath))
                    {
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));
                    }
                    var imagepath = Utilities.SavePostedFile(rootPath, userDetails.Image);
                    user.UserDetail.ProfileImageName = userDetails.Image.FileName;
                    user.UserDetail.ProfileImagePath = rootPath + '/' + imagepath;
                }
                user.LastName = userDetails.LastName;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "User Details Updated Successfully."
                };
            }
        }


        ActionOutput<apiUserDetail> IUserManager.UpdateUserDetailsApp(UserModel userDetails, bool ProfileUpdate)
        {
            var user = Context.Users.Where(z => z.UserID == userDetails.UserId && !z.IsDeleted).FirstOrDefault();
            if (user == null)
            {
                return new ActionOutput<apiUserDetail>
                {
                    Status = ActionStatus.Error,
                    Message = "User Not Exist."
                };
            }
            var existngUser = Context.Users.Where(z => z.Email.Trim().ToLower() == userDetails.Email.Trim().ToLower() && z.UserID != userDetails.UserId && z.IsDeleted != true).FirstOrDefault();
            if (existngUser != null)
            {
                return new ActionOutput<apiUserDetail>
                {
                    Status = ActionStatus.Error,
                    Message = "This email-id already exists for another user."
                };
            }
            else
            {
                if (user.Email == null)
                    user.Email = userDetails.Email.Trim().ToLower();
                user.FirstName = userDetails.FirstName;
                if (user.UserDetail == null)
                    user.UserDetail = new UserDetail();
                user.UserDetail.Country = userDetails.CountryID;
                user.UserDetail.City = userDetails.CityID;
                user.UserDetail.State = userDetails.StateID;
                user.UserDetail.Zip = userDetails.Zip;

                user.UserDetail.Address = userDetails.Address;
                user.UserDetail.Phone = userDetails.Phone;

                // user.UserDetail.Address = userDetails.Address;
                // user.UserDetail.Phone = userDetails.Phone;

                if (userDetails.ImageString != null)
                {
                    var image = Utilities.LoadImage(userDetails.ImageString);
                    var imagename = Guid.NewGuid().ToString() + ".png";

                    var rootPath = AttacmentsPath.UserProfileImages + user.FirstName.Replace(" ", "") + "-" + user.UserID + "/ProfileImage";
                    if (!Directory.Exists(rootPath))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(rootPath));

                    image.Save(HttpContext.Current.Server.MapPath(rootPath) + "/" + imagename);

                    //var imagepath = Utilities.SavePostedFile(rootPath, userDetails.Image);

                    //user.UserDetail.ProfileImageName = userDetails.Image.FileName;
                    //user.UserDetail.ProfileImagePath = rootPath + '/' + imagepath;

                    user.UserDetail.ProfileImageName = imagename;
                    user.UserDetail.ProfileImagePath = rootPath + '/' + imagename;
                }
                user.LastName = userDetails.LastName;
                Context.SaveChanges();

                var model = BindUserDetailsWithSession(user.UserID, ProfileUpdate);

                return new ActionOutput<apiUserDetail>
                {
                    Status = ActionStatus.Successfull,
                    Message = "User Details Updated Successfully.",
                    Object = model
                };
            }
        }



        ActionOutput IUserManager.AddUserDetails(AddUserModel userDetails)
        {
            var existngUser = Context.Users.Where(z => z.Email.Trim().ToLower() == userDetails.Email.Trim().ToLower()).FirstOrDefault();
            if (existngUser != null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "This email-id already exists for another user."
                };
            }
            else
            {
                Context.Users.Add(new User
                {
                    FirstName = userDetails.FirstName,
                    LastName = userDetails.LastName,
                    Email = userDetails.Email.Trim().ToLower(),
                    Password = Utilities.EncryptPassword(userDetails.Password, true),
                    CreatedAt = DateTime.Now,
                    IsActivated = true,
                    ActivatedOn = DateTime.UtcNow,
                    AuthenticationType = (int)AuthenticationType.Normal,
                    UserType = 2
                });
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "User Added Successfully."
                };
            }
        }

        ActionOutput IUserManager.CheckAlreadyFaceBookAuthenticated(FacebookUserDetails userDetails)
        {
            var existngUser = Context.Users.Where(z => z.UserDetail.FacebookID == userDetails.ID && z.Email.Trim().ToLower() == userDetails.Email.Trim().ToLower()).FirstOrDefault();
            if (existngUser != null)
                return new ActionOutput() { Message = "User Already Exist.", Status = ActionStatus.Error };
            else
                return new ActionOutput() { Message = "User Not Exist.", Status = ActionStatus.Error };
        }

        UserModel IUserManager.LoginWithFaceBook(FacebookUserDetails model)
        {
            var user = Context.Users.Where(z => z.UserDetail.FacebookID == model.ID && !z.IsDeleted).FirstOrDefault();
            if (user == null)
                return null;
            else
                return new UserModel(user);
        }

        ActionOutput<apiUserDetailShort> IUserManager.FaceBookAuthentication(FacebookUserDetails userDetails)
        {
            try
            {
                var existngUser = Context.Users.Where(z => z.UserDetail.FacebookID == userDetails.ID || z.Email == userDetails.Email).FirstOrDefault();
                if (existngUser != null)
                {
                    return new ActionOutput<apiUserDetailShort>
                    {
                        Status = ActionStatus.Successfull,
                        Message = "Already Registered.",
                        Object = BindUserDetailsWithSessionFb(existngUser.UserID)
                    };
                }
                else
                {
                    var user = new User();
                    user.FirstName = userDetails.Name;
                    user.LastName = "";
                    if (userDetails.Email != null)
                        user.Email = userDetails.Email.Trim().ToLower();
                    else
                        user.Email = "";
                    user.CreatedAt = DateTime.Now;
                    user.IsActivated = true;
                    user.IsDeleted = false;
                    user.ActivatedOn = DateTime.UtcNow;
                    user.AuthenticationType = (int)AuthenticationType.Facebook;
                    user.MemberShipType = 1;
                    user.CardsCount = 10;
                    user.UserType = (int)UserTypes.User;
                    user.UserDetail = new UserDetail() { Address = userDetails.Location, FacebookID = userDetails.ID, DOB = Convert.ToDateTime(userDetails.BirthDay) };
                    user.Facebook_ID = userDetails.ID;
                    Context.Users.Add(user);
                    Context.SaveChanges();
                    //IUserManager _u = new UserManager();
                    //if (userDetails.Email != null)
                    //    _u.SendNewUserRegistrationEmail(user.UserID, user);

                    IEmailManager _e = new EmailManager();
                    _e.SendWelcomeMailToNewUser(user);

                    return new ActionOutput<apiUserDetailShort>
                    {
                        Status = ActionStatus.Successfull,
                        Message = "User Added Successfully.",
                        Object = BindUserDetailsWithSessionFb(user.UserID)
                    };
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }


        ActionOutput<apiUserDetail> IUserManager.FaceBookAuthenticationApp(FacebookUserDetailsApp userDetails)
        {
            try
            {
                var existngUser = Context.Users.Where(z => z.UserDetail.FacebookID == userDetails.ID || z.Email == userDetails.Email).FirstOrDefault();
                if (existngUser != null)
                {
                    var data = BindUserDetailsWithSession(existngUser.UserID);
                    data.IsLoginWithFb = true;
                    return new ActionOutput<apiUserDetail>
                    {
                        Status = ActionStatus.Successfull,
                        Message = "Already Registered.",
                        Object = data
                    };
                }
                else
                {
                    var user = new User();
                    user.FirstName = userDetails.Name;
                    user.LastName = "";
                    if (userDetails.Email != null)
                        user.Email = userDetails.Email.Trim().ToLower();
                    else
                        user.Email = "";
                    user.CreatedAt = DateTime.Now;
                    user.IsActivated = true;
                    user.IsDeleted = false;
                    user.ActivatedOn = DateTime.UtcNow;
                    user.AuthenticationType = (int)AuthenticationType.Facebook;
                    user.MemberShipType = 1;
                    user.CardsCount = 10;
                    user.UserType = (int)UserTypes.User;
                    user.UserDetail = new UserDetail() { Address = userDetails.Location, FacebookID = userDetails.ID };
                    user.Facebook_ID = userDetails.ID;
                    Context.Users.Add(user);
                    Context.SaveChanges();
                    //IUserManager _u = new UserManager();
                    //if (userDetails.Email != null)
                    //    _u.SendNewUserRegistrationEmail(user.UserID, user);

                    IEmailManager _e = new EmailManager();
                    _e.SendWelcomeMailToNewUser(user);

                    var data = BindUserDetailsWithSession(existngUser.UserID);
                    data.IsLoginWithFb = true;
                    return new ActionOutput<apiUserDetail>
                    {
                        Status = ActionStatus.Successfull,
                        Message = "User Added Successfully.",
                        Object = data
                    };



                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        ActionOutput IUserManager.SignUpUser(UserRegistrationModel userDetails)
        {
            try
            {
                var existngUser = Context.Users.Where(z => z.Email.Trim().ToLower() == userDetails.Step1.Email.Trim().ToLower()).FirstOrDefault();
                if (existngUser != null)
                {
                    return new ActionOutput
                    {
                        Status = ActionStatus.Error,
                        Message = "This email-id already exists for another user."
                    };
                }
                else
                {
                    var user = new User();
                    user.FirstName = userDetails.Step1.FullName;
                    user.LastName = "";
                    user.Email = userDetails.Step1.Email.Trim().ToLower();
                    user.Password = Utilities.EncryptPassword(userDetails.Step1.Password, true);
                    user.CreatedAt = DateTime.Now;
                    //user.IsActivated = true;
                    //user.ActivatedOn = DateTime.UtcNow;
                    user.IsDeleted = false;
                    user.UserType = (int)UserTypes.User;
                    user.AuthenticationType = (int)AuthenticationType.Normal;
                    user.UserDetail = new UserDetail() { City = userDetails.Step2.CityID, State = userDetails.Step2.StateID, Country = userDetails.Step2.CountryID, Address = userDetails.Step2.Address, Zip = userDetails.Step2.Zip };
                    user.MemberShipType = 1;
                    user.CardsCount = 10;
                    Context.Users.Add(user);
                    Context.SaveChanges();
                    IEmailManager _e = new EmailManager();
                    //_e.SendVerifyMailToNewUser(user.UserID, user);
                    _e.SendWelcomeMailToNewUser(user);

                    MailChimpService.AddOrUpdateListMember(subscriberEmail: user.Email, listId: System.Configuration.ConfigurationManager.AppSettings["SignUpListId"]);
                    if (userDetails.Step2.ReceiveEmail)
                        MailChimpService.AddOrUpdateListMember(subscriberEmail: user.Email, listId: System.Configuration.ConfigurationManager.AppSettings["SubListId"]);

                    return new ActionOutput
                    {
                        Status = ActionStatus.Successfull,
                        Message = "User Added Successfully. Please check your mail and verify your account."
                    };
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        UserModel IUserManager.UserLogin(LoginModal model)
        {
            string decryptPass = Utilities.EncryptPassword(model.Password, true);
            string enc = Utilities.DecryptPassword("uQS5cQ33xnhUathFPYSbuw==", true);
            string enc1 = Utilities.DecryptPassword("vebnVuzqNff7fny2ImOsAQ==", true);
            string enc2 = Utilities.DecryptPassword("XX9cLlgM4FQ=", true);

            var user = Context.Users.Where(z => z.Email.ToLower() == model.UserName.ToLower() && !z.IsDeleted && z.Password == decryptPass).FirstOrDefault();
            if (user == null)
                return null;
            else
                return new UserModel(user);
        }

        UserModel IUserManager.AdminLogin(LoginModal model)
        {
            string decryptPass = Utilities.EncryptPassword(model.Password, true);

            var user = Context.Users.Where(z => z.Email.ToLower() == model.UserName.ToLower() && !z.IsDeleted && z.Password == decryptPass && z.UserType == 1).FirstOrDefault();
            if (user == null)
                return null;
            else
                return new UserModel(user);
        }

        UserModel IUserManager.GetAdminDetails()
        {
            var user = Context.Users.Where(z => z.UserType == 1).FirstOrDefault();
            if (user == null)
                return null;
            else
                return new UserModel(user);
        }

        bool IUserManager.SendNewUserRegistrationEmail(int userID, User model)
        {
            IEmailManager _emailManager = new EmailManager();
            var retVal = false;

            var user = Context.Users.Where(x => x.UserID == userID).FirstOrDefault();
            //Send welcome Email
            EmailTemplateModel emaildata = _emailManager.GetEmailTemplateByType(Convert.ToInt32(TemplateTypes.WelcomeEmail));
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.ActivatedUID = token;
            Context.SaveChanges();
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "USERNAME" + gt, user.Email);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "EMAIL" + gt, user.Email);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "PASSWORD" + gt, Utilities.DecryptPassword(user.Password, true));
            //   emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "STATE" + gt, user.state.stateName);

            //  emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "COUNTRY" + gt, user.country.countryName);
            //    emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "PHONE" + gt, user);
            var Domain = Config.Link;
            string url = string.Format("{0}/Account/EmailVerification?token={1}", Domain, token);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "URL" + gt, url);

            var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;
        }

        ActionOutput<UserModel> IUserManager.GetUserDetailsByUserId(int userId)
        {
            IUserManager _um = new UserManager();
            var user = Context.Users.Where(z => z.UserID == userId).FirstOrDefault();
            var model = new UserModel(user);
            model.CountryList = _um.GetCountries();
            if (user.UserDetail != null)
            {
                //var state = user.UserDetail.State ?? 0;
                //model.StateList = _um.GetStateNameByStateID(state);
                //var city = user.UserDetail.City ?? 0;
                //model.CityList = _um.GetCityNameByCityID(city);
            }
            if (user == null)
                return new ActionOutput<UserModel>() { Object = null, Message = "No Record Found.", Status = ActionStatus.Error };
            else
                return new ActionOutput<UserModel>() { Object = model, Message = "User Record Found.", Status = ActionStatus.Successfull };
        }

        ActionOutput IUserManager.DeleteUser(int userId)
        {
            var user = Context.Users.Where(z => z.UserID == userId).FirstOrDefault();
            var message = "";
            if (user == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "User Not Exist."
                };
            }
            else
            {
                if (user.IsDeleted == true)
                {
                    user.IsDeleted = false;
                    user.DeletedOn = null;
                    message = "User unblocked Successfully.";
                }
                else
                {
                    user.IsDeleted = true;
                    user.DeletedOn = DateTime.Now;
                    message = "User blocked Successfully.";
                }
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = message
                };
            }
        }

        ActionOutput IUserManager.ActivateUser(int userId)
        {
            var user = Context.Users.Where(z => z.UserID == userId).FirstOrDefault();
            if (user == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "User Not Exist."
                };
            }
            else
            {
                var statusMessage = "";
                user.IsActivated = true;
                user.IsDeleted = false;
                statusMessage = "User account activated successfully.";
                //if (user.IsActive == true)
                //{
                //    user.IsActive = false;
                //    user.IsDeleted = true;
                //    statusMessage = "User Account De-Activated successfully.";
                //}
                //else
                //{
                //    user.IsActive = true;
                //    user.IsDeleted = false;
                //    statusMessage = "User Account Activated successfully.";
                //}
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = statusMessage
                };
            }
        }

        ActionOutput IUserManager.SendAccountVerificationMail(int userId)
        {
            try
            {
                var existngUser = Context.Users.Where(z => z.UserID == userId).FirstOrDefault();
                if (existngUser != null)
                {
                    IEmailManager _e = new EmailManager();
                    _e.SendWelcomeMailToNewUser(existngUser);

                    return new ActionOutput
                    {
                        Status = ActionStatus.Error,
                        Message = "Email has been sent."
                    };
                }
                else
                {
                    return new ActionOutput
                    {
                        Status = ActionStatus.Successfull,
                        Message = "Invalid user account."
                    };
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
        }

        UserModel IUserManager.GetUserByForgotToken(string token)
        {
            token = token.Replace(' ', '+');
            if (token != null)
            {
                var user = Context.Users.Where(z => z.ForgotUID == token && z.IsActivated == true && !z.IsDeleted).FirstOrDefault();
                if (user == null)
                    return null;
                else
                    return new UserModel(user);
            }
            return null;
        }

        UserModel IUserManager.GetUserById(int userid)
        {
            if (userid > 0)
            {
                var user = Context.Users.Where(z => z.UserID == userid && z.IsActivated == true && !z.IsDeleted).FirstOrDefault();
                if (user == null)
                    return null;
                else
                    return new UserModel(user);
            }
            return null;
        }


        bool IUserManager.ResetPassword(UserModel user)
        {
            bool result = false;
            if (user != null)
            {
                var checkUser = Context.Users.Where(z => z.UserID == user.UserId && !z.IsDeleted).FirstOrDefault();
                if (checkUser != null)
                {
                    string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    IUserManager _userManager = new UserManager();
                    var sendMail = _userManager.SendForgotPassword(checkUser.UserID, checkUser);
                    if (sendMail == true)
                        result = true;
                }
            }
            return result;
        }

        ActionOutput IUserManager.ChangePassword(ResetPasswordModel model)
        {
            var result = new ActionOutput();
            if (model.UserId > 0)
            {
                var checkUser = Context.Users.Where(z => z.UserID == model.UserId).FirstOrDefault();
                if (checkUser != null)
                {
                    string newHashPass = Utilities.EncryptPassword(model.NewPassword, true);
                    checkUser.Password = newHashPass;
                    checkUser.ForgotUID = "";
                    Context.SaveChanges();
                    result = new ActionOutput() { Message = "Password updated successfully. Please login with new password.", Status = ActionStatus.Successfull };
                }
                else
                    result = new ActionOutput() { Message = "No record found for this user.", Status = ActionStatus.Error };
            }
            return result;
        }

        string IUserManager.SetUserActive(int Id)
        {
            string result = "";
            if (Id > 0)
            {
                var user = Context.Users.Where(z => z.UserID == Id && !z.IsDeleted).FirstOrDefault();
                if (user != null)
                {
                    if (user.IsActivated == true)
                    {
                        result = "Your Account already verified";
                    }
                    else
                    {
                        user.IsActivated = true;
                        user.ActivatedOn = DateTime.UtcNow;
                        Context.SaveChanges();
                        result = "Account verified successfully you can access your account.";
                        IUserManager _u = new UserManager();
                        //Send email to User with details
                        _u.SendNewUserRegistrationEmail(user.UserID, user);
                    }
                }
            }
            return result;
        }


        bool IUserManager.SendVerifyMailToNewUser(int userID, User model)
        {
            var retVal = false;
            IEmailManager _emailManager = new EmailManager();
            var user = Context.Users.Where(x => x.UserID == userID).FirstOrDefault();
            //Send welcome Email
            EmailTemplateModel emaildata = _emailManager.GetEmailTemplateByType(Convert.ToInt32(TemplateTypes.WelcomeEmail));
            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            user.ActivatedUID = token;
            Context.SaveChanges();
            var lt = "&lt;%";
            var gt = "%&gt;";
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "USERNAME" + gt, user.Email);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "EMAIL" + gt, user.Email);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "PASSWORD" + gt, Utilities.DecryptPassword(user.Password, true));
            //   emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "STATE" + gt, user.state.stateName);

            //  emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "COUNTRY" + gt, user.country.countryName);
            //emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "PHONE" + gt, user.Phone);
            var Domain = Config.Link;
            string url = string.Format("{0}/Account/EmailVerification?token={1}", Domain, token);
            emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "URL" + gt, url);

            var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
            if (result == true)
            {
                retVal = true;
            }
            return retVal;

        }

        ActionOutput IUserManager.UpdateUserPassword(ChangePasswordModel model)
        {
            try
            {
                var user = Context.Users.FirstOrDefault(x => x.UserID == model.userID);
                if (user.Password == Utilities.EncryptPassword(model.OldPassword, true))
                {
                    user.Password = Utilities.EncryptPassword(model.NewPassword, true);
                    SaveChanges();
                    return new ActionOutput() { Message = "You have logged out from your dashboard because your password is updated successfully.Please login with your new password.", Status = ActionStatus.Successfull };
                }
                else
                {
                    return new ActionOutput() { Message = "Old password is incorrect", Status = ActionStatus.Error };
                }
            }
            catch (Exception ex)
            {
                return new ActionOutput() { Message = "Internal server error", Status = ActionStatus.Error };
            }
        }

        UserModel IUserManager.GetUserByToken(string token)
        {
            if (token != null)
            {
                token = token.Replace(' ', '+');
                var user = Context.Users.Where(z => z.ActivatedUID == token).FirstOrDefault();
                if (user == null)
                    return null;
                else
                    return new UserModel(user);
            }
            return null;
        }

        bool IUserManager.SendForgotPassword(int userID, User model)
        {
            var retVal = false;
            IEmailManager _emailManager = new EmailManager();
            var user = Context.Users.Where(x => x.UserID == userID).FirstOrDefault();
            //Send welcome Email
            EmailTemplateModel emaildata = new EmailTemplateModel();
            if (user.IsActivated == true)
                emaildata = _emailManager.GetEmailTemplateByType(Convert.ToInt32(TemplateTypes.ResetPassword));
            if (emaildata != null)
            {
                string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                user.ForgotUID = token;
                Context.SaveChanges();
                var lt = "&lt;%";
                var gt = "%&gt;";
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "NAME" + gt, user.FirstName + ' ' + user.LastName);
                var Domain = Config.Link;
                string url = string.Format("{0}/Home/ResetPassword?token={1}", Domain, token);
                if (user.IsActivated != true)
                {
                    string verficationLink = string.Format("{0}/Account/EmailVerification?token={1}", Domain, user.ActivatedUID);
                    emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "VERIFICATIONURL" + gt, verficationLink);
                }
                emaildata.TemplateContent = emaildata.TemplateContent.Replace(lt + "URL" + gt, url);
                IErrorLogManager _errorManager = new ErrorLogManager();
                var result = Utilities.SendEMail(user.Email, emaildata.EmailSubject, emaildata.TemplateContent);
                if (result == true)
                {
                    retVal = true;
                    _errorManager.LogStringExceptionToDatabase("Mail Sent Successfully");
                }
                else
                    _errorManager.LogStringExceptionToDatabase("Mail Sent Failed");
            }
            return retVal;

        }

        List<SelectListItem> IUserManager.GetCityByState(Int64 state)
        {
            return Context.cities.Where(x => x.state_id == state).OrderByDescending(x => x.cityName).ToList().Select(c =>
                new SelectListItem { Text = c.cityName, Value = Convert.ToString(c.cityID) }).ToList();
        }

        List<SelectListItem> IUserManager.GetStateByCountry(Int32 country)
        {
            return Context.states.Where(x => x.country_id == country).OrderBy(x => x.name + " " + "Asc").ToList().Select(c =>
                new SelectListItem { Text = c.name, Value = Convert.ToString(c.id) }).ToList();
        }

        List<SelectListItem> IUserManager.GetCountries()
        {
            var list = Context.countries.ToList().Select(c =>
                new SelectListItem { Text = c.name, Value = Convert.ToString(c.id) }).ToList();
            return list;
        }

        List<SelectListItem> IUserManager.GetStates()
        {
            var list = Context.states.ToList().Select(c =>
                new SelectListItem { Text = c.name, Value = Convert.ToString(c.id) }).ToList();
            return list;
        }

        List<SelectListItem> IUserManager.GetCities()
        {
            var list = Context.cities.ToList().Select(c =>
                new SelectListItem { Text = c.cityName, Value = Convert.ToString(c.cityID) }).ToList();
            return list;
        }

        UserModel IUserManager.GetUserByEmail(string Email)
        {
            if (Email != null)
            {
                var model = new UserModel();
                var user = Context.Users.Where(z => z.Email.ToLower() == Email.ToLower() && !z.IsDeleted).FirstOrDefault();
                if (user == null)
                    return null;
                else
                    return new UserModel(user);
            }
            return null;
        }


        List<SelectListItem> IUserManager.GetCityNameByCityID(int cityID)
        {
            return Context.cities.Where(x => x.cityID == cityID).OrderByDescending(x => x.cityName).ToList().Select(c =>
                         new SelectListItem { Text = c.cityName, Value = Convert.ToString(c.cityID) }).ToList();
        }

        List<SelectListItem> IUserManager.GetStateNameByStateID(int stateID)
        {
            return Context.states.Where(x => x.id == stateID).OrderByDescending(x => x.name).ToList().Select(c =>
                         new SelectListItem { Text = c.name, Value = Convert.ToString(c.id) }).ToList();
        }

        #region API Manager

        /// <summary>
        ///  Used to verify before creating new session, existing sessions are expired or not.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsAlreadySessionExist(LoginModal model)
        {
            var found = Context.Users.FirstOrDefault(p => p.Email == model.UserName && p.Password == model.Password && p.IsActivated == true && p.IsDeleted != true);
            if (found != null)
            {
                var session = Context.Sessions.Where(x => x.UserId == found.UserID)
                      .OrderByDescending(a => a.CreatedOn)
                      .FirstOrDefault();
                if (session != null)
                {
                    return session.ExpiredOn == null; // if ExpiresOn exist then User Logout properly.
                }
                return false;
            }
            return false;
        }

        /// <summary>
        ///  Get Last Session details from Login User Name and password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DeviceModel GetLastSessionDetails(LoginModal model)
        {
            var found = Context.Users.FirstOrDefault(p => p.Email == model.UserName && p.Password == model.Password && p.IsActivated == true && p.IsDeleted != true);
            if (found != null)
            {
                var session = Context.Sessions.Where(x => x.UserId == found.UserID)
                      .OrderByDescending(a => a.CreatedOn)
                      .FirstOrDefault();
                if (session != null)
                {
                    var retVal = new DeviceModel();
                    retVal.DeviceToken = session.DeviceToken;
                    retVal.DeviceType = session.DeviceType != null ? (DeviceType)session.DeviceType : DeviceType.Android;
                    retVal.UserId = found.UserID;
                    return retVal;
                }
                return null;
            }
            return null;
        }
        public ActionOutput<apiUserDetail> AuthenticateUserOnMobile(LoginModal model)
        {
            var EncryptedPassord = Utilities.EncryptPassword(model.Password, true);
            //var dec = Utilities.DecryptPassword("vItkf86cpJ4=", true);
            var found = Context.Users.FirstOrDefault(p => p.Email.ToLower() == model.UserName.ToLower() && p.UserType != (int)UserTypes.Admin && p.IsDeleted != true && p.IsActivated == true);
            if (found != null)
            {
                if (found.Password == EncryptedPassord)
                {
                    var apiModel = BindUserDetailsWithSession(found.UserID);
                    apiModel.IsLoginWithFb = false;
                    return ReturnSuccess<apiUserDetail>(apiModel);
                }
                else
                    //In correct Password
                    return ReturnError<apiUserDetail>("Incorrect Password");
            }
            else
                //User Doesn't exist
                return ReturnError<apiUserDetail>("Email doesn't exists");
        }

        public apiUserDetail BindUserDetailsWithSession(int userID, bool IsUpdateProfile = false)
        {
            var user = Context.Users.FirstOrDefault(x => x.UserID == userID);
            //Verification pending by user
            // Expire old sessions
            if (!IsUpdateProfile)
            {
                Context.Sessions.Where(p => p.UserId == user.UserID).ToList().ForEach(p => { p.IsExpired = true; });
                // Create new session
                var newSession = new Session()
                {
                    CreatedOn = DateTime.Now,
                    Token = Guid.NewGuid(),
                    UserId = user.UserID,
                };

                Context.Sessions.Add(newSession);
                var apiModel = PopulateUD(user, newSession.Token.ToString());
                user.LastLogin = DateTime.Now;
                SaveChanges();
                return apiModel;
            }
            else
            {
                var newSession = Context.Sessions.SingleOrDefault(p => p.UserId == user.UserID && p.IsExpired == null && p.ExpiredOn == null);
                var apiModel = PopulateUD(user, newSession.Token.ToString());
                return apiModel;
            }
        }

        public apiUserDetailShort BindUserDetailsWithSessionFb(int userID)
        {
            var user = Context.Users.FirstOrDefault(x => x.UserID == userID);
            //Verification pending by user
            // Expire old sessions
            Context.Sessions.Where(p => p.UserId == user.UserID).ToList().ForEach(p => { p.IsExpired = true; });
            // Create new session
            var newSession = new Session()
            {
                CreatedOn = DateTime.Now,
                Token = Guid.NewGuid(),
                UserId = user.UserID,
            };
            Context.Sessions.Add(newSession);
            var apiModel = PopulateUDFb(user, newSession.Token.ToString());
            user.LastLogin = DateTime.Now;
            SaveChanges();
            return apiModel;
        }

        #region api logout

        public ActionOutput APILogout(string sessionId, int userID)
        {
            if (sessionId != null)
            {
                var sessionToken = new Guid(sessionId);
                var list = Context.Sessions.Where(p => p.Token == sessionToken && p.IsExpired == null && p.UserId == userID)
                    .ToList();
                foreach (var session in list)
                {
                    session.IsExpired = true;
                    session.ExpiredOn = DateTime.Now;
                }
                Context.SaveChanges();
                return new ActionOutput { Status = ActionStatus.Successfull, Message = "Logout successfully." };
            }
            else return new ActionOutput { Status = ActionStatus.Error, Message = "Error occured during logout." };
        }

        public void ExpirePreviousSessions(LoginModal user)
        {
            var userid = Context.Users.FirstOrDefault(p => p.Email == user.UserName).UserID;
            if (userid > 0)
            {
                var list = Context.Sessions.Where(p => p.IsExpired == null && p.UserId == userid)
                    .ToList();
                foreach (var session in list)
                {
                    session.IsExpired = true;
                    session.ExpiredOn = DateTime.Now;
                }
                Context.SaveChanges();
            }
        }



        #endregion api logout

        #region Contact Us
        ActionOutput IUserManager.RequestContactUs(ContactUsModel model)
        {
            try
            {
                var contact = new ContactU();
                contact = Mapper.Map<ContactUsModel, ContactU>(model, contact);
                contact.AddedOn = DateTime.UtcNow;
                contact.IsDeleted = false;
                contact.IsResolved = false;
                contact.IsFromWeb = false;
                Context.ContactUs.Add(contact);
                Context.SaveChanges();
                return new ActionOutput() { Message = "Request sent successfully.", Status = ActionStatus.Successfull };
            }
            catch (Exception ex)
            {
                return new ActionOutput() { Message = "Internal server error", Status = ActionStatus.Error };
            }
        }
        #endregion


        #endregion

        #region Web Contact Us
        ActionOutput IUserManager.WebRequestContactUs(WebContactUsModel model)
        {
            try
            {
                var contact = new ContactU();
                contact = Mapper.Map<WebContactUsModel, ContactU>(model, contact);
                contact.AddedOn = DateTime.UtcNow;
                contact.IsDeleted = false;
                contact.IsResolved = false;
                contact.IsFromWeb = true;
                Context.ContactUs.Add(contact);
                Context.SaveChanges();
                return new ActionOutput() { Message = "Request sent successfully.", Status = ActionStatus.Successfull };
            }
            catch (Exception ex)
            {
                return new ActionOutput() { Message = "Internal server error", Status = ActionStatus.Error };
            }
        }
        #endregion


        ActionOutput<UserNotificationSetting> IUserManager.GetUserNotificationSettings(int userId)
        {
            var result = new ActionOutput<UserNotificationSetting>();
            var user = Context.Users.Where(z => z.UserID == userId).FirstOrDefault();
            if (user == null)
            {
                return new ActionOutput<UserNotificationSetting>()
                {
                    Status = ActionStatus.Error,
                    Message = "User Not Exist."
                };
            }
            else
            {
                var userDet = new UserNotificationSetting();
                userDet.OrderPlaced = user.OrderPlacedNotification ?? false;
                userDet.OrderStatus = user.OrderStatusNotification ?? false;
                userDet.IsToolTipShow = user.IsToolTipShow ?? false;
                userDet.UserID = user.UserID;
                return new ActionOutput<UserNotificationSetting>()
                {
                    Status = ActionStatus.Successfull,
                    Message = "User Notification Details",
                    Object = userDet
                };
            }
        }

        ActionOutput IUserManager.SetUserNotificationSettings(UserNotificationSetting model)
        {
            var result = new ActionOutput<UserNotificationSetting>();
            var user = Context.Users.Where(z => z.UserID == model.UserID).FirstOrDefault();
            if (user == null)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "User Not Exist."
                };
            }
            else
            {
                user.OrderPlacedNotification = model.OrderPlaced;
                user.OrderStatusNotification = model.OrderStatus;
                user.IsToolTipShow = model.IsToolTipShow;
                user.UserID = user.UserID;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "User Notification Details updated successfully.",
                };
            }
        }

        ActionOutput IUserManager.GetUserCardStatus(int userID)
        {
            var userCards = Context.Users.Find(userID).CardsCount;
            if (userCards <= 0)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Greetings! You have used up all your tokens.  Would you like to purchase a membership?"
                };
            }
            else
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "You have '" + userCards + "' post cards left in your account"
                };
            }

        }

        ActionOutput IUserManager.DontShowAgain(int userID)
        {
            var userCards = Context.Users.FirstOrDefault(x => x.UserID == userID);
            if (userCards != null)
            {
                userCards.IsToolTipShow = false;
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Now you will not see any helping tooltip. To activate please set this from setting profile page."
                };
            }
            else
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "User not found."
                };
            }
        }

        bool IUserManager.GetUserCardStepGuidance(int userID)
        {
            return Context.Users.Find(userID).IsToolTipShow ?? false;
        }

        PagingResult<UserHistoryModel> IUserManager.GetUserHistoryPageList(PagingModel model)
        {
            var result = new PagingResult<UserHistoryModel>();
            var query = Context.UserHistories.Where(x => x.UserFK == model.UserID).OrderBy(model.SortBy + " " + model.SortOrder);

            if (!string.IsNullOrEmpty(model.Search))
                query = query.Where(z => z.Type.Contains(model.Search));
            var list = query.Skip((model.PageNo - 1) * model.RecordsPerPage).Take(model.RecordsPerPage).ToList();
            result.List = Mapper.Map<List<UserHistory>, List<UserHistoryModel>>(list.ToList(), result.List);



            result.Status = ActionStatus.Successfull;
            result.Message = "User History List";
            result.TotalCount = query.Count();
            return result;
        }

        ActionOutput IUserManager.AddHistory(UserHistoryModel UserHistoryModel)
        {
            try
            {
                var newUserHistory = new UserHistory();
                newUserHistory = Mapper.Map<UserHistoryModel, UserHistory>(UserHistoryModel, newUserHistory);
                Context.UserHistories.Add(newUserHistory);
                Context.SaveChanges();
                return new ActionOutput
                {
                    Status = ActionStatus.Successfull,
                    Message = "Sucessfully Added."
                };
            }
            catch (Exception ex)
            {
                return new ActionOutput
                {
                    Status = ActionStatus.Error,
                    Message = "Internal Server error."
                };
            }
        }

        public ReceiptentOrder ViewRecipientOrder(int OrderID)
        {
            var orderReceiptent = Context.UserPostCardRecipients.FirstOrDefault(x => x.ID == OrderID);
            return orderReceiptent != null ? new ReceiptentOrder(orderReceiptent) : null;
        }


        int IUserManager.GetSessionByToken(string Token)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                Guid guidOutput;
                bool isValid = Guid.TryParse(Token, out guidOutput);
                if (isValid)
                {
                    var session = Context.Sessions.FirstOrDefault(p => p.Token == guidOutput && p.IsExpired == null && p.ExpiredOn == null);
                    if (session == null)
                        return 0;
                    else
                        return session.UserId;
                }
                else
                    return 0;
            }
            else
                return 0;
        }
    }
}

