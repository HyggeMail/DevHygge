using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Reflection;
using HyggeMail.BLL.Models;

namespace HyggeMail.BLL.Common
{
    public static class Utilities
    {
        public static List<SelectListItem> EnumToList(Type en)
        {
            var itemValues = en.GetEnumValues();
            var list = new List<SelectListItem>();

            foreach (var value in itemValues)
            {
                var name = en.GetEnumName(value);
                var member = en.GetMember(name).Single();
                var desc = ((DescriptionAttribute)member.GetCustomAttributes(typeof(DescriptionAttribute), false).Single()).Description;
                list.Add(new SelectListItem { Text = desc, Value = ((int)value).ToString() });
            }
            return list;
        }
        /// <summary>
        /// Get the whitespace separated values of an enum
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public static string ToEnumWordify(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            string pascalCaseString = memInfo[0].Name;
            Regex r = new Regex("(?<=[a-z])(?<x>[A-Z])|(?<=.)(?<x>[A-Z])(?=[a-z])");
            return r.Replace(pascalCaseString, " ${x}");
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        public static string EnumDescription(Type val, int curVal)
        {
            var itemValues = val.GetEnumValues();
            var name = val.GetEnumName(curVal);
            var member = val.GetMember(name).Single();
            var desc = ((DescriptionAttribute)member.GetCustomAttributes(typeof(DescriptionAttribute), false).Single()).Description;
            return desc;
        }
        public static string GetDescription(Type en, object value, bool getText = false)
        {
            try
            {
                var name = en.GetEnumName(value);
                var member = en.GetMember(name).Single();
                var desc = getText ? name : value.ToString();
                var descAttr = (DescriptionAttribute)member.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault();
                if (descAttr != null) desc = descAttr.Description;
                return desc;
            }
            catch { return ""; };
        }

        public static string GetCurrentCultureDateTime(DateTime datetime, string cul)
        {
            DateTime dt = DateTime.Now;
            // Sets the CurrentCulture property to U.S. English.
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cul);
            // Displays dt, formatted using the ShortDatePattern
            // and the CurrentThread.CurrentCulture.
            Console.WriteLine(dt.ToString());

            // Creates a CultureInfo for German in Germany.
            CultureInfo ci = new CultureInfo(cul);
            // Displays dt, formatted using the ShortDatePattern
            // and the CultureInfo.
            return (dt.ToString(ci));
        }

        public static string FormatJsonString(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return string.Empty;
            }

            if (json.StartsWith("["))
            {
                // Hack to get around issue with the older Newtonsoft library
                // not handling a JSON array that contains no outer element.
                json = "{\"list\":" + json + "}";
                var formattedText = JObject.Parse(json).ToString(Formatting.Indented);
                formattedText = formattedText.Substring(13, formattedText.Length - 14).Replace("\n  ", "\n");
                return formattedText;
            }
            return JObject.Parse(json).ToString(Formatting.Indented);
        }

        public static bool SendEMail(string email, string Subject, string Content)
        {
            var msg = new MailMessage();
            string bodyHtml = Content;
            msg.IsBodyHtml = true;
            msg.Body = bodyHtml;
            msg.Subject = Subject;
            msg.To.Add(email);
            var iSSend = EmailClient.Send(msg);

            return iSSend;
        }

        /// <summary>
        /// Encrypt a string using dual encryption method. Return a encrypted cipher Text
        /// </summary>
        /// <param name="toEncrypt">string to be encrypted</param>
        /// <param name="useHashing">use hashing? send to for extra security</param>
        /// <returns></returns>
        public static string EncryptPassword(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader =
                                                new AppSettingsReader();
            // Get the key from config file

            string key = "Syed Moshiur Murshed";
            //(string)settingsReader.GetValue("SecurityKey",
            //                                             typeof(String));
            //System.Windows.Forms.MessageBox.Show(key);
            //If hashing use get hashcode regards to your key
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                //Always release the resources and flush data
                // of the Cryptographic service provide. Best Practice

                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            //set the secret key for the tripleDES algorithm
            tdes.Key = keyArray;
            //mode of operation. there are other 4 modes.
            //We choose ECB(Electronic code Book)
            tdes.Mode = CipherMode.ECB;
            //padding mode(if any extra byte added)

            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            //transform the specified region of bytes array to resultArray
            byte[] resultArray =
              cTransform.TransformFinalBlock(toEncryptArray, 0,
              toEncryptArray.Length);
            //Release resources held by TripleDes Encryptor
            tdes.Clear();
            //Return the encrypted data into unreadable string format
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// DeCrypt a string using dual encryption method. Return a DeCrypted clear string
        /// </summary>
        /// <param name="cipherString">encrypted string</param>
        /// <param name="useHashing">Did you use hashing to encrypt this data? pass true is yes</param>
        /// <returns></returns>
        public static string DecryptPassword(string cipherString, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            //  string key = (string)settingsReader.GetValue("SecurityKey", typeof(String));
            string key = "Syed Moshiur Murshed";
            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                hashmd5.Clear();
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        public static string ImagePathToBase64(string path)
        {
            var currentImagePath = HttpContext.Current.Server.MapPath(path);
            using (Image image = Image.FromFile(currentImagePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return "data:image/png;base64," + base64String;
                }
            }
        }

        public static string SaveImage(HttpPostedFileBase image, string MainPath, string ThumbPath)
        {
            string fileName = "";
            string savelocation = HttpContext.Current.Server.MapPath(MainPath);

            if (image != null && image.ContentLength > 0)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetFileName(image.FileName);
                string savePath = savelocation + fileName;
                image.SaveAs(savePath);

                //-----------Thumbnail----------//
                var orignalImage = System.Drawing.Image.FromStream(image.InputStream);
                // Rotating image 360 degrees to discart internal thumbnail image
                orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                orignalImage.RotateFlip(RotateFlipType.Rotate180FlipNone);

                int newHeight = orignalImage.Height * 300 / orignalImage.Width;
                int newWidth = 300;

                // New height is greater than our thumbHeight so we need to keep height fixed and calculate the width accordingly
                if (newHeight > 300)
                {
                    newWidth = orignalImage.Width *
                    300 / orignalImage.Height;
                    newHeight = 300;
                }

                //Generate a thumbnail image
                Image thumbImage = orignalImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);

                // Save resized picture
                thumbImage.Save(HttpContext.Current.Server.MapPath(ThumbPath) + fileName);
            }

            return fileName;
        }

        public static string Slugify(this string name)
        {
            if (string.IsNullOrEmpty(name)) name = string.Empty;
            string str = name.ToLower();
            str = System.Text.RegularExpressions.Regex.Replace(str, @"[^a-z0-9\s-]", ""); // Remove all non valid chars          
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = System.Text.RegularExpressions.Regex.Replace(str, @"\s", "-"); // //Replace spaces by dashes
            return str;

        }

        public static Image Base64ToImage(string base64String)
        {
            base64String = base64String.Split(',')[1];
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

        public static string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to base 64 string
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        public static void SaveAttachedFile(string path, HttpPostedFileBase attachment, string fileName, Image img = null)
        {
            //    path += "/";
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                }
            }
            catch (Exception ex)
            {
                //      ErrorLogManager.LogExceptionToDatabase(ex);
            }

            attachment.SaveAs(HttpContext.Current.Server.MapPath(path) + fileName);
            if (img != null)
                img.Save(HttpContext.Current.Server.MapPath(path) + fileName);
        }
        public static string SavePostedFile(string FolderName, HttpPostedFileBase fileData)
        {
            var FileDataContent = fileData;
            var UploadPath = HostingEnvironment.MapPath(FolderName + "/");
            bool exists = System.IO.Directory.Exists(UploadPath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(UploadPath);
            }
            var FilePath = Path.Combine(UploadPath, FileDataContent.FileName);
            var extension = Path.GetExtension(FilePath);
            Guid FileId = Guid.NewGuid();
            var FileCreated = FileId + extension;
            var NewFile = UploadPath + FileCreated;
            FileDataContent.SaveAs(NewFile);
            return FileCreated;
        }
        public static void SaveUserCroppedImage(string path, string fileName, Image img = null)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
            if (img != null)
                img.Save(HttpContext.Current.Server.MapPath(path) + fileName);

            // Utilities.CropImage(200, 200, HttpContext.Current.Server.MapPath(path) + fileName, HttpContext.Current.Server.MapPath(path) + fileName);
        }


        public static string DiffreneceBetweenTwoDates(DateTime date2, DateTime date1)
        {
            var diff = date2 - date1;
            if (diff.TotalDays < 1)
            {
                if (diff.Hours < 1)
                {
                    if (diff.Minutes <= 1)
                        return string.Format("{0} minute ago", diff.Minutes);
                    else
                        return string.Format("{0} minutes ago", diff.Minutes);
                }
                else
                {
                    if (diff.Hours > 0)
                    {
                        if (diff.Hours <= 1)
                            return string.Format("{0} hour ago", diff.Hours);
                        else
                            return string.Format("{0} hours ago", diff.Hours);
                    }
                }
            }
            else
            {
                if (diff.Days <= 1)
                    return string.Format("{0} day ago", diff.Days);
                else
                    return string.Format("{0} days ago", diff.Days);
            }
            return null;
        }

        public static string CreateUniqueID(string lastID)
        {
            var splitValue = lastID.Split('-');
            string str = splitValue[1];
            char[] charArray = str.ToCharArray();
            string[] strArray = str.Select(x => x.ToString()).ToArray();

            int amont = Convert.ToInt32(str) + 1;
            string abc = "";
            foreach (string s in strArray)
            {
                if (s == "0")
                {
                    abc += "0";
                }
                else
                {
                    break;
                }

            }
            var newval = abc + amont.ToString();
            if (newval.Length > str.Length)
            {
                newval = newval.Remove(0, 1);
            }
            newval = splitValue[0] + "-" + newval;
            return newval;
        }

        public static string CreateFolderStructure(string folderTree, string serverPath)
        {
            var folders = folderTree.Split(new string[] { "/" }, StringSplitOptions.None);
            var path = "";
            var folderPath = HttpContext.Current.Server.MapPath(serverPath);
            foreach (var f in folders)
            {
                path += "/" + f;
                if (!Directory.Exists(folderPath + path)) Directory.CreateDirectory(folderPath + path);
            }

            return (serverPath + folderTree);
        }

        /// <summary>
        /// generates the hash code from the string passed
        /// </summary>
        public static string HashCode(string str)
        {
            SHA1 hash = SHA1CryptoServiceProvider.Create();
            byte[] plainTextBytes = Encoding.ASCII.GetBytes(str);
            byte[] hashBytes = hash.ComputeHash(plainTextBytes);
            string localChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            return localChecksum;
        }

        public static string GetPath(string folder, string fileName)
        {
            var path = HttpContext.Current.Server.MapPath(folder.ToString() + "/" + fileName); return path;
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static CurrentWeek GetCurrentWeek()
        {
            var model = new CurrentWeek();
            var date = DateTime.Now;
            var day = date.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)date.DayOfWeek;
            model.Start = date.AddDays(-(day - (int)DayOfWeek.Monday)).Date;
            model.End = date.AddDays(7 - day).Date;
            return model;
        }

        public static CurrentWeek GetCurrentMonth()
        {
            var model = new CurrentWeek();
            DateTime now = DateTime.Now;
            model.Start = new DateTime(now.Year, now.Month, 1);
            model.End = model.Start.AddMonths(1).AddDays(-1);
            return model;
        }

        public static CurrentWeek GetCurrentYear()
        {
            var model = new CurrentWeek();
            DateTime now = DateTime.Now;
            model.Start = new DateTime(now.Year, 1, 1);
            model.End = new DateTime(now.Year, 12, 31);
            return model;
        }

        public static string EncodeString(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }

        public static string DecodeString(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return System.Text.Encoding.UTF8.GetString(encoded);
        }

        public static Image LoadImage(string base64String)
        {
            // base64String = base64String.Split(',')[1];
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                Image image = Image.FromStream(ms, true);
                return image;
            }
        }

    }

}
