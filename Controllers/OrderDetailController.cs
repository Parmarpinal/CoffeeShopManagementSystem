using Microsoft.AspNetCore.Mvc;
using MVCDemo1.Models;
using System.Data.SqlClient;
using System.Data;

namespace MVCDemo1.Controllers
{
    [CheckAccess]
    public class OrderDetailController : Controller
    {
        private IConfiguration _configuration;
        public OrderDetailController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region OrderDetailList
        public IActionResult Index()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_OrderDetail_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(sqlDataReader);

            return View(table);
        }
        #endregion

        #region OrderDropDown
        public void OrderDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_Order_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<OrderDropDownModel> orderList = new List<OrderDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                OrderDropDownModel order = new OrderDropDownModel();
                order.OrderID = Convert.ToInt32(data["OrderID"]);
                order.OrderNumber = Convert.ToInt32(data["OrderNumber"]);
                orderList.Add(order);
            }
            ViewBag.OrderList = orderList;
        }
        #endregion

        #region UserDropDown
        public void UserDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command2 = connection1.CreateCommand();
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

        #region ProductDropDown
        public void ProductDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command3 = connection1.CreateCommand();
            command3.CommandType = System.Data.CommandType.StoredProcedure;
            command3.CommandText = "PR_Product_DropDown";
            SqlDataReader reader3 = command3.ExecuteReader();
            DataTable dataTable3 = new DataTable();
            dataTable3.Load(reader3);
            List<ProductDropDownModel> productList = new List<ProductDropDownModel>();
            foreach (DataRow data in dataTable3.Rows)
            {
                ProductDropDownModel product = new ProductDropDownModel();
                product.ProductID = Convert.ToInt32(data["ProductID"]);
                product.ProductName = data["ProductName"].ToString();
                productList.Add(product);
            }
            ViewBag.ProductList = productList;
        }
        #endregion

        #region OrderDetailForm
        public IActionResult OrderDetailForm(int? orderDetailID)
        {
            OrderDropDown();
            UserDropDown();
            ProductDropDown();

            if (orderDetailID == null)
            {
                return View(new OrderDetailModel());
            }

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command = connection1.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_OrderDetail_SelectByPK";
            command.Parameters.AddWithValue("@OrderDetailID", orderDetailID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            OrderDetailModel orderDetail = new OrderDetailModel();

            foreach (DataRow dataRow in table.Rows)
            {
                orderDetail.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                orderDetail.ProductID = Convert.ToInt32(dataRow["ProductID"]);
                orderDetail.Quantity = Convert.ToInt32(dataRow["Quantity"]);
                orderDetail.Amount = Convert.ToDecimal(dataRow["Amount"]);
                orderDetail.TotalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                orderDetail.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View(orderDetail);
        }
        #endregion

        #region OrderDetailAddEdit
        public IActionResult onOrderDetailSave(OrderDetailModel order)
        {
            if(order.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity should be greater than zero.");
            }
            if (order.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Amount should be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (order.OrderDetailID == 0)
                {
                    command.CommandText = "PR_OrderDetail_Insert";
                }
                else
                {
                    command.CommandText = "PR_OrderDetail_Update";
                    command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = order.OrderDetailID;
                }
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = order.OrderID;
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = order.ProductID;
                command.Parameters.Add("@Quantity", SqlDbType.Int).Value = order.Quantity;
                command.Parameters.Add("@Amount", SqlDbType.Decimal).Value = order.Amount;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = order.TotalAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = order.UserID;
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("Index");
            }

            OrderDropDown();
            UserDropDown();
            ProductDropDown();
            return View("OrderDetailForm", order);
        }
        #endregion

        #region OrderDetailDelete
        public IActionResult OrderDetailDelete(int OrderDetailID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_OrderDetail_Delete";
                command.Parameters.Add("@OrderDetailID", SqlDbType.Int).Value = OrderDetailID;
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
