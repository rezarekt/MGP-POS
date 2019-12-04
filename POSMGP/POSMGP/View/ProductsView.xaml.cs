﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using POSMGP.Model;

namespace POSMGP.View
{
    /// <summary>
    /// Interaction logic for ProductsView.xaml
    /// </summary>
    public partial class ProductsView : UserControl
    {
        const String connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=db_mgppos";
        //ProductsModel products;
        List<SupplierModel> supplier = new List<SupplierModel>();

        public ProductsView()
        {
            InitializeComponent();
            loadProducts();
            loadSupplier();
        }

        void loadProducts()
        {
            String query = "SELECT * FROM tbl_products";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {

                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        ProductsModel productList = new ProductsModel{ productID = reader.GetInt16(0), productName= reader.GetString(1), productPrice= reader.GetDouble(2), supplierID= reader.GetInt16(3), dateModified= reader.GetDateTime(4).ToString("yyyy-MM-dd")};
                        lvProducts.Items.Add(productList);
                    }
                    databaseConnection.Close();
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        void loadSupplier()
        {
            String query = "SELECT * FROM tbl_supplier";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {

                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        //SupplierModel productList = new SupplierModel { productID = reader.GetInt16(0), productName = reader.GetString(1), productPrice = reader.GetDouble(2), productSupplier = reader.GetString(3), productDateAdded = reader.GetDateTime(4) };
                        TextBlock supplierName = new TextBlock();
                        supplier.Add(new SupplierModel { supplierID = reader.GetInt16(0), supplierName = reader.GetString(1) });
                        supplierName.Inlines.Add(reader.GetInt16(0).ToString() + " - " + reader.GetString(1));
                        cbProductSupplier.Items.Add(supplierName);
                    }
                    databaseConnection.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            lvProducts.Items.Clear();
            string query = "INSERT INTO tbl_products(`PName`,`PPrice`,`supplierID`,`dateModified`) VALUES (@productName, @productPrice, @supplierID, @dateModified)";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            commandDatabase.Parameters.AddWithValue("@productName", tbProductName.Text);
            commandDatabase.Parameters.AddWithValue("@productPrice", tbProductPrice.Text);
            commandDatabase.Parameters.AddWithValue("@supplierID", supplier[cbProductSupplier.SelectedIndex].supplierID);
            commandDatabase.Parameters.AddWithValue("@dateModified", DateTime.Now);


            if (tbProductID.Text != "" || (tbProductName.Text == "" || tbProductPrice.Text == "" || cbProductSupplier.Text == ""))
            {

                MessageBox.Show("Missing Fields or User already exist!");

            }
            else
            {
                try
                {

                    databaseConnection.Open();
                    MySqlDataReader reader = commandDatabase.ExecuteReader();
                    MessageBox.Show("Product Successfully Added!");
                    databaseConnection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    loadProducts();
                }
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            string query = "UPDATE `tbl_products` SET `PName`=@productName,`PPrice`=@productPrice, `PSupplier`=@productSupplier, `dateAdded`=@productDateAdded WHERE PID = @productID";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);

            commandDatabase.Parameters.AddWithValue("@productID", Convert.ToInt16(tbProductID.Text));
            commandDatabase.Parameters.AddWithValue("@productName", tbProductName.Text);
            commandDatabase.Parameters.AddWithValue("@productPrice", Convert.ToDouble(tbProductPrice.Text));
            commandDatabase.Parameters.AddWithValue("@productDateAdded", DateTime.Now.ToString("MM-dd-YYYY"));
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();

                // Succesfully updated

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Ops, maybe the id doesn't exists ?
                MessageBox.Show(ex.Message);
            }
            finally
            {
                loadProducts();
            }
        }

        private void btnResetFields_Click(object sender, RoutedEventArgs e)
        {
            tbProductID.Text = "";
            tbProductName.Text = "";
            tbProductPrice.Text = "";
            cbProductSupplier.Text = "";
        }

        private void lvProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductsModel selectedItem = (ProductsModel)lvProducts.SelectedItem;
            tbProductID.Text = selectedItem.productID.ToString();
            tbProductName.Text = selectedItem.productName;
            tbProductPrice.Text = selectedItem.productPrice.ToString();
            foreach(SupplierModel tmp in supplier)
            {
                if(tmp.supplierID == selectedItem.supplierID)
                {
                    cbProductSupplier.Text = tmp.supplierID.ToString() + " - " + tmp.supplierName; 
                }
            }
        }
    }
}
