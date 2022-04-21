using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace ZebraRFIDApp.Pages.Database
{
	public class DatabasePage : ContentPage
	{
		//empty page
		StackLayout Content = null;
		public DatabasePage ()
		{
			Content = new StackLayout();

			Content.Children.Add(new Label { Text = "Test 1" });
		}
	}
}