using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using MySql.Data.MySqlClient;
/// <summary>
/// This page is called when you click on an item in the Database menu. It will be used to display the item about that specific asset, as well as feature the ability to modify it
/// </summary>
/// 



namespace ZebraRFIDApp.Model
{
    public class DatabaseItemPage : ContentPage
    {
        //use this to build the table section, then copy it to content
        TableSection cells = null;
		//page
		TableView page = null;
		//list that has labels for entry cells
		List<string> cellLabels = new List<string>() {
		   "ItemID: ",
		   "Item: ",
		   "RFID Tag: ",
		   "Inspector Name: ",
		   "Inspection Result: "
		};

		public DatabaseItemPage(string tagID)
        {

			//create new TableSection
			TableSection cells = new TableSection();

			page = new TableView()
			{
				Intent = TableIntent.Form,
				Root = new TableRoot
				{
					cells
				}
			};

			//connect to SQL, retrieve information

			string connStr = "server=webdb.uvm.edu;user=jjung2_admin;database=JJUNG2_RFID_TEST;port=3306;password=UzAn4dsM6VIZigk1";
			MySqlConnection conn = new MySqlConnection(connStr);

			conn.Open();


			//build query string
			string sql = "SELECT assetItemID, assetItem, tagID, inspectName, inspectResult Location from tblTestTagInfo where tagID = '";
			sql += tagID;
			sql += "';";

			//execute SQL statement
			MySqlCommand cmd = new MySqlCommand(sql, conn);
			MySqlDataReader rdr = cmd.ExecuteReader();
			//Read from rdr
			rdr.Read();

			for (int i = 0; i < 5; ++i)
            {	//create new buttons for Table
				var itemField = new EntryCell()
				{
					Label = cellLabels[i],
					Text = (string) rdr[i],
					
				};
				//add cell to list
				cells.Add(itemField);
            }

            
            var submit = new Button()
			{
				Text = "Submit",
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				TextColor = Color.Black,
				BorderColor = Color.Blue,
				BackgroundColor = Color.Gray,
				FontSize = 20,
				Margin = new Thickness(10, 0)
            };


			//event handler for click
			//TODO: 
			submit.Clicked += (sender, args) =>
			{   //new page with rdr 0
				foreach (EntryCell i in cells)
                {
					string sql = String.Format("UPDATE tblTestTagInfo SET assetItem={1} tagID={2} inspectName={3} inspectResult={4} where tagID = '{0}';",
						cells[0].GetValue(), cells[1].GetValue(), cells[2].GetValue(), cells[3].GetValue(), cells[4].GetValue());
					
					//execute SQL statement
					MySqlCommand cmd = new MySqlCommand(sql, conn);
					MySqlDataReader rdr = cmd.ExecuteReader();
                }
			};


			//create content page
			Content = new StackLayout
			{
				Children =
				{
					page,
					submit
				}
			};
			
		}
    }
}