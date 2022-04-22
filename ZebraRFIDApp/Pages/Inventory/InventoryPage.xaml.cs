using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZebraRFIDApp.API;
using ZebraRFIDApp.Model;
using ZebraRfidSdk;

namespace ZebraRFIDApp.Pages.Inventory
{
    /// <summary>
    /// InventoryPage
    /// </summary>
    public partial class InventoryPage : ContentPage
    {

            bool disable = false; 
            List<TagData> groupTagsData = new List<TagData>();
            List<TagSeenCount> tagSeenCounts = new List<TagSeenCount>();
            List<TagData> GroupTagsData { get => groupTagsData; set => groupTagsData = value; }
            List<TagSeenCount> TagSeenCounts { get => tagSeenCounts; set => tagSeenCounts = value; }

            List<TagDataModel> reverseTagDataList = new List<TagDataModel>();
            IList<TagDataModel> tagDataList = SdkHandler.GetTagDataList();
      
            Readers readerManager;

            public InventoryPage()
            {
                InitializeComponent();
          
                try
                {
                    readerManager = SdkHandler.GetInstance().ReaderManager;
                    readerManager.TagDataEvent += ReaderNotifyDataEvent;
                    readerManager.OperationBatchmode += ReaderManager_OperationBatchmode;
                    //Not sure what this does, did not work on installation readerManager.TriggerNotifyEvent += ReaderManagerTriggerEvent;



            }
                catch (Exception e)
                {
                    Console.WriteLine("Exception " + e.Message);
                }
            }

            /// <summary>
            /// Page on appearing
            /// </summary>
            protected override void OnAppearing()
            {
                base.OnAppearing();
         
                lbHeaderDetail.HeightRequest = ConstantsString.HeaderDefaultHeight;

                Globals.InvetoryViewAppeared = true;
                if (Globals.ReaderBatchMode == Globals.BatchModeState.BatchModeConnected)
                {

                    btnInventory.Text = ConstantsString.Stop;
                    ListViewHederHeightSet();
                    Globals.IsInventoryStart = true;
                }
                else if (Globals.StartPressInventory == Globals.InventoryState.Start)
                {
                    btnInventory.Text = ConstantsString.Stop;
                    Globals.IsInventoryStart = true;
                }
                else if (Globals.IsInventoryStart)
                {
                    btnInventory.Text = ConstantsString.Stop;
                    Globals.IsInventoryStart = true;

                }
                else
                {
                    btnInventory.Text = ConstantsString.Start;
                    Globals.IsInventoryStart = false;
                    UpdateUI();
                }

           
           

            }

     

            /// <summary>
            /// update the ui
            /// </summary>
            void UpdateUI()
            {
                /* Code for testing
                TagDataModel testTag = new TagDataModel();
                testTag.tagID = "2016093008447A01101001A6";
                tagDataList.Add(testTag);
                */

                reverseTagDataList = new List<TagDataModel>(tagDataList);
                reverseTagDataList.Reverse();
                lstTagData.ItemsSource = reverseTagDataList;


                lbTotalTags.Text = string.Format(ConstantsString.TotalTagStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetTotalTag());

                lbUniqueTags.Text = string.Format(ConstantsString.UniqueStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetUniqueTotalTag());

            }


            /// <summary>
            /// Selecte a Tag in Listview
            /// </summary>
            /// <param name="sender">Sender</param>
            /// <param name="tappedEventArg">Event Argument</param>
            private void OnItemSelected(Object sender, ItemTappedEventArgs tappedEventArg)
            {
                //open DatabaseItemPage for that tag
                Navigation.PushAsync(new DatabaseItemPage(tagDataList[tappedEventArg.ItemIndex].tagID));
                /* Original Code
                if (!this.disable)
                {
                    this.disable = true;
                    System.Diagnostics.Debug.WriteLine("OnItemSelected");

                    if (((ListView)sender).SelectedItem == null)
                    {
                        return;
                    }

                    if (Globals.SelectedTagObject == null && reverseTagDataList[tappedEventArg.ItemIndex].tagData != null)
                    {
                        SdkHandler.UpdateTagDataList(reverseTagDataList[tappedEventArg.ItemIndex].tagData, true);
                        Globals.SelectedTagObject = reverseTagDataList[tappedEventArg.ItemIndex].tagData;
                    }
                    else
                    {

                        if ((Globals.SelectedTagObject == reverseTagDataList[tappedEventArg.ItemIndex].tagData))
                        {
                            SdkHandler.UpdateTagDataList(reverseTagDataList[tappedEventArg.ItemIndex].tagData, false);
                            Globals.SelectedTagObject = null;
                        }
                        else
                        {
                            SdkHandler.UpdateTagDataList(Globals.SelectedTagObject, false);
                            SdkHandler.UpdateTagDataList(reverseTagDataList[tappedEventArg.ItemIndex].tagData, true);
                            Globals.SelectedTagObject = reverseTagDataList[tappedEventArg.ItemIndex].tagData;
                        }

                    }

            

                
                    reverseTagDataList = new List<TagDataModel>(tagDataList);
                    reverseTagDataList.Reverse();
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        this.lstTagData.ItemsSource = null;
                        lstTagData.ItemsSource = reverseTagDataList;
                    });
             

                    this.disable = false;

                }
                */
         
            }

            /// <summary>
            /// clear the list view
            /// </summary>
            void ClearList()
            {
                lstTagData.ItemsSource = null;
                SdkHandler.ClearTagataList();
                SdkHandler.ClearTagSeenList();

                SdkHandler.ClearGroupTagsData();
                SdkHandler.totalTag = ConstantsString.ZeroValue;
                SdkHandler.totalUniqueTag = ConstantsString.ZeroValue;
                lbUniqueTags.Text = string.Format(ConstantsString.UniqueStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetTotalTag());
                lbTotalTags.Text = string.Format(ConstantsString.TotalTagStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetUniqueTotalTag());

        

            }

            /// <summary>
            /// Set height for listview header  
            /// </summary>
            void ListViewHederHeightSet()
            {
                if (Globals.ReaderBatchMode == Globals.BatchModeState.BatchModeConnected || Globals.ReaderBatchMode == Globals.BatchModeState.Enable)
                {
                    lbHeaderDetail.Text = ConstantsString.InventoryInBatchMode;
                    lbHeaderDetail.HeightRequest = ConstantsString.HeaderExpandHeight;

                }
                else
                {
                    lbHeaderDetail.Text = "";
                    lbHeaderDetail.HeightRequest = ConstantsString.HeaderDefaultHeight ;
                }
            }

            /// <summary>
            /// Button click event Start/Stop inventory
            /// </summary>
            /// <param name="sender">Sender</param>
            /// <param name="eventArgs">Event Argument</param>
            void OnButtonClickedStartStop(object sender, EventArgs eventArgs)
            {
                System.Diagnostics.Debug.WriteLine("OnButtonClickedResetDefault Click");
                try
                {
                    if (SdkHandler.ConnectedReader != null)
                    {
                        

                        if (btnInventory.Text == ConstantsString.Start)
                        {
                            ClearList();
                            ListViewHederHeightSet();


                            SdkHandler.ConnectedReader.Actions.Inventory.Start();
                            btnInventory.Text = ConstantsString.Stop;
                            Globals.IsInventoryStart = true;
                        }
                        else
                        {
                            
                             SdkHandler.ConnectedReader.Actions.Inventory.Stop();
                       
                             btnInventory.Text = ConstantsString.Start;
                             Globals.IsInventoryStart = false;

                            if(SdkHandler.ConnectedReader.Configuration.BatchModeConfiguration == BatchMode.ENABLE)
                            {
                                Globals.ReaderBatchMode = Globals.BatchModeState.Enable;
                            }

                            lbHeaderDetail.Text = "";
                            if (Globals.ReaderBatchMode == Globals.BatchModeState.BatchModeConnected)
                            {
                                
                                lbHeaderDetail.HeightRequest = ConstantsString.HeaderDefaultHeight;
                                Globals.ReaderBatchMode = Globals.BatchModeState.Auto;
                                SdkHandler.ConnectedReader.Actions.Inventory.GetBatchData();
                                Application.Current.Properties[ConstantsString.BatchModeType] = ConstantsString.BatchModeTypeAuto;
                            }
                            else if (Globals.ReaderBatchMode == Globals.BatchModeState.Enable)
                            {
                                SdkHandler.ConnectedReader.Actions.Inventory.GetBatchData();
                                Application.Current.Properties[ConstantsString.BatchModeType] = ConstantsString.BatchModeTypeEnable;
                            }

                        
                            SdkHandler.ConnectedReader.Actions.Inventory.PurgeData();
                            Globals.StartPressInventory = Globals.InventoryState.Stop;
                            
                    }


                 
                   
                    }
                    else
                    {

                        DisplayAlert(ConstantsString.Msg, ConstantsString.MsgNoActiveReader, ConstantsString.MsgActionOk);

                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception " + e.Message);
                    DisplayAlert(ConstantsString.Msg, ConstantsString.MsgUnableToPerformStartStopInventory, ConstantsString.MsgActionOk);

                }



            }

            /// <summary>
            /// Update the list view
            /// </summary>
            /// <param name="tagSeenCountsList">TagSeenCount List</param>
            void UpdateListView(List<TagSeenCount> tagSeenCountsList)
            {

                try
                {

                    Console.WriteLine("tag seen counts list " + tagSeenCountsList);
                    if (tagSeenCountsList != null && tagSeenCountsList.Count > 0)
                    {
                        foreach (TagSeenCount tagSeenCount in tagSeenCountsList)
                        {
                            SdkHandler.UpdateTagDetail(tagSeenCount.TagData, tagSeenCount);
                        }

                        int tagSum = 0;

                        tagSum = SdkHandler.TagSeenCountList.Sum(a => a.SeenCount);

                        if (Globals.UniqueTagEnabled && Globals.ReaderBatchMode != Globals.BatchModeState.Enable)
                        {
                            SdkHandler.totalTag = SdkHandler.GroupTagsData.Count.ToString();
                        }
                        else
                        {
                            SdkHandler.totalTag = tagSum.ToString();

                        }

                        SdkHandler.totalUniqueTag = SdkHandler.GroupTagsData.Count.ToString();

                        tagDataList = SdkHandler.GetTagDataList();
                        reverseTagDataList = new List<TagDataModel>(tagDataList);
                        reverseTagDataList.Reverse();
                        lstTagData.ItemsSource = reverseTagDataList;
                        if (Globals.UniqueTagEnabled && Globals.ReaderBatchMode != Globals.BatchModeState.Enable && Globals.ReaderBatchMode != Globals.BatchModeState.BatchModeConnected)
                        {
                            lbTotalTags.Text = string.Format(ConstantsString.TotalTagStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetUniqueTotalTag());
                        }
                        else
                        {
                            lbTotalTags.Text = string.Format(ConstantsString.TotalTagStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetTotalTag());

                        }

                        lbUniqueTags.Text = string.Format(ConstantsString.UniqueStringFormat, System.Environment.NewLine, System.Environment.NewLine, SdkHandler.GetUniqueTotalTag());


                    }

                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception " + e.Message);
                }
               


        }

       

            /// <summary>
            /// Handle the search button click event
            /// </summary>
            /// <param name="sender">Sender</param>
            /// <param name="eventArgs">Event Argument</param>
            void Handle_SearchButtonPressed(object sender, System.EventArgs eventArgs)
            {
                System.Diagnostics.Debug.WriteLine("Handle_SearchButtonPressed Click");
            }

            /// <summary>
            /// Is the os reader notify data event.
            /// </summary>
            /// <param name="tagData">Tag data.</param>
            private void ReaderNotifyDataEvent(TagData tagData)
            {
          
                   System.Diagnostics.Debug.WriteLine("Inventory ReaderNotifyDataEvent "+ tagData.Id);

               
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        UpdateListView(SdkHandler.GetTagSeenList());

                    });


            }


            /// <summary>
            /// Event handler of Reader in batchmode.
            /// </summary>
            /// <param name="eventStatus">Event status.</param>
            private void ReaderManager_OperationBatchmode(EventStatus eventStatus)
            {
                if (Globals.ReaderBatchMode == Globals.BatchModeState.BatchModeConnected
                        || Globals.ReaderBatchMode == Globals.BatchModeState.Enable)
                {
                    lbHeaderDetail.Text = ConstantsString.InventoryInBatchMode;
                    lbHeaderDetail.HeightRequest = ConstantsString.HeaderExpandHeight;
            }
                else
                {
                    lbHeaderDetail.Text = "";
                    lbHeaderDetail.HeightRequest = ConstantsString.HeaderDefaultHeight;
            }
            }


        /// <summary>
        /// The event trigger notify.
        /// </summary>
        /// <param name="readerID">Reader identifier.</param>
        /// <param name="triggerEvent">Trigger event.</param>
        void ReaderManagerTriggerEvent(int readerID, TriggerType triggerEvent)
        {	
                Console.WriteLine("TriggerType  " + triggerEvent);
                try
                {
                    if (SdkHandler.ConnectedReader != null)
                    {
                        if (triggerEvent == TriggerType.TRIGGERTYPE_PRESS && btnInventory.Text == ConstantsString.Start)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ClearList();
                                ListViewHederHeightSet();
                                btnInventory.Text = ConstantsString.Stop;
                            });
                        
                            SdkHandler.ConnectedReader.Actions.Inventory.Start();
                            Globals.IsInventoryStart = true;
                        }
                        else 
                        {
                          
                           Device.BeginInvokeOnMainThread(() =>
                           {
                             btnInventory.Text = ConstantsString.Start;
                             lbHeaderDetail.Text = "";
                            });
                            SdkHandler.ConnectedReader.Actions.Inventory.Stop();
                       
                            Globals.IsInventoryStart = false;

                            if (SdkHandler.ConnectedReader.Configuration.BatchModeConfiguration == BatchMode.ENABLE)
                            {
                                Globals.ReaderBatchMode = Globals.BatchModeState.Enable;
                            }

                         
                            if (Globals.ReaderBatchMode == Globals.BatchModeState.BatchModeConnected)
                            {

                                lbHeaderDetail.HeightRequest = ConstantsString.HeaderDefaultHeight;
                                Globals.ReaderBatchMode = Globals.BatchModeState.Auto;
                                SdkHandler.ConnectedReader.Actions.Inventory.GetBatchData();
                                Application.Current.Properties[ConstantsString.BatchModeType] = ConstantsString.BatchModeTypeAuto;
                            }
                            else if (Globals.ReaderBatchMode == Globals.BatchModeState.Enable)
                            {
                                SdkHandler.ConnectedReader.Actions.Inventory.GetBatchData();
                                Application.Current.Properties[ConstantsString.BatchModeType] = ConstantsString.BatchModeTypeEnable;
                            }


                            SdkHandler.ConnectedReader.Actions.Inventory.PurgeData();
                            Globals.StartPressInventory = Globals.InventoryState.Stop;

                        }

                    }
                    else
                    {
                        DisplayAlert(ConstantsString.Msg, ConstantsString.MsgNoActiveReader, ConstantsString.MsgActionOk);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception " + e.Message);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        btnInventory.Text = ConstantsString.Start;
                        lbHeaderDetail.Text = "";
                    });
                    DisplayAlert(ConstantsString.Msg, ConstantsString.MsgUnableToPerformStartStopInventory, ConstantsString.MsgActionOk);
                  
             }

             

            }

    }
}
