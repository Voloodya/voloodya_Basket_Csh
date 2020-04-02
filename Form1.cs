using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shop
{   
    public partial class Form1 : Form
    {

        List<Product> listProducts = new List<Product>();
        Order order = new Order(new Dictionary<Product, (int, float)>());
        
        public Form1()
        {
            InitializeComponent();            
            
            StreamReader fileStream = new StreamReader("inputData.txt", Encoding.GetEncoding("Windows-1251"));
            string s = "";

            while (!fileStream.EndOfStream)
            {
               s = fileStream.ReadLine();               
               listProducts.Add(new Product(s.Substring(0, s.IndexOf(' ')), float.Parse(s.Substring(s.IndexOf(' ') + 1))));
            }
            dataGridViewStore.DataSource = listProducts;
        }

        public class Product
        {
            private string name;
            private float price;

            public Product(string name, float price)
            {
                this.name = name;
                this.price = price;
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
                set
                {
                    this.name = value;
                }
            }

            public float Price
            {
                get
                {
                    return this.price;
                }
                set
                {
                    this.price = value;
                }
            }
        }

        public class Order
        {
            private Dictionary<Product, (int,float)> order_products;

            public Order(Dictionary<Product, (int,float)> order_products)
            {
                this.order_products = order_products;
            }

            public Dictionary<Product, (int, float)> Order_products
            {
                get
                {
                    return this.order_products;
                }
                set
                {
                    this.order_products = value;
                }
            }

            public void addProduct(Product product)
            {
                if (this.order_products.ContainsKey(product))
                {
                    int count = this.order_products[product].Item1;
                    this.order_products[product] = (++count, product.Price * count);
                }
                else this.order_products.Add(product, (1, product.Price));
            }

            public void addProducts(List<Product> products)
            {
                foreach (Product product in products)
                {
                     this.order_products.Add(product, (1, product.Price));
                }
            }

            public void dellProduct(String name=null, float price = 0)
            {
                if (name == null) this.order_products.Clear();
                else this.order_products.Remove(this.order_products.First(x => x.Key.Name.Equals(name) & x.Key.Price == price).Key);
            }

            public float summOrder()
            {
                float summAll = 0;
                foreach (KeyValuePair<Product, (int, float)> keyValuePair in this.order_products)
                {
                    summAll += keyValuePair.Value.Item2;
                }

                return summAll;
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridViewStore_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int idStr = dataGridViewStore.CurrentRow.Index;
            Product prdct = listProducts.Find(x => (x.Name.Equals((string)dataGridViewStore.Rows[idStr].Cells[0].Value) &
                                              x.Price == (float)dataGridViewStore.Rows[idStr].Cells[1].Value));

            order.addProduct(prdct);
            
            dataGridViewOrder.DataSource = order.Order_products.Select(x => new { x.Key.Name, x.Key.Price, x.Value.Item1,x.Value.Item2}).ToList();
            dataGridViewOrder.Columns[2].HeaderText = "Count";
            dataGridViewOrder.Columns[3].HeaderText = "Summ";


            textBoxSumm.Text = order.summOrder().ToString();
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            String nameProduct = textBoxSearch.Text;
            dataGridViewStore.DataSource = listProducts.FindAll(x => x.Name.Contains(nameProduct)).ToList();
        }

       
        private void AddAllOrder_Click(object sender, EventArgs e)
        {
            order.addProducts(listProducts);
            dataGridViewOrder.DataSource = order.Order_products.Select(x => new { x.Key.Name, x.Key.Price, x.Value.Item1, x.Value.Item2}).ToList();
            dataGridViewOrder.Columns[2].HeaderText = "Count";
            dataGridViewOrder.Columns[3].HeaderText = "Summ";

            textBoxSumm.Text = order.summOrder().ToString();
        }

        private void buttonClearOrder_Click(object sender, EventArgs e)
        {
            order.dellProduct();
            dataGridViewOrder.DataSource = null;
            textBoxSumm.Text = "0";
        }


        private void buttonDell_Click(object sender, EventArgs e)
        {            
            int idStr = dataGridViewOrder.CurrentRow.Index;
            order.dellProduct((string)dataGridViewOrder.Rows[idStr].Cells[0].Value, (float)dataGridViewOrder.Rows[idStr].Cells[1].Value);            
            dataGridViewOrder.DataSource = order.Order_products.Select(x => new { x.Key.Name, x.Key.Price, x.Value.Item1, x.Value.Item2 }).ToList();
            dataGridViewOrder.Columns[2].HeaderText = "Count";
            dataGridViewOrder.Columns[3].HeaderText = "Summ";
            textBoxSumm.Text = order.summOrder().ToString();

        }
    }
}
