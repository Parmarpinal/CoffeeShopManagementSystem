using Microsoft.AspNetCore.Mvc;
using MVCDemo1.Models;
using System.Data.SqlClient;
using System.Data;

namespace MVCDemo1.Controllers
{
    [CheckAccess]
    public class BillController : Controller
    {
        private IConfiguration _configuration;
        public BillController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region BillList
        public IActionResult Index()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Bills_SelectAll";
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

        #region BillForm
        public IActionResult BillForm(int? billID)
        {
            OrderDropDown();
            UserDropDown(); 

            if(billID == null)
            {
                var m = new BillModel
                {
                    BillDate = DateTime.Now
                };
                return View(m);
            }

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command = connection1.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Bills_SelectByID";
            command.Parameters.AddWithValue("@BillID", billID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            BillModel model = new BillModel();

            foreach (DataRow dataRow in table.Rows)
            {
                model.BillNumber = dataRow["BillNumber"].ToString();
                model.BillDate = Convert.ToDateTime(dataRow["BillDate"]);
                model.OrderID = Convert.ToInt32(dataRow["OrderID"]);
                model.TotalAmount = Convert.ToDecimal(dataRow["TotalAmount"]);
                model.NetAmount = Convert.ToDecimal(dataRow["NetAmount"]);
                model.Discount = Convert.ToDecimal(dataRow["Discount"]);
                model.UserID = Convert.ToInt32(dataRow["UserID"]);
            }
            return View("BillForm", model);
        }
        #endregion

        #region BillAddEdit
        public IActionResult onBillSave(BillModel bill)
        {
            if (bill.TotalAmount <= 0)
            {
                ModelState.AddModelError("TotalAmount", "Total amount should be greater than zero.");
            }
            if (bill.Discount < 0)
            {
                ModelState.AddModelError("Discount", "Discount should not be less than zero.");
            }

            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (bill.BillID == 0)
                {
                    command.CommandText = "PR_Bills_Insert";
                }
                else
                {
                    command.CommandText = "PR_Bills_Update";
                    command.Parameters.Add("@BillID", SqlDbType.Int).Value = bill.BillID;
                }
                command.Parameters.Add("@BillNumber", SqlDbType.VarChar).Value = bill.BillNumber;
                command.Parameters.Add("@BillDate", SqlDbType.DateTime).Value = bill.BillDate;
                command.Parameters.Add("@OrderID", SqlDbType.Int).Value = bill.OrderID;
                command.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = bill.TotalAmount;
                command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = bill.Discount;
                command.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = bill.NetAmount;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = bill.UserID;
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("Index");
            }

            OrderDropDown();
            UserDropDown();
            return View("BillForm", bill);
        }
        #endregion

        #region BillDelete
        public IActionResult BillDelete(int BillID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Bills_Delete";
                command.Parameters.Add("@BillID", SqlDbType.Int).Value = BillID;
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
