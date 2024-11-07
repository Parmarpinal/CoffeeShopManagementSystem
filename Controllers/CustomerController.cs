using Microsoft.AspNetCore.Mvc;
using MVCDemo1.Models;
using System.Data.SqlClient;
using System.Data;

namespace MVCDemo1.Controllers
{
    [CheckAccess]
    public class CustomerController : Controller
    {
        private IConfiguration _configuration;
        public CustomerController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region CustomerList
        public IActionResult Index()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Customer_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(sqlDataReader);

            return View(table);
        }
        #endregion

        #region UserDropDown
        public void UserDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand cmd = sqlConnection.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_User_DropDown";
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(sqlDataReader);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow row in table.Rows)
            {
                UserDropDownModel user = new UserDropDownModel();
                user.UserID = Convert.ToInt32(row["UserID"]);
                user.UserName = row["UserName"].ToString();
                userList.Add(user);
            }
            ViewBag.UserList = userList;
        }
        #endregion

        #region CustomerForm
        public IActionResult CustomerForm(int? customerID)
        {
            UserDropDown();

            if (customerID == null)
            {
                return View(new CustomerModel()); // Return empty form for creating a new product
            }
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand command = sqlConnection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Customer_SelectByID";
            command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
            SqlDataReader reader = command.ExecuteReader();
            DataTable table1 = new DataTable();
            table1.Load(reader);
            CustomerModel model = new CustomerModel();

            foreach (DataRow dataRow in table1.Rows)
            {
                model.CustomerName = dataRow["CustomerName"].ToString();
                model.Email = dataRow["Email"].ToString();
                model.HomeAddress = dataRow["HomeAddress"].ToString();
                model.MobileNo = dataRow["MobileNo"].ToString();
                model.GSTNo = dataRow["GSTNo"].ToString();
                model.CityName = dataRow["CityName"].ToString();
                model.Pincode = dataRow["Pincode"].ToString();
                model.NetAmount = Convert.ToDecimal(dataRow["NetAmount"]);
                model.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            return View(model);
        }
        #endregion

        #region CustomerAddEdit
        public IActionResult onCustomerSave(CustomerModel customer)
        {
            if (customer.NetAmount <= 0)
            {
                ModelState.AddModelError("NetAmount", "Net amount should be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (customer.CustomerID == 0)
                {
                    command.CommandText = "PR_Customer_Insert";
                }
                else
                {
                    command.CommandText = "PR_Customer_Update";
                    command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customer.CustomerID;
                }
                command.Parameters.Add("@CustomerName", SqlDbType.VarChar).Value = customer.CustomerName;
                command.Parameters.Add("@Email", SqlDbType.VarChar).Value = customer.Email;
                command.Parameters.Add("@HomeAddress", SqlDbType.VarChar).Value = customer.HomeAddress;
                command.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = customer.MobileNo;
                command.Parameters.Add("@GSTNo", SqlDbType.VarChar).Value = customer.GSTNo;
                command.Parameters.Add("@CityName", SqlDbType.VarChar).Value = customer.CityName;
                command.Parameters.Add("@Pincode", SqlDbType.VarChar).Value = customer.Pincode;
                command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = customer.NetAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = customer.UserID;
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("Index");
            }

            UserDropDown();
            return View("CustomerForm", customer);
        }
        #endregion

        #region CustomerDelete
        public IActionResult CustomerDelete(int customerID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Customer_Delete";
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = customerID;
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
    }
}
