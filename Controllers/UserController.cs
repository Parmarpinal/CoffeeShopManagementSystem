using Microsoft.AspNetCore.Mvc;
using MVCDemo1.Models;
using System.Data.SqlClient;
using System.Data;

namespace MVCDemo1.Controllers
{
    public class UserController : Controller
    {
        private IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [CheckAccess]
        #region UserList
        public IActionResult Index()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_User_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(sqlDataReader);

            return View(table);
        }
        #endregion

        #region UserForm
        public IActionResult UserForm(int? UserID)
        {
            if (UserID == null)
            {
                return View(new UserModel()); // Return empty form for creating a new product
            }

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_User_SelectByID";
            command.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            UserModel model = new UserModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.UserName = dataRow["UserName"].ToString();
                model.Email = dataRow["Email"].ToString();
                model.Password = dataRow["Password"].ToString();
                model.MobileNo = dataRow["MobileNo"].ToString();
                model.Address = dataRow["Address"].ToString();
            }
            return View(model);
        }
        #endregion

        #region UserAddEdit
        public IActionResult onUserSave(UserModel user)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (user.UserID == 0)
                {
                    command.CommandText = "PR_User_Insert";
                }
                else
                {
                    command.CommandText = "PR_User_Update";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = user.UserID;
                }
                command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = user.UserName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = user.Email;
                command.Parameters.Add("@Password", SqlDbType.VarChar).Value = user.Password;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = user.MobileNo;
                command.Parameters.Add("@Address", SqlDbType.VarChar).Value = user.Address;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = 0;
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("Index");
            }

            return View("UserForm", user);
        }
        #endregion

        [CheckAccess]
        #region UserDelete
        public IActionResult UserDelete(int userID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_user_Delete";
                command.Parameters.Add("@userID", SqlDbType.Int).Value = userID;
                command.ExecuteNonQuery();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Login Page
        public IActionResult Login()
        {
            return View();
        }
        #endregion

        #region UserLogin
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this._configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_Login";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["ErrorMsg"] = "User can not login successfully";
                        return RedirectToAction("Login");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        #endregion

        #region UserLogout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        #endregion
    }
}
