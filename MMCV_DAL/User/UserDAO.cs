using MMCV_Model.Document;
using MMCV_Model.DocumentSign;
using MMCV_Model.DocumentType;
using MMCV_Model.User;
using SAB.Library.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMCV_DAL.User
{
    public class UserDAO : BaseDAO
    {
        #region Constructor

        public UserDAO() : base()
        {
        }

        public UserDAO(IData objIData)
            : base(objIData)
        {
        }

        #endregion

        #region Method

        public UserBO GetUser(UserBO user)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $@"SELECT u.*, d.DepartmentName, p.PositionName 
FROM [User] u
LEFT JOIN Department d ON d.DepartmentID = u.DepartmentID
LEFT JOIN Position p ON p.PositionID = u.PositionID
WHERE u.[Username] = '{user.Username}' AND u.[Password] = '{user.Password}'";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<UserBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<UserSignatureBO> GetUserSignatures(int userId)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"SELECT * FROM [UserSignature] WHERE [UserID] = {userId} AND Active = 1";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<UserSignatureBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public UserSignatureBO AddUserSignature(UserSignatureBO us)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"INSERT INTO UserSignature(UserID, Base64Signature, IsDefault, CreatedDate, Active) " +
                    $"VALUES ({us.UserID}, '{us.Base64Signature}', {us.IsDefault}, '{us.CreatedDate.ToString("yyyy/MM/dd hh:mm:ss")}', {us.Active})" +
                    $";DECLARE @Id INT = SCOPE_IDENTITY(); SELECT* FROM UserSignature WHERE UserSignatureID = @Id";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<UserSignatureBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public bool SetDefaultSignature(UserSignatureBO us)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"UPDATE UserSignature " +
                    $"SET IsDefault = 0 " +
                    $"WHERE UserID = {us.UserID}; " +
                    $"UPDATE UserSignature " +
                    $"SET IsDefault = 1 " +
                    $"WHERE UserSignatureID = {us.UserSignatureID}";
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public bool DeleteSignature(UserSignatureBO us)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"UPDATE UserSignature SET Active = 0 WHERE UserSignatureID = {us.UserSignatureID}";
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public List<UserBO> GetUsers()
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = @"SELECT u.*, d.DepartmentName, p.PositionName 
FROM [User] u
LEFT JOIN Department d ON d.DepartmentID = u.DepartmentID
LEFT JOIN Position p ON p.PositionID = u.PositionID";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<UserBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }


        public UserBO AddUser(UserBO us)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"INSERT INTO [User](Username, Password, Fullname, Email, EmployeeID, DepartmentID, PositionID) " +
                    $"VALUES ('{us.Username}', '123456', N'{us.Fullname}', '{us.Email}', '{us.EmployeeID}', {us.DepartmentID}, {us.PositionID})" +
                    $";DECLARE @Id INT = SCOPE_IDENTITY(); SELECT * FROM [User] WHERE UserID = @Id";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<UserBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public bool UpdateUser(UserBO us)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"UPDATE [User] " +
                    $"SET Email = '{us.Email}', Fullname = N'{us.Fullname}', EmployeeID = '{us.EmployeeID}', DepartmentID = {us.DepartmentID}, PositionID = {us.PositionID} " +
                    $"WHERE UserID = {us.UserID}";
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public bool UpdateUserStamp(UserBO us)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"UPDATE [User] " +
                    $"SET StampBase64 = '{us.StampBase64}'" +
                    $"WHERE UserID = {us.UserID}";
                BeginTransactionIfAny(objIData);
                objIData.ExecUpdate(query);
                CommitTransactionIfAny(objIData);
                return true;
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        public UserBO GetUserById(int userId)
        {
            IData objIData = this.CreateIData();
            try
            {
                string query = $"SELECT * " +
                    $"FROM [User] " +
                    $"WHERE UserID = {userId}";
                BeginTransactionIfAny(objIData);
                var reader = objIData.ExecQueryToDataReader(query);
                var list = ConvertToListObject<UserBO>(reader);
                reader.Close();
                CommitTransactionIfAny(objIData);
                return list.FirstOrDefault();
            }
            catch (Exception objEx)
            {
                RollBackTransactionIfAny(objIData);
                throw objEx;
            }
            finally
            {
                this.DisconnectIData(objIData);
            }
        }

        #endregion
    }
}
