using Microsoft.AspNetCore.Mvc;
using MVCDemo1.Models;
using OfficeOpenXml;
using System.Data;
using System.Data.SqlClient;

namespace MVCDemo1.Controllers
{
    [CheckAccess]
public class ProductController : Controller
    {
        private IConfiguration _configuration;
        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region ProductList
        public IActionResult Index()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);  
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure; 
            sqlCommand.CommandText = "PR_Product_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(sqlDataReader);

            return View(table);
        }
        #endregion

        #region User Dropdown
        public void UserDropDown()
        {
            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection1 = new SqlConnection(connectionString);
            connection1.Open();
            SqlCommand command1 = connection1.CreateCommand();
            command1.CommandType = System.Data.CommandType.StoredProcedure;
            command1.CommandText = "PR_User_DropDown";
            SqlDataReader reader1 = command1.ExecuteReader();
            DataTable dataTable1 = new DataTable();
            dataTable1.Load(reader1);
            List<UserDropDownModel> userList = new List<UserDropDownModel>();
            foreach (DataRow data in dataTable1.Rows)
            {
                UserDropDownModel user = new UserDropDownModel();
                user.UserID = Convert.ToInt32(data["UserID"]);
                user.UserName = data["UserName"].ToString();
                userList.Add(user);
            }
            ViewBag.UserList = userList;
        }
        #endregion

        #region ProductForm
        public IActionResult ProductForm(int? ProductID)
        {
            UserDropDown();

            if (ProductID == null)
            {
                return View(new ProductModel()); // Return empty form for creating a new product
            }

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Product_SelectByID";
            command.Parameters.AddWithValue("@ProductID", ProductID);
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            ProductModel productModel = new ProductModel();

            foreach (DataRow dataRow in table.Rows)
            {
                productModel.ProductID = Convert.ToInt32(@dataRow["ProductID"]);
                productModel.ProductName = @dataRow["ProductName"].ToString();
                productModel.ProductCode = @dataRow["ProductCode"].ToString();
                productModel.ProductPrice = Convert.ToDecimal(@dataRow["ProductPrice"]);
                productModel.Description = @dataRow["Description"].ToString();
                productModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            return View("ProductForm", productModel);
        }
        #endregion

        #region ProductAddEdit
        public IActionResult onProductSave(ProductModel product)
        {
            if (product.UserID <= 0)
            {
                ModelState.AddModelError("UserID", "A valid User is required.");
            }

            if (product.ProductPrice <= 0)
            {
                ModelState.AddModelError("ProductPrice", "Price should be greater than zero.");
            }
                
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (product.ProductID == 0)
                {
                    command.CommandText = "PR_Product_Insert";
                }
                else
                {
                    command.CommandText = "PR_Product_Update";
                    command.Parameters.Add("@ProductID", SqlDbType.Int).Value = product.ProductID;
                }
                command.Parameters.Add("@ProductName", SqlDbType.VarChar).Value = product.ProductName;
                command.Parameters.Add("@ProductCode", SqlDbType.VarChar).Value = product.ProductCode;
                command.Parameters.Add("@ProductPrice", SqlDbType.Decimal).Value = product.ProductPrice;
                command.Parameters.Add("@Description", SqlDbType.VarChar).Value = product.Description;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = product.UserID;
                command.ExecuteNonQuery();
                TempData["SuccessMsg"] = "Task performed successfully";
                return RedirectToAction("Index");
            }

            UserDropDown();
            return View("ProductForm", product);
        }
        #endregion

        #region ProductDelete
        public IActionResult ProductDelete(int ProductID)
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Product_Delete";
                command.Parameters.Add("@ProductID", SqlDbType.Int).Value = ProductID;
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

        #region Export to excel
        public IActionResult ExportToExcel()
        {
            string connectionString = _configuration.GetConnectionString("ConnectionString");
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
            sqlCommand.CommandText = "PR_Product_SelectAll";
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(sqlDataReader);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("DataSheet");

                // Add headers
                worksheet.Cells[1, 1].Value = "ProductID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Code";
                worksheet.Cells[1, 4].Value = "Price";
                worksheet.Cells[1, 5].Value = "Description";
                worksheet.Cells[1, 6].Value = "User name";

                // Add data
                int row = 2;
                foreach (DataRow item in data.Rows)
                {
                    worksheet.Cells[row, 1].Value = item["ProductID"]; // Replace with your property
                    worksheet.Cells[row, 2].Value = item["ProductName"]; // Replace with your property
                    worksheet.Cells[row, 3].Value = item["ProductPrice"];
                    worksheet.Cells[row, 4].Value = item["ProductCode"];
                    worksheet.Cells[row, 5].Value = item["Description"];
                    worksheet.Cells[row, 6].Value = item["UserName"];
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                string excelName = $"Data-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            #endregion
        }
    }
}
