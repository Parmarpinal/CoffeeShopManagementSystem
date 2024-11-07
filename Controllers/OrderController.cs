using Microsoft.AspNetCore.Mvc;
using MVCDemo1.Models;
using System.Data.SqlClient;
using System.Data;

namespace MVCDemo1.Controllers
{
    [CheckAccess]
    public class OrderController : Controller
    {
        private IConfiguration _configuration;
        public OrderController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region OrderList
        public IActionResult Index()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Order_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(sqlDataReader);

            return View(table);
        }
        #endregion

        #region CustomerDropDown
        public void CustomerDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Customer_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<CustomerDropDownModel> customerList = new List<CustomerDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                CustomerDropDownModel customerDropDownModel = new CustomerDropDownModel();
                customerDropDownModel.CustomerID = Convert.ToInt32(data["CustomerID"]);
                customerDropDownModel.CustomerName = data["CustomerName"].ToString();
                customerList.Add(customerDropDownModel);
            }
            ViewBag.CustomerList = customerList;
        }
        #endregion

        #region UserDropDown
        public void UserDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection2 = new SqlConnection(connectionString);
            connection2.Open();
            SqlCommand command2 = connection2.CreateCommand();
            command2.CommandType = System.Data.CommandType.StoredProcedure;
            command2.CommandText = "PR_User_DropDown";
            SqlDataReader reader2 = command2.ExecuteReader();
            DataTable dataTable2 = new DataTable();
            dataTable2.Load(reader2);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable2.Rows)
            {
                UserDropDownModel userDropDownModel = new UserDropDownModel();
                userDropDownModel.UserID = Convert.ToInt32(data["UserID"]);
                userDropDownModel.UserName = data["UserName"].ToString();
                userList.Add(userDropDownModel);
            }
            ViewBag.UserList = userList;
        }
        #endregion

        #region OrderForm
        public IActionResult OrderForm(int? orderID)
        {
            CustomerDropDown();
            UserDropDown();

            if (orderID == null)
            {
                var m = new OrderModel
                {
                    OrderDate = DateTime.Now
                };
                return View(m);
            }

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Order_SelectByID";
            command.Parameters.AddWithValue("@OrderID", orderID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderModel model = new OrderModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.OrderDate = Convert.ToDateTime(dataRow["OrderDate"]);
                model.OrderNumber = Convert.ToInt32(dataRow["OrderNumber"]);
                model.CustomerID = Convert.ToInt32(dataRow["CustomerID"]);
                model.PaymentMode = dataRow["PaymentMode"].ToString();
                model.TotalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                model.ShippingAddress = dataRow["ShippingAddress"].ToString();
                model.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View(model);
        }
        #endregion

        #region OrderAddEdit
        public IActionResult onOrderSave(OrderModel order)
        {
            if (order.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }
            if (order.CustomerID <= 0)
            {
                ModelState.AddModelError("CustomerID", "A valid customer is required.");
            }
            if (order.OrderNumber <= 0)
            {
                ModelState.AddModelError("OrderNumber", "A positive order number is required.");
            }
            if (order.TotalAmount <= 0)
            {
                ModelState.AddModelError("TotalAmount", "A positive total amount is required.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (order.OrderID == 0)
                {
                    command.CommandText = "PR_Order_Insert";
                }
                else
                {
                    command.CommandText = "PR_Order_Update";
                    command.Parameters.Add("@OrderID", SqlDbType.Int).Value = order.OrderID;
                }
                command.Parameters.Add("@OrderDate", SqlDbType.DateTime).Value = order.OrderDate;
                command.Parameters.Add("@OrderNumber", SqlDbType.Int).Value = order.OrderNumber;
                command.Parameters.Add("@CustomerID", SqlDbType.Int).Value = order.CustomerID;
                command.Parameters.Add("@PaymentMode", SqlDbType.VarChar).Value = order.PaymentMode;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = order.TotalAmount;
                command.Parameters.Add("@ShippingAddress", SqlDbType.VarChar).Value = order.ShippingAddress;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = order.UserID;
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("Index");
            }

            UserDropDown();
            CustomerDropDown();
            return View("OrderForm", order);
        }
        #endregion

        #region OrderDelete
        public IActionResult OrderDelete(int OrderID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Order_Delete";
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = OrderID;
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
