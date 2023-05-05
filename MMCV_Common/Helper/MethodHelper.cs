using SAB.Library.Core.FileService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

namespace MMCV_Common
{
    public class MethodHelper
    {
        //Config 
        //public static string ProductImageUrl = ConfigurationManager.AppSettings["ProductImageUrl"];
        //public static string NewsImageUrl = ConfigurationManager.AppSettings["NewsImageUrl"];
        //public static string AdvImageUrl = ConfigurationManager.AppSettings["AdvImageUrl"];

        public string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            // convert to char array of the string
            char[] letters = source.ToCharArray();
            // upper case the first char
            letters[0] = char.ToUpper(letters[0]);
            // return the array made of the new char array
            return new string(letters);
        }

        #region -- Static (implement Singleton pattern) --

        /// <summary>
        /// The instance
        /// </summary>
        private static volatile MethodHelper _instance;

        /// <summary>
        /// The synchronize root
        /// </summary>
        private static readonly object _syncRoot = new Object();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static MethodHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new MethodHelper();
                        }
                    }
                }
                return _instance;
            }
        }

        #endregion

        public string MergeEventStr(MethodBase objMethodBase)
        {
            return MergeEventStr(objMethodBase.DeclaringType.FullName, objMethodBase.Name);
        }

        public string MergeEventStr(string strFullClass, string strMethodName)
        {
            return string.Format("{0} -> {1}", strFullClass, strMethodName);
        }

        public static DateTime ConvertStringFromDate(string dtBegin)
        {
            return string.IsNullOrEmpty(dtBegin) ? new DateTime(1970, 1, 1).Date : DateTime.ParseExact(dtBegin, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date;
        }

        public static DateTime ConvertStringToDate(string dtEnd)
        {
            return string.IsNullOrEmpty(dtEnd) ? DateTime.Now.Date.AddDays(1).AddSeconds(-1) : DateTime.ParseExact(dtEnd, "dd/MM/yyyy", CultureInfo.InvariantCulture).Date.AddDays(1).AddSeconds(-1);
        }

        public string ConvertListToString(List<string> lstString = null, List<int> lstInt = null)
        {
            string strConvert = "";
            if (lstString != null)
                strConvert = string.Join(",", lstString);
            if (lstInt != null)
                strConvert = string.Join(",", lstInt);
            return strConvert;
        }

        public string GetTimeFormat(DateTime dtBegin, DateTime dtEnd)
        {

            string result = string.Empty;
            TimeSpan ts = dtEnd - dtBegin;
            if (ts.Days >= 365)
            {
                result += ts.Days / 365 + " năm, ";
                if (ts.Days % 365 > 30)
                    result += (ts.Days % 365) / 30 + " tháng";
                else
                    result += ts.Days % 365 + " ngày ";
            }
            else
            {
                if (ts.Days >= 30)
                {
                    result += ts.Days / 30 + " tháng, ";
                    if (ts.Days % 30 > 0)
                        result += ts.Days % 30 + " ngày ";
                }
                else
                {
                    if (ts.Days > 0)
                    {
                        result += ts.Days + " ngày, ";
                        if (ts.Hours > 0)
                            result += ts.Hours + " giờ ";
                    }
                    else
                    {
                        if (ts.Minutes > 0)
                            result += ts.Minutes + " phút ";
                        //if (ts.Seconds > 0)
                        //    result += ts.Seconds + " giây ";
                    }
                }
            }
            return result;
        }

        public string ConvertBoolToStr(bool bolValue)
        {
            return bolValue ? "1" : "0";
        }

        public string GetErrorMessage(Exception objEx, string strErrorMessage)
        {
            if (objEx.Message.IndexOf("[+") != -1)
            {
                int i = objEx.Message.IndexOf("[+");
                int j = objEx.Message.IndexOf("+]") + 2;
                if (i < j)
                {
                    return objEx.Message.Substring(i + 2, j - i - 4);
                }
            }
            return strErrorMessage;
        }

        public ResultMessageBO FillResultMessage(bool bolIsError, ErrorTypes enErrorType, string strMessage, string strMessageDetail)
        {
            ResultMessageBO objResultMessageBO = new ResultMessageBO
            {
                IsError = bolIsError,
                ErrorType = enErrorType,
                Message = strMessage,
                MessageDetail = strMessageDetail
            };
            return objResultMessageBO;
        }

        //public List<T> DataReaderMapToList<T>(IDataReader dr)
        //{
        //    try
        //    {
        //        List<T> list = new List<T>();
        //        T obj = default(T);
        //        while (dr.Read())
        //        {
        //            obj = Activator.CreateInstance<T>();
        //            foreach (PropertyInfo prop in obj.GetType().GetProperties())
        //            {
        //                if (prop.CanWrite)
        //                {
        //                    try
        //                    {
        //                        if (!object.Equals(dr[prop.Name], DBNull.Value))
        //                        {
        //                            //prop.PropertyType.Name == typeof(DateTime).Name
        //                            prop.SetValue(obj, dr[prop.Name], null);
        //                        }
        //                    }
        //                    catch (Exception)
        //                    {
        //                        prop.SetValue(obj, null, null);
        //                    }
        //                }
        //            }
        //            list.Add(obj);
        //        }
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return null;
        //    }
        //}

        public List<T> DataReaderMapToList<T>(IDataReader reader)
        {
            List<T> lstObj = new List<T>();
            try
            {
                while (reader.Read())
                {
                    Exception objError = null;
                    dynamic obj = Activator.CreateInstance(lstObj.GetType().GetGenericArguments()[0]);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (!Convert.IsDBNull(reader[i]))
                        {
                            dynamic type = obj.GetType().GetProperty(reader.GetName(i));

                            if (type != null)
                            {
                                //DLL Npgsql mới phân biệt giữa timespan với datetime nên phải trick
                                if (reader[i].GetType().Name == typeof(TimeSpan).Name
                                    && (type.PropertyType.Name == typeof(DateTime).Name
                                    || (type.PropertyType.Name == typeof(Nullable<>).Name
                                        && type.PropertyType.GenericTypeArguments[0].Name == typeof(DateTime).Name))
                                    )
                                {
                                    TimeSpan time = (TimeSpan)reader[i];
                                    type.SetValue(obj, new DateTime() + time, null);

                                }
                                else
                                {
                                    try
                                    {
                                        type.SetValue(obj, reader[i], null);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.Instance.WriteLog(ex.Message, "Convert value: " + reader[i] + " to " + type.ToString() + " Class " + obj.ToString(), MethodBase.GetCurrentMethod().Name, "ConvertToObject");
                                        objError = ex;
                                    }
                                }
                            }
                        }
                    }

                    if (objError != null)
                        throw objError;

                    lstObj.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lstObj;
        }

        public void ConvertToObject(IDataReader reader, dynamic lstObj)
        {
            try
            {
                while (reader.Read())
                {
                    Exception objError = null;
                    dynamic obj = Activator.CreateInstance(lstObj.GetType().GetGenericArguments()[0]);
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (!Convert.IsDBNull(reader[i]))
                        {
                            dynamic type = obj.GetType().GetProperty(reader.GetName(i));

                            if (type != null)
                            {
                                //DLL Npgsql mới phân biệt giữa timespan với datetime nên phải trick
                                if (reader[i].GetType().Name == typeof(TimeSpan).Name
                                    && (type.PropertyType.Name == typeof(DateTime).Name
                                    || (type.PropertyType.Name == typeof(Nullable<>).Name
                                        && type.PropertyType.GenericTypeArguments[0].Name == typeof(DateTime).Name))
                                    )
                                {
                                    TimeSpan time = (TimeSpan)reader[i];
                                    type.SetValue(obj, new DateTime() + time, null);

                                }
                                else
                                {
                                    try
                                    {
                                        type.SetValue(obj, reader[i], null);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHelper.Instance.WriteLog(ex.Message, "Convert value: " + reader[i] + " to " + type.ToString() + " Class " + obj.ToString(), MethodBase.GetCurrentMethod().Name, "ConvertToObject");
                                        objError = ex;
                                    }
                                }
                            }
                        }
                    }

                    if (objError != null)
                        throw objError;

                    lstObj.Add(obj);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Hàm tính số ngày giao nhau giữa 2 khoản thời gian
        /// </summary>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="dateOutFrom"></param>
        /// <param name="dateOutTo"></param>
        /// <returns></returns>
        public static int CalculatorJoinDate(DateTime dateInFrom, DateTime dateInTo, DateTime dateOutFrom, DateTime dateOutTo)
        {
            List<DateTime> listTimeIn = new List<DateTime>();
            List<DateTime> listTimeOut = new List<DateTime>();

            while (dateInTo.Date >= dateInFrom.Date)
            {
                listTimeIn.Add(dateInFrom.Date);
                dateInFrom = dateInFrom.AddDays(1);
            }

            while (dateOutTo >= dateOutFrom)
            {
                listTimeOut.Add(dateOutFrom.Date);
                dateOutFrom = dateOutFrom.AddDays(1);
            }

            return listTimeIn.Intersect(listTimeOut).Count();
        }

        /// <summary>
        ///  Hàm tính số ngày giao nhau giữa 1 khoản thời gian và ds các khoản thời gian còn lại
        /// </summary>
        /// <param name="dateInFrom"></param>
        /// <param name="dateInTo"></param>
        /// <param name="listDateCompare"></param>
        /// <returns></returns>
        public static int CalculatorJoinDate(DateTime dateInFrom, DateTime dateInTo, List<DateTime[]> listDateCompare)
        {
            List<DateTime> listTimeIn = new List<DateTime>();
            List<DateTime> listTimeOut = new List<DateTime>();

            listTimeIn = Enumerable.Range(0, 1 + dateInTo.Subtract(dateInFrom).Days)
                                      .Select(offset => dateInFrom.AddDays(offset))
                                      .ToList();

            foreach (var date in listDateCompare)
            {
                DateTime dateOutFrom = date.FirstOrDefault();
                DateTime dateOutTo = date.LastOrDefault();

                listTimeOut.AddRange(Enumerable.Range(0, 1 + dateOutTo.Subtract(dateOutFrom).Days)
                                    .Select(offset => dateOutFrom.AddDays(offset)));
            }

            return listTimeIn.Intersect(listTimeOut).Distinct().Count();
        }

        public static string ConvertDataTableToString(DataTable dt, bool bolIsUpperColumnName = false)
        {
            var result = "";
            var serializer = new JavaScriptSerializer { MaxJsonLength = int.MaxValue };
            var rows = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                if (bolIsUpperColumnName)
                {
                    var row = dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName.ToUpper(), col => dr[col]);
                    rows.Add(row);
                }
                else
                {
                    var row = dt.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col]);
                    rows.Add(row);
                }
            }

            result = serializer.Serialize(rows);
            return result;
        }

        public string ConvertNullToStr(object objValue)
        {
            return (objValue == null || objValue == DBNull.Value) ? string.Empty : objValue.ToString();
        }

        /// <summary>
        /// Copy an object to destination object, only matching fields will be copied
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceObject">An object with matching fields of the destination object</param>
        /// <param name="destObject">Destination object, must already be created</param>
        public static void CopyObject<T>(object sourceObject, ref T destObject)
        {
            //  If either the source, or destination is null, return
            if (sourceObject == null || destObject == null)
                return;

            //  Get the type of each object
            Type sourceType = sourceObject.GetType();
            Type targetType = destObject.GetType();

            //  Loop through the source properties
            foreach (PropertyInfo p in sourceType.GetProperties())
            {
                //  Get the matching property in the destination object
                PropertyInfo targetObj = targetType.GetProperty(p.Name);
                //  If there is none, skip
                if (targetObj == null)
                    continue;

                //  Set the value in the destination
                targetObj.SetValue(destObject, p.GetValue(sourceObject, null), null);
            }
        }

        public static bool ValidatePassword(string password)
        {
            const int MIN_LENGTH = 8;
            const int MAX_LENGTH = 150;

            if (password == null) throw new ArgumentNullException();

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit
                        ;
            return isValid;

        }

        //public static byte[] Export2XLS(DataTable dtData)
        //{
        //    using (OfficeOpenXml.ExcelPackage pck = new OfficeOpenXml.ExcelPackage())
        //    {
        //        OfficeOpenXml.ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");

        //        ws.Cells["A1"].LoadFromDataTable(dtData, true);

        //        using (OfficeOpenXml.ExcelRange rng = ws.Cells["A1:BZ1"])
        //        {
        //            rng.Style.Font.Bold = true;
        //            rng.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;   //Set Pattern for the background to Solid
        //            rng.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.WhiteSmoke);  //Set color to dark blue
        //            rng.Style.Font.Color.SetColor(System.Drawing.Color.Black);
        //        }

        //        for (int i = 0; i < dtData.Columns.Count; i++)
        //        {
        //            if (dtData.Columns[i].DataType == typeof(DateTime))
        //            {
        //                using (OfficeOpenXml.ExcelRange col = ws.Cells[2, i + 1, 2 + dtData.Rows.Count, i + 1])
        //                {
        //                    col.Style.Numberformat.Format = "dd/MM/yyyy";
        //                }
        //            }
        //            if (dtData.Columns[i].DataType == typeof(TimeSpan))
        //            {
        //                using (OfficeOpenXml.ExcelRange col = ws.Cells[2, i + 1, 2 + dtData.Rows.Count, i + 1])
        //                {
        //                    col.Style.Numberformat.Format = "d.hh:mm";
        //                    col.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                }
        //            }

        //        }
        //        return pck.GetAsByteArray();
        //    }
        //}

        /// <summary>
        /// Xóa toàn bộ file trong thư mục
        /// truongnv 20200218
        /// </summary>
        /// <param name="path">đường dẫn</param>
        public static void RemoveFileInDirectory(string path)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
            catch { }
        }

        /// <summary>
        /// Đọc dữ liệu trong file
        /// </summary>
        /// <param name="fileName">tên file: vd: key.text hoặc key.json</param>
        /// <returns></returns>
        public static string ReadDataFile(string fileName)
        {
            string data = string.Empty;
            try
            {
                //get the Json filepath  
                string file = HttpContext.Current.Server.MapPath($"~/Data/Secret/{fileName}");
                //deserialize JSON from file  
                data = System.IO.File.ReadAllText(file);
            }
            catch { data = string.Empty; }
            return data;
        }
    }
}
