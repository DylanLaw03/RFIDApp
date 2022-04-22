﻿
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZebraRFIDApp.API;
using ZebraRFIDApp.Model;
using ZebraRFIDApp.Pages.About;
using ZebraRFIDApp.Pages.Settings;
using ZebraRFIDApp.Pages.Tab;
using ZebraRFIDApp.Pages.Database;
//New dependencies below
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ZebraRFIDApp
{
    /// <summary>
    /// MainPage
    /// </summary>
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new HomeMenuViewModel();

        }

        /// <summary>
        /// Select menu item from listview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="tappedEventArg"></param>
        private void OnItemSelected(Object sender, ItemTappedEventArgs tappedEventArg)
        {
            ///Dylan Lawrence
            ///Here I modified the options to open the menu.
            if (tappedEventArg.ItemIndex == 0)
            {
                Navigation.PushAsync(new TabbedPageContainner(ConstantsString.InventoryLoad));
            }
            else if (tappedEventArg.ItemIndex == 1)
            {
                Navigation.PushAsync(new TabbedPageContainner(ConstantsString.LocateTagLoad));
            }
            else if (tappedEventArg.ItemIndex == 2)
            {
                Navigation.PushAsync(new SettingPage());
            }
            else if (tappedEventArg.ItemIndex == 3)
            {
                Navigation.PushAsync(new DatabasePage());
            }
            else if (tappedEventArg.ItemIndex == 4)
            {
                Navigation.PushAsync(new AboutPage());
            }
            else
            {
                DisplayAlert(ConstantsString.Msg, ConstantsString.MsgNotSupported, ConstantsString.MsgActionOk);
            }
        }
    }
}
